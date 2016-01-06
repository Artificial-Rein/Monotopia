Shader "Holograph"
{
	
	Properties
	{
		_Color ("Color Tint (A = Opacity)", Color) = (0,0,0,0)
		_MainTex ("Texture (A = Transparency)", 2D) = ""
	}

	SubShader
	{
		Tags {Queue = Transparent}
		ZWrite On
		Blend SrcAlpha One
		Cull Off
		Pass
		{
			Lighting Off
			SetTexture[_MainTex] 
			{
				constantColor[_Color]
				Combine texture * constant, texture
			}
		}
	}

}