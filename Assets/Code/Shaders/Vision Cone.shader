Shader "Unlit/VisionCone" {
    Properties
    {
		_Color ("Color", Color) = (1, 1, 1, 1)
    	_Position ("Position", Vector) = (0, 0, 0)
    	_Range ("Range", Float) = 5
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

	            struct vert_data
	            {
					float4 vertex : POSITION;
	            };

	            struct frag_data
	            {
					float4 position : SV_POSITION;
	            	float4 world_pos : TEXCOORD0;
	            };

	            // --- Import Properties ---
	            float4 _Color;
	            float4 _Position;
	            float _Range;

	            frag_data vert(const vert_data vert_data)
	            {
					frag_data frag_data;

	            	frag_data.position = UnityObjectToClipPos(vert_data.vertex);
	            	frag_data.world_pos = mul(unity_ObjectToWorld, vert_data.vertex);

	            	return frag_data;
	            }

	            // SV_TARGET semantic tells Unity that we're outputting a fixed4 to be rendered.
	            fixed4 frag(const frag_data frag_data) : SV_Target
	            {
		            const float dist = distance(frag_data.world_pos, _Position) / _Range;
	            	_Color.w *= 1.f - dist * dist; // Taper off vision cone alpha
	            	_Color.w *= step(0.2f, dist);  // Start vision cone after certain distance from actor

					return _Color;
	            }
            ENDCG
        }
    }
}