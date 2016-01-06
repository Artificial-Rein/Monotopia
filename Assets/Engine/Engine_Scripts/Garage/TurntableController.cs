using UnityEngine;
using System.Collections;

public class TurntableController : ShidouGUIObject
{
	public GameObject[] hangars;
	string[] hangar_desc = 
	{
		"The last of Lockheed Martin's stealth prototypes, the X-51 was created in a time shortly " +
			"before the great AI wars. Only this prototype was created and it's truly a miracle that it " +
			"still functions after all this time.\n" +
			"The X-51 has a forward facing hardpoint for weapons.\n" +
			"It comes with the standard Machine Gun.",

		"This disgruntled robot is a modification of the X-51 which allows for more advanced weaponry. " +
		"While the X-51 can adapt itself on the fly to new weaponry, the Golden Chariot has the capability " +
		"to use any weapon in its database.\n" +
		"The Golden Chariot has a forward facing hardpoint for weapons.\n" +
		"It comes with every weapon you have won a fight with."
	};
	public static string[] hangar_names =
	{
		"X-51", "Golden Chariot"
	};
	public int selected;
	GameObject hangar;
	public float rotate_time = 1f;
	bool rotating;
	int unlocked_ships;
	
	//UI
	float entry, dialogue;
	int characters;
	float sec_since_char;
	public float sec_per_char = 0.05f;
	char[] current_display;
	string cur_string, cur_string_part;
	int wait_frames = 2;
	bool leaving;
	
	// Use this for initialization
	void Start ()
	{
		unlocked_ships = 0;
		for (int i = 0; i < Settings.unlocked_ships.Length; i++)
			if (Settings.unlocked_ships[i])
				unlocked_ships++;

		if (Map.m != null)
			Destroy(Map.m.gameObject);
		
		entry = dialogue = -1f;
		leaving = false;
		wait_frames = 2;
		
		if (!Settings.unlocked_ships[selected])
			selected = -1;
		
		rotating = false;
		StartCoroutine(Select(true));
	}
	
	IEnumerator Select(bool forward)
	{
		rotating = true;
		
		bool rotate = (hangar != null);
		
		characters = 0;
		current_display = hangar_desc[selected].ToCharArray();
		cur_string = "";
		cur_string_part = "";
		float rot = (forward ? -90f : 90f);
		
		GameObject next = (GameObject)Instantiate(hangars[selected]);
		GameObject previous = hangar;
		hangar = next;
		if (rotate)
			transform.Rotate(0f, rot, 0f);
		next.transform.parent = transform;
		//next.transform.position = Vector3.zero;
		if (rotate)
		{
			transform.rotation = Quaternion.identity;
			float rotated = 0f;
			while (rotated < 1f)
			{
				rotated += Time.deltaTime * rotate_time;
				if (Quaternion.Angle(Quaternion.Euler(0f, rot, 0f),
					transform.rotation) < rotate_time * rotate_time) rotated = 1f;
				transform.rotation = Quaternion.Lerp(transform.rotation, 
					Quaternion.Euler(0f, rot, 0f), rotated);
				yield return null;
			}
			
			Destroy(previous);
			
			next.transform.parent = null;
			transform.rotation = Quaternion.identity;
			next.transform.parent = transform;
		}
		
		rotating = false;
	}
	
	void SelectNext()
	{
		if (rotating) return;

		do
		{
			selected++;
			if (selected >= hangars.Length)
				selected = 0;
		}
		while (!Settings.unlocked_ships[selected] && selected != 0);
		
		StartCoroutine(Select(true));
	}
	
	void SelectPrevious()
	{
		if (rotating) return;
		
		do
		{
			selected--;
			if (selected < 0)
				selected = hangars.Length - 1;
		}
		while (!Settings.unlocked_ships[selected] && selected != 0);
		
		StartCoroutine(Select(false));
	}
	
	IEnumerator ClickOnShip()
	{
		while (Input.GetMouseButton(0))
			yield return null;
		
		Ray scr = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(scr, 100f, 1 << LayerMask.NameToLayer("player")))
				Launch ();
	}
	
	void Update()
	{
		if (wait_frames > 0)
		{
			wait_frames--;
			return;
		}
		
		if (Settings.right.Down)
			SelectNext();
		else if (Settings.left.Down)
			SelectPrevious();
		
		//Click on ship
		if (Input.GetMouseButtonDown(0))
		{
			Ray scr = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(scr, 100f, 1 << LayerMask.NameToLayer("player")))
				StartCoroutine(ClickOnShip());
		}
		
		//UI
		if (!leaving && dialogue > 0f && dialogue < 1f)
			dialogue += Time.deltaTime * TitleScene.button_speed;
		else if (leaving)
			dialogue -= Time.deltaTime * TitleScene.button_speed;
		
		if (entry < 1f)
			entry += (leaving ? -1f : 1f) * Time.deltaTime * TitleScene.line_speed * 0.5f;
		if (entry > 0f)
		{
			//Text
			if (sec_since_char < sec_per_char)
				sec_since_char += Time.deltaTime;
			else
			{
				while (characters < current_display.Length && sec_since_char > sec_per_char)
				{
					cur_string_part += current_display[characters];
					cur_string = cur_string_part + "\nClick to launch";
					characters++;
					sec_since_char -= sec_per_char;
				}
			}
		}
	}
	
	IEnumerator Leave(string next_scene, Chassis chass = null)
	{
		if (!leaving)
		{
			entry = 0f;
			
			leaving = true;
			
			AsyncOperation o = Application.LoadLevelAsync(next_scene);
			o.allowSceneActivation = false;
			
			while (entry > -1f || rotating)
				yield return null;
			
			yield return null;
			
			if (chass != null)
				chass.transform.parent = null;
			
			o.allowSceneActivation = true;
		}
	}
	
	void Launch()
	{
		// Detach chassis from hangar so that it doesn't get destroyed
		Chassis chass = hangar.GetComponentInChildren<Chassis>();
		chass.StartGame();
		// Launch with selected ship
		StartCoroutine(Leave("scene_garage", chass));
		// Unlock start game achievement
		Utilities.u.UnlockAchievement("Good Morning");
	}
	
	protected override void _OnGUI ()
	{
		// Launch button and info
		float w  = Mathf.Max(0, Mathf.Lerp (0, 1280, entry + 1f));
		Rect centerfold = new Rect(Screen.width / 2 - Mathf.Min (w, 1168) / 2, Screen.height - 175, Mathf.Min (w, 1168), 175);
		if (w < 1168 && w > 0)
			GUI.Label(centerfold, "");
		else if (w >= 1168)
		{
			if (GUI.Button(centerfold,
				hangar_names[selected] + "\n" + cur_string))
			{
				Launch();
			}
		}
		
		//Nav buttons
		if (unlocked_ships > 1)
		{
			if (w > 1186 && w < 1280)
			{
				GUI.Label(new Rect(centerfold.x - (w - 1168) / 2, centerfold.y, (w - 1168) / 2, centerfold.height), "");
				GUI.Label(new Rect(centerfold.x + centerfold.width, centerfold.y, (w - 1168) / 2, centerfold.height), "");
			}
			else if (w >= 1280)
			{
					// Left button
				if (GUI.Button(new Rect(centerfold.x - 56, centerfold.y, 56, centerfold.height),
					"<"))
					SelectPrevious();
				
				// Right button
				if (GUI.Button(new Rect(centerfold.x + centerfold.width, centerfold.y, 56, centerfold.height),
					">"))
					SelectNext();
			}
		}
		
		// Cursor
		GUI.DrawTexture(new Rect(Input.mousePosition.x, Screen.height - Input.mousePosition.y, cursor.width, cursor.height), cursor);
		
		// Line
		float entry_time = -entry;
		float logo_t = Mathf.Lerp(Screen.width, Screen.width / 2 - 130f, 1f - entry_time);
		if (!leaving)
		{
			if (entry_time > 0f)
				Utilities.DrawLine(new Vector2(Screen.width, 50f), 
					new Vector2(
						logo_t, 
						50f), 
					5f, TitleScene.line_color);
			else if (entry_time > -1f)
				Utilities.DrawLine(new Vector2(Screen.width / 2 - 130f, 50f), 
					new Vector2(
						Mathf.Lerp(Screen.width, Screen.width / 2 - 130f, -entry_time), 
						50f), 
					5f, TitleScene.line_color);
		}
		
		if (logo_t < Screen.width / 2 + 70)
		{
			if (logo_t > Screen.width / 2 - 130)
			{
				float sasw = (Screen.width / 2 + 70) - logo_t;
				GUI.Label(new Rect(Screen.width / 2 + 70 - sasw, 50, sasw, 60), "");
			}
			else
				// Select a ship dialogue
				GUI.Label(new Rect(Screen.width / 2 - 130, 50, 200, 60), "Select a ship");
		}
		
		if (logo_t < Screen.width / 2 + 130)
		{
			if (logo_t > Screen.width / 2 + 130)
			{
				float xw = (Screen.width / 2 + 130) - logo_t;
				GUI.Label(new Rect(Screen.width / 2 + 130 - xw, 50, xw, 60), "");
			}
			else
			// Return to main menu button
			if (GUI.Button(new Rect(Screen.width / 2 + 70, 50, 60, 60), "X"))
				StartCoroutine(Leave("load_main_menu"));
		}
	}
}
