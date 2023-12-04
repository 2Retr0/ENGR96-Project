Shader "Sci-Fi Space Station/PBR_Screen" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0

		_EmissionColor("Emission Color", Color) = (1,1,1,1)
		_EmissionMap("Emissive", 2D) = "black" {}
		_EmissionFactor("Emissive Factor", Float) = 1.0
		_ScreenTwinkleSpeed ("Emissive Twinkle Speed", Float) = 0.5
		_ScreenScanningSpeed("Emissive Scanning Speed", Float) = 0.5
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex, _EmissionMap;

		struct Input {
			float2 uv_MainTex;
		};

		half _Glossiness;
		half _Metallic;
		half _EmissionFactor, _ScreenTwinkleSpeed, _ScreenScanningSpeed;
		fixed4 _Color, _EmissionColor;

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;

			fixed3 EmTexture = tex2D(_EmissionMap, IN.uv_MainTex).rgb;
			
			float2 ScreenUV = IN.uv_MainTex - float2(0.0, _Time.z * _ScreenScanningSpeed) + ( EmTexture.rg * 0.25);
			
			fixed ScreenFX = tex2D(_EmissionMap, ScreenUV).a;

			float EmFactor = (clamp( ( (sin(_Time.z * _ScreenTwinkleSpeed) * 0.5 ) + _EmissionFactor), 0, 1.25) + ScreenFX * _EmissionFactor)  * _EmissionFactor ;
			
			
			o.Emission = EmTexture.rgb * _EmissionColor * EmFactor;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
