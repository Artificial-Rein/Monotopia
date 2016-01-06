using UnityEngine;
using System.Collections;

public class GoldenChariotChassis : Chassis
{
	protected override void _StartGame ()
	{
		base._StartGame ();

		for (int i = 0; i < Settings.won_with_which_weapons.Length; i++)
			if (Settings.won_with_which_weapons[i])
				GarageController.UnlockWeapon(i);
	}
}
