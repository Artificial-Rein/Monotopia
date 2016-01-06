using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HittableObject : ShidouGameObject
{
	public delegate void BulletDamageEventHandler(HittableObject hit, Bullet bullet);
	public event BulletDamageEventHandler dmgBullet;
	
	public delegate void CollisionDamageEventHandler(HittableObject hit, HittableObject enemy);
	public event CollisionDamageEventHandler dmgEnemy;
	
	public delegate void GenericDamageEventHandler(HittableObject hit);
	public event GenericDamageEventHandler dmgGeneric;
	public event GenericDamageEventHandler dmgNoData;
	
	public delegate void CollideEventHandler(HittableObject away, HittableObject home);
	public event CollideEventHandler Collide;
	
	public virtual void Damage()
	{
		if (!Utilities.IsThisVisible(gameObject))
			return;
		
		if (dmgNoData != null)
			dmgNoData(this);
		if (dmgGeneric != null)
			dmgGeneric(this);
	}
	
	public virtual void Damage(Bullet b)
	{
		if (!Utilities.IsThisVisible(gameObject))
			return;
		
		if (dmgBullet != null)
			dmgBullet(this, b);
		if (dmgGeneric != null)
			dmgGeneric(this);
	}
	
	public virtual void Damage(HittableObject h)
	{
		if (!Utilities.IsThisVisible(gameObject))
			return;
		
		if (dmgEnemy != null)
			dmgEnemy(this, h);
		if (dmgGeneric != null)
			dmgGeneric(this);
	}
	
	protected virtual void OnTriggerEnter(Collider c)
	{
		HittableObject away;
		Transform t = c.transform;
		away = t.GetComponentInChildren<HittableObject>();
		while (away == null && t.parent != null)
		{
			t = t.parent;
			away = t.GetComponent<HittableObject>();
		}
		
		if (away != null)
		{
			//Debug.Log("Fire the baby cannon!", this);
			
			if (Collide != null)
				Collide(away, this);
		}
	}
}
