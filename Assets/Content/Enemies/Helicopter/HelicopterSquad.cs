using UnityEngine;
using System.Collections;

public class HelicopterSquad : BasicListSquad
{
	protected override void _Update ()
	{
		base._Update ();
		
		bool finished = true;
		foreach (Enemy e in this)
		{
			HelicopterEnemy h = e as HelicopterEnemy;
			if (h != null && !h.fired)
			{
				finished = false;
				break;
			}
		}
		
		if (finished)
			NewDestination();
	}
	
	protected virtual void NewDestination()
	{
		Vector3 new_dest = new Vector3(Random.Range(-125f, 125f), 0f, Random.Range(-125f, -75f));
		
		int j = -NumMembers / 2;
		for (int i = 0; i < members.Length; i++)
		{
			if (members[i] != null)
			{
				members[i].Destination = new Vector3(new_dest.x + j * 7.5f, 0f, new_dest.z - Mathf.Abs(j) * 5f);
				members[i].destination.rotation = Quaternion.identity;
				HelicopterEnemy h = members[i] as HelicopterEnemy;
				if (h != null)
					h.fired = false;
				j++;
			}
		}
	}
}
