using UnityEngine;
using System.Collections;

public class EmptyMagazineWeapon : Weapon
{
	public bool emptying_mag;
	
	protected override void Start ()
	{
		base.Start ();
		
		emptying_mag = false;
	}
	
	public override void Fire ()
	{
		if (cooldown <= 0f && ammo > 0)
			StartCoroutine(EmptyMagazineCoroutine());
	}
	
	IEnumerator EmptyMagazineCoroutine()
	{
		emptying_mag = true;
		
		while (ammo > 0)
		{
			while (cooldown > 0f)
				yield return null;
			base.Fire ();
		}
		
		emptying_mag = false;
	}
}
