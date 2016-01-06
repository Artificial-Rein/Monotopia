using UnityEngine;
using System.Collections;

public class PolyTester : MonoBehaviour
{
	public GameObject cube;
	public GUIText poly;
	int n;
	public int poly_per_click = 250;
	
	void Start() { n = 0; }
	
	// Update is called once per frame
	void Update ()
	{
		if (Input.anyKeyDown)
		{
			for (int i = 0; i < poly_per_click; i += 12)
			{
				((GameObject)Instantiate(cube)).transform.position = new Vector3(Random.Range(-25f, 25f), Random.Range(-10f, 10f), Random.Range(0f, 50f));
				n += 12;
			}
			
			poly.text = n + " poly";
		}
	}
}
