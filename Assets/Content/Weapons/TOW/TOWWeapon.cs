using UnityEngine;
using System.Collections;

public class TOWWeapon : Weapon
{
	protected Bullet last_shot;
	
	public override void ReleaseFire ()
	{
		base.ReleaseFire ();
		
		if (last_shot != null)
			last_shot.Explode();
	}
	
	public override void BeginFire ()
	{
		base.BeginFire ();
		base.Fire();
	}
	
	public override void Fire () { }
	
	protected override Bullet InstantiateBullet ()
	{
		last_shot = base.InstantiateBullet ();
		return last_shot;
	}
}
