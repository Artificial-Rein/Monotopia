using UnityEngine;
using System.Collections;

public class BattleshipEnemy : Enemy
{
	public Enemy[] turrets;
	
	public override void _Initialize ()
	{
		base._Initialize ();
		
		turrets = GetComponentsInChildren<Enemy>();
		squad.total_units += turrets.Length - 1;
		foreach (Enemy e in turrets)
			if (e != this)
			{
				squad.Add(e);
				Fight.f.NewEnemy(e);
			}
		
		health = turrets.Length - 1;
		
		squad.LoseSquadMember += LoseTurret;
		
		DestinationReached += NewDestination;
		NewDestination(this);
		
		dmgGeneric -= GotHit;
		dmgBullet += PassOnHit;
		dmgEnemy += PassOnHit;
		dmgNoData += DmgNoData;
	}
	
	protected virtual void PassOnHit(HittableObject h, HittableObject b)
	{
		Enemy target = null;
		foreach (Enemy e in turrets)
		{
			if (e == this) continue;
			if (e == null) continue;
			if (target == null || Vector3.Distance(target.transform.position, b.transform.position) >
				Vector3.Distance(e.transform.position, b.transform.position))
				target = e;
		}
		
		target.Damage(b);
	}
	
	protected virtual void PassOnHit(HittableObject h, Bullet b)
	{
		Enemy target = null;
		foreach (Enemy e in turrets)
		{
			if (e == this) continue;
			if (e == null) continue;
			if (target == null || Vector3.Distance(target.transform.position, b.transform.position) >
				Vector3.Distance(e.transform.position, b.transform.position))
				target = e;
		}
		
		if (target == null)
			DestroyInstant();
		else target.Damage(b);
	}
	
	protected virtual void DmgNoData(HittableObject h)
	{
		int r = Random.Range(0, turrets.Length);
		while (turrets[r] == this || turrets[r] == null)
		{
			r++;
			if (r == turrets.Length)
				r = 0;
		}
		
		turrets[r].Damage();
	}
	
	protected virtual void LoseTurret(Enemy e, Squad s)
	{
		GotHit(this);
	}
	
	protected virtual void NewDestination(Enemy e)
	{
		Destination = new Vector3(Random.Range(-125f, 125f), 0f, Random.Range(-175f, -75f));
	}
}
