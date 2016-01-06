using UnityEngine;
using System.Collections;

public class MachineGun : Weapon
{
	public float post_mortem_cooldown;
	public float artillery_cooldown;
	public int artillery_shots;
	public int artillery_shots_left;
	public Bullet artillery;
	public float sec_until_artillery_fire;
	
	protected override void Start ()
	{
		base.Start ();
		
		sec_until_artillery_fire = 0f;
		artillery_shots_left = artillery_shots;
	}
	
	protected override void _Update ()
	{
		base._Update ();
		
		if (sec_until_artillery_fire > 0f)
		{
			sec_until_artillery_fire -= Time.deltaTime;
			if (sec_until_artillery_fire > 0f)
				artillery_shots_left = 0;
			else artillery_shots_left = artillery_shots;
		}
	}
	
	public override void ReleaseFire ()
	{
		base.ReleaseFire ();
		
		sec_until_artillery_fire = post_mortem_cooldown;
	}
	
	public override void Fire ()
	{
		if (sec_until_artillery_fire <= 0f && artillery_shots_left > 0)
		{
			if (cooldown <= 0f)
				artillery_shots_left--;
			
			Bullet tb = bullet;
			bullet = artillery;
			float acc = maximum_delta_degrees;
			maximum_delta_degrees = 0f;
			
			if (artillery_shots_left > 0)
			{
				float tc = sec_to_cooldown;
				sec_to_cooldown = artillery_cooldown;
				
				base.Fire ();
				
				sec_to_cooldown = tc;
			}
			else base.Fire();
			
			bullet = tb;
			maximum_delta_degrees = acc;
		}
		else
		{
			sec_until_artillery_fire = post_mortem_cooldown;
			base.Fire ();
		}
	}
}
