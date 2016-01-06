using UnityEngine;
using System.Collections;

public class ShidouGUIObject : MonoBehaviour
{
	// Class to contain data for a Shidou window. MAYBE DO THIS LATER? NOT SURE I CAN MAKE IT AS ROBUST AS I WANT
		// This window would contain a group of buttons and/or 
		//		labels which would appear and disappear using the iconic lines.
		// Also need a class for a label, button, and checkbox thingy. Not sure about that last one.
		// This will manage all line management and etc
	// This will also contain a LoadLevel and LoadLevelAdditive which will cause the lines
	//		to remove all labels and etc while asynchronously loading
	//		the next scene.
	// Player can set a flag "destroy on LoadLevel which will
	//		destroy the object when LoadLevel or LoadLevelAdditive
	//		finish their duties.
	
	public static GUISkin skin;
	public static Texture cursor;
	public static Texture aimer;
	
	void OnGUI()
	{
		// PreGUI code
		GUI.skin = skin;
		
		_OnGUI();
	}
	
	protected virtual void _OnGUI() { }
}
