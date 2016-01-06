using UnityEngine;
using System.Collections;

public class Railgun : EmptyMagazineWeapon
{
	protected override void Start ()
	{
		base.Start ();
		
		_AmmoRegained += ReloadAllAmmoAtOnce;
	}
}
