using UnityEngine;
using System.Collections;

public class BasicFiringEnemy : Enemy
{
	public Bullet bullet;
	
	public Transform fire_loc;
	
	protected virtual void OnValidate()
	{
		if (fire_loc == null)
			fire_loc = transform;
	}
	
	protected virtual Bullet InstantiateBullet()
	{
		if (bullet == null) return null;
		if (fire_loc == null) return null;
		
		GameObject gobj = (GameObject)Instantiate(bullet.gameObject);
		
		gobj.transform.position = fire_loc.position;
		gobj.transform.rotation = fire_loc.rotation;
		
		return gobj.GetComponent<Bullet>();
	}
	
	protected virtual void FireBullet(Enemy e)
	{
		InstantiateBullet();
	}
	
	public override void _Initialize ()
	{
		base._Initialize ();
		
		_Fire += FireBullet;
	}
}
