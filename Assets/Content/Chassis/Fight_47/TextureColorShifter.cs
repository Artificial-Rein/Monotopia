using UnityEngine;
using System.Collections;
using System.Threading;

public class TextureColorShifter : MonoBehaviour
{
	public Texture2D tex;
	public Color plastic, metal;
	static Color _plastic, _metal;
	
	bool applying = false;
	public static bool need_set = false;
	Color[] colors;
	
	static Color[] cs = { };
	
	// Use this for initialization
	void Start ()
	{
		if (cs.Length == 0)
			cs = tex.GetPixels();
		
		applying = false;
		need_set = false;
		
		colors = new Color[cs.Length];
	}
	
	void _Apply()
	{
		_plastic = plastic;
		_metal = metal;
		
		for (int i = 0; i < colors.Length; i++)
		{
			Color c = cs[i];
			if (c.r < 0.5f)
			{
				colors[i] = plastic;
			}
			/*else if (c.r < 1f)
			{
				colors[i] = Color.Lerp(
					plastic, metal, c.r);
			}*/
			else
			{
				colors[i] = metal;
			}
		}
		
		applying = false;
	}
	
	void Update()
	{
		if (need_set && !applying)
		{
			tex.SetPixels(colors);
			tex.Apply();
			need_set = false;
		}
		
		if (_plastic != plastic || _metal != metal)
		{
			if (applying || need_set) return;
			applying = true;
			need_set = true;
			Thread t = new Thread(new ThreadStart(_Apply));
			t.Start();
		}
	}
}
