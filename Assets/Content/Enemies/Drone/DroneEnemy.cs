using UnityEngine;
using System.Collections;

public class DroneEnemy : Enemy
{
	protected bool follower;
	protected bool need_new_dest;
	protected bool leader;
	protected bool bombing;
	public bool Bombing { get { return bombing; } }
	
	public float bombing_speed_mult = 3f;
	public float bombing_angular_speed_mult = 4f;
	
	protected DroneEnemy leading_drone;
	
	public float sec_between_bombs = 30f;

	float time_off_screen, time_on_screen;
	
	public static float last_bombing = 0f;
	
	public override void _Initialize ()
	{
		base._Initialize ();
		
		follower = false;
		leader = false;
		bombing = false;

		time_off_screen = 10f;
	}
	
	protected override void Start ()
	{
		base.Start ();
		
		need_new_dest = true;
		
		squad.NewSquadMember += AddFollower;
		
		base.AtDestination += delegate(Enemy enemy) 
		{
			need_new_dest = true;
		};
	}
	
	void AddFollower(Enemy follower, Squad s)
	{
		DroneEnemy de = follower as DroneEnemy;
		if (de == null) return;
		
		de.Follow(this);
		
		s.NewSquadMember -= AddFollower;
	}
	
	public void Follow(DroneEnemy t)
	{
		leading_drone = t;
		
		destination.parent = t.transform;
		destination.position = t.transform.position;
		destination.localPosition = Vector3.back * 3f;
		destination.localRotation = Quaternion.identity;
		
		follower = true;
		t.leader = true;
	}
	
	protected override void _Update ()
	{
		if (!Utilities.IsThisVisible(gameObject))
		{
			time_off_screen -= Time.deltaTime;
			time_on_screen = 0f;
			if (time_off_screen <= 0f)
				Destroy(gameObject);
		}
		else
		{
			time_off_screen = 10f;
			time_on_screen += Time.deltaTime;
		}

		if (follower && destination == null)
		{
			need_new_dest = true;
			follower = false;
			CreateNewDestinationObj();
		}
		
		base._Update ();
		
		if (!bombing && !follower && need_new_dest)
		{
			Destination = new Vector3(Random.Range(-100f, 100f),
				0f, Random.Range(-60f, -125f));
			need_new_dest = false;
		}
		
		if (!squad.Spawning && !bombing && Chassis.c != null && Chassis.c.alive && !leader && last_bombing <= 0f &&
			Mathf.RoundToInt(Random.Range(0, 1 / Time.deltaTime)) == 1 && time_on_screen >= 5f && transform.position.z < -50f &&
		    Mathf.Abs(Chassis.c.transform.position.x - transform.position.x) < 50f)
		{
			bombing = true;
			last_bombing = sec_between_bombs;
			
			speed *= bombing_speed_mult;
			angular_speed *= bombing_angular_speed_mult;
			
			destination.parent = null;
			destination.rotation = no_rotation;
			Destination = Chassis.c.transform.position;
			
			AtDestination += ContinueBombing;
			
			if (leading_drone != null)
				leading_drone.leader = false;
		}
		
		if (bombing && transform.position.z > 50f)
		{
			transform.position = new Vector3(transform.position.x, transform.position.y, -200f);
			bombing = false;
			
			JumpTrails();
			
			speed /= bombing_speed_mult;
			angular_speed /= bombing_angular_speed_mult;
			
			if (follower)
			{
				if (leading_drone == null)
					follower = false;
				else
					Follow(leading_drone);
			}
		}
	}
	
	protected virtual void ContinueBombing(Enemy e)
	{
		AtDestination -= ContinueBombing;
		
		Destination += Vector3.forward * 100f;
	}
}
