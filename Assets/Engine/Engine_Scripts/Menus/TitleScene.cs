using UnityEngine;
using System.Collections;
using System;
using System.Diagnostics;

public class TitleScene : ShidouGUIObject
{
	int wait_frames = 2;
	
	public static float button_speed = 2.5f;
	public static float line_speed = 1.5f;
	public static Color line_color = new Color(21f/255f, 21f/255f, 21f/255f, 242f/255f);
	
	float entry_time = 1f;
	float start, sett, quit, achieve, newv;
	
	bool leaving;
	
	void Start()
	{
		start = sett = quit = achieve = newv = -1f;
		leaving = false;
	}
	
	void Update()
	{
		if (wait_frames > 0)
		{
			wait_frames--;
			return;
		}
		
		if (entry_time > -1f) entry_time -= Time.deltaTime * line_speed;
		if (!leaving)
		{
			if (start >= 0f && start < 1f) start += Time.deltaTime * button_speed;
			if (sett >= 0f && sett < 1f) sett += Time.deltaTime * button_speed;
			if (quit >= 0f && quit < 1f) quit += Time.deltaTime * button_speed;
			if (achieve >= 0f && achieve < 1f) achieve += Time.deltaTime * button_speed;
			if (newv >= 0f && newv < 1f) newv += Time.deltaTime * button_speed;
		}
		else
		{
			start -= Time.deltaTime * button_speed;
			sett -= Time.deltaTime * button_speed;
			quit -= Time.deltaTime * button_speed;
			achieve -= Time.deltaTime * button_speed;
			newv -= Time.deltaTime * button_speed;
		}
	}
	
	IEnumerator Leave(string next_scene, bool additive)
	{
		if (!leaving)
		{
			leaving = true;
			
			AsyncOperation o = additive ? Application.LoadLevelAdditiveAsync(next_scene)
				: Application.LoadLevelAsync(next_scene);
			o.allowSceneActivation = false;
			
			while (start > 0f) yield return null;
			
			yield return null;
			
			o.allowSceneActivation = true;
			Destroy(gameObject);
		}
	}
	
	protected override void _OnGUI()
	{
		// Line spawner thing
		float t = Mathf.Lerp(Screen.height, Screen.height / 2 - 60, 1f - entry_time);
		
		if (entry_time > 0f)
			Utilities.DrawLine(new Vector2(Screen.width / 2 - 102.5f, Screen.height), 
				new Vector2(Screen.width / 2 - 102.5f, t), 
				5f, line_color);
		else if (entry_time > -1f)
			Utilities.DrawLine(new Vector2(Screen.width / 2 - 102.5f, Screen.height / 2 - 60), 
				new Vector2(Screen.width / 2 - 102.5f, Mathf.Lerp(Screen.height, Screen.height / 2 - 60, -entry_time)),
				5f, line_color);
		
		// Start button
		if (t <= Screen.height / 2 && start < 0f)
			start = 0f;
		
		if (start >= 1f)
		{
			if (GUI.Button(new Rect(Screen.width / 2 - 100, Screen.height / 2 - 60, 200, 60), "START"))
			{
				// Start game
				StartCoroutine(Leave ("scene_select_ship", false));
			}
		}
		else if (start > 0f)
			GUI.Box(new Rect(Screen.width / 2 - 100, Screen.height / 2 - 60, Mathf.Lerp(0f, 200f, start), 60), "");
		
		// Settings button
		if (t <= Screen.height / 2 + 60 && sett < 0f)
			sett = 0f;
		
		if (sett >= 1f)
		{
			if (GUI.Button(new Rect(Screen.width / 2 - 100, Screen.height / 2, 200, 60), "SETTINGS"))
			{
				// Go to settings menu
				StartCoroutine(Leave ("scene_settings_menu", true));
			}
		}
		else if (sett > 0f)
			GUI.Box(new Rect(Screen.width / 2 - 100, Screen.height / 2, Mathf.Lerp(0f, 200f, sett), 60), "");
		
		// Achievements button
		if (t <= Screen.height / 2 + 120 && achieve < 0f)
			achieve = 0f;
		if (achieve >= 1f)
		{
			if (GUI.Button(new Rect(Screen.width / 2 - 100, Screen.height / 2 + 60, 200, 60), "ACHIEVEMENTS"))
			{
				// Go to achievements screen
				StartCoroutine(Leave ("scene_achievements", true));
			}
		}
		else if (achieve > 0f)
			GUI.Box(new Rect(Screen.width / 2 - 100, Screen.height / 2 + 60, Mathf.Lerp(0f, 200f, achieve), 60), "");

#if UNITY_WEBPLAYER
#else
		// Quit button
		if (t <= Screen.height / 2 + 180 && quit < 0f)
			quit = 0f;
		if (quit >= 1f)
		{
			if (GUI.Button(new Rect(Screen.width / 2 - 100, Screen.height / 2 + 120, 200, 60), "QUIT"))
			{
				// Quit the game!
				StartCoroutine(Utilities.Quit());
			}
		}
		else if (quit > 0f)
			GUI.Box(new Rect(Screen.width / 2 - 100, Screen.height / 2 + 120, Mathf.Lerp(0f, 200f, quit), 60), "");

		if (LoadMainMenu.new_version)
		{
			// New version
			if (t <= Screen.height / 2 + 240 && newv < 0f)
				newv = 0f;
			if (newv >= 1f)
			{
				if (GUI.Button(new Rect(Screen.width / 2 - 100, Screen.height / 2 + 180, 200, 60), "NEW VERSION AVAILABLE"))
				{
					Process.Start("http://artificialilliteracy.com/patch_notes.html");
				}
			}
			else if (newv > 0f)
				GUI.Box(new Rect(Screen.width / 2 - 100, Screen.height / 2 + 120, Mathf.Lerp(0f, 200f, quit), 60), "");
		}
#endif
		
		
		// Logo
		// Line thing
		/*float logo_t = Mathf.Lerp(Screen.width, Screen.width / 2 - 100f, 1f - entry_time);
		if (entry_time > 0f)
			Utilities.DrawLine(new Vector2(Screen.width, Screen.height / 2 - 240f), 
				new Vector2(
					logo_t, 
					Screen.height / 2 - 240f), 
				5f, line_color);
		else if (entry_time > -1f)
			Utilities.DrawLine(new Vector2(Screen.width / 2 - 100f, Screen.height / 2 - 240f), 
				new Vector2(
					Mathf.Lerp(Screen.width, Screen.width / 2 - 100f, -entry_time), 
					Screen.height / 2 - 240f), 
				5f, line_color);
		
		Rect logo_rect = new Rect( Screen.width / 2 - 100f, Screen.height / 2 - 240f, 200f, 60f);
		if (logo_t > Screen.width / 2 + 100f);
		else if (logo_t > Screen.width / 2 - 100f)
		{
			float w = Mathf.Max(0, 
				Mathf.Min (logo_rect.width, logo_rect.x + logo_rect.width - logo_t));
			logo_rect.x += logo_rect.width - w;
			logo_rect.width = w;
			
			GUI.Box(logo_rect, "");
		}
		else if (!leaving)
			GUI.Label(logo_rect, "SHIDOU");
		else if (quit > 0f)
		{
			logo_rect.width = Mathf.Lerp(0f, logo_rect.width, quit);
			GUI.Box(logo_rect, "");
		}*/
		
		
		// Cursor
		GUI.DrawTexture(new Rect(Input.mousePosition.x, Screen.height - Input.mousePosition.y, cursor.width, cursor.height), ShidouGUIObject.cursor);
	}
}
