using UnityEngine;
using System.Collections;

public class DroneSquad : BasicListSquad
{
	protected override void Start ()
	{
		base.Start ();

		int pos = Random.Range(0, 4);
		if (pos == 0)
			transform.position = new Vector3(-200f, 0f, Random.Range(-50f, -150f));
		else if (pos == 1)
			transform.position = new Vector3(200f, 0f, Random.Range(-50f, -150f));
		else
			transform.position = new Vector3(Random.Range(-125f, 125f), 0f, -200f);
	}

	protected override void _Update ()
	{
		base._Update ();
		
		if (Spawned)
			DroneEnemy.last_bombing -= Time.deltaTime * total_units;
	}
}
