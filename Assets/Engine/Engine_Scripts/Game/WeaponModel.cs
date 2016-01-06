using UnityEngine;
using System.Collections;

public class WeaponModel : ShidouGameObject
{
	public Transform barrel;
	
	public Weapon owner;
	
	public Animator anim;
	
	bool initialized;
	protected bool Initialized { get { return initialized; } }
	
	void Start()
	{
		initialized = false;
	}
	
	protected override void _Update()
	{
		if (!initialized && owner != null)
		{
			initialized = true;
			_Start ();
		}
	}
	
	protected virtual void _Start()
	{
		
	}
}
