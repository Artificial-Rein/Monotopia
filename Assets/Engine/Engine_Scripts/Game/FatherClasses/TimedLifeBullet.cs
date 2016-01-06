using UnityEngine;
using System.Collections;

public class TimedLifeBullet : Bullet
{
	public float duration;
	
	protected override void _Update ()
	{
		base._Update ();
		
		if (alive > 0)
		{
			if (duration <= 0f)
				Explode();
			duration -= Time.deltaTime;
		}
	}
}
