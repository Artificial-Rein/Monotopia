using UnityEngine;
using System.Collections;

public class BasicListSquad : Squad
{
	public Enemy[] enemies;
	int last_index;
	float time;
	public float[] timing;
	
	protected virtual void OnValidate()
	{
		total_units = enemies.Length;
	}
	
	protected override void Start ()
	{
		base.Start ();
		
		time = -1f;
		last_index = 0;
	}
	
	protected override void _Update ()
	{
		base._Update ();
		
		if (time >= 0f)
		{
			time += Time.deltaTime;
			
			for (int i = last_index; i < timing.Length && i < enemies.Length; i++)
			{
				if (time < timing[i])
					break;
				
				if (time >= timing[i])
				{
					GameObject e = (GameObject)Instantiate(enemies[i].gameObject);
					e.transform.position = transform.position;
					Enemy enemy = e.GetComponent<Enemy>();
					Add (enemy);
					Fight.f.NewEnemy(enemy);
					
					last_index = i + 1;
				}
			}

			if (last_index >= timing.Length)
			{
				time = -1f;
				spawning = false;
			}
		}
	}
	
	public override void _Spawn()
	{
		time = 0f;

		spawning = true;
	}
}
