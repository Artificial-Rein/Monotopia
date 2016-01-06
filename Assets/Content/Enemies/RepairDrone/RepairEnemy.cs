using UnityEngine;
using System.Collections;

public class RepairEnemy : BasicFiringEnemy
{
	public float operating_range = 5f;
	
	protected override void _Update ()
	{
		base._Update ();
		
		RepairSquad s = squad as RepairSquad;
		
		//while not needing to repair, meander aimlessly
		//while repairing, go to a bit outside of thing to repair and fire at it
		if (s.ToRepair.Count > 0 && s.ToRepair.Peek() != null)
		{
			Enemy c = s.ToRepair.Peek();
			
			Vector3 dest = c.transform.position + 
				(transform.position - c.transform.position).normalized * operating_range * 0.75f;
			Destination = dest;
			destination.rotation = Quaternion.LookRotation(c.transform.position - Destination,
				transform.up);
			
			if ((transform.position - c.transform.position).magnitude <= operating_range)
				Fire();
		}
		else if (at_destination)
			Destination = new Vector3(Random.Range(-125f, 125f), 0f, Random.Range(-75f, -125f));
	}
	
	protected override Bullet InstantiateBullet ()
	{
		Bullet ob = base.InstantiateBullet ();
		RepairBullet b = ob as RepairBullet;
		
		if (b != null)
			b.target = (squad as RepairSquad).ToRepair.Peek();
		
		return ob;
	}
}
