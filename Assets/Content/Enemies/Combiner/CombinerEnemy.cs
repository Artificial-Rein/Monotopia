using UnityEngine;
using System.Collections;

public class CombinerEnemy : BasicFiringEnemy
{
	public bool focus;
	public bool ute;
	
	public int charges;
	
	public Bullet charging_shot, charged_shot;
	
	protected override void _Update ()
	{
		base._Update ();
		
		if (at_destination && ute)
		{
			bullet = focus ? charged_shot : charging_shot;
			Fire ();
		}
	}
	
	protected override void FireBullet (Enemy e)
	{
		Bullet b = InstantiateBullet();
		
		CombinerChargedShot c = b as CombinerChargedShot;
		if (c != null)
		{
			c.Charge(charges);
			charges = 0;
			
			(squad as CombinerSquad).Scatter();
		}
		
		CombinerChargingShot s = b as CombinerChargingShot;
		if (s != null)
		{
			foreach (CombinerEnemy ce in squad)
				if (ce.focus)
				{
					s.target = ce;
					break;
				}
		}
	}
	
	public virtual void CommenceFiring(Enemy en)
	{
		cooldown = sec_to_cooldown * 0.5f;
		
		foreach (CombinerEnemy e in squad)
			e.ute = true;
	}
}
