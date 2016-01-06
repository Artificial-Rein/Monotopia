using UnityEngine;
using System.Collections;

public class PauseMenu : ShidouGUIObject
{
	public static PauseMenu p;
	
	bool can_unpause, display;
	float t;
	
	void Start()
	{
		p = this;
		display = true;
	}
	
	// Update is called once per frame
	void Update ()
	{
		// Dim music
		if (Utilities.paused && Fight.f != null && Fight.f.aud != null && Fight.f.aud.volume > 0.1f)
		{
			Fight.f.aud.volume -= Time.deltaTime * 2f;
			if (Fight.f.aud.volume < 0.1f)
				Fight.f.aud.volume = 0.1f;
		}

		// Pause
		if (!Utilities.paused && Input.GetKeyDown(KeyCode.Escape))
		{
			can_unpause = false;
			Utilities.paused = true;
			
			t = 0f;
		}
		
		// Unpause
		if (Input.GetKeyUp(KeyCode.Escape))
		{
			if (can_unpause && t >= 1f && display)
			{
				Utilities.paused = false;
				t = -1f;
			}
			else can_unpause = true;
		}
		
		if (t >= 0f && t < 1f)
			t += Time.deltaTime * TitleScene.button_speed;
		if (t > 1f) t = 1f;
	}
	
	IEnumerator Quit()
	{
		Destroy(Fight.f);
		
		AsyncOperation o = Application.LoadLevelAsync("load_main_menu");
		o.allowSceneActivation = false;
		
		while (o.progress < 0.9f) yield return null;
		
		Destroy(GameObject.FindGameObjectWithTag("Player"));
		o.allowSceneActivation = true;
	}

	IEnumerator WaitForSettingsToQuit()
	{
		GameObject gobj = null;
		while (gobj == null)
		{
			gobj = GameObject.FindGameObjectWithTag("SettingsScene");
			yield return null;
		}
		gobj.GetComponent<SettingsScene>().next_scene = "";

		while (gobj != null)
			yield return null;

		display = true;
		t = 0f;
	}

	protected override void _OnGUI ()
	{
		if (Utilities.paused)
		{
			if (display)
			{
				// Draw pause menu
				if (t >= 1f)
				{
					GUI.Label(new Rect(Screen.width / 2 - 150, Screen.height / 2 - 90, 300, 60), Fight.f.fight_name);
					if (GUI.Button(new Rect(Screen.width / 2 - 100, Screen.height / 2 - 30, 200, 60), "UNPAUSE"))
						Utilities.paused = false;
					if (GUI.Button(new Rect(Screen.width / 2 - 100, Screen.height / 2 + 30, 200, 60), "QUIT"))
					{
						Utilities.paused = false;
						StartCoroutine(Quit ());
					}
					if (GUI.Button(new Rect(Screen.width / 2 - 100, Screen.height / 2 + 90, 200, 60), "SETTINGS"))
					{
						Application.LoadLevelAdditiveAsync("scene_settings_menu");
						display = false;
						StartCoroutine(WaitForSettingsToQuit());
					}
				}
				else
				{
					GUI.Label(new Rect(Screen.width / 2 - 150 + Mathf.Lerp (150f, 0f, t), Screen.height / 2 - 90, Mathf.Lerp(0f, 300f, t), 60), "");
					GUI.Label(new Rect(Screen.width / 2 - 100, Screen.height / 2 - 30, Mathf.Lerp(0f, 200f, t), 60), "");
					GUI.Label(new Rect(Screen.width / 2 - 100 + Mathf.Lerp(200f, 0f, t), Screen.height / 2 + 30, Mathf.Lerp(0f, 200f, t), 60), "");
					GUI.Label(new Rect(Screen.width / 2 - 100, Screen.height / 2 + 90, Mathf.Lerp(0f, 200f, t), 60), "");
				}
			}
			
			GUI.DrawTexture(new Rect(Input.mousePosition.x, Screen.height - Input.mousePosition.y, cursor.width, cursor.height),
				cursor);
		}
		else if ((Chassis.c != null && Chassis.c.alive) && Chassis.c.sc_class_2 && Chassis.c.turret != null)
			GUI.DrawTexture(new Rect(Input.mousePosition.x - aimer.width * 0.5f, Screen.height - Input.mousePosition.y - aimer.width * 0.5f, 
				aimer.width, aimer.height),
				aimer);
		// Draw cursor
	}
}
