using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Pierce : Bullet
{
	protected int destroyed;

	protected Stack<HittableObject> hit_already;
	
	protected override void Start ()
	{
		base.Start ();

		destroyed = 0;

		Hit += PierceAndDestroy;
		Hit -= DamageOnHit;
		
		hit_already = new Stack<HittableObject>();
	}
	
	protected void PierceAndDestroy(HittableObject target, Bullet bullet, RaycastHit rch, ref bool pierce)
	{
		if (!hit_already.Contains(target))
		{
			Explode ();
			alive = 1;

			Enemy e = target as Enemy;
			if (e != null && e.Health <= 1)
			{
				destroyed++;
				if (destroyed >= 5)
					Utilities.u.UnlockAchievement("Railgun");
			}

			DamageOnHit(target, bullet, rch, ref pierce);
			hit_already.Push (target);
		}
	}
}
