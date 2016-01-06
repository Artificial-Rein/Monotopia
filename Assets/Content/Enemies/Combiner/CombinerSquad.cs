using UnityEngine;
using System.Collections;

public class CombinerSquad : Squad
{
	public GameObject combiner_fab;
	const int max_bonus = 3;
	
	protected virtual void OnValidate()
	{
		total_units = max_bonus + 1;
	}
	
	public override void _Spawn()
	{
		for (int i = 0; i < max_bonus + 1; i++)
		{
			GameObject e = (GameObject)Instantiate(combiner_fab);
			e.transform.position = transform.position;
			Enemy enemy = e.GetComponent<Enemy>();
			Add (enemy);
			Fight.f.NewEnemy(enemy);
		}
		
		Scatter();
		
		LoseSquadMember += Scatter;
	}
	
	public virtual void Scatter(Enemy en = null, Squad s = null)
	{
		foreach (CombinerEnemy e in this)
		{
			e.Destination = new Vector3(Random.Range(-125f, 125f), 0f, Random.Range(-75f, -150f));
			e.destination.rotation = e.no_rotation;
			e.ute = false;
			
			e.DestinationReached += Gather;
			e.DestinationReached -= e.CommenceFiring;
			e.charges = 0;
		}
	}
	
	protected virtual void Gather(Enemy en = null)
	{
		Vector3 avg = Vector3.zero;
		foreach (Enemy e in this)
			avg += e.Destination;
		avg /= NumMembers;
		avg.z = -50f;
		
		int mod = 0;
		foreach (CombinerEnemy e in this)
		{
			e.DestinationReached -= Gather;
			e.ute = false;
			
			Vector3 dest = avg;
			if (mod == 0)
			{
				e.Destination = avg;
				avg = e.Destination;
				e.destination.rotation = Quaternion.identity;
				e.focus = true;
				e.DestinationReached += e.CommenceFiring;
			}
			else
			{
				dest += new Vector3(15f - 15f * (mod - 1), 0f, -15f);
				e.Destination = dest;
				e.destination.rotation = Quaternion.LookRotation(avg - e.Destination, e.transform.up);
				e.focus = false;
			}
			mod++;
		}
	}
}
