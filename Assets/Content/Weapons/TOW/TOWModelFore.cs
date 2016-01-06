using UnityEngine;
using System.Collections;

public class TOWModelFore : WeaponModel
{
	protected override void _Update ()
	{
		base._Update ();
		
		TOWWeapon t = owner as TOWWeapon;
		if (t != null && Fight.f != null && Fight.f.fight_active)
			anim.SetBool("charging", Settings.shoot_fore.Held);
	}
}
