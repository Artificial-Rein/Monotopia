using UnityEngine;
using System.Collections;

public class CombinerChargingShot : BasicDestroyOnCollideBullet
{
	public CombinerEnemy target;
	public float lifespan;
	
	protected override void Start ()
	{
		base.Start ();
		
		Hit -= DamageOnHit;
		Hit += ChargeOnHit;
		
		HitFilter += CheckTarget;
	}
	
	protected virtual void CheckTarget (RaycastHit rch, ref bool hit)
	{
		Transform c = rch.transform;
		CombinerEnemy h = c.GetComponent<CombinerEnemy>();
		while (h == null && c.parent != null)
		{
			c = c.parent;
			h = c.GetComponent<CombinerEnemy>();
		}
		
		if (h != target)
			hit = false;
	}
	
	protected void ChargeOnHit(HittableObject target, Bullet bullet, RaycastHit rch, ref bool pierce)
	{
		CombinerEnemy c = target as CombinerEnemy;
		c.charges++;
	}
	
	protected override void _Update ()
	{
		base._Update ();
		
		if (alive > 0)
		{
			lifespan -= Time.deltaTime;
			if (lifespan < 0)
				Explode();
		}
	}
}
