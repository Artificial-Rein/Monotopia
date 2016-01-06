Shader "Custom/ExplosionShader"
{
	Properties
	{
		_RampTex ("Ramp", 2D) = "white" {}
		_MainTex ("Displacement Texture", 2D) = "gray" {}
		_Displacement ("Displacement", float) = 0.1
		_ChannelFactor ("ChannelFactor (r,g,b)", Vector) = (1,0,0)
		_Radius ("Radius", float) = 1.0
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		Cull Off
		
		CGPROGRAM
		#pragma target 3.0
		#pragma glsl
		#pragma surface surf Lambert vertex:vert

		sampler2D _RampTex;
		sampler2D _MainTex;
		half _Displacement;
		half _Radius;
		float4 _ChannelFactor;

		struct Input
		{
			float2 uv_MainTex;
		};

		void vert (inout appdata_full v)
		{
			v.vertex.xyz += v.normal * max (-_Radius, _Displacement *
				(
					tex2D (_MainTex, v.texcoord.xy).r * _ChannelFactor.x +
					tex2D (_MainTex, v.texcoord.xy).g * _ChannelFactor.y +
					tex2D (_MainTex, v.texcoord.xy).b * _ChannelFactor.z
				));
		}

		void surf (Input IN, inout SurfaceOutput o)
		{
			float4 disp_rgb = tex2D (_MainTex, IN.uv_MainTex)
				* _ChannelFactor;
			
			half disp = max(-_Radius, disp_rgb.r + disp_rgb.b + disp_rgb.g);
			
			float4 c = tex2D(_RampTex, float2(disp * disp, 0));
			
			o.Albedo.rgb = c.rgb;
			o.Emission = o.Albedo * c.a;
			
			clip (0.95 - disp);
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
