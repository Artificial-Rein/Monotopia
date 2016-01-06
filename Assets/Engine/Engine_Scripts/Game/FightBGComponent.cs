using UnityEngine;
using System.Collections;

public class FightBGComponent : MonoBehaviour
{
	public DepthOfFieldScatter dof;

	void Start()
	{
		if (dof != null && Chassis.c != null)
		{
			dof.focalTransform = Chassis.c.transform;
		}
	}
}
