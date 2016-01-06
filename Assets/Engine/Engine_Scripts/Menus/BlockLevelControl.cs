using UnityEngine;
using System.Collections;

public class BlockLevelControl : MonoBehaviour
{
	public GameObject hq, lq;
	
	public GameObject model;
	bool model_lq;
	
	public const float lq_dist = 20f;
	
	void OnValidate()
	{
		if (model == null)
		{
			Animator a = GetComponentInChildren<Animator>();
			if (a != null)
				model = a.gameObject;
		}
	}
	
	void Start()
	{
		if (model != null)
			model_lq = false;
		else _UpdateModel();
	}
	
	void _UpdateModel()
	{
		Destroy(model);
		
		model_lq = (Settings.low_poly || Vector3.Distance(transform.position, Camera.main.transform.position) > lq_dist);
		
		if (model_lq)
			model = (GameObject)Instantiate(lq);
		else
			model = (GameObject)Instantiate(hq);
		Quaternion r = transform.rotation;
		transform.rotation = Quaternion.identity;
		model.transform.localScale = new Vector3(1f,1f,1f);
		model.transform.position += transform.position;
		model.transform.parent = transform;
		transform.rotation = r;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (model_lq != (Settings.low_poly || Vector3.Distance(transform.position, Camera.main.transform.position) > lq_dist))
			_UpdateModel();
	}
}
