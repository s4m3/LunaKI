Shader "Custom/LunaAssetShader" {
	Properties {

		_MainTex ("Base (RGB)", 2D) = "white" {}
		_EmissiveTex("Emissive Texture RGB", 2D) = "black" {}
		_SelfEmission("Self Illumination", Range(0,1)) = 0.3 
		_SelfEmissionColor("Self Emission Color", Color) = (1,1,1)
		_AngleToSun("Angle to sun",range(-1,1)) = 0
		_halfLambertPower("Half Power", range(1,10)) = 3.3
		_toonWarpTexture("Diffuse Warp Texture", 2D) = "white" {}
		_OutlineWidth("Outline Width",range(0,0.35)) = 0.1
		_ColorMerge("Colormerge", range(0,100)) = 18
		
	}

	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		Pass {
			Cull Front
			Lighting Off 
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag 
			#include "UnityCG.cginc"

			struct a2v
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			}; 
 
			struct v2f
			{
				float4 pos : POSITION;
			};
 
			float _OutlineWidth;
 
			v2f vert (a2v v)
			 {
				 v2f o;
				 float4 pos = mul( UNITY_MATRIX_MV, v.vertex); 
				 float3 normal = mul( (float3x3)UNITY_MATRIX_IT_MV, v.normal);  
				 normal.z = -0.4;
				 pos = pos + float4(normalize(normal),0) * _OutlineWidth;
				 o.pos = mul(UNITY_MATRIX_P, pos);
 
				 return o;
			 }
 
			float4 frag (v2f IN) : COLOR
			{
				return float(0);
			} 
			ENDCG		
		}
		
		CGPROGRAM
		#include "LightingModels/FunLightingModels.cginc"
		#pragma surface surf CustomHalfLambertToonWarp

		sampler2D _MainTex;
		half _AngleToSun;
		half _ColorMerge;
		half _SelfEmission;
		sampler2D _EmissiveTex;
		half3 _SelfEmissionColor;

		struct Input {
			float2 uv_MainTex;			
		};

		void surf (Input IN, inout SurfaceOutput o) {

			IN.uv_MainTex.x *= 0.5;
			_AngleToSun = clamp((_AngleToSun + 1) * 0.5,0,1);
			half4 c1 = tex2D(_MainTex, IN.uv_MainTex);
			half4 c2 = tex2D(_MainTex, IN.uv_MainTex + half2(0.5,0));

			half4 finalColor = lerp(c2,c1, _AngleToSun);
			finalColor.rgb = (floor(finalColor.rgb*_ColorMerge)/_ColorMerge);

			o.Albedo = finalColor;

			half4 emissionColor1 = tex2D(_EmissiveTex, IN.uv_MainTex);
			half4 emissionColor2 = tex2D(_EmissiveTex, IN.uv_MainTex + half2(0.5,0));

			half4 finalEmissionColor = lerp(emissionColor2,emissionColor1,_AngleToSun);

			o.Emission = finalEmissionColor + (finalColor * _SelfEmissionColor * _SelfEmission);
			o.Alpha = finalColor.a;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
