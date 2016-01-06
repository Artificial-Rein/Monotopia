using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShieldScaleControl : MonoBehaviour
{
	public struct Hit
	{
		public Vector3 normal;
		public float time;
	}
	
	public ShieldHexLeveler[] pieces;
	
	Queue<Hit> hit_q;
	
	public float hit_duration = 1f;
	float hit_mult;
	
	void Start()
	{
		pieces = GetComponentsInChildren<ShieldHexLeveler>();
		
		hit_q = new Queue<Hit>();
		hit_mult = 1f / hit_duration;
		
		Transform t = transform;
		HittableObject h;
		do
		{
			t = t.parent;
			h = t.GetComponent<HittableObject>();
		}
		while (h == null && t.parent != null);
		
		if (h == null)
			Destroy(gameObject);
		else
		{
			h.dmgNoData += DamageND;
			h.dmgBullet += DamageB;
			h.dmgEnemy += DamageE;
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		while (hit_q.Count != 0 && hit_q.Peek().time + hit_duration < Time.time)
			hit_q.Dequeue();
		
		foreach (ShieldHexLeveler h in pieces)
		{
			Vector3 normal = (h.render.transform.position - transform.position).normalized;
			
			float s = 0f;
			foreach (Hit hit in hit_q)
			{
				float s_ = (Vector3.Dot(normal, hit.normal) * 0.5f + 0.5f) * (1f - (Time.time - hit.time) / hit_duration);
				s += s_*s_;
			}
			
			h.Scale = s;
		}
		
		if (Input.GetKeyDown(KeyCode.Q))
			Damage(new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)));
	}
	
	void DamageND(HittableObject hit)
	{
		Damage(transform.forward);
	}
	
	void DamageB(HittableObject hit, Bullet b)
	{
		Damage(b.transform.position - transform.position);
	}
	
	void DamageE(HittableObject hit, HittableObject enemy)
	{
		Damage(enemy.transform.position - transform.position);
	}
	
	void Damage(Vector3 normal)
	{
		Hit h = new Hit();
		h.normal = normal.normalized;
		h.time = Time.time;
		
		hit_q.Enqueue(h);
	}
}
