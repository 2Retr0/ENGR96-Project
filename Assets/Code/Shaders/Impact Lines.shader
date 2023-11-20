Shader "Unlit/ImpactLines" {
    Properties
    {
//    	_MainTex ("Texture", 2D) = "white" {}
		_Weight ("Weight", Float) = 0.8
    	_BiasWeight ("Bias Weight", Float) = 0.0
    	_BiasAngle ("Bias Angle", Float) = 0.0
    }

    SubShader
    {
    	Tags{"RenderPipeline" = "UniversalPipeline"}
    	Cull Off Blend Off ZTest Off ZWrite Off

        Pass
        {
            CGPROGRAM
	            #pragma vertex vert
	            #pragma fragment frag
	            #pragma target 3.0

	            #include "UnityCG.cginc"

	            #define PI 3.14159265359

	            struct vert_data
	            {
					float4 vertex : POSITION;
	            	float4 uv : TEXCOORD0;
	            };

	            struct frag_data
	            {
					float4 position : SV_POSITION;
	            	float2 uv : TEXCOORD0;
	            };

	            // --- Import Properties ---
				sampler2D _MainTex;
	            float _Weight;
	            float _BiasWeight;
	            float _BiasAngle;

				// signed distance to a n-star polygon with external angle en
				// SDF Function adapted from Iñigo Quiles https://iquilezles.org/articles/distfunctions2d/
				// p : position, r : radius, n : num sides, m : angle divisor
				float lines_sdf(float2 p, float r, int n, float m) // m=[2,n]
				{
				    // these 4 lines can be precomputed for a given shape
				    float an = 3.14159265358979323846 / float(n);
				    float en = 3.14159265358979323846 / m;
				    float2 acs = float2(cos(an), sin(an));
				    float2 ecs = float2(cos(en), sin(en)); // ecs=float2(0,1) and simplify, for regular polygon,

				    // reduce to first sector
				    float bn = fmod(atan2(p.x, p.y), 2. * an) - an;
				    p = length(p) * float2(cos(bn), abs(sin(bn)));

				    // line sdf
				    p -= r * acs;
				    p += ecs * clamp(-dot(p, ecs), 0., r * acs.y / ecs.y);
				    return length(p) * sign(p.x);
				}

				float rand(float n) {
				    return frac(sin(n * 1234.5 + 5432.1) * 5432.1);
				}

				float rotate(float vec, const float a) {
					float2x2 rot = float2x2(cos(a), -sin(a), sin(a), cos(a));

				    return float2(dot(vec, rot[0]), dot(vec, rot[1]));
				}

	            frag_data vert(const vert_data vert_data)
	            {
					frag_data frag_data;

	            	frag_data.position = UnityObjectToClipPos(vert_data.vertex);
	            	frag_data.uv = vert_data.uv.xy;

	            	return frag_data;
	            }

	            // SV_TARGET semantic tells Unity that we're outputting a fixed4 to be rendered.
	            fixed4 frag(const frag_data frag_data) : SV_Target
	            {
	            	float3 tex = tex2D(_MainTex, frag_data.uv).xyz;
			        float2 st = frag_data.uv.xy - 0.5;

			        // These should be calculated on the CPU
			        float strength = _Weight; // Use a property instead of Weight variable
			        float str_norm = (1. - 0.8) * strength + 0.8;
			        float bias = _BiasWeight;
			        st += -0.08 * bias * float2(cos(_BiasAngle), sin(_BiasAngle));

			        // Normalize angle to be between 0 and 1.
			        float angle = atan2(st.y, st.x) + PI;
			        float angle_norm = (0.5 * angle) / PI;

			        // These should be calculated on the CPU
			        float angle_bias = bias * cos(angle - _BiasAngle) + (1. - bias);

			        // Generate a seed for each line based on its angle.
			        float s_rand = 0.5 * rand(floor(angle_norm * 90.0)) + 0.5;

			        // Denote parameters for animation progress
			        float s_time = s_rand * _Time * 5.;
			        float p_shape = frac(s_time); // Animation progress
			        float p_iter = floor(s_time); // Animation Iteration

			        float s_radius = 1. - pow(max(0., 1.6 * p_shape - 0.6), 2.0);

			        st *= 0.6; // Scale out space for 'sharper' lines.
			        st = rotate(st, s_rand + rand(p_iter)); // Rotate by a random amount

			        // Alpha when line begins to recede
			        float s_alpha = 1. - 2.0 * p_shape * 0.2 * strength;

			        float w = 162.0 * s_radius;
			        w *= 0.4 * s_rand; // Add random factor to spikiness.
			        w *= angle_bias * str_norm;
			        w = max(2., w);

			        // sdf
			        float d = lines_sdf(st, 0.7, 90., w);

			        // colorize
			        float3 col = (d > 0.0) ? float3(1., 1., 1.) : tex;
			        col = lerp(col, float3(1., 1., 1.), 1. - smoothstep(0.f, 0.001, abs(d))); // smooth line edges
			        col = lerp(col, tex, s_alpha); // 'transparency' of line

			        // Draw debug lines (red is path of line).
			        // col = frac(0.5 * (atan(st.y, st.x) / 3.14159265359 + 1.) * _n) > 0.95 ? float3(1., 0.25, 0.3) : col;

					// return float4(col, 1.);
	            	return float4(1.0, 1.0, 1.0, 1.0);
	            }
            ENDCG
        }
    }
}