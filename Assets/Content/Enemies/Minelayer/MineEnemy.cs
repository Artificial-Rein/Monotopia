using UnityEngine;
using System.Collections;

public class MineEnemy : BasicFiringEnemy
{
	public float time;
	public float flash_duration;
	protected float flash_time;
	
	public int bullets_fired;
	
	public GameObject blinker;
	
	protected override void _Update ()
	{
		base._Update ();
		
		time -= Time.deltaTime;
		flash_time -= Time.deltaTime;
		if (time < 1f)
			flash_time -= Time.deltaTime * 4;
		
		if (time <= 0f)
		{
			Fire ();
			DestroyInstant();
		}
		else if (flash_time <= 0f)
		{
			flash_time = flash_duration;
			
			blinker.SetActive(!blinker.activeSelf);
		}
	}
	
	protected override void FireBullet (Enemy e)
	{
		Quaternion q = fire_loc.rotation;
		for (int i = 0; i < bullets_fired; i++)
		{
			fire_loc.Rotate(Vector3.up, 360f / bullets_fired);
			base.FireBullet (e);
		}
		fire_loc.rotation = q;
	}
}
