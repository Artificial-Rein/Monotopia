using UnityEngine;
using System.Collections;

public class LoadSelectShip : MonoBehaviour
{
	public float t = 2.5f;
	
	public static float progress;
	
	AsyncOperation o;

	// Use this for initialization
	void Start ()
	{
		progress = 0.5f;
		
		o = Application.LoadLevelAsync("scene_select_ship");
		o.allowSceneActivation = true;
	}
	
	void Update()
	{
		//if (o == null) return;
		progress = o.progress;
	}
	
	void OnGUI()
	{
		Utilities.DrawLine(new Vector2(0f, Screen.height * 3 / 4),
			new Vector2(Mathf.Lerp(0f, Screen.width, progress), 
			Screen.height * 3 / 4), 20f, Color.white);
	}
}
