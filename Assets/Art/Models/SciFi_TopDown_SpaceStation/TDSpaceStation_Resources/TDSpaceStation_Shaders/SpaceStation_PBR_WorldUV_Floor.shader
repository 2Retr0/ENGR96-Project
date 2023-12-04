Shader "Sci-Fi Space Station/PBR World UV Floor" {
	Properties {

		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,2)) = 1.0
		
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_MetallicGlossAOMap("Metallic (R) Gloss (G) AO (A)", 2D) = "black" {}
	
		_OcclusionStrength("Occlusion Strength", Range(0.0, 1.0)) = 1.0

		_EmissionColor("Emission Color", Color) = (1,1,1,1)
		_EmissionMap("Emissive", 2D) = "black" {}
		_EmissionFactor("Emissive Factor", Float) = 1.0

		_BumpScale("Scale", Float) = 1.0
		_BumpMap("Normal Map", 2D) = "bump" {}

		_UVScale("UV Scale", Vector) =  (1,1,1,1)
		_UVWorldScale("UV World Scale", float) = 1.0
		
	}

	

	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex, _MetallicGlossMap, _EmissionMap, _BumpMap, _Rough;
	
		struct Input {
			float2 uv_MainTex;
			float3 worldPos;
		};

		half _Glossiness, _Metallic, _EmissionFactor, _BumpScale, _OcclusionStrength, _RoughLerp, _UVWorldScale;
	
		fixed4 _Color, _EmissionColor, _UVScale;

		

		void surf (Input IN, inout SurfaceOutputStandard o) {
			
			half2 RescaledUV = IN.worldPos.xz;
			RescaledUV.x *= _UVScale.x * _UVWorldScale;
			RescaledUV.y *= _UVScale.y * _UVWorldScale;
			
			fixed4 c = tex2D (_MainTex, RescaledUV) * _Color;
			o.Albedo = c.rgb;

			float MetMap = tex2D(_MetallicGlossMap, RescaledUV).r;
			float GlossMap = tex2D(_MetallicGlossMap, RescaledUV).r;
			float AOMap = tex2D(_MetallicGlossMap, RescaledUV).r;
						
			o.Metallic = _Metallic * MetMap;

			o.Smoothness = _Glossiness * GlossMap;

			o.Normal = UnpackScaleNormal(tex2D(_BumpMap, RescaledUV), _BumpScale);
			
			o.Occlusion = LerpOneTo(AOMap, _OcclusionStrength );
			
			o.Alpha = c.a;

			o.Emission = tex2D(_EmissionMap, RescaledUV) * _EmissionColor * _EmissionFactor;
		}
		ENDCG
	} 

	FallBack "Specular"
}
