using UnityEngine;
using System.Collections;

public class FitBG : IResizer
{
	public enum Anchors : int { TopLeft=0, Top=1, TopRight=2, Left=3, Center=4, Right=5, BottomLeft=6, Bottom=7, BottomRight=8 };
	public enum Preference { MaximumSize, EntirePicture };
	
	public GUITexture to_fit = null;
	public Anchors anchor = Anchors.Top;
	public Preference maintain = Preference.EntirePicture;
	
	void OnValidate()
	{
		if (to_fit == null)
			to_fit = GetComponent<GUITexture>();
		
		if (to_fit == null)
			Debug.LogError("FitBG needs a GUITexture.");
	}
	
	protected override void StartChild () {}
	
	protected override void Resize()
	{
		if (to_fit != null)
		{
			Rect r = new Rect(0, 0, Screen.width, Screen.width * to_fit.texture.height / to_fit.texture.width);
			Rect r2 = new Rect(0, 0, Screen.height * to_fit.texture.width / to_fit.texture.height, Screen.height);
			if ((r2.height * r2.width < r.height * r.width && maintain == Preference.EntirePicture)
				|| (r2.height * r2.width > r.height * r.width && maintain == Preference.MaximumSize))
				r = r2;

			//to_fit.pixelInset.height = Screen.height;
			//to_fit.pixelInset.width = Screen.width;
			
			switch ((int)anchor%3)
			{
			case 1: r.x = -r.width / 2;
				break;
			case 2: r.x = -r.width;
				break;
			}
			
			switch((int)anchor/3)
			{
			case 0: r.y = -r.height;
				break;
			case 1: r.y = -r.height / 2;
				break;
			}
			
			to_fit.pixelInset = r;
		}
	}
}
