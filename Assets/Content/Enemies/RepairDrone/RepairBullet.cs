using UnityEngine;
using System.Collections;

public class RepairBullet : TimedLifeBullet
{
	public float amt;
	public Enemy target;
	
	protected override void Start ()
	{
		base.Start ();
		
		Hit -= DamageOnHit;
		Hit += HealOnHit;
		Hit += DestroyOnHit;
		HitFilter += CheckTarget;
	}
	
	protected void HealOnHit(HittableObject target, Bullet bullet, RaycastHit rch, ref bool pierce)
	{
		Enemy e = target as Enemy;
		if (e == null) return;
		
		e.Heal(amt);
	}
	
	protected virtual void CheckTarget(RaycastHit rch, ref bool hit)
	{
		Transform c = rch.transform;
		Enemy h = c.GetComponent<Enemy>();
		while (h == null && c.parent != null)
		{
			c = c.parent;
			h = c.GetComponent<Enemy>();
		}
		
		if (h != target)
			hit = false;
	}
}
