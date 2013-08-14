Shader "Custom/LunaPlanetShader" {
	Properties {
		
		_Color1 ("Light Side Emission", Color) = (1,0,0)
		_Color2 ("Dark Side Emission", Color) = (0,1,0)
		_TransitionZone("TransitionZone", Float) = 1
		_LightPos ("Light Position", Vector) = (0,0,0)
		_PlanetPos("PlanetPosition", Vector) = (0,0,0)
		_TransitionTexture ("TransitionTexture (RGBA)", 2D) = "white" {}
		_LightSideTexture(" LightSideTexture ", 2D) = "white" {}
		_LightSideNormal("Light Normal",2D) = "white"{}
		_DarkSideNormal("Dark Normal",2D) = "white"{}
		_DarkSideTexture(" DarksideTexutre ", 2D) = "white" {}
		_halfLambertPower("Half Power", range(1,10)) = 2
		_RimPower("Rim Power", Float) = 1
		_RimAmountTexture(" Rim Texture ", 2D) = "white" {}
		_toonWarpTexture("Warp Texture", 2D) = "white" {}
		_OutlineWidth("Outline Width", range(0,0.3)) = 0.1
		_ColorMerge("Color Merge", range(0,100)) = 16

	}

	SubShader {
		
		Tags { "RenderType"="Opaque"} 
		
		Pass {
			Cull Front
			Lighting Off 			
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
		
		#pragma surface surf CustomHalfLambertToonWarp
		#pragma target 3.0
		#include "LightingModels/FunLightingModels.cginc"


    	sampler2D _MainTex;
		sampler2D _TransitionTexture;
		sampler2D _LightSideTexture;
		sampler2D _DarkSideTexture;
		sampler2D _RimAmountTexture;
		sampler2D _DarkSideNormal;
		sampler2D _LightSideNormal;

		half3 _PlanetPos;
		half3 _LightPos;
		half3 _Color1;
		half3 _Color2;
		half _TransitionZone;
		half _RimPower;
		half _ColorMerge;

		struct Input {
			float2 uv_MainTex;
			float2 uv_LightSideTexture;
			float2 uv_DarkSideTexture;
			float2 uv_RimAmountTexture;
			float2 uv_LightSideNormal;
			float2 uv_DarkSideNormal;
			float3 worldPos;
			float3 viewDir;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			
			//diffuse	
			half3 LightDir = normalize(IN.worldPos - _LightPos);
			half3 PlanetNormal = normalize(IN.worldPos - _PlanetPos);

			half ColorPosition = pow( ( (dot(LightDir, PlanetNormal) + 1) / 2) ,_TransitionZone  );
			half4 TransitionColorAtPosition = tex2D (_TransitionTexture, half2(ColorPosition, 0));
			half TransitionAmount = TransitionColorAtPosition.a;
		

			half3 LightSideColor = tex2D(_LightSideTexture, IN.uv_LightSideTexture).rgb;
			half3 DarkSideColor = tex2D (_DarkSideTexture, IN.uv_DarkSideTexture).rgb;
			
			o.Normal = half3(0,0,1);

			//rim
			half Rim = pow( ( ( dot(o.Normal, normalize(IN.viewDir)) + 1) * 0.5), _RimPower);
			half RimAmount = tex2D(_RimAmountTexture, half2( Rim, 0)).xyz;
			half RimColorPosition = 1 - ( 1+  dot(LightDir,PlanetNormal  )) * 0.5;
			half3 RimColor = tex2D(_TransitionTexture, half2( RimColorPosition,0)) * RimAmount;
			
			//normal
			//half3 LightNormal = UnpackNormal(tex2D(_LightSideNormal, IN.uv_LightSideNormal));
			//half3 DarkNormal = UnpackNormal(tex2D(_DarkSideNormal, IN.uv_DarkSideNormal));
			//o.Normal = lerp(LightNormal, DarkNormal, TransitionAmount);

			float3 col = lerp(LightSideColor, DarkSideColor, TransitionAmount);
			
			o.Albedo =  (floor(col.rgb*_ColorMerge)/_ColorMerge);

			//o.Specular =  lerp(LightSideColor, DarkSideColor, TransitionAmount);


			o.Emission = RimColor; 

			//o.Alpha = c.a;
		}
	
		ENDCG


	} 
		
	FallBack "Diffuse"
}
