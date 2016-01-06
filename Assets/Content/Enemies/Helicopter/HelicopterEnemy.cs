using UnityEngine;
using System.Collections;

public class HelicopterEnemy : BasicFiringEnemy
{
	public float prewait = 1f, postwait = 0.25f;
	public bool fired;
	public float speed_loss_mult = 0.5f;
	
	public override void _Initialize ()
	{
		base._Initialize ();
		
		DestinationReached += FireAtArrival;
		dmgGeneric += LoseSpeedOnHit;
		fired = true;
	}
	
	protected virtual void FireAtArrival(Enemy e)
	{
		StartCoroutine(WaitFireWait(prewait, postwait));
	}
	
	protected virtual IEnumerator WaitFireWait(float t1, float t2)
	{
		yield return new WaitForSeconds(t1);
		
		Fire();
		
		yield return new WaitForSeconds(t2);
		
		fired = true;
	}
	
	protected virtual void LoseSpeedOnHit(HittableObject e)
	{
		speed *= speed_loss_mult;
	}
}
