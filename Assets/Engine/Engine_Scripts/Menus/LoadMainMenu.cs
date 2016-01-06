using UnityEngine;
using System.Collections;
using System.IO;
using System.Net;
using System;

public class LoadMainMenu : MonoBehaviour
{
	public float t = 2.5f;
	public GUISkin skin;
	public Texture cursor, aimer;
	
	public static float progress;
	public static bool new_version = false;
	float local_progress;
	
	AsyncOperation o;

	// Use this for initialization
	void Start ()
	{
		new_version = false;

#if UNITY_WEBPLAYER
#else
		try
		{
			int lv, lsv, lb,
				cv, csv, cb;

			using (StreamReader sr = new StreamReader("v.data"))
			{
				string[] s = sr.ReadLine().Split('.');

				lv = int.Parse(s[0]);
				lsv = int.Parse (s[1]);
				lb = int.Parse (s[2]);

				sr.Close();
			}

			using (WebClient wc = new WebClient())
			{
				string[] s = wc.DownloadString("http://artificialilliteracy.com/cloudver.data").Split('.');

				cv = int.Parse(s[0]);
				csv = int.Parse(s[1]);
				cb = int.Parse(s[2]);
			}

			if (lv != cv || lsv != csv || lb != cb)
				new_version = true;
		}
		catch (Exception e)
		{

		}
#endif

		if (Fight.f != null)
			Destroy(Fight.f);
		if (Map.m != null)
			Destroy(Map.m);
		DontDestroyOnLoad(gameObject);
		
		progress = 0f;
		local_progress = 0f;
		
		Screen.showCursor = false;
		ShidouGUIObject.skin = skin;
		StartCoroutine(Load());
		
		o = Application.LoadLevelAdditiveAsync("load_main_menu");
		o.allowSceneActivation = false;
	}
	
	IEnumerator Load()
	{
		Settings.LoadSettings();
		local_progress = 0.2f;
		
		yield return null;
		
		if (Settings.fullscreen)
			Screen.SetResolution(Screen.resolutions[Screen.resolutions.Length - 1].width, 
				Screen.resolutions[Screen.resolutions.Length - 1].height, true);
		
		local_progress = 0.5f;
		
		while (TextureColorShifter.need_set)
			yield return null;
		
		yield return null;
		
		local_progress = 0.9f;
		
		yield return null;
		
		ShidouGUIObject.cursor = cursor;
		ShidouGUIObject.aimer = aimer;
		
		local_progress = 1f;
	}
	
	void Update()
	{
		//if (o == null) return;
		if (!o.isDone)
			progress = local_progress / 10f + o.progress * 90f / 100f;
		else progress = 1f;
		if (local_progress >= 1f)
			o.allowSceneActivation = true;
		if (o.isDone && local_progress >= 1f)
		{
			camera.enabled = false;

			StartCoroutine(fadeOut());
		}
	}

	IEnumerator fadeOut()
	{
		local_progress = 0f;

		while (guiTexture.color.a > 0f)
		{
			guiTexture.color = new Color (guiTexture.color.r, guiTexture.color.g, guiTexture.color.b, 
			                              guiTexture.color.a - Time.deltaTime);
			yield return null;
		}

		Destroy(gameObject);
	}

	void OnGUI()
	{
		Utilities.DrawLine(new Vector2(0f, Screen.height * 0.5f),
			new Vector2(Mathf.Lerp(0f, Screen.width, progress), 
			Screen.height * 0.5f), 20f, Color.white);
	}
}
