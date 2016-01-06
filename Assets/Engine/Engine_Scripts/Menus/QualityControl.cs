using UnityEngine;
using System.Collections;

public class QualityControl : MonoBehaviour
{
	public PostEffectsBase[] fx;
	int qual;
	
	// Use this for initialization
	void Start ()
	{
		qual = 4;
		if (Settings.quality < 4)
			SetFX();
	}
	
	void SetFX()
	{
		qual = Settings.quality;
		if (Settings.quality < 4)
		{
			foreach (PostEffectsBase f in fx)
				if ((f as Bloom) == null || Settings.quality < 3)
					f.enabled = false;
				else
					f.enabled = true;
		}
		else
		{
			foreach (PostEffectsBase f in fx)
				f.enabled = true;
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Settings.quality != qual)
			SetFX();
	}
}
