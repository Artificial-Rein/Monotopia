using UnityEngine;
using System.Collections;
using System;

public class Bullet : ShidouGameObject
{
	public float speed;
	public float length;
	public float radius;
	
	public bool player;
	
	public bool kill_when_invis = true;
	protected float alive;
	
	public GameObject explosion;
	
	public delegate void HitEventHandler(HittableObject target, Bullet bullet, RaycastHit rch, ref bool pierce);
	public event HitEventHandler Hit;
	
	public delegate void HitFilterEventHandler(RaycastHit rch, ref bool hit);
	public event HitFilterEventHandler HitFilter;

	public AudioClip[] spawn_sounds;
	
	protected static void DamageOnHit(HittableObject target, Bullet bullet, RaycastHit rch, ref bool pierce)
	{
		target.Damage(bullet);
	}
	
	protected static void DestroyOnHit(HittableObject target, Bullet bullet, RaycastHit rch, ref bool pierce)
	{
		pierce = false;
		
		bullet.transform.position += Vector3.Project(rch.point - bullet.transform.position, bullet.transform.forward);
		
		bullet.Explode();
	}
	
	protected RaycastHit[] FilterHits(RaycastHit[] rch)
	{
		if (HitFilter == null) return rch;
		
		int num = 0;
		RaycastHit[] rch_up = new RaycastHit[rch.Length];
		for (int i = 0; i < rch.Length; i++)
		{
			bool cur = true;
			HitFilter(rch[i], ref cur);
			if (cur)
				rch_up[num++] = rch[i];
		}
		RaycastHit[] to_ret = new RaycastHit[num];
		for (int i = 0; i < num; i++)
			to_ret[i] = rch_up[i];
		return to_ret;
	}
	
	protected virtual void Start()
	{
		alive = 1;
		
		Hit += DamageOnHit;

		if (spawn_sounds != null && spawn_sounds.Length > 0 && !Settings.mute_music)
		{
			GameObject gobj = new GameObject(name + "_spawn_sfx");
			AudioSource ad = gobj.AddComponent<AudioSource>();
			ad.clip = spawn_sounds[UnityEngine.Random.Range(0, spawn_sounds.Length)];
			ad.loop = false;
			ad.maxDistance = 1000;
			ad.rolloffMode = AudioRolloffMode.Linear;
			ad.Play();

			Destroy(gobj, ad.clip.length + 0.1f);
		}
	}
	
	public virtual void Explode()
	{
		alive = 0;
		
		if (explosion != null)
		{
			GameObject exp = (GameObject)Instantiate(explosion);
			exp.transform.position = transform.position;
			exp.GetComponent<ExplosionController>().Init();
		}
	}
	
	protected virtual int CompareRCH (RaycastHit x, RaycastHit y)
	{
		return Mathf.RoundToInt(Vector3.Distance(transform.position, x.point) - Vector3.Distance(transform.position, y.point));
	}
	
	protected virtual RaycastHit[] CastForHits(Vector3 newPos)
	{
		Vector3 delta_pos = newPos - transform.position;
		
		RaycastHit[] rch = Physics.CapsuleCastAll(new Vector3(transform.position.x, -10f, transform.position.z), 
				new Vector3(transform.position.x, 10f, transform.position.z), radius, -transform.forward, speed * length - delta_pos.magnitude,
				1 << LayerMask.NameToLayer(player ? "enemy" : "player"));
		RaycastHit[] rch_secondary = Physics.CapsuleCastAll(new Vector3(transform.position.x, -10f, transform.position.z), 
				new Vector3(transform.position.x, 10f, transform.position.z), radius, transform.forward, delta_pos.magnitude,
				1 << LayerMask.NameToLayer(player ? "enemy" : "player"));
		RaycastHit[] rch_total = new RaycastHit[rch.Length + rch_secondary.Length];
		int i;
		for (i = 0; i < rch.Length; i++)
			rch_total[i] = rch[i];
		for (; i < rch.Length + rch_secondary.Length; i++)
			rch_total[i] = rch_secondary[i - rch.Length];
		Array.Sort(rch_total, CompareRCH);
		return FilterHits(rch_total);
	}
	
	// Update is called once per frame
	protected override void _Update ()
	{
		if (alive > 0)
		{
			// Cast for a hit and there is a hit,
			//	call our hit method and then theirs
			RaycastHit[] rch = CastForHits(transform.position + transform.forward * speed * Time.deltaTime);
			for (int i = 0; i < rch.Length; i++)
			{
				Transform c = rch[i].transform;
				HittableObject h = c.GetComponent<HittableObject>();
				while (h == null)
				{
					c = c.parent;
					h = c.GetComponent<HittableObject>();
				}
				
				bool cont = true;
				if (Hit != null)
					Hit(h, this, rch[i], ref cont);
				if (!cont) break;
			}
			
			transform.position += transform.forward * speed * Time.deltaTime;
		}
		else
		{
			alive -= Time.deltaTime;
			if (alive < -length)
				Destroy(gameObject);
		}
		
		if (kill_when_invis)
		{
			// Check for offscreen
			if (!(new Rect(-200f, -200f, 400f, 300f)).Contains(
				new Vector2(transform.position.x, transform.position.z)))
				Destroy(gameObject);
		}
	}
}
