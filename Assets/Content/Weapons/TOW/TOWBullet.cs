using UnityEngine;
using System.Collections;

public class TOWBullet : BasicDestroyOnCollideBullet
{
	public Bullet spray;
	public int num_shots;
	public float explosion_radius;
	public float degrees_offset = 45f;
	
	public override void Explode ()
	{
		if (alive > 0)
			for (int i = 0; i < num_shots; i++)
			{
				GameObject gobj = (GameObject)Instantiate(spray.gameObject);
				gobj.transform.position = transform.position;
				gobj.transform.Rotate(Vector3.up, i * 360f / num_shots + degrees_offset);
			}
		
		/*
		Collider[] rch = Physics.OverlapSphere(transform.position, explosion_radius,
			1 << (player ? LayerMask.NameToLayer("enemy") : 
			LayerMask.NameToLayer("player")));
		foreach (Collider r in rch)
		{
			Transform t = r.transform;
			HittableObject e = t.GetComponent<HittableObject>();
			
			while (e == null && t.parent != null)
			{
				t = t.parent;
				e = t.GetComponent<HittableObject>();
			}
			
			if (e != null)
			{
				e.Damage(this);
				if (Vector3.Distance(transform.position, e.transform.position) < explosion_radius * 0.5f)
					e.Damage(this);
			}
		}
		*/
		
		base.Explode ();
	}
}
