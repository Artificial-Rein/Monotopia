using UnityEngine;
using System.Collections;

public class SettingsScene : ShidouGUIObject
{	
	public Texture check, uncheck;
	
	public GUIStyle key_selection;
	public GUIStyle key_selected;

	public string next_scene = "scene_main_menu";
	
	Settings.KeyControl requested;
	bool requested_primary;
	
	int key_control_id;
	int wait_frames = 2;
	
	float t, t_bottom, fs, ret, quality, restart, camera_pos, mute_pos, frames;
	bool leaving;
	
	void Start()
	{	
		requested = null;
		t_bottom = t = 1f;
		fs = ret = quality = restart = camera_pos = mute_pos = frames = -1f;
	}
	
	void Update()
	{
		if (wait_frames > 0)
		{
			wait_frames--;
			return;
		}
		
		if (requested != null)
		{
			for (int i = 0; i < 512; i++)
			{
				if (Input.GetKeyDown((KeyCode)i))
				{
					if (requested_primary)
						requested.primary = (KeyCode)i;
					else requested.secondary = (KeyCode)i;
					requested = null;
					Settings.SaveSettings();
					break;
				}
			}
		}
		
		if (t > -1f && !leaving) t -= Time.deltaTime * TitleScene.line_speed / 2f;
		else if (leaving && t < 1f)
		{
			if (t < 0f) t = 0f;
			t += Time.deltaTime * TitleScene.line_speed / 2f;
		}
		
		if (t_bottom > -1f) t_bottom -= Time.deltaTime * TitleScene.line_speed * 2f;
		
		if (!leaving)
		{
			if (ret >= 0f && ret < 1f) ret += Time.deltaTime * TitleScene.button_speed;
			if (fs >= 0f && fs < 1f) fs += Time.deltaTime * TitleScene.button_speed;
			if (quality >= 0f && quality < 1f) quality += Time.deltaTime * TitleScene.button_speed;
			if (restart >= 0f && restart < 1f) restart += Time.deltaTime * TitleScene.button_speed;
			if (camera_pos >= 0f && camera_pos < 1f) camera_pos += Time.deltaTime * TitleScene.button_speed;
			if (mute_pos >= 0f && mute_pos < 1f) mute_pos += Time.deltaTime * TitleScene.button_speed;
			if (frames >= 0f && frames < 1f) frames += Time.deltaTime * TitleScene.button_speed;
		}
		else
		{
			ret -= Time.deltaTime * TitleScene.button_speed;
			fs -= Time.deltaTime * TitleScene.button_speed;
			quality -= Time.deltaTime * TitleScene.button_speed;
			restart -= Time.deltaTime * TitleScene.button_speed;
			camera_pos -= Time.deltaTime * TitleScene.button_speed;
			mute_pos -= Time.deltaTime * TitleScene.button_speed;
			frames -= Time.deltaTime * TitleScene.button_speed;
		}
	}
	
	protected override void _OnGUI()
	{
		// Line spawner thingy
		float h = Mathf.Lerp(Screen.height, Screen.height / 2 + 120 - check.height * 4, 1f - t_bottom);
		if (t_bottom > 0f)
			Utilities.DrawLine(new Vector2(Screen.width / 2 - 100, Screen.height),
				new Vector2(Screen.width / 2 - 100, Mathf.Lerp(Screen.height, Screen.height / 2 + 120 - check.height * 4, 1f - t_bottom)),
				5f, TitleScene.line_color);
		else 
			Utilities.DrawLine(new Vector2(Screen.width / 2 - 100, Screen.height / 2 + 120 - check.height * 4),
				new Vector2(Screen.width / 2 - 100, Mathf.Lerp(Screen.height, Screen.height / 2 + 120 - check.height * 4, -t_bottom)),
				5f, TitleScene.line_color);
		
		// Fullscreen checkbox
		if (fs < 0f && h <= Screen.height / 2 + 120)
			fs = 0f;
		
		if (fs >= 1f)
		{
			if (GUI.Button(new Rect(Screen.width / 2 - 100, Screen.height / 2 + 120 - check.height, 200, check.height), ""))
			{
				if (Settings.fullscreen)
				{
					Screen.SetResolution(1280, 720, false);
				}
				else
				{
					Screen.SetResolution(Screen.resolutions[Screen.resolutions.Length - 1].width, 
						Screen.resolutions[Screen.resolutions.Length - 1].height, true);
				}
				
				Settings.fullscreen = !Settings.fullscreen;
				
				Settings.SaveSettings();
			}
			GUI.Label(new Rect(Screen.width / 2 - 80 + check.width, Screen.height / 2 + 120 - check.height, 150, check.height), "Fullscreen");
			GUI.Label(new Rect(Screen.width / 2 - 80, Screen.height / 2 + 120 - check.height, check.width, check.height),  Screen.fullScreen ? check : uncheck);
		}
		else if (fs > 0f)
			GUI.Box(new Rect(Screen.width / 2 - 100, Screen.height / 2 + 120 - check.height, Mathf.Lerp(0f, 200f, fs), check.height), "");
		
		// Camera movement checkbox
		if (camera_pos < 0f && h <= Screen.height / 2 + 120 - check.height)
			camera_pos = 0f;
		
		if (camera_pos >= 1f)
		{
			if (GUI.Button(new Rect(Screen.width / 2 - 100, Screen.height / 2 + 120 - check.height * 3, 200, check.height * 2), ""))
			{
				Settings.camera_movements = !Settings.camera_movements;
				
				Settings.SaveSettings();
			}
			GUI.Label(new Rect(Screen.width / 2 - 80 + check.width, Screen.height / 2 + 120 - check.height * 3, 150, check.height * 2), "Camera Movement");
			GUI.Label(new Rect(Screen.width / 2 - 80, Screen.height / 2 + 120 - check.height * 2.5f, check.width, check.height),  Settings.camera_movements ? check : uncheck);
		}
		else if (camera_pos > 0f)
			GUI.Box(new Rect(Screen.width / 2 - 100, Screen.height / 2 + 120 - check.height * 3, Mathf.Lerp(0f, 200f, camera_pos), check.height * 2), "");

		// Mute checkbox
		if (mute_pos < 0f && h <= Screen.height / 2 + 120 + check.height)
			mute_pos = 0f;
		
		if (mute_pos >= 1f)
		{
			if (GUI.Button(new Rect(Screen.width / 2 - 100, Screen.height / 2 + 120, 200, check.height), ""))
			{
				Settings.mute_music = !Settings.mute_music;
				
				if (Fight.f != null && Fight.f.fight_active && Fight.f.aud != null)
				{
					if (Settings.mute_music)
						Fight.f.aud.Pause();
					else
						Fight.f.aud.Play ();
				}
				
				Settings.SaveSettings();
			}
			GUI.Label(new Rect(Screen.width / 2 - 80 + check.width, Screen.height / 2 + 120, 150, check.height), "Mute");
			GUI.Label(new Rect(Screen.width / 2 - 80, Screen.height / 2 + 120, check.width, check.height),  Settings.mute_music ? check : uncheck);
		}
		else if (mute_pos > 0f)
			GUI.Box(new Rect(Screen.width / 2 - 100, Screen.height / 2 + 120, Mathf.Lerp(0f, 200f, mute_pos), check.height), "");

		// Throttle checkbox
		if (frames < 0f && h <= Screen.height / 2 + 120 + check.height * 2)
			frames = 0f;
		
		if (frames >= 1f)
		{
			if (GUI.Button(new Rect(Screen.width / 2 - 100, Screen.height / 2 + 120 + check.height, 200, check.height), ""))
			{
				Settings.limit_framerate = !Settings.limit_framerate;
				
				Settings.SaveSettings();
			}
			GUI.Label(new Rect(Screen.width / 2 - 80 + check.width, Screen.height / 2 + 120 + check.height, 150, check.height), "Limit FPS");
			GUI.Label(new Rect(Screen.width / 2 - 80, Screen.height / 2 + 120 + check.height, check.width, check.height),  Settings.limit_framerate ? check : uncheck);
		}
		else if (frames > 0f)
			GUI.Box(new Rect(Screen.width / 2 - 100, Screen.height / 2 + 120 + check.height, Mathf.Lerp(0f, 200f, frames), check.height), "");
		
		// Quality selector
		if (quality < 0f && h <= Screen.height / 2 + 120 - check.height * 3)
			quality = 0f;
		if (quality >= 1f)
		{
			string[] strings = {"low", "", "", "", "high"};
			int q = Settings.quality;
			Settings.quality = GUI.Toolbar(
				new Rect(Screen.width / 2 - 167, Screen.height / 2 + 120 - check.height * 4, 334f, check.height), 
				Settings.quality, strings);
			if (Settings.quality != q)
				Settings.SaveSettings();
			
			if (restart < 0f && (Settings.quality == 0) != Settings.low_poly)
				restart = 0f;
		}
		else if (quality > 0f)
			GUI.Toolbar(
				new Rect(Screen.width / 2 - Mathf.Lerp(100f, 167f, quality), Screen.height / 2 + 120 - check.height * 4, 
					Mathf.Lerp(0f, 334f, quality), check.height), 
				Settings.quality, new string[5]);
		
		
		// Return to main menu button
		if (ret < 0f && h <= Screen.height / 2 + 250)
			ret = 0f;
		
		if (ret >= 1f)
		{
			if (GUI.Button(new Rect(Screen.width / 2 - 100, Screen.height / 2 + 190, 200, 60), "RETURN"))
				StartCoroutine(Leave("scene_main_menu"));
		}
		else if (ret > 0f)
			GUI.Box(new Rect(Screen.width / 2 - 100, Screen.height / 2 + 190, Mathf.Lerp(0f, 200f, ret), 60), "");
		
		// Key setup
		key_control_id = 1;
		KeySelector(
			new Rect(Screen.width / 2 - 200, Screen.height / 2 - 150, 400, 30), 
			Settings.shoot_fore);
		KeySelector(
			new Rect(Screen.width / 2 - 200, Screen.height / 2 - 120, 400, 30), 
			Settings.left);
		KeySelector(
			new Rect(Screen.width / 2 - 200, Screen.height / 2 - 90, 400, 30), 
			Settings.right);
		
		// Draw cursor
		GUI.DrawTexture(new Rect(Input.mousePosition.x, Screen.height - Input.mousePosition.y, cursor.width, cursor.height), cursor);
	}
	
	void KeySelector(Rect r, Settings.KeyControl k)
	{
		Rect label = new Rect(r.x, r.y, r.width / 3, r.height);
		Rect primary = new Rect(r.x + label.width, r.y, r.width / 3, r.height);
		Rect secondary = new Rect(primary.x + primary.width, r.y, 
			r.width / 3, r.height);
		
		bool l = true, p = true, s = true;
		
		// Line!
		Vector2 line_end = new Vector2(0, label.y);
		if (t > 0f)
		{
			if (key_control_id % 2 == 0)
			{
				line_end.x = Mathf.Lerp(Screen.width, label.x, 1f - t);
				if (!leaving) Utilities.DrawLine(new Vector2(Screen.width, label.y), line_end, 5f, TitleScene.line_color);
			}
			else
			{
				line_end.x = Mathf.Lerp(0, secondary.x + secondary.width, 1f - t);
				if (!leaving) Utilities.DrawLine(new Vector2(0f, label.y), line_end, 5f, TitleScene.line_color);
			}
		}
		else if (t > -1f)
		{
			if (key_control_id % 2 == 0)
			{
				line_end.x = Mathf.Lerp(Screen.width, label.x, -t);
				if (!leaving) Utilities.DrawLine(new Vector2(label.x, label.y), line_end, 5f, TitleScene.line_color);
			}
			else
			{
				line_end.x = Mathf.Lerp(0, secondary.x + secondary.width, -t);
				if (!leaving) Utilities.DrawLine(new Vector2(secondary.x + secondary.width, label.y), line_end, 5f, TitleScene.line_color);
			}
		}
		key_control_id++;
		
		// Restrict them rects!
		if (t > 0f)
		{
			if (key_control_id % 2 == 0)
			{
				float w = Mathf.Max (0, Mathf.Min(label.width, line_end.x - label.x));
				l = w >= label.width;
				label.width = w;
				
				w = Mathf.Max (0, Mathf.Min(primary.width, line_end.x - primary.x));
				p = w >= primary.width;
				primary.width = w;
				
				w = Mathf.Max (0, Mathf.Min(secondary.width, line_end.x - secondary.x));
				s = w >= secondary.width;
				secondary.width = w;
			}
			else
			{
				float w = Mathf.Max(0, Mathf.Min (secondary.width, secondary.x + secondary.width - line_end.x));
				s = w >= secondary.width;
				secondary.x += secondary.width - w;
				secondary.width = w;
				
				w = Mathf.Max(0, Mathf.Min (primary.width, primary.x + primary.width - line_end.x));
				p = w >= primary.width;
				primary.x += primary.width - w;
				primary.width = w;
				
				w = Mathf.Max(0, Mathf.Min (label.width, label.x + label.width - line_end.x));
				l = w >= label.width;
				label.x += label.width - w;
				label.width = w;
			}
		}
		
		if (label.width > 0) GUI.Label(label, l ? k.name : "", key_selection);
		if (requested == null)
		{
			if (p)
			{
				if (GUI.Button(primary, k.primary.ToString(), key_selection))
				{
					requested = k;
					requested_primary = true;
				}
			}
			else
				if (primary.width > 0) GUI.Box(primary, "");
			
			
			if (s)
			{
				if (GUI.Button(secondary, k.secondary.ToString(), key_selection))
				{
					requested = k;
					requested_primary = false;
				}
			}
			else
				if (secondary.width > 0) GUI.Box (secondary, "");
		}
		else
		{
			GUI.Label(primary, 
				(k == requested && requested_primary) ? "<Press any key>" : k.primary.ToString(), 
				(k == requested && requested_primary) ? key_selected : key_selection);
			GUI.Label(secondary, 
				(k == requested && !requested_primary) ? "<Press any key>" : k.secondary.ToString(),
				(k == requested && !requested_primary) ? key_selected : key_selection);
		}
	}
	
	IEnumerator Leave(string scene)
	{
		if (!leaving)
		{
			leaving = true;

			AsyncOperation o = null;
			if (!next_scene.Equals(""))
			{
				o = Application.LoadLevelAdditiveAsync(scene);
				o.allowSceneActivation = false;
			}
			
			while (t < 0.75f)
				yield return null;

			if (o != null)
				o.allowSceneActivation = true;
			Destroy(gameObject);
		}
	}
}
