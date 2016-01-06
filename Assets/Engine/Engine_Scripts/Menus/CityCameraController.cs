using UnityEngine;
using System.Collections;

public class CityCameraController : MonoBehaviour
{
	Quaternion b;
	public float degrees_per_frame = 15f;

	public DepthOfFieldScatter dof;
	
	// Use this for initialization
	void Start ()
	{
		b = transform.localRotation;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (dof != null && dof.focalTransform == null)
		{
			GameObject gobj = GameObject.FindGameObjectWithTag("Player");
			if (gobj != null)
			{
				dof.focalTransform = gobj.transform;
				dof.enabled = true;
			}
		}

		if (!Settings.camera_movements)
		{
			transform.localRotation = b;
			return;
		}
		
		transform.localRotation = b;
		
		transform.Rotate(new Vector3(0,1,0), (Mathf.Clamp(Input.mousePosition.x, 0, Screen.width) - Screen.width / 2) / (Screen.width / 2) * degrees_per_frame);
		transform.Rotate(transform.right, -(Mathf.Clamp(Input.mousePosition.y, 0, Screen.height) - Screen.height) / Screen.height * degrees_per_frame);
	}
}
