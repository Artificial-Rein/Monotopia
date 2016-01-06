Shader "Custom/shield_fx"
{
	Properties
	{
		_Scale ("AdditiveAmount", range(0, 1)) = 1.0
		_Amount ("Refract Amount", float) = 10
		_Spec ("Specular", Range(0, 1)) = 0.07
		_Gloss ("Gloss", Range(0, 1)) = 1.0
		
		_SpecColor ("Specular Color", Color) = (1,1,1,1)
	}

	SubShader
	{
		Tags { "Queue" = "Transparent" }
	
		GrabPass {
		"_GrabTexture"
		}
		
		Cull Off
			
		CGPROGRAM
		#pragma target 3.0
		#pragma exclude_renderers gles
		#pragma vertex vert
		#pragma surface frag BlinnPhong
		
		sampler2D _GrabTexture : register(s0);
		float4 _GrabTexture_TexelSize;
		half _Amount;
		half _Spec;
		half _Gloss;
		half _Scale;
		
		struct Input
		{
			float3 worldNormal;
			//float4 uvrefr : TEXCOORD1;
			float4 grabUV;
		};
			
		void vert (inout appdata_full v, out Input o)
		{
			o.grabUV = ComputeGrabScreenPos(mul (UNITY_MATRIX_MVP, v.vertex));
		}
		
		void frag (Input i, inout SurfaceOutput o)// : COLOR
		{
			i.grabUV.xy = (i.worldNormal.xy * _GrabTexture_TexelSize.xy) * (_Amount * i.grabUV.z) + i.grabUV.xy;
			
		
			half4 refr = tex2Dproj( _GrabTexture, UNITY_PROJ_COORD(i.grabUV));
			o.Emission.rgb = _SpecColor.rgb * _Scale + refr.rgb;
			
			o.Specular = _Spec;
			o.Gloss = _Gloss;
		}
		ENDCG
	}
	
	FallBack "Specular"
}
