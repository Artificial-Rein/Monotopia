using UnityEngine;
using System.Collections;

public class MinelayerEnemy : Enemy
{
	public MineEnemy mine_fab;
	
	protected virtual void LayMine(Enemy e)
	{
		GameObject gobj = (GameObject)Instantiate(mine_fab.gameObject);
		gobj.transform.position = transform.position;
		gobj.transform.rotation = transform.rotation;
		Enemy enemy = gobj.GetComponent<Enemy>();
		squad.Add(enemy);
		Fight.f.NewEnemy(enemy);
	}
	
	public override void _Initialize ()
	{
		base._Initialize ();
		
		DestinationReached += Relocate;
		_Fire += LayMine;
		
		Relocate(this);
	}
	
	protected override void _Update ()
	{
		base._Update();
		
		if (Mathf.Abs(transform.position.x) < 100f && transform.position.z < -50f && transform.position.z > -110f)
			Fire ();
	}
	
	protected virtual void Relocate(Enemy e)
	{
		Destination = new Vector3(Random.Range(-125f, 125f), 0f, Random.Range(-50f, -150f));
	}
}
