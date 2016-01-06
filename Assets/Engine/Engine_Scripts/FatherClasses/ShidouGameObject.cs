using UnityEngine;
using System.Collections;

public class ShidouGameObject : MonoBehaviour
{
	void FixedUpdate()
	{
		// Check if paused
		if (Utilities.paused) return;
		
		_FixedUpdate();
	}
	
	void Update ()
	{
		// Check if paused
		if (Utilities.paused) return;
		
		_Update();
	}
	
	void OnGUI()
	{
		// Check if paused
		if (Utilities.paused) return;
		
		_OnGUI();
	}
	
	protected virtual void _FixedUpdate() { }
	
	protected virtual void _Update() { }
	
	protected virtual void _OnGUI() { }
}
