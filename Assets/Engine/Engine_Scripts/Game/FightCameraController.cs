using UnityEngine;
using System.Collections;

public class FightCameraController : ShidouGameObject
{
	float px_to_cx;
	Vector3 start;
	Chassis player;
	
	void Start()
	{
		GameObject p = GameObject.FindGameObjectWithTag("Player");
		player = p.GetComponentInChildren<Chassis>();
		
		start = transform.position;
		
		px_to_cx = 0f;
		Plane[] frust = GeometryUtility.CalculateFrustumPlanes(camera);
		bool inside = GeometryUtility.TestPlanesAABB(frust, new Bounds(new Vector3(125f, 0f, 0f), new Vector3(10, 10, 10)));
		
		while (!inside)
		{
			px_to_cx += 0.1f;
			
			transform.position = start + px_to_cx * new Vector3(125f, 0f, 0f);
			
			frust = GeometryUtility.CalculateFrustumPlanes(camera);
			inside = GeometryUtility.TestPlanesAABB(frust, new Bounds(new Vector3(125f, 0f, 0f), new Vector3(10, 10, 10)));
		}
	}
	
	protected override void _Update()
	{
		if (player != null)
		{
			transform.position = start + new Vector3(player.transform.position.x * px_to_cx, 0f, 0f);
		}
	}
}
