using UnityEngine;
using System.Collections;

public class MinelayerSquad : Squad
{
	public MinelayerEnemy minelayer_fab;
	
	protected virtual void OnValidate()
	{
		total_units = 1;
	}
	
	public override void _Spawn ()
	{
		GameObject e = (GameObject)Instantiate(minelayer_fab.gameObject);
		e.transform.position = transform.position;
		Enemy enemy = e.GetComponent<Enemy>();
		Add (enemy);
		Fight.f.NewEnemy(enemy);
		total_units = 1;
	}
	
	public override void Add (Enemy right)
	{
		base.Add (right);
		
		total_units++;
	}
}
