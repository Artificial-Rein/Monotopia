using UnityEngine;
using System.Collections;

public class GunboatEnemy : BasicFiringEnemy
{
	protected bool firing;
	
	public float charge_time = 1f;
	public float fire_time = 1f;
	
	public Transform[] fire_slots;
	
	void InitializeGunboatFiring(Enemy e)
	{
		DestinationReached -= InitializeGunboatFiring;
		
		StartCoroutine(InitializeGunboatFiring());
	}
	
	IEnumerator InitializeGunboatFiring()
	{
		if (!firing)
		{
			firing = true;
			
			anim.SetBool("firing", true);
			
			anim.speed = 1f / charge_time;
			yield return new WaitForSeconds(charge_time);
			anim.SetBool("firing", false);
			anim.speed = 1f / fire_time;
			
			float fire_yield = fire_time;
			if (armor.Length > 2)
				fire_yield /= armor.Length - 1;
			
			for (int i = 0; i < armor.Length; i++)
			{
				if (fire_slots[i % fire_slots.Length].gameObject.activeInHierarchy)
				{
					fire_loc = fire_slots[i % fire_slots.Length];
					Fire();
				}
				yield return new WaitForSeconds(fire_yield);	
			}
			
			anim.speed = 1f / charge_time;
			yield return new WaitForSeconds(charge_time);
			
			MoveOn();
			
			firing = false;
		}
	}
	
	void MoveOn()
	{
		Destination = new Vector3(Random.Range(-100f, 100f), 0f, -125f);
		
		// Go to new location
		DestinationReached += NextStep;
	}
	
	void NextStep(Enemy e)
	{
		if (Destination.z > -100f)
		{
			destination.rotation = no_rotation;
			Destination = new Vector3(Destination.x, 0f, -50f);
			
			DestinationReached += InitializeGunboatFiring;
			DestinationReached -= NextStep;
		}
		else
		{
			Destination = new Vector3(Destination.x, 0f, -70f);
			destination.rotation = Quaternion.identity;
		}
	}
	
	public override void _Initialize ()
	{
		base._Initialize ();
		
		MoveOn();
	}
}
