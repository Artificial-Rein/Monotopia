using UnityEngine;
using System.Collections;

public class BackgroundRoller : ShidouGameObject
{
	public float speed = 150f;
	
	protected override void _Update()
	{
		transform.position += new Vector3(0f, 0f, speed * Time.deltaTime);
	}
}
