using UnityEngine;
using System.Collections;

public class CombinerChargedShot : BasicDestroyOnCollideBullet
{
	public int charged;
	
	protected override void Start ()
	{
		base.Start ();
		
		Hit -= DamageOnHit;
		Hit += ChargedDamageOnHit;
	}
	
	public virtual void Charge(int amt)
	{
		// Increase size and radius and damage
		TrailRenderer r = GetComponent<TrailRenderer>();
		r.startWidth = 1f * amt + 1f;
		r.endWidth = r.startWidth;
		r.time = amt * 0.25f + 0.25f;
		
		radius = r.startWidth * 0.5f;
		length = r.time;
		
		charged = amt + 1;
	}
	
	protected virtual void ChargedDamageOnHit(HittableObject target, Bullet bullet, RaycastHit rch, ref bool pierce)
	{
		for (int i = 0; i < charged; i++)
			target.Damage(bullet);
	}
}
