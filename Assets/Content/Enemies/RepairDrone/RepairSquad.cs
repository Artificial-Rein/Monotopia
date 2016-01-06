using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RepairSquad : BasicListSquad
{
	Queue<Enemy> to_repair;
	public Queue<Enemy> ToRepair { get { return to_repair; } }
	
	protected override void Start ()
	{
		base.Start ();
		
		to_repair = new Queue<Enemy>();
		Fight.f._NewEnemy += AddDamageEvent;
	}
	
	protected virtual void AddDamageEvent(Enemy e)
	{
		if (e.repairable)
			e.dmgGeneric += AddToQueue;
	}
	
	protected virtual void AddToQueue(HittableObject h)
	{
		Enemy e = h as Enemy;
		if (e == null || to_repair.Contains(e)) return;
		
		to_repair.Enqueue(e);
	}
	
	protected override void _Update ()
	{
		base._Update ();
		
		if (to_repair.Count > 0)
		{
			Enemy c = to_repair.Peek();
			
			while (c == null || c.Health >= c.max_health)
			{
				to_repair.Dequeue();
				if (to_repair.Count == 0) break;
				c = to_repair.Peek();
			}
		}
	}
}
