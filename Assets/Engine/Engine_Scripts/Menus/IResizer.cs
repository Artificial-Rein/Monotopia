using UnityEngine;
using System.Collections;

public abstract class IResizer : MonoBehaviour
{	
	int h, w;
	
	// Use this for initialization
	void Start ()
	{
		StartChild();
		
		h = Screen.height;
		w = Screen.width;
		Resize ();
	}
	
	// Update is called once per frame
	protected virtual void Update ()
	{
		if (h != Screen.height || w != Screen.width)
		{
			h = Screen.height;
			w = Screen.width;
			Resize();
		}
	}
	
	/// <summary>
	/// This will be called whenever the object needs to be resized.
	/// </summary>
	protected abstract void Resize();
	
	/// <summary>
	/// Use this instead of Start().
	/// It will be called before IResizer's Start function.
	/// </summary>
	protected abstract void StartChild();
}
