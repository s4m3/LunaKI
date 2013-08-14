Shader "Custom/LunaPlanetAtmosphereShader" {
	Properties {
		
		
		_LightPos ("Light Position", Vector) = (0,0,0)
		_PlanetPos("PlanetPosition", Vector) = (0,0,0)
		_AtmosphereTexture ("AtmosphereTexture (RGBA)", 2D) = "white" {}
		_AtmosphereGradient ("AtmosphereTexture (RGBA)", 2D) = "white" {}
		_RimPower ("RimPower", Float) = 1
		
	}

	SubShader {
		
		Tags { "RenderType"="Transparent" "Queue"="Background" }
		LOD 200
		Lighting Off
		Zwrite Off
		CGPROGRAM
		
		#pragma surface surf Lambert alpha noambient addshadow
	

    	sampler2D _AtmosphereTexture;
		sampler2D _AtmosphereGradient;

		half3 _PlanetPos;
		half3 _LightPos;
		half _RimPower;

		struct Input {
			float2 uv_AtmosphereTexture;
			float3 worldPos;
			float3 viewDir;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			
			//rim Color	
			half3 LightDir = normalize(IN.worldPos - _LightPos);
			half3 PlanetNormal = normalize(IN.worldPos - _PlanetPos);
			half RimLightPos = 1 - (1 + dot(LightDir, PlanetNormal)) * 0.5f;
			o.Emission = tex2D(_AtmosphereTexture, half2(RimLightPos, 0));

			//rim alpha
			half Rim = pow( ( ( dot(o.Normal, normalize(IN.viewDir)) + 1) * 0.5), _RimPower);
			half RimFromTexture =  tex2D(_AtmosphereGradient, half2(Rim,0)).rgb;
			o.Alpha = RimFromTexture;
		}
	
		ENDCG
	} 
		
	FallBack "Diffuse"
}


