using UnityEngine;
using System.Collections;
using System.Threading;
using System;

public class GarageController : ShidouGUIObject
{
	// Weapon unlocks
	public static bool[] weapons_can_spawn = 
	{
		true, true, false, true, false
	};
	public static bool[] unlocked_weapons = 
	{
		true, false, false, false, false
	};
	public static int num_unlocked_weapons = 1;
	public static Weapon[] weapons;
	
	public static void WipeUnlocks()
	{
		num_unlocked_weapons = 0;
		for (int i = 0; i < unlocked_weapons.Length; i++)
		{
			unlocked_weapons[i] = false || (Application.isEditor && Input.GetKey(KeyCode.C)); //CHEAT
			if (unlocked_weapons[i]) num_unlocked_weapons++;
		}
	}

	public static void UnlockWeapon(int i)
	{
		if (unlocked_weapons[i]) return;
		
		num_unlocked_weapons++;
		unlocked_weapons[i] = true;
	}

	public static void UnlockWeaponAchieve(string s)
	{
		for (int i = 0; i < weapons.Length; i++)
			if (weapons[i].weapon_name.Equals(s))
		{
			UnlockWeaponAchieve(i);
			return;
		}
	}
	public static void UnlockWeaponAchieve(int i)
	{
		weapons_can_spawn[i] = true;
	}

	// Map
	bool map_open;
	public static Sector selected_sector;
	Sector hovering_sector;
	
	// UI
	Vector2 scroll_pos;
	bool dragging, leaving;
	float want_to_return;
	float t, tut_t, map_t;
	Weapon dragged_weapon;
	public Transform camera_control, ship, map;
	float hangar_to_map;
	
	// Tutorial
	int tutorial_stage;
	
	// The player's chassis
	Chassis chassis;
	
	void Start()
	{
		if (Map.m != null)
		{
			Map.m.gameObject.SetActive(true);
			Map.m.RaiseCyan();
		}

		map_open = false;
		dragging = false;
		leaving = false;
		want_to_return = 0f;
		t = 0f;
		map_t = 0f;
		tutorial_stage = 0;
		tut_t = 0f;
		hangar_to_map = 0f;

		selected_sector = null;
		
		chassis = GameObject.FindGameObjectWithTag("Player").GetComponent<Chassis>();
		chassis.shield = chassis.max_shield;
		chassis.speed = 0f;
	}
	
	void _UpdateHangar()
	{
		if (dragging)
		{
			// check if close enough to player's slot
			// if it is, make current equip vanish and click dragging item into place
			// if it is not, make sure current equip is visible and move dragging item to cursor
			int click = 0;
			Plane floor = new Plane(Vector3.up, -0.3f);
			float d;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			floor.Raycast(ray, out d);
			Vector3 floor_point = ray.GetPoint(d);
			RaycastHit rch;
			if (Physics.Raycast(ray, out rch, 50f, 1 << LayerMask.NameToLayer("slot")))
			{
				if (chassis.fore_slot != null && chassis.sc_class_1 && rch.collider.transform.position == chassis.fore_slot.position)
				{
					click = 1;
					
					dragged_weapon.transform.position = chassis.fore_slot.position;
					if (chassis.fore != null)
						if (chassis.fore.gameObject.activeSelf)
							chassis.fore.gameObject.SetActive(false);
				}
				else if (chassis.turret_slot != null && chassis.sc_class_2 && rch.collider.transform.position == chassis.turret_slot.position)
				{
					click = 2;
					
					dragged_weapon.transform.position = chassis.turret_slot.position;
					if (chassis.turret != null)
						if (chassis.turret.gameObject.activeSelf)
							chassis.turret.gameObject.SetActive(false);
				}
			}
			
			if (click == 0)
			{
				dragged_weapon.transform.position = floor_point;
				if (chassis.fore != null)
					if (!chassis.fore.gameObject.activeSelf)
						chassis.fore.gameObject.SetActive(true);
				if (chassis.turret != null)
					if (!chassis.turret.gameObject.activeSelf)
						chassis.turret.gameObject.SetActive(true);
			}
			
			// if the mouse is no longer down,
			if (!Input.GetMouseButton(0))
			{
				// if dragging item is clicked into place
				if (click == 1)
				{
					// equip dragging item
					if (tutorial_stage == 1)
					{
						tutorial_stage = 2;
						tut_t = 0f;
					}
					chassis.EquipFore(dragged_weapon);
				}
				else if (click == 2)
				{
					chassis.EquipTurret(dragged_weapon);
				}
				// else
				else if (click == 0)
				{
					if (tutorial_stage == 1)
					{
						tutorial_stage = -1;
						tut_t = 0f;
					}
					Destroy(dragged_weapon.gameObject);
					// destroy dragging item
				}
				dragging = false;
				// set dragging to false
			}
		}
		else
		{
			float scr = Input.GetAxis("Mouse ScrollWheel");
			if (scr > 0)// Go one left
				scroll_pos.x = (((int)scroll_pos.x) / 300 - 1) * 300;
			else if (scr < 0)// Go one right
				scroll_pos.x = (((int)scroll_pos.x) / 300 + 1) * 300;
		}
	}
	
	void _UpdateMap()
	{
		// RaycastAll mouse position
		Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit[] rch = Physics.RaycastAll(r, 50f, 1 << LayerMask.NameToLayer("map"));
		// Sort the raycast by depth
		int[] order = new int[rch.Length];
		for (int i = 0; i < order.Length; i++)
		{
			int j = 0;
			for (; j < i; j++)
			{
				if (rch[i].distance < rch[order[j]].distance)
				{
					for (int k = i; k > 0; k--)
						order[k] = order[k - 1];
					
					order[j] = i;
					j = i + 10;
				}
			}
			if (j == i)
				order[i] = i;
		}
		// Check each hit and whichever is the closest that's a block from a legal sector
		hovering_sector = null;
		for (int i = 0; i < order.Length; i++)
		{
			// Set hover to that sector, or -1 if one couldn't be found
			MapBlock b = rch[order[i]].collider.GetComponent<MapBlock>();
			if (b == null || b.walls == null || 
				b.walls.activeSelf == false || 
				b.sector == null || 
				b.sector.color == Sector.BlockColor.Cyan) continue;
			
			hovering_sector = b.sector;
			break;
		}
		
		// if mouse down
		if (Input.GetMouseButtonUp(0))
		{
			// Select hover
			if (hovering_sector != null)
			{
				if (selected_sector == hovering_sector)
				{
					_Launch();
				}
				else
				{
					if (selected_sector != null)
						selected_sector.Selected = false;
					selected_sector = hovering_sector;
					selected_sector.Selected = true;
					
					if (tutorial_stage == 3)
					{
						tutorial_stage = 4;
						tut_t = 0f;
					}
				}
			}
		}
	}
	
	void Update()
	{
		if (want_to_return > 0f)
			want_to_return -= Time.deltaTime;
		
		if (t < 1f && !leaving)
			t += Time.deltaTime * TitleScene.line_speed;
		else if (leaving)
			t -= Time.deltaTime * TitleScene.line_speed;
		
		if (tut_t < 1f && !leaving)
			tut_t += Time.deltaTime * TitleScene.line_speed * 2f;
		else if (leaving)
			tut_t -= Time.deltaTime * TitleScene.line_speed * 2f;
		
		if (map_t < 1f && !leaving && (chassis.turret != null || chassis.fore != null))
			map_t += Time.deltaTime * TitleScene.line_speed * 2f;
		else if (leaving)
			map_t -= Time.deltaTime * TitleScene.line_speed * 2f;
		
		if (map_open)
		{
			if (hangar_to_map < 1f)
			{
				hangar_to_map += Time.deltaTime;
				if (hangar_to_map >= 1f)
					Map.OpenMap(true);
			}
			else if (hangar_to_map < 3f)
				hangar_to_map += Time.deltaTime;
			if (hangar_to_map >= 3f)
			{
				hangar_to_map = 3f;
			}
			
			_UpdateMap();
		}
		else
		{
			if (hangar_to_map > 0f)
			{
				if (hangar_to_map >= 3f)
				{
					Map.OpenMap(false);
					hangar_to_map = 1f;
				}
				hangar_to_map -= Time.deltaTime;
			}
			if (hangar_to_map < 0f)
				hangar_to_map = 0f;
				
			_UpdateHangar();
		}
		
		camera_control.transform.position = Vector3.Lerp(ship.position, map.position + new Vector3(0f, 0f, 0.75f), hangar_to_map);
		camera_control.transform.localScale = Vector3.Lerp(ship.localScale, map.localScale, hangar_to_map);
		camera_control.transform.localRotation = Quaternion.Lerp(ship.rotation, map.rotation, hangar_to_map);
	}
	
	void _Launch()
	{
		// Launch fight at selected tile
		if (tutorial_stage == 4)
		{
			tutorial_stage = 5;
			Settings.tutorial = false;
			Settings.SaveSettings();
		}
		
		selected_sector.Selected = false;

		StartCoroutine(IEnumLaunch());
	}
	
	IEnumerator IEnumLaunch()
	{
		Map.m.ConvertSector(selected_sector);
		Map.m.LowerAll();

		int num_cyan_sects = 0;
		foreach (Sector s in Map.m.sectors)
			if (s.color == Sector.BlockColor.Cyan)
				num_cyan_sects++;
		if (num_cyan_sects * 2 > Map.m.sectors.Length)
			Utilities.u.UnlockAchievement("Buckshot");
		
		yield return new WaitForSeconds(2f);

		GameObject fight = (GameObject)Instantiate(selected_sector.fight.gameObject);
	}
	
	protected override void _OnGUI ()
	{
		float dx = Mathf.Clamp(Mathf.Lerp(0, Screen.width / 2, t), 0, Screen.width / 2);
		float mapx = Mathf.Clamp(Mathf.Lerp(0, Screen.width / 2, map_t), 0, Screen.width / 2);
		int w = Mathf.Min (Screen.width / 2 - 64, 300 * num_unlocked_weapons);
		
		if (hangar_to_map <= 0f)
		{
			// Gear scrollview
			if (dx > 0)
			{
				scroll_pos = GUI.BeginScrollView(new Rect(Mathf.Max (Screen.width / 2 - dx, Screen.width / 2 - w), Screen.height - 175, Mathf.Min (w * 2, dx * 2), 175), 
					scroll_pos, new Rect(-(Screen.width / 2 - w) + Mathf.Max (Screen.width / 2 - dx, Screen.width / 2 - w), 0, 600 * num_unlocked_weapons, 160));
			
				// Loop through all gear
				int mod = 0;
				for (int i = 0; i < num_unlocked_weapons; i++)
				{
					while (!unlocked_weapons[i + mod]) mod++;
					
					if (GUI.RepeatButton(new Rect(i * 600, 0, 160, 160), weapons[i + mod].icon) |
						GUI.RepeatButton(new Rect(i * 600 + 156, 0, 444, 160), 
						weapons[i + mod].weapon_name + "\n" + weapons[i + mod].description) &&
						!dragging)
					{
						// Start dragging
						dragging = true;
						if (tutorial_stage <= 0)
						{
							tutorial_stage = 1;
							tut_t = 0f;
						}
						// Instantiate item and set it as dragging
						GameObject wep = (GameObject)Instantiate(weapons[i + mod].gameObject);
						dragged_weapon = wep.GetComponent<Weapon>();
					}
				}
				
				GUI.EndScrollView();
			}
			
			// Nav buttons
			if (w >= Screen.width / 2 - 64)
			{
				if (t >= 1f)
				{
					if (GUI.Button(new Rect(0, Screen.height - 175, 64, 175), "<"))
					{
						// Go one left
						scroll_pos.x = (((int)scroll_pos.x) / 600 - 1) * 600;
					}
				}
				else if (dx > Screen.width / 2 - 64)
					GUI.Label(new Rect(Screen.width / 2 - dx, Screen.height - 175, dx - Screen.width / 2 + 64, 175), "");
				
				if (t >= 1f)
				{
					if (GUI.Button(new Rect(Screen.width - 64, Screen.height - 175, 64, 175), ">"))
					{
						// Go one right
						scroll_pos.x = (((int)scroll_pos.x) / 600 + 1) * 600;
					}
				}
				else if (dx > Screen.width / 2 - 64)
					GUI.Label(new Rect(Screen.width / 2 - 64, Screen.height - 175, dx - Screen.width / 2 + 64, 175), "");
			}
		}
		else if (hangar_to_map >= 1f)
		{	
			string desc = "";
			if (selected_sector != null && selected_sector.color != Sector.BlockColor.Cyan)
			{
				desc =
					selected_sector.sector_name + "\n" + selected_sector.fight.fight_info + "\n" +
					"Difficulty: " + selected_sector.difficulty + "\n" +
					"Reward: ";
				if (selected_sector.weapon_unlock < 0)
					desc += "None";
				else
					desc += weapons[selected_sector.weapon_unlock].weapon_name;
				
				desc += "\nClick here to launch.";
			}
			else if (selected_sector != null && selected_sector.color == Sector.BlockColor.Cyan)
				desc = "LAUNCHING";
			else desc = "No sector selected";
			
			if (!leaving)
			{
				if (GUI.Button(new Rect(0, Screen.height - 175, Screen.width, 175), desc))
				{
					// Launch fight at that tile, yo
					_Launch();
				}
			}
			else if (dx > 0)
				GUI.Label(new Rect(Screen.width / 2 - dx, Screen.height - 175, dx * 2, 175), "");
		}
		else
		{
			float desc_w = w < Screen.width * 0.5f - 64 ? Mathf.Lerp(w, Screen.width * 0.5f, hangar_to_map) : Screen.width * 0.5f;
			GUI.Label(new Rect(Screen.width / 2 - desc_w, Screen.height - 175, desc_w * 2, 175), "");
		}
		
		// Map button
		if (chassis.fore != null || chassis.turret != null)
		{
			if (mapx >= 300)
			{
				if (hangar_to_map <= 0f ||hangar_to_map >= 3f)
				{
						if (GUI.Button(new Rect(Screen.width / 2 - 300, Screen.height - 235, 300, 60), map_open ? "CLOSE MAP" : "OPEN MAP"))
					{
						// Open the map
						map_open = !map_open;
						if (tutorial_stage == 2)
						{
							tut_t = 0f;
							tutorial_stage = 3;
						}
					}
				}
				else
					GUI.Label(new Rect(Screen.width / 2 - 300, Screen.height - 235, 300, 60), map_open ? "OPENING" : "CLOSING");
			}
			else if (mapx > 0) GUI.Label(new Rect(Screen.width / 2 - mapx, Screen.height - 235, mapx, 60), "");
		}
		
		// Quit button
		if (dx >= 300)
		{
			if (want_to_return > 0f)
			{
				if (!leaving)
				{
					if (GUI.Button(new Rect(Screen.width / 2, Screen.height - 235, 300, 60), "CLICK AGAIN TO QUIT"))
					{
						// Return to main menu
						StartCoroutine(Leave("load_main_menu"));
						want_to_return = 10f;
					}
				}
				else GUI.Label(new Rect(Screen.width / 2, Screen.height - 235, 300, 60), "");
			}
			else
			{
				if (GUI.Button(new Rect(Screen.width / 2, Screen.height - 235, 300, 60), "QUIT"))
				{
					// Open "are you sure"
					want_to_return = 5f;
				}
			}
		}
		else if (dx > 0) GUI.Label(new Rect(Screen.width / 2, Screen.height - 235, dx, 60), "");
		
		// Tutorial!
		if (Settings.tutorial)
		{
			if (!map_open)
			{
				// Stage 0 : Drag this, man
				if (tutorial_stage == 0)
				{	
					if (tut_t >= 1f)
						GUI.Label(new Rect(Screen.width / 2 + w - 200, Screen.height - 355, 200, 120), 
							"Click and drag a weapon below (hold down left mouse button)...");
					else if (tut_t > 0f)
						GUI.Label(new Rect(Screen.width / 2 + w - Mathf.Lerp(0, 200, tut_t),
							Screen.height - 355, Mathf.Lerp(0, 200, tut_t), 120), "");
				}
				else if (tutorial_stage == -1)
				{
					if (tut_t >= 1f)
						GUI.Label(new Rect(Screen.width / 2 + w - 300, Screen.height - 355, 300, 120), 
							"You didn't drop it on your ship. If you're on the right place, the weapon will snap onto the ship. Try again.");
					else if (tut_t > 0f)
						GUI.Label(new Rect(Screen.width / 2 + w - Mathf.Lerp(0, 300, tut_t),
							Screen.height - 355, Mathf.Lerp(0, 300, tut_t), 120), "");
				}
				else if (tutorial_stage == 1)
				{
					Vector3 scrn = Camera.main.WorldToScreenPoint(chassis.fore_slot.position);
					Vector2 start = new Vector2(scrn.x, Screen.height - scrn.y);
					float len = Mathf.Lerp(0, 500, tut_t);
					Utilities.DrawLine(start, start + new Vector2(Mathf.Min (300, len), 0), 5f, TitleScene.line_color);
					if (len >= 500)
						GUI.Label(new Rect(start.x + 300, start.y, 200, 120), "...and drop it on your ship to equip the weapon.");
					else if (len >= 300)
						GUI.Label(new Rect(start.x + 300, start.y, Mathf.Clamp(len - 300, 0, 200), 120), "");
				}
				else if (tutorial_stage == 2)
				{
					if (tut_t >= 1f)
						GUI.Label(new Rect(Screen.width / 2 - 300, Screen.height - 355, 300, 120), "Click on OPEN MAP when you're ready to move on.");
					else if (tut_t > 0f)
						GUI.Label(new Rect(Screen.width / 2 - Mathf.Lerp(0, 300, tut_t), Screen.height - 355, Mathf.Lerp(0, 300, tut_t), 120), "");
				}
			}
			else
			{
				if (tutorial_stage == 3)
				{
					if (tut_t >= 1f)
						GUI.Label(new Rect(Screen.width / 2 + w - 300, Screen.height - 355, 300, 120), 
							"Click on a red sector to select it. You cannot select the Cyan sector as it is your territory.");
					else if (tut_t > 0f)
						GUI.Label(new Rect(Screen.width / 2 + w - Mathf.Lerp(0, 300, tut_t),
							Screen.height - 355, Mathf.Lerp(0, 300, tut_t), 120), "");
				}
				if (tutorial_stage == 4)
				{
					if (tut_t >= 1f)
						GUI.Label(new Rect(Screen.width / 2 - 300, Screen.height - 355, 300, 120), 
							"Info about the sector you selected has appeared below. Click on the info box to travel to that sector.");
					else if (tut_t > 0f)
						GUI.Label(new Rect(Screen.width / 2 - 300,
							Screen.height - 355, Mathf.Lerp(0, 300, tut_t), 120), "");
				}
			}
		}
		
		// Cursor
		GUI.DrawTexture(new Rect(Input.mousePosition.x, Screen.height - Input.mousePosition.y, cursor.width, cursor.height), cursor);
	}
	
	IEnumerator Leave(string scene)
	{
		if (!leaving)
		{
			leaving = true;
			
			AsyncOperation o = Application.LoadLevelAsync(scene);
			o.allowSceneActivation = false;
			
			// wait for shit to vanish
			while (t > 0f)
				yield return null;
			
			yield return null;
			
			Destroy(GameObject.FindGameObjectWithTag("Player"));
			o.allowSceneActivation = true;
		}
	}
}
