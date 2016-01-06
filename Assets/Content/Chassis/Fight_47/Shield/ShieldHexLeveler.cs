using UnityEngine;
using System.Collections;

public class ShieldHexLeveler : MonoBehaviour
{
	float scale;
	public GameObject render;
	
	public float Scale
	{
		set
		{
			float soon_scale = Mathf.Clamp(value, 0f, 1f);
			if (soon_scale == 0f)
				render.SetActive(false);
			else if (scale == 0f && soon_scale > 0f)
				render.SetActive(true);
			scale = soon_scale;
			
			animation["anim"].time = animation["anim"].length * (1f - scale);
			
			foreach (Renderer r in render.GetComponentsInChildren<Renderer>())
				r.material.SetFloat("_Scale", scale);
		}
	}
	
	void OnValidate()
	{
		Renderer r = GetComponentInChildren<Renderer>();
		if (r != null)
		{
			render = r.gameObject;
			
			name = "shieldpiece_" + r.name;
		}
	}
	
	void Start()
	{
		animation["anim"].speed = 0;
		
		Scale = 1f;
	}
}
