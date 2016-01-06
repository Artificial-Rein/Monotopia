using UnityEngine;
using System.Collections;

public class BlockColorShifter : MonoBehaviour
{
	// List of the materials for different colors.
	public Material[] mats;
	public int color = -1;
	
	public void Shift(Sector.BlockColor c)
	{
		// find all renderers and shift their mat to the passed one
		color = (int)c;
	}
	
	void Update()
	{
		if (color != -1)
		{
			foreach (Renderer r in GetComponentsInChildren<Renderer>())
			{
				r.material = mats[color];
			}
		}
	}
}
