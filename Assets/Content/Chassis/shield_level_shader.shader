Shader "Custom/shield_level_shader" {
	Properties {
		_Shininess ("Shininess", Range(0.03, 1)) = 0.07
		_SpecColor ("Specular Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
		_ShieldTex ("Shield Level (RGB) Transparency (A)", 2D) = "black" {}
		_BumpMap ("Bumpmap", 2D) = "bump" {}
		_ShieldColor ("Shield Color", Color) = (1,1,1,1)
		_ErrorColor ("No Shield Color", Color) = (1,0,0,0)
		
		_ShieldLevel ("Shield Level", Range(0, 1)) = 0.5
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf BlinnPhong

		sampler2D _MainTex;
		sampler2D _BumpMap;
		sampler2D _ShieldTex;
		half _Shininess;
		half _ShieldLevel;
		half4 _ShieldColor;
		half4 _ErrorColor;

		struct Input {
			float2 uv_MainTex;
			float2 uv_BumpMap;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			half4 c = tex2D (_MainTex, IN.uv_MainTex);
			half4 s = tex2D (_ShieldTex, IN.uv_MainTex);
			
			if (s.a > 0)
			{
				half alph = 1 - s.a;
				
				if (s.r <= _ShieldLevel)
				{
					o.Albedo = _ShieldColor.rgb * s.a
						+ c.rgb * alph;
					o.Gloss = c.a * alph + s.a * _ShieldColor.a;
				}
				else
				{
					o.Albedo = _ErrorColor.rgb * s.a + c.rgb * alph;
					o.Gloss = _ErrorColor.a * s.a + c.a * alph;
				}
			}
			else
			{
				o.Albedo = c.rgb;
				o.Gloss = c.a;
			}
			o.Specular = _Shininess;
			
			o.Normal = UnpackNormal (tex2D (_BumpMap, IN.uv_BumpMap));
		}
		ENDCG
	} 
	FallBack "Specular"
}
