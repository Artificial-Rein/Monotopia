using UnityEngine;
using System.Collections;

public class BuildingGridAligner : MonoBehaviour
{
	
	public int x, y;
	public bool APPLY = false;
	
	void OnValidate()
	{
		if (APPLY)
		{
			x = Mathf.RoundToInt((transform.position.x - 144.05f) / (2f * 144.05f));
			y = Mathf.RoundToInt(transform.position.z / 345.9f);
		}
		
		APPLY = false;
		
		transform.position = new Vector3(x * 2f * 144.05f + 144.05f, -250f, y * 345.9f);
	}
}
