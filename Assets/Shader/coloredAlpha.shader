Shader "Custom/coloredAlpha" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Color("Main Color", Color) = (0,1,0)
	}
	SubShader {
		Tags { "RenderType"="Opaque" "Queue" = "Transparent"  }
		LOD 200
		Cull Off
		CGPROGRAM
		#pragma surface surf Lambert alpha
	
		sampler2D _MainTex;

		half3 _Color;

		struct Input {
			float2 uv_MainTex;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			half4 c = tex2D (_MainTex, IN.uv_MainTex);
			o.Emission = _Color;
			o.Alpha = c;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
