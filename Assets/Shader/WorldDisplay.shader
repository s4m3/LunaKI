Shader "Custom/WorldDisplay" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_OverlayTex ("Overlay (Kontur)", 2D) = "white" {}
		_Color ("Color Overlay", Color) = (0,1,0)
		_OverlayColor("Outline Color", Color) = (1,0,0)
	}


	SubShader {
		Tags { "Queue" = "Transparent"  }
		LOD 200
	

		Lighting Off 
		CGPROGRAM
		#pragma surface surf Lambert alpha
		
		sampler2D _OverlayTex;
		sampler2D _MainTex;
		half4 _Color;
		half4 _OverlayColor;

		struct Input {
				float2 uv_OverlayTex;
				float2 uv_MainTex;			
		};

		void surf (Input IN, inout SurfaceOutput o) {
			half fill = tex2D(_MainTex, IN.uv_MainTex).x;
			half overLay = tex2D(_OverlayTex, IN.uv_OverlayTex).x;

			half4 overlayColor = _OverlayColor * overLay;
			o.Emission = overlayColor + fill * _Color;
			o.Alpha = overLay + fill;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
