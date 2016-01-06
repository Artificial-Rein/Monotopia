using UnityEngine;
using System.Collections;

public class PlayerController : ShidouGameObject
{
	Chassis player;

	bool fore_down, tur_down, abil_down;
	
	void Start()
	{
		GameObject ch = GameObject.FindGameObjectWithTag("Player");
		player = ch.GetComponent<Chassis>();

		fore_down = tur_down = abil_down = false;
	}
	
	protected override void _Update ()
	{
		if (!(Chassis.c != null && Chassis.c.alive))
			return;
		
		// Aim class 2
		if (player.sc_class_2 && player.turret != null)
		{
			// Aim like a boss
			Vector3 target;
			
			Ray scr = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit rch;
			if (Physics.Raycast(scr, out rch, 100f, 1 << LayerMask.NameToLayer("enemy")))
				target = rch.transform.position;
			else
			{
				float d;
				Plane floor = new Plane(Vector3.up, Chassis.c.turret_slot.position.y);
				floor.Raycast(scr, out d);
				target = scr.GetPoint(d);
			}
			
			target.y = Chassis.c.turret_slot.position.y;
			
			Chassis.c.turret_slot.rotation = Quaternion.LookRotation((Chassis.c.turret_slot.position - target).normalized, Chassis.c.turret_slot.up);
			
			// Fire class 2
			if (Settings.shoot_turret.Held)
			{
				if (!tur_down)
					player.turret.BeginFire();

				player.turret.Fire();

				tur_down = true;
			}
			else if (tur_down)
			{
				player.turret.ReleaseFire();

				tur_down = false;
			}
			if (Settings.shoot_turret.Down && !tur_down)
				player.turret.BeginFire();
			if (Settings.shoot_turret.Up && tur_down)
				player.turret.ReleaseFire();
		}
		
		// Move left
		// vv/2a + i = x
		if (Settings.left.Held && !Settings.right.Held && player.transform.position.x + player.speed * player.speed / (2 * player.accel) < 115)
			// Go go go
			player.Accelerate(player.accel);
		// Move right
		else if (Settings.right.Held && !Settings.left.Held && player.transform.position.x - player.speed * player.speed / (2 * player.accel) > -115)
			// Go go go
			player.Accelerate(-player.accel);
		else
			// Stop stop stop
			player.Accelerate(-player.speed / Time.deltaTime);
		
		// Fire class 1
		if (player.sc_class_1 && player.fore != null)
		{
			if (Settings.shoot_fore.Held)
			{
				if (!fore_down)
					player.fore.BeginFire();
				
				player.fore.Fire();
				
				fore_down = true;
			}
			else if (fore_down)
			{
				player.fore.ReleaseFire();
				
				fore_down = false;
			}
			if (Settings.shoot_fore.Down && !fore_down)
				player.fore.BeginFire();
			if (Settings.shoot_fore.Up && fore_down)
				player.fore.ReleaseFire();
		}
		
		// Ability, man
		if (player.ability)
		{
			
		}
	}
}
