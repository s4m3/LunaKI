Shader "Custom/AnimatedAlphaOverlay" {
	Properties {
		_Color ("MainColor", Color) = (1,1,1,1)
		_AnimatedTexture ("AnimatedTexture (RGB)", 2D) = "white" {}
		_AlphaOverlay ("Alpha Overlay", 2D) = "white" {}
		_Velocity ("Animation Speed X", Range(0,5)) = 0.0
		
	}
	SubShader {
		Tags { "RenderType"="Opaque" "Queue" = "Transparent"  }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Lambert alpha

		sampler2D _AnimatedTexture;
		sampler2D _AlphaOverlay;

		half _Velocity;
		half _VelocityY;

		half3 _Color;


		struct Input {
			float2 uv_AnimatedTexture;
			float2 uv_AlphaOverlay;
		};

		void surf (Input IN, inout SurfaceOutput o) {

			half2 translation = half2(0, -_Velocity)  * _Time ;

			half overlay = tex2D (_AlphaOverlay, IN.uv_AlphaOverlay).x;
			half animated =  tex2D (_AnimatedTexture, IN.uv_AnimatedTexture + translation);

			half alpha = overlay * animated;

			o.Emission = _Color;

			o.Albedo = _Color;
			o.Alpha = alpha;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
