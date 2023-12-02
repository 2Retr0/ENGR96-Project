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

	            fixed4 frag(const frag_data frag_data) : SV_Target
	            {
				    float2 st = frag_data.screen_pos.xy / frag_data.screen_pos.w;
	            	st = rotate(st, PI / 4);
	            	st *= 20.f;

	            	return _Color0 * (cos(st.x * 50.f) * 0.05f + 0.9f);
	            }
            ENDCG
        }
    }
}