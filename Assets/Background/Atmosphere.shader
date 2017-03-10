Shader "Solar System/Titan Atmosphere"
{
	Properties
	{
		//_Color ("Main Color", Color) = (1,1,1,1) 
		_Tint ("Tint Color", Color) = (.8, .8, .8, 1)
		_Brightness ("Brightness", Float) = 1
		//_MainTex ("Base (RGB)", 2D) = "white" {}
		_Strength ("Strength", Float) = 1
		_AutoExp ("Auto Exposure", Range (0, 0.5)) = 0.2
		_Ramp2DF ("Front BRDF Ramp", 2D) = "gray" {}
		_Ramp2DB ("Back BRDF Ramp", 2D) = "gray" {}
	}
	
	SubShader
	{
		Tags {"Queue"="Transparent" "RenderType"="Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
		ZWrite Off
		LOD 200
		
		CGPROGRAM
			#pragma surface surf BRDF alpha
			#pragma target 3.0
	
			//sampler2D _MainTex;
			sampler2D _Ramp2DF;
			sampler2D _Ramp2DB;
			//fixed4 _Color; 
			float4 _Tint;
			half _Strength;
			half _Brightness;
			half _AutoExp;
			
			inline half4 LightingBRDF (SurfaceOutput s, half3 lightDir, half3 viewDir, half atten)
			{
				//BRDF
				float NdotL = dot(s.Normal, lightDir);
				float NdotE = dot(s.Normal, viewDir);
				float LdotE = dot(lightDir, viewDir);
				
				float diff = NdotL * 0.5 + 0.5;
				float2 brdfUV = float2(NdotE, diff);
				float4 BRDF = tex2D(_Ramp2DB, brdfUV.xy);//lerp(tex2D(_Ramp2DB, brdfUV.xy), tex2D(_Ramp2DF, brdfUV.xy), LdotE * 0.5 + 0.5);
				
				//Blinn-Phong
				//half3 h = normalize (lightDir + viewDir);
				
				//fixed diff_BP = max (0, dot (s.Normal, lightDir));
				
				//float nh = max (0, dot (s.Normal, h));
				//float spec = pow (nh, s.Specular*128.0) * s.Gloss;
				
				//fixed4 c;
				//c.rgb = (s.Albedo * _LightColor0.rgb * diff_BP + _LightColor0.rgb * _SpecColor.rgb * spec) * (atten * 2);
				//c.a = s.Alpha + _LightColor0.a * _SpecColor.a * spec * atten;
				//return c;
				
				float4 c;
				c.rgb = s.Albedo + _LightColor0.rgb * BRDF * atten * _Tint.rgb * _Brightness / (dot(lightDir, viewDir) * _AutoExp + (1 - _AutoExp));
					//(s.Albedo * _LightColor0.rgb * BRDF + _LightColor0.rgb * _SpecColor.rgb * spec) * (atten * 2);
				c.a = s.Alpha;//BRDF.a;// + _LightColor0.a * _SpecColor.a * spec * atten;
				return c;
			}
	
			struct Input
			{
				float2 uv_MainTex;
				float3 viewDir;
				//float2 uv_NormalMap;
			};
	
			void surf (Input IN, inout SurfaceOutput o)
			{
				//fixed4 c = _Color;//float4(0.5, 0.5, 0.5, 1);//tex2D (_MainTex, IN.uv_MainTex);
				
				//o.Albedo = c.rgb * _Tint.rgb;
				//o.Specular = _Shininess;
				//o.Alpha = c.a;
				
				o.Alpha = tex2D(_Ramp2DF, float2(dot(o.Normal, normalize(IN.viewDir)), 0.9)).a * _Strength;
			}
		ENDCG
	} 
	//FallBack "Diffuse"
}
