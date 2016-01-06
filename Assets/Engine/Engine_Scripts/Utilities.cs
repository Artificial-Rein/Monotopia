using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;
using System.Diagnostics;

public class Utilities : ShidouGUIObject
{
	#region statics
	#region DRAWLINE
	// DRAWLINE
	static Texture2D line_tex;
	static Color last_color;
	
	public static void DrawLine(Vector2 start, Vector2 end) { DrawLine(start, end, 1f, Color.white); }
	public static void DrawLine(Vector2 start, Vector2 end, float line_width) { DrawLine(start, end, line_width, Color.white); }
	public static void DrawLine(Vector2 start, Vector2 end, float line_width, Color color)
	{
		try
		{
			if (line_tex == null)
				line_tex = new Texture2D(1, 1);
			
			if (color != last_color)
			{
				last_color = color;
				line_tex.SetPixel(0, 0, color);
				line_tex.Apply();
			}
			
			float angle = Vector3.Angle(end - start, Vector2.right);
			if (start.y > end.y) angle *= -1;
			
			Matrix4x4 m = GUI.matrix;
			GUIUtility.RotateAroundPivot(angle, new Vector2(start.x, start.y + line_width / 2));
			
			//if (line_tex == null)
			//	Utilities.ThrowException(new Exception("How did this even happen? line_tex is null."));
			
			GUI.DrawTexture(new Rect(start.x, start.y, (end - start).magnitude, line_width), line_tex);
			
			GUI.matrix = m;
		}
		catch (Exception e)
		{
			//Utilities.ThrowException(e);
		}
	}
	#endregion
	
	public static bool paused = false;
	
	#region QUIT
	static bool quitting = false;
	
	public static IEnumerator Quit()
	{
		if (!quitting)
		{
			quitting = true;
			
			Screen.SetResolution(1280, 720, false);
			
			yield return null;
			
			Application.Quit();
		}
	}
	#endregion
	
	#region EXCEPTION
	public static Exception e;
	
	public static void Exception(string info)
	{
		// Throw an Exception
		e = new Exception(info);
	}
	#endregion
	
	#region VISIBILTY
	public static bool IsThisVisible(GameObject g)
	{
		foreach (Renderer r in g.GetComponentsInChildren<Renderer>())
			if (r.isVisible)
				return true;
		return false;
	}
	#endregion
	#endregion
	
	#region GameObject
	public Texture[] achievement_icons;
	
	public static Utilities u = null;

	public Weapon[] weapons;

	public Texture bug_icon, fight_tut0, fight_tut1;

	void Start()
	{
		if (u == null)
			u = this;
		else
		{
			Destroy(gameObject);
			return;
		}

		Fight.fight_tut0 = fight_tut0;
		Fight.fight_tut1 =fight_tut1;

		GarageController.weapons = weapons;
		
		DontDestroyOnLoad(gameObject);

		Application.targetFrameRate = 20;
		
		// Other startup code perhaps?
		// Only gets called at the initial launch of the game, and then never again.
		for (int i = 0; i < achievement_icons.Length && i < Settings.achievements.Length; i++)
			Settings.achievements[i].icon = achievement_icons[i];
		
		achievement_time = -1f;
		achievement_ids = new Queue<int>();
	}
	
	// Need function to pop up a window when the player gets an achievement.
	// Window needs to vanish after a moment, or when the player clicks it.
	float achievement_time = -1;
	int achievement_id = -1;
	Queue<int> achievement_ids;
	public float achievement_hide_time = 0.5f;
	public float achievement_display_time = 5f;
	public void UnlockAchievement(string name, bool display = true)
	{
		for (int i = 0; i < Settings.achievements.Length; i++)
		{
			if (Settings.achievements[i].name.Equals(name))
			{
				UnlockAchievement(i, display);
				return;
			}
		}
	}

	public void UnlockAchievement(int id, bool display = true)
	{
		if (Settings.achievements[id].unlocked) return;
		
		Settings.achievements[id].unlocked = true;
		
		if (display)
		{
			Settings.SaveSettings();
			
			achievement_ids.Enqueue(id);
		}

		for (int i = 0; i < GarageController.weapons.Length; i++)
		{
			if (GarageController.weapons[i].weapon_name.Equals(Settings.achievements[id].name))
				GarageController.UnlockWeaponAchieve(i);
		}

		for (int i = 0; i < TurntableController.hangar_names.Length; i++)
		{
			if (TurntableController.hangar_names[i].Equals(Settings.achievements[id].name))
			    Settings.unlocked_ships[i] = true;
		}
	}
	
	void DisplayAchievement()
	{
		if (achievement_time < 0f || achievement_id < 0) return;
		
		int fs = GUI.skin.label.fontSize;
		GUI.skin.label.fontSize = 18;
		
		if (achievement_time < achievement_hide_time)
			GUI.Label(new Rect(0, Screen.height - 64, 
					Mathf.Lerp(0, 264, achievement_time * 2f), 64f),
				"");
		else if (achievement_time < achievement_hide_time + achievement_display_time)
		{
			GUI.Label(new Rect(0, Screen.height - 64, 64, 64), Settings.achievements[achievement_id].icon);
			GUI.Label(new Rect(64, Screen.height - 64, 200, 64), "Unlocked Achievement:\n" + Settings.achievements[achievement_id].name);
		}
		else if (achievement_time < achievement_hide_time + achievement_hide_time + achievement_display_time)
			GUI.Label(new Rect(0, Screen.height - 64, 
					Mathf.Lerp(264, 0, (achievement_time - achievement_hide_time - achievement_display_time) * 2), 64),
				"");
		
		GUI.skin.label.fontSize = fs;
	}
	
	// Need function to draw pause menu.
	// This class cannot pause the game.
	
	protected override void _OnGUI ()
	{
		DisplayAchievement();

#if UNITY_WEBPLAYER
#else
		// BUG REPORT
		if (Fight.f == null || !Fight.f.fight_active || Utilities.paused)
		{
			if (GUI.Button(new Rect(Screen.width - bug_icon.width, 0, bug_icon.width, bug_icon.height), bug_icon))
			{
				Process.Start("https://docs.google.com/forms/d/1r0YS3i5h1SYxxrvsTiYiOmWiiv901H50yMv-2QObDu0/viewform");
			}
		}
#endif
	}

	const int fps = 120;
	private int ms_wait = 0;
	void Update()
	{
		// Throw exception here.
		if (e != null)
		{
			throw e;
		}
		
		if ((achievement_id < 0 || achievement_time >= achievement_display_time + achievement_hide_time) && achievement_ids.Count > 0)
		{
			achievement_id = achievement_ids.Dequeue();
			
			if (achievement_time < 0f)
				achievement_time = 0f;
			else if (achievement_time >= achievement_display_time + achievement_hide_time)
				achievement_time = achievement_hide_time - (achievement_time - achievement_hide_time - achievement_display_time);
		}
		
		if (achievement_id >= 0)
		{
			if (achievement_time >= achievement_display_time + achievement_hide_time + achievement_hide_time)
				achievement_id = -1;
			else achievement_time += Time.deltaTime;
		}

		if (Settings.limit_framerate)
		{
			float overflow = 1f / fps - Time.deltaTime;
			if (overflow > 0f)
				ms_wait++;
			else if (overflow < 0f)
				ms_wait--;
			Thread.Sleep(Mathf.RoundToInt(Mathf.Clamp(ms_wait, 0f, Mathf.FloorToInt(1f/fps * 1000))));
		}
	}
	#endregion
	
	#region MATH
	
	public static float Quadratic_Positive(float a, float b, float c)
	{
		if (b*b - 4*a*c < 0 || a == 0)
			throw new Exception("Illegal ABC");
		
		return (-b + Mathf.Sqrt(b*b - 4f*a*c)) / (2f * a);
	}
	
	public static float Quadratic_Negative(float a, float b, float c)
	{
		if (b*b - 4*a*c < 0 || a == 0)
			throw new Exception("Illegal ABC");
		
		return (-b - Mathf.Sqrt(b*b - 4f*a*c)) / (2f * a);
	}
	
	#endregion
}
