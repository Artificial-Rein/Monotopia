using UnityEngine;
using System.Collections;

public class ScatterShotWeapon : EmptyMagazineWeapon
{
	public float max_charge;
	public float charge_per_second;
	
	public float charge;
	protected int cone_id;
	
	protected override void Start ()
	{
		base.Start ();
		
		cone_id = 0;
	}
	
	public override void Fire ()
	{
		if (cooldown <= 0f && ammo > 0)
		{
			charge += charge_per_second * Time.deltaTime;
			if (charge > max_charge)
				charge = max_charge;
		}
	}
	
	public override void ReleaseFire ()
	{
		float spray = maximum_delta_degrees;
		maximum_delta_degrees = spray * (1f - charge);
		charge = 0f;
		base.Fire();
		maximum_delta_degrees = spray;
	}
	
	protected override Bullet InstantiateBullet ()
	{
		Bullet b = base.InstantiateBullet ();
		
		b.transform.rotation = model.barrel.rotation;
		b.transform.Rotate(Vector3.up, -maximum_delta_degrees + 
			cone_id * (maximum_delta_degrees * 2) / max_ammo +
			Random.Range(0f, maximum_delta_degrees * 2 / max_ammo));
		
		cone_id++;
		if (cone_id >= max_ammo) cone_id = 0;
		
		return b;
	}
}
