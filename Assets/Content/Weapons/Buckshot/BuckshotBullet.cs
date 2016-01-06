using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BuckshotBullet : BasicDestroyOnCollideBullet
{
	public Stack<Transform> exception;
	public float max_degrees_delta;
	public int num_shots;
	public int depth;
	public float radius_base;
	public float speed_mod;
	
	protected override void Start ()
	{
		base.Start ();
		
		HitFilter += FilterException;
		Hit += Buckshot;
		
		transform.localScale = new Vector3(depth + 1, depth + 1, depth + 1);
		foreach (TrailRenderer tr in GetComponentsInChildren<TrailRenderer>())
		{
			tr.startWidth = radius_base * (depth + 1);
			radius = tr.startWidth * 0.5f;
		}
		
		speed += speed_mod;
	}
	
	protected virtual void Buckshot 
		(HittableObject target, Bullet bullet, RaycastHit rch, ref bool pierce)
	{
		if (depth <= 0)
			return;
		
		for (int i = 0; i < (num_shots == 0 ? depth + 1 : num_shots); i++)
		{
			GameObject gobj = (GameObject)Instantiate(gameObject);
			gobj.transform.position = transform.position;
			gobj.transform.rotation = transform.rotation;
			gobj.transform.Rotate(Vector3.up, 
				Random.Range (-max_degrees_delta, max_degrees_delta));
			BuckshotBullet bsb = gobj.GetComponent<BuckshotBullet>();
			bsb.exception = new Stack<Transform>();
			if (exception != null)
				foreach (Transform t in exception)
					bsb.exception.Push(t);
			bsb.exception.Push(target.transform);
			bsb.depth = depth - 1;
		}
	}
	
	protected virtual void FilterException(RaycastHit rch, ref bool hit)
	{
		if (exception == null)
			return;
		
		Transform t = rch.transform;
		while (t != null)
		{
			if (exception.Contains(t))
			{
				hit = false;
				return;
			}
			
			t = t.parent;
		}
	}
}
