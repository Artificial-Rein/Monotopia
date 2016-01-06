using UnityEngine;
using System.Collections;

public class NoDelete : MonoBehaviour
{
	// Use this for initialization
	void Start ()
	{
		DontDestroyOnLoad(gameObject);
	}
}
