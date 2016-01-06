using UnityEngine;
using System.Collections;

public class TurretSubEnemy : BasicFiringEnemy
{
	protected Quaternion orig_rot;
	
	static float cd_offset;
	
	public override void _Initialize ()
	{
		base._Initialize ();
		
		if (destination != null)
			Destroy(destination.gameObject);
		destination = transform;
		
		orig_rot = transform.localRotation;
		
		cooldown = cd_offset;
		cd_offset += sec_to_cooldown * Random.Range(0.0f, 0.25f);
		if (cd_offset > sec_to_cooldown)
			cd_offset = 0f;
	}
	
	protected override void _Update ()
	{
		base._Update ();
		
		if ((Chassis.c != null && Chassis.c.alive))
		{
			transform.rotation = Quaternion.LookRotation(Chassis.c.transform.position - transform.parent.position, transform.up);
			transform.localRotation = Quaternion.RotateTowards(orig_rot, transform.localRotation, 90f);
		}

		if (Utilities.IsThisVisible(gameObject))
			Fire ();
	}
}
