Shader "Unlit/Occluded Base" {
    Properties
    {
    	_Color0 ("Color", Color) = (1.0, 0.06, 0.06, 0.35)
    }

    SubShader
    {
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}

		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
	            #pragma vertex vert
	            #pragma fragment frag
	            #pragma target 3.0

	            #include "UnityCG.cginc"

	            #define PI 3.14159265358979323846

	            struct vert_data
	            {
					float4 vertex : POSITION;
	            };

	            struct frag_data
	            {
					float4 position : SV_POSITION;
	            	float4 screen_pos : TEXCOORD0;
	            };

	            float2 rotate(const float2 vec, const float a) {
				    const float2x2 rot = float2x2(cos(a), -sin(a), sin(a), cos(a));
				    return float2(dot(vec, rot[0]), dot(vec, rot[1]));
				}

	            // --- Import Properties ---
	            float4 _Color0;
	            float4 _Color1;
	            float4 _Position;

	            frag_data vert(const vert_data vert_data)
	            {
					frag_data frag_data;

	            	frag_data.position = UnityObjectToClipPos(vert_data.vertex);
	            	frag_data.screen_pos = ComputeScreenPos(frag_data.position);

	            	return frag_data;
	            }

	            // SV_TARGET semantic tells Unity that we're outputting a fixed4 to be rendered.
	            fixed4 frag(const frag_data frag_data) : SV_Target
	            {
		            // const float dist = distance(frag_data.position, _Position);
	            	// float2 st = frag_data.position.xy / frag_data.position.w;
	            	// float4 fragCoord = fixed4_position / fixed4_position.w;
				// fragCoord.xy = 0.5 * (fragCoord.xy + 1.0);
				    float2 st = frag_data.screen_pos.xy / frag_data.screen_pos.w;
	            	st = rotate(st, PI / 4);
	            	st *= 20.f;

		            // const float dist = distance(frag_data.world_pos, float3(0,0,0));

		            // float2 uv = frag_data.screen_position.xy / frag_data.screen_position.w;
	             //    float4 cpos = UnityObjectToClipPos(float3(0,0,0));
	             //    uv -= cpos.xy / cpos.w;
	             //    uv *= cpos.w / UNITY_MATRIX_P._m11;
	                // uv.x *= _ScreenParams.x / _ScreenParams.y;


					// const float progress = lerp(start_offset, 1.f, 0.9f);
	    //         	const bool alerted = 0.9f >= 1.0f;

	            	// _Color0.w *= 1.f - dist * dist; // Taper off vision cone alpha
	            	// _Color0.w *= alerted ? 1.15f : 1.f;       // Increase alpha if alerted
	            	// _Color0.w *= step(start_offset, dist);    // Start vision cone after certain distance from actor
		            //
	            	// _Color1 *= alerted ? 1.f : cos(dist * 50.f) * 0.05f + 0.9f; // Add 'strips' to detection portion of cone
	            	// _Color1.w = _Color0.w;

	            	// Mix detection and undetected parts of cone together.
					// return step(dist, progress) * _Color1 + step(progress, dist) * _Color0;
	            	return _Color0 * (cos(st.x * 50.f) * 0.05f + 0.9f);
	            }
            ENDCG
        }
    }
}