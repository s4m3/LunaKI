Shader "Custom/LunaCharacterShader" {
	Properties {
	
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_EmissiveTex("Emissive Texture RGB", 2D) = "black" {}
		_SelfEmission("Self Illumination", Range(0,1)) = 0 
		_RimRange ("RimRange", Range(0,15)) = 0
		_RimStrength("Rim Strength", Range(0,1)) = 1
		_RimColor("Rim Color", Color) = (0,1,0)
		_halfLambertPower("Half Power", range(1,10)) = 2
		_toonWarpTexture("Diffuse Warp Texture", 2D) = "white" {}
		_OutlineWidth("Outline Width",range(0,0.35)) = 0
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

		//Pass {	

			CGPROGRAM
			#include "LightingModels/FunLightingModels.cginc"
			#pragma surface surf CustomHalfLambertToonWarp

			sampler2D _MainTex;
			sampler2D _EmissiveTex;
			half _RimRange;
			half3 _RimColor;
			half _RimStrength;
			half _SelfEmission;

			struct Input {
				float2 uv_MainTex;
				float2 uv_EmissiveTex;
				float3 viewDir;
			};

			void surf (Input IN, inout SurfaceOutput o) {
				half4 c = tex2D (_MainTex, IN.uv_MainTex);
				half3 ec = tex2D(_EmissiveTex, IN.uv_EmissiveTex);
				half rim = 1 - saturate(dot(o.Normal, normalize(IN.viewDir)));
				rim = pow(rim, _RimRange);

				o.Emission = ec + ( rim * _RimColor * _RimStrength ) + _SelfEmission*c;
				o.Albedo = c.rgb;
				o.Alpha = c.a;
			}
			ENDCG	
			
		//}
	}
	FallBack "Diffuse"
	
}


