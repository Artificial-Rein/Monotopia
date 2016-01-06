using UnityEngine;
using System.Collections;

public class EOCWeapon : EmptyMagazineWeapon
{
	public int additional_ammo_per_tick;
	public int max_additional_ammo;
	public float sec_to_charge_one_ammo;
	public float sec_charging;
	
	protected override void Start ()
	{
		base.Start ();
		
		sec_charging = 0f;
		
		_AmmoRegained += ReloadAllAmmoAtOnce;
	}
	
	public override void Fire ()
	{
		if (ammo > 0)
		{
			if (ammo < max_ammo + max_additional_ammo)
			{
				sec_charging += Time.deltaTime;
				while (sec_charging >= sec_to_charge_one_ammo)
				{
					sec_charging -= sec_to_charge_one_ammo;
					ammo += additional_ammo_per_tick;
				}
			}
			else ReleaseFire();
		}
	}
	
	public override void ReleaseFire ()
	{
		sec_charging = 0f;
		base.Fire();
		base.ReleaseFire ();
	}
}
