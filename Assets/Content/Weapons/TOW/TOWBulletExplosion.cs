using UnityEngine;
using System.Collections;

public class TOWBulletExplosion : TimedLifeBullet
{
	public Transform target;
	public float angular_velocity;
	
	protected override void Start ()
	{
		base.Start ();
		
		Hit += DestroyOnHit;
		HitFilter += FilterNoHP;		
	}
	
	protected virtual void FilterNoHP(RaycastHit rch, ref bool hit)
	{
		Transform t = rch.transform;
		Enemy e = t.GetComponent<Enemy>();
		while (e == null && t.parent != null)
		{
			t = t.parent;
			e = t.GetComponent<Enemy>();
		}
		
		if (e == null || e.Health <= 0)
			hit = false;
	}
	
	protected override void _Update ()
	{
		if (alive > 0)
		{
			if (target == null)
				LocateTarget();
			
			if (target != null)
				transform.rotation = 
					Quaternion.RotateTowards(transform.rotation, 
					Quaternion.LookRotation(target.position - transform.position,
					Vector3.up), angular_velocity * Time.deltaTime);
		}
		
		base._Update ();
	}
	
	protected virtual void LocateTarget()
	{
		Transform t = null;

		Vector3 pos_m = transform.position + transform.forward * speed * duration * 0.5f;

		foreach (Squad s in Fight.f.squads)
		{
			foreach (Enemy e in s)
			{
				if (t == null ||
					Vector3.Distance(pos_m, e.transform.position)
					< Vector3.Distance(pos_m, t.position))
					t = e.transform;
			}
		}
		
		if (t != null)
			target = t;
	}
}
