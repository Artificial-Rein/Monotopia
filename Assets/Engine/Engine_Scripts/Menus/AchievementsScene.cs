using UnityEngine;
using System.Collections;

public class AchievementsScene : ShidouGUIObject
{
	public Texture locked;
	
	Vector2 scroll_pos;
	
	float t, ret;
	float[] achieves_t;
	float[] tex_t;
	
	bool leaving;
	int wait_frames;
	
	void Start()
	{
		scroll_pos = Vector2.zero;
		
		t = 1f;
		ret = -1f;
		
		achieves_t = new float[Settings.achievements.Length];
		tex_t = new float[achieves_t.Length];
		for (int i = 0; i < achieves_t.Length; i++)
		{
			achieves_t[i] = -1f;
			tex_t[i] = -1f;
		}
		
		leaving = false;
		wait_frames = 2;
	}
	
	void Update()
	{
		if (wait_frames > 0)
		{
			wait_frames--;
			return;
		}
		
		if (t > -1f) t -= Time.deltaTime * TitleScene.line_speed;
		else if (t > -1000f)
		{
			for (int i = 0; i < achieves_t.Length; i++)
			{
				if (achieves_t[i] < 0f) achieves_t[i] = 0f;
				if (tex_t[i] < 0f) tex_t[i] = 0f;
			}
			t = -1001f;
		}
		
		if (!leaving)
		{
			if (ret >= 0f && ret < 1f) ret += Time.deltaTime * TitleScene.button_speed;
			
			for (int i = 0; i < achieves_t.Length; i++)
			{
				if (achieves_t[i] >= 0f && achieves_t[i] < 1f) achieves_t[i] += Time.deltaTime * TitleScene.button_speed;
				if (tex_t[i] >= 0f && tex_t[i] < 1f) tex_t[i] += Time.deltaTime * TitleScene.button_speed;
			}
		}
		else
		{
			ret -= Time.deltaTime * TitleScene.button_speed;
			for (int i = 0; i < achieves_t.Length; i++)
			{
				achieves_t[i] -= Time.deltaTime * TitleScene.button_speed;
				tex_t[i] -= Time.deltaTime * TitleScene.button_speed;
			}
		}
	}
	
	protected override void _OnGUI ()
	{	
		// Line spawner thingy from below
		float h_below = Mathf.Lerp(Screen.height, Screen.height / 2 - 300, 1f - t);
		if (t > 0f)
			Utilities.DrawLine(new Vector2(Screen.width / 2 - 200, Screen.height),
				new Vector2(Screen.width / 2 - 200, h_below),
				5f, TitleScene.line_color);
		else 
			Utilities.DrawLine(new Vector2(Screen.width / 2 - 200, Screen.height / 2 - 300),
				new Vector2(Screen.width / 2 - 200, Mathf.Lerp(Screen.height, Screen.height / 2 - 300, -t)),
				5f, TitleScene.line_color);
		
		// Line spawner thingy from above
		float h_above = Mathf.Lerp(0, Screen.height / 2 + 120, 1f - t);
		if (t > 0f)
			Utilities.DrawLine(new Vector2(Screen.width / 2 + 200, 0),
				new Vector2(Screen.width / 2 + 200, h_above),
				5f, TitleScene.line_color);
		else 
			Utilities.DrawLine(new Vector2(Screen.width / 2 + 200, Screen.height / 2 + 120),
				new Vector2(Screen.width / 2 + 200, Mathf.Lerp(0, Screen.height / 2 + 120, -t)),
				5f, TitleScene.line_color);
		
		// Achievements display
		scroll_pos = GUI.BeginScrollView(new Rect(Screen.width / 2 - 200, Screen.height / 2 - 300, 400, 420), 
			scroll_pos, new Rect(0, 0, 385, 128 * Settings.achievements.Length));
		
		int mod = 0;
		for (int i = 0; i < Settings.achievements.Length; i++)
			if (Settings.achievements[i].unlocked)
				DrawAchieve(i, mod++, h_below, h_above);
		for (int i = 0; i < Settings.achievements.Length; i++)
			if (!Settings.achievements[i].unlocked)
				DrawAchieve(i, mod++, h_below, h_above);
		
		GUI.EndScrollView();
		
		// Return to main menu button
		if (ret < 0f && h_below <= Screen.height / 2 + 180)
			ret = 0f;
		
		if (ret >= 1f)
		{
			if (GUI.Button(new Rect(Screen.width / 2 - 100, Screen.height / 2 + 120, 200, 60), "RETURN"))
				StartCoroutine(Leave("scene_main_menu"));
		}
		else if (ret > 0f)
			GUI.Box(new Rect(Screen.width / 2 - 100, Screen.height / 2 + 120, Mathf.Lerp(0f, 200f, ret), 60), "");
		
		GUI.DrawTexture(new Rect(Input.mousePosition.x, Screen.height - Input.mousePosition.y, ShidouGUIObject.cursor.width, ShidouGUIObject.cursor.height),
			ShidouGUIObject.cursor);
	}
	
	void DrawAchieve(int id, int pos, float h_below, float h_above)
	{
		Color text_color = GUI.color;
		if (!Settings.achievements[id].unlocked)
			GUI.color = new Color(0.5f, 0.5f, 0.5f);
		
		//Texture
		if (tex_t[id] < 0f && h_below <= Screen.height / 2 - 240 + 128 * pos)
			tex_t[id] = 0f;
		else if (tex_t[id] >= 1f)
			GUI.Label(new Rect(0, 128 * pos, 128, 128),
				Settings.achievements[id].unlocked ?
					Settings.achievements[id].icon :
					locked);
				//"");
		else if (tex_t[id] > 0f)
			GUI.Label (new Rect(0, 128 * pos, Mathf.Lerp(0, 128, tex_t[id]), 128), "");
		
		//Description & name
		if (achieves_t[id] < 0f && h_above >= Screen.height / 2 - 300 + 128 * pos)
			achieves_t[id] = 0f;
		else if (achieves_t[id] >= 1f)
			GUI.Label(new Rect(128, 128 * pos, 257, 128), Settings.achievements[id].name + "\n" + Settings.achievements[id].description);
		else if (achieves_t[id] > 0f)
		{
			float w = Mathf.Lerp (0, 257, achieves_t[id]);
			GUI.Label(new Rect(128 + 257 - w, 128 * pos, w, 128), "");
		}
		
		GUI.color = text_color;
	}
	
	IEnumerator Leave(string scene)
	{
		if (!leaving)
		{
			leaving = true;
			
			AsyncOperation o = Application.LoadLevelAdditiveAsync(scene);
			o.allowSceneActivation = false;
			
			while (ret > 0f)
				yield return null;
			
			yield return null;
			
			o.allowSceneActivation = true;
			Destroy(gameObject);
		}
	}
}
