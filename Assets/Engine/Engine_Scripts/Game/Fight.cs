using UnityEngine;
using System.Collections;

public class Fight : ShidouGameObject
{
	public static Fight f;
	
	public bool fight_active;
	
	public Squad[] squads;
	public float[] squad_times;
	protected int squads_active;
	protected int squad_index;
	
	protected float time;
	
	public delegate void NewBulletEventHandler(Bullet n);
	public event NewBulletEventHandler _NewBullet;
	
	public delegate void NewEnemyEventHandler(Enemy n);
	public event NewEnemyEventHandler _NewEnemy;
	
	public delegate void NewSquadEventHandler(Squad n);
	public event NewSquadEventHandler NewSquad;
	
	public string background_level;
	
	protected float bg_prog;

	public int difficulty;

	public string fight_name, fight_info;

	public bool[] enemies_in_this_fight;

	public AudioSource aud;

	int tutorial_stage;

	public static Texture fight_tut0, fight_tut1;

	protected override void _Update ()
	{
		if (!fight_active) return;

		if (aud != null && aud.volume < 1f)
		{
			Fight.f.aud.volume += Time.deltaTime;
			if (Fight.f.aud.volume > 1f)
				Fight.f.aud.volume = 1f;
		}

		if (Settings.fight_tutorial)
		{
			if (tutorial_stage == 0 &&
			    (Settings.left.Held || Settings.right.Held || Settings.right.primary != KeyCode.D || Settings.left.primary != KeyCode.A))
				tutorial_stage++;
			else if (tutorial_stage == 1 &&
			         (Settings.shoot_fore.Held || Settings.shoot_fore.primary != KeyCode.Space))
			{
				tutorial_stage++;
				Settings.fight_tutorial = false;
				Settings.SaveSettings();
			}

			return;
		}

		time += Time.deltaTime;
		
		if (squads_active <= 0 && squad_index != 0)
		{
			if (squad_index >= squads.Length)
			{
				StartCoroutine(End());
				fight_active = false;
			}
			else
				time = squad_times[squad_index];
		}
		
		for (int i = squad_index; i < squads.Length; i++)
		{
			if (!squads[i].Spawned && time >= squad_times[i])
			{
				squad_index = i + 1;
				
				squads[i].Spawn();
				squads_active++;
				squads[i].SquadDestroyed += LoseSquad;
				if (NewSquad != null)
					NewSquad(squads[i]);
			}
		}
	}
	
	public IEnumerator LoadAndBegin()
	{
		AsyncOperation o = Application.LoadLevelAsync("scene_fight");
		
		while (!o.isDone)
			yield return null;
		
		if (background_level != "")
		{
			o = Application.LoadLevelAdditiveAsync(background_level);
			
			while (!o.isDone)
			{
				bg_prog = o.progress;
				yield return null;
			}
		}
		
		bg_prog = 1f;
		
		GameObject controller = new GameObject("Controller");
		PlayerController player = controller.AddComponent<PlayerController>();
		
		for (int i = 0; i < squads.Length; i++)
		{
			GameObject gobj = (GameObject)Instantiate(squads[i].gameObject);
			squads[i] = gobj.GetComponent<Squad>();
			gobj.transform.parent = transform;
		}

		yield return null;
		Utilities.paused = false;
		
		GameObject pause = new GameObject("pause_obj");
		pause.AddComponent<PauseMenu>();

		yield return null;
		aud.volume = 0f;
		if (!Settings.mute_music)
		{
			aud.Play();
		}
	}
	
	public void Start()
	{
		tutorial_stage = 0;

		aud = audio;
		if (aud != null)
		{
			aud.Stop ();
		}

		squads_active = 0;
		squad_index = 0;
		fight_active = true;
		
		bg_prog = 0f;
		
		DontDestroyOnLoad(this);
		
		Map.m.gameObject.SetActive(false);
		
		f = this;
		Utilities.paused = true;
		
		StartCoroutine(LoadAndBegin());
	}
	
	public void NewBullet(Bullet b)
	{
		if (_NewBullet != null)
			_NewBullet(b);
	}
	
	public void NewEnemy(Enemy e)
	{
		if (_NewEnemy != null)
			_NewEnemy(e);
	}
	
	void LoseSquad(Squad s)
	{
		squads_active--;
	}
	
	public IEnumerator End()
	{
		bool victory = true;
		if (!Input.GetKey(KeyCode.C))
			for (int i = 0; i < Map.m.tower_fabs.Length + 1; i++)
				if (Map.m.sectors[i].color != Sector.BlockColor.Cyan)
			{
				victory = false;
				break;
			}

		AsyncOperation o = null;
		if (!victory)
		{
			o = Application.LoadLevelAsync("scene_garage");
			o.allowSceneActivation = false;
		}
			
		GameObject p = Chassis.c.gameObject;

		while (p.transform.position.z > -200f)
		{
			p.transform.position += new Vector3(0f, 0f, -100f * Time.deltaTime);
			aud.volume -= Time.deltaTime * 0.5f;
			yield return null;
		}

		// Do clean up or anything else HERE

		// Unlocked a weapon!?
		if (GarageController.selected_sector.weapon_unlock >= 0)
			GarageController.UnlockWeapon(GarageController.selected_sector.weapon_unlock);

		// Won with a new weapon!?
		bool save_settings = false;
		for (int i = 0; i < GarageController.weapons.Length; i++)
		{
			if ((Chassis.c.sc_class_1 && Chassis.c.fore != null &&
			    Chassis.c.fore.weapon_name.Equals(GarageController.weapons[i].weapon_name))
			    ||
			    (Chassis.c.sc_class_2 && Chassis.c.turret != null &&
			 	Chassis.c.turret.weapon_name.Equals(GarageController.weapons[i].weapon_name)))
			{
				Settings.won_with_which_weapons[i] = true;

				bool unlocked_jack_achieve = true;
				for (int j = 0; j < Settings.won_with_which_weapons.Length; j++)
				{
					if (!Settings.won_with_which_weapons[j])
					{
						unlocked_jack_achieve = false;
						j = Settings.won_with_which_weapons.Length;
					}
				}
				if (unlocked_jack_achieve)
					Utilities.u.UnlockAchievement("Jack of All Guns");
				save_settings = true;
			}
		}

		if (save_settings)
			Settings.SaveSettings();
		// Okay clean up is all done.

		if (!victory)
		{
			o.allowSceneActivation = true;
			while (!o.isDone)
				yield return true;
			
			p.transform.position = Vector3.zero;
			p.transform.rotation = Quaternion.identity;
		}
		else
		{
			//Achievement
			Utilities.u.UnlockAchievement("Golden Chariot");
			
			Destroy(Chassis.c.gameObject);
			Destroy(Map.m);
			
			Application.LoadLevelAsync("scene_victory");
		}
		
		Destroy(gameObject);
	}
	
	void OnGUI()
	{
		if (bg_prog < 1f)
			Utilities.DrawLine(new Vector2(0f, Screen.height * 2 / 3), new Vector2(Mathf.Lerp(0f, Screen.width, bg_prog), Screen.height * 2 / 3), 10f, Color.white);

		if (Settings.fight_tutorial && bg_prog >= 1f)
		{
			if (tutorial_stage == 0)
				GUI.DrawTexture(new Rect(Screen.width / 2 - fight_tut0.width / 2, Screen.height - fight_tut0.height, fight_tut0.width, fight_tut0.height),
				                fight_tut0);
			else if (tutorial_stage == 1)
				GUI.DrawTexture(new Rect(Screen.width / 2 - fight_tut1.width / 2, Screen.height - fight_tut1.height - 256, fight_tut1.width, fight_tut1.height),
				                fight_tut1);
		}
	}

	protected virtual void OnValidate()
	{
		if (squads == null || squad_times == null || squads.Length != squad_times.Length)
			Debug.LogError("squads.Length and squad_times.Length do not match.", gameObject);
		fight_active = false;
	}
}
