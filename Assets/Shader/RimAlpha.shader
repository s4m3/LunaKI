Shader "Custom/RimAlpha" {
	Properties {
		_TintColor ("Base (RGB)", Color) = (1,1,1)
		_Power ("Rim Power", Range(0,3)) = 1

	}
	SubShader {
		Tags { "RenderType"="Opaque" "Queue"="Transparent"}
		LOD 200
		AlphaTest Greater .01
		ColorMask RGB
		Cull Off Lighting Off ZWrite Off
		BindChannels {
			Bind "Color", color
			Bind "Vertex", vertex
			Bind "TexCoord", texcoord
		}
		CGPROGRAM
		#pragma surface surf Lambert alpha

		half4 _TintColor;
		half _Power;

		
	

		struct Input {
			float2 uv_MainTex;
			float3 viewDir;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			half rim = 1 - saturate(dot(o.Normal, normalize(IN.viewDir)));
			rim = pow(rim, _Power);
			o.Emission = _TintColor;
			o.Alpha = rim* _TintColor.a;

		}

		ENDCG
	} 

	FallBack "Diffuse"
}

