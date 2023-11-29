Shader "Unlit/VisionCone" {
    Properties
    {
		_Color0 ("Undetected Color", Color) = (0.5, 0.93, 0.5, 0.65)
    	_Color1 ("Detected Color", Color) = (1.0, 0.06, 0.06, 0.65)
    	_Position ("Position", Vector) = (0, 0, 0)
    	_Range ("Range", Float) = 5.0
    	_Progress ("Detection Progress", Float) = 0.0
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

				static const float start_offset = 1.3f;

	            // --- Import Properties ---
	            float4 _Color0;
	            float4 _Color1;
	            float4 _Position;
	            float _Range;
	            float _Progress;

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
		            const float dist = distance(frag_data.world_pos, _Position);
	            	const float dist_norm = dist / _Range;
					const float progress = lerp(start_offset / _Range, 1.f, _Progress);
	            	const bool alerted = _Progress >= 1.0f;

	            	_Color0.w *= 1.f - dist_norm * dist_norm; // Taper off vision cone alpha
	            	_Color0.w *= alerted ? 1.15f : 1.f;       // Increase alpha if alerted
	            	_Color0.w *= step(start_offset, dist);    // Start vision cone after certain distance from actor

	            	_Color1 *= alerted ? 1.f : cos(dist * 50.f) * 0.05f + 0.9f; // Add 'strips' to detection portion of cone
	            	_Color1.w = _Color0.w;

	            	// Mix detection and undetected parts of cone together.
					return step(dist_norm, progress) * _Color1 + step(progress, dist_norm) * _Color0;
	            }
            ENDCG
        }
    }
}