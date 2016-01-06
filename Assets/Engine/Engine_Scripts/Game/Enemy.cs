using UnityEngine;
using System.Collections;
using System;

public class Enemy : HittableObject
{
	public Quaternion no_rotation = Quaternion.LookRotation(Vector3.up);
	
	public float speed;
	public float angular_speed;
	public bool strafing = false;
	public bool repairable = true;
	
	public float Speed
	{
		get { return speed; }
		set
		{
			speed = value;
			float circumference = 360f / angular_speed * speed;
			turn_radius = circumference / (2 * Mathf.PI);
		}
	}
	
	public float AngularSpeed
	{
		get { return angular_speed; }
		set
		{
			angular_speed = value;
			float circumference = 360f / angular_speed * speed;
			turn_radius = circumference / (2 * Mathf.PI);
		}
	}
	
	float turn_radius;
	
	protected bool at_destination;
	public Transform destination;
	public Vector3 Destination
	{
		get { return destination.position; }
		set
		{
			destination.position = value;
			
			if (strafing) return;
			
			float circumference = 360f / angular_speed * speed;
			turn_radius = circumference / (2 * Mathf.PI);
			
			float radius = turn_radius;
			Vector2 normal = 
				new Vector2(-transform.forward.z, 
					transform.forward.x).normalized;
			Vector2 pos = new Vector2(transform.position.x,
				transform.position.z);
			Vector2 secant = 
				new Vector2(value.x - transform.position.x,
					value.z - transform.position.z).normalized;
			if (Vector2.Dot(normal, secant) < Vector2.Dot(-normal, secant))
				normal = -normal;
			
			Vector2 center = normal * radius
				+ pos;
			
			//Visual
			for (int i = 0; i < 32; i++)
			{
				Debug.DrawLine(new Vector3(radius * Mathf.Cos(i * Mathf.PI / 16f) +
					center.x, 0f, radius * Mathf.Sin(i * Mathf.PI / 16f) + center.y),
					new Vector3(radius * Mathf.Cos((i + 1) * Mathf.PI / 16f) +
					center.x, 0f, radius * Mathf.Sin((i + 1) * Mathf.PI / 16f) + center.y),
					Color.red, 5f);
			}
			
			if (value.x == transform.position.x)
			{
				value.x += 0.5f;
				Debug.Log("Fixed it.");
			}
			float m =  secant.y / secant.x;
			float b =  m * -transform.position.x + transform.position.z;
			
			Debug.DrawLine(new Vector3(-100f, 0f, m*(-100f) + b),
				new Vector3(100f, 0f, m*100f + b), Color.blue, 5f);
			
			try
			{
				float quad_a = (1f + m*m);
				float quad_b = (-2f*center.x + 2f*m*b - 2f*m*center.y);
				float quad_c = (center.x * center.x + b*b - 2f*b*center.y + center.y * center.y - radius * radius);
				
				float x_1 = Utilities.Quadratic_Positive(quad_a, quad_b, quad_c);
				
				Debug.DrawRay(new Vector3(x_1, 0f, m * x_1 + b), Vector3.up, Color.white, 5f);
				
				float x_2 = Utilities.Quadratic_Negative(quad_a, quad_b, quad_c);
				
				Debug.DrawRay(new Vector3(x_2, 0f, m * x_2 + b), Vector3.up, Color.black, 5f);
				
				Vector3 alt_dest = value;
				if (Vector2.Distance(new Vector2(x_1, m * x_1 + b), pos) <
					Vector2.Distance(new Vector2(x_2, m * x_2 + b), pos))
				{
					alt_dest.x = x_2;
					alt_dest.z = x_2 * m + b;
				}
				else
				{
					alt_dest.x = x_1;
					alt_dest.z = x_1 * m + b;
				}
				
				if (Vector3.Distance(alt_dest, transform.position) > Vector3.Distance(value, transform.position))
					destination.position = alt_dest;
			}
			catch (Exception e) {}

			destination.position = new Vector3(Mathf.Clamp(destination.position.x, -200f, 200f), 
				destination.position.y, Mathf.Clamp(destination.position.z, -250f, 150f));
		}
	}
	public delegate void DestinationReachedEventHandler(Enemy enemy);
	public event DestinationReachedEventHandler DestinationReached;
	public event DestinationReachedEventHandler AtDestination;
	
	public delegate void FireEventHandler(Enemy firing);
	public event FireEventHandler _Fire;
	
	public delegate void DeathEventHandler(Enemy dying);
	public event DeathEventHandler Death;
	
	public float sec_to_cooldown;
	protected float cooldown;
	
	public Squad squad;
	
	public int max_health;
	public int Health { get { return health; } }
	protected int health;
	protected float repair_charge;
	
	public Animator anim;
	
	public GameObject[] armor;
	
	public GameObject armor_explosion, main_explosion;
	
	bool initialized = false;
	
	protected virtual void Start()
	{
		if (!initialized)
			_Initialize();
	}
	
	public virtual void _Initialize()
	{
		initialized = true;
		
		CreateNewDestinationObj();
		
		dmgGeneric += GotHit;
		Collide += Collided;
		
		health = max_health;
	}
	
	protected void CreateNewDestinationObj()
	{
		if (destination != null)
			Destroy(destination);
		
		GameObject dest_gobj = new GameObject(name + "_destination");
		destination = dest_gobj.transform;
		destination.position = transform.position;
		destination.rotation = no_rotation;
	}
	
	public virtual void DestroyInstant()
	{
		if (Death != null)
			Death(this);
		
		// Make explosion
		if (main_explosion != null)
		{
			GameObject exp = (GameObject)Instantiate(main_explosion);
			exp.transform.position = transform.position;
			exp.GetComponent<ExplosionController>().Init();
		}
		Destroy(gameObject);
		Destroy(destination.gameObject);
	}
	
	protected virtual void Collided(HittableObject away, HittableObject home)
	{
		away.Damage(this);
		Damage(away);
	}
	
	protected virtual void GotHit(HittableObject hit)
	{
		repair_charge = 0f;
		
		health--;
		
		if (health <= 0)
		{
			DestroyInstant();
		}
		else if (max_health > 1)
		{
			// Lose armor
			float percent = 1f - (float)(health - 1) / (float)(max_health - 1);

			for (int i = 0; i < Mathf.RoundToInt(armor.Length * percent); i++)
			{
				if (armor[i].activeSelf)
				{
					if (armor_explosion != null)
					{
						GameObject arexp = (GameObject)Instantiate(armor_explosion);
						arexp.transform.position = armor[i].transform.position;
						arexp.GetComponent<ExplosionController>().Init();
					}
					
					armor[i].SetActive(false);
				}
			}
		}
	}
	
	public virtual void Heal(float amt)
	{
		if (!repairable || health >= max_health)
			return;
		
		repair_charge += amt;
		
		if (repair_charge >= 1f)
		{
			repair_charge = 0f;
			
			health++;
			
			// Lose armor
			float percent = 1f - (float)(health - 1) / (float)(max_health - 1);
	
			for (int i = Mathf.RoundToInt(armor.Length * percent); 
				i < armor.Length; i++)
			{
				if (!armor[i].activeSelf)
				{
					armor[i].SetActive(true);
					
					//	Start coroutine to modify the armor's shader values so that
					//		the armor pieces come back into reality
				}
			}
		}
	}
	
	protected void Fire()
	{
		if (cooldown <= 0f)
		{
			cooldown = sec_to_cooldown;
			
			if (_Fire != null)
				_Fire(this);
		}
	}
	
	protected override void _Update ()
	{
		base._Update();

		if (cooldown >= 0f) cooldown -= Time.deltaTime;
		
		// Fly you fool!
		Fly ();
		
		// Animations
		if (anim != null)
		{
			anim.SetInteger("health", health);
		}
	}
	
	const float MARGIN_OF_ERROR = 2.5f;
	
	protected virtual void Fly()
	{
		at_destination = true;
		float dist = (destination.position - transform.position).magnitude;
		Quaternion des_rot = destination.rotation;
		if (dist > MARGIN_OF_ERROR || (Quaternion.Angle(transform.rotation, des_rot) > MARGIN_OF_ERROR && des_rot != no_rotation))
		{
			if (!strafing)
			{
				if (dist > MARGIN_OF_ERROR)
				{
					transform.rotation = 
						Quaternion.RotateTowards(transform.rotation, 
							Quaternion.LookRotation((destination.position - transform.position).normalized,
							transform.up), angular_speed * Time.deltaTime);
					
					transform.position += transform.forward * Mathf.Min(speed * Time.deltaTime, 
						dist);
					
					dist = (destination.position - transform.position).magnitude;
				}
				else
					transform.rotation = 
						Quaternion.RotateTowards(transform.rotation, 
							des_rot, angular_speed * Time.deltaTime);
			}
			else
			{
				transform.position += (destination.position - transform.position).normalized
					* Mathf.Min(speed * Time.deltaTime, dist);
				if (des_rot != no_rotation)
					transform.rotation = 
						Quaternion.RotateTowards(transform.rotation, 
							des_rot, angular_speed * Time.deltaTime);
			}
			
			if (dist <= MARGIN_OF_ERROR && (Quaternion.Angle(transform.rotation, des_rot) <= MARGIN_OF_ERROR || des_rot == no_rotation) && DestinationReached != null)
				DestinationReached(this);
			else at_destination = false;
		}
		else if (AtDestination != null)
			AtDestination(this);
	}
	
	protected void JumpTrails()
	{
		StartCoroutine(JumpTrails_enum());
	}
	
	IEnumerator JumpTrails_enum()
	{
		TrailRenderer[] trails = GetComponentsInChildren<TrailRenderer>();
		for (int i = 0; i < trails.Length; i++)
			trails[i].enabled = false;
		
		yield return null;
		
		for (int i = 0; i < trails.Length; i++)
		{
			TrailRenderer t = trails[i];
			t.enabled = true;
	
			GameObject gobj = (GameObject)Instantiate(t.gameObject);
			gobj.transform.parent = t.transform.parent;
			gobj.transform.localPosition = t.transform.localPosition;
			
			Destroy(t);
		}
	}
}
