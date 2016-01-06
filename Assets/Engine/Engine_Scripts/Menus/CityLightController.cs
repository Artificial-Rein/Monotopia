using UnityEngine;
using System.Collections;

public class CityLightController : MonoBehaviour
{
	new public Light light;
	float degrees;
	
	void OnValidate()
	{
		if (light == null)
			light = GetComponentInChildren<Light>();
	}
	
	void Start()
	{
		degrees = -90f;
	}
	
	// Update is called once per frame
	void Update ()
	{
		transform.rotation = Quaternion.AngleAxis(10f * Time.deltaTime, transform.right) * transform.rotation;
		degrees += 10f * Time.deltaTime;
		if (degrees > 180f) degrees -= 360f;
		
		float c = (1f - (Mathf.Abs(degrees) / 100f)) * 2f;
		if (Camera.main != null)
			Camera.main.backgroundColor = new Color(c,c,c);
		
		LightShadows setting = Settings.shadow ? LightShadows.Hard : LightShadows.None;
		if (light.shadows != setting) light.shadows = setting;
	}
}
