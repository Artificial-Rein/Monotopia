using UnityEngine;
using System.Collections;

public class VictoryScreen : ShidouGUIObject
{
	public float wait_time = 10f;
	public float min_time = 1f;
	float time;
	void Start()
	{
		time = 0f;
	}
	
	void Update()
	{
		time += Time.deltaTime;
		
		if (Input.anyKeyDown && time > min_time)
			Application.LoadLevel("load_main_menu");
	}
	
	protected override void _OnGUI ()
	{
		base._OnGUI ();
		
		GUI.Label(new Rect(Screen.width * 0.5f - 300f, Screen.height * 0.5f - 80f, 600f, 120f), 
			"The artificial intelligence system known as Kokkino has been destroyed.\n" +
			"Only yourself, Prasino, and Blay remain.");
		
		if (time >= wait_time)
			GUI.Label(new Rect(Screen.width * 0.5f - 150f, Screen.height * 0.5f + 40f, 300f, 60f), 
			"Press any key to return to main menu.");
	}
}
