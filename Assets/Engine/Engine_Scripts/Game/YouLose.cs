using UnityEngine;
using System.Collections;

public class YouLose : ShidouGUIObject
{
	float t;
	
	void Start()
	{
		t = 0;
		
		Destroy(PauseMenu.p.gameObject);
		Utilities.paused = false;
	}
	
	void Update()
	{
		if (t < 1f)
			t += Time.deltaTime;
		if (t > 1f) t = 1f;
	}
	
	protected override void _OnGUI ()
	{
		base._OnGUI ();
		
		if (t >= 1f)
		{
			GUI.Label(new Rect(Screen.width / 2 - 150, Screen.height / 2 - 30, 300, 60), GarageController.selected_sector.weapon_unlock == -1 ? 
			          "YOU BLEW UP" : 
			          "THE " + GarageController.weapons[GarageController.selected_sector.weapon_unlock].weapon_name + " HAS BEEN LOST" );
			if (GUI.Button(new Rect(Screen.width / 2 - 100, Screen.height / 2 + 30, 200, 60), "MAIN MENU"))
			{
				Destroy(Chassis.c.gameObject);
				Application.LoadLevel("load_main_menu");
			}
			if (GUI.Button(new Rect(Screen.width / 2 - 100, Screen.height / 2 + 90, 200, 60), "TRY AGAIN"))
			{
				Chassis.c.gameObject.SetActive(true);
				Chassis.c.transform.position = new Vector3(0f, 0f, 0f);
				Chassis.c.alive = true;
				Chassis.c.Reset();

				GarageController.selected_sector.weapon_unlock = -1;

				GameObject fight = (GameObject)Instantiate(GarageController.selected_sector.fight.gameObject);
			}
		}
		else
		{
			GUI.Label(new Rect(Screen.width / 2 - 150, Screen.height / 2 - 30, Mathf.Lerp(0f, 300f, t), 60), "");
			GUI.Label(new Rect(Screen.width / 2 - 100 + Mathf.Lerp(200f, 0f, t), Screen.height / 2 + 30, Mathf.Lerp(0f, 200f, t), 60), "");
			GUI.Label(new Rect(Screen.width / 2 - 100, Screen.height / 2 + 90, Mathf.Lerp(0f, 200f, t), 60), "");
		}
		
		GUI.DrawTexture(new Rect(Input.mousePosition.x, Screen.height - Input.mousePosition.y, cursor.width, cursor.height),
				cursor);
	}
}
