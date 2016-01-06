using UnityEngine;
using System.Collections;

public class OpenMainMenu : MonoBehaviour
{
	// Use this for initialization
	void Start ()
	{
		Utilities.u.UnlockAchievement("Power On");
		
		Application.LoadLevelAdditiveAsync("scene_main_menu");
		Destroy(gameObject);
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
}
