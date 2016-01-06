using UnityEngine;
using System.Collections;

public class BasicDestroyOnCollideBullet : Bullet
{
	protected override void Start ()
	{
		base.Start ();
		
		Hit += DestroyOnHit;
	}
}
