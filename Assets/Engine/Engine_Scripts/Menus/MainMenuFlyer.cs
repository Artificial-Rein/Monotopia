using UnityEngine;
using System.Collections;

public class MainMenuFlyer : MonoBehaviour
{
	Vector3 velocity;
	public float speed = 1f;
	
	// Use this for initialization
	void Start ()
	{
		velocity = -transform.forward * speed;
	}
	
	// Update is called once per frame
	void Update ()
	{
		Vector3 acceleration = new Vector3 (0f, Random.Range(2.5f, 3f), 0f) - transform.position;
		
		velocity += acceleration;
		velocity = velocity.normalized * speed;
		
		transform.rotation = 
			Quaternion.LookRotation(-velocity.normalized, -acceleration.normalized);
		
		transform.position += velocity * Time.deltaTime;
	}
}
