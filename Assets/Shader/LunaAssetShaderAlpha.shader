Shader "Custom/LunaAssetShaderALPHA" {
	Properties {

		_MainTex ("Base (RGB)", 2D) = "white" {}
		_EmissiveTex("Emissive Texture RGB", 2D) = "black" {}
		_SelfEmission("Self Illumination", Range(0,1)) = 0.3 
		_SelfEmissionColor("Self Emission Color", Color) = (1,1,1)
		_AngleToSun("Angle to sun",range(-1,1)) = 0
		_halfLambertPower("Half Power", range(1,10)) = 3.3
		_toonWarpTexture("Diffuse Warp Texture", 2D) = "white" {}
		_ColorMerge("Colormerge", range(0,100)) = 18
		_Cutoff("Alpha Test", range(0,1)) = 0.5
		
	}

	SubShader {
		Tags {"Queue"="AlphaTest" "IgnoreProjector"="True"  "RenderType"="TransparentCutout"}
		LOD 200
		
		CGPROGRAM
		#include "LightingModels/FunLightingModels.cginc"
		#pragma surface surf CustomHalfLambertToonWarp alphatest:_Cutoff

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

			
			_AngleToSun = clamp((_AngleToSun + 1) * 0.5,0,1);

			half4 c1 = tex2D(_MainTex, IN.uv_MainTex * half2(0.5,1));
			half4 c2 = tex2D(_MainTex, IN.uv_MainTex  * half2(0.5,1) + half2(0.5,0));

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
	Fallback "Transparent/Cutout/VertexLit"
}
