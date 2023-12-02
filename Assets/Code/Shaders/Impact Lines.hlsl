//UNITY_SHADER_NO_UPGRADE
#ifndef IMPACTLINES_INCLUDED
#define IMPACTLINES_INCLUDED

#define PI 3.14159265358979323846
#define SUBDIVISIONS 90.0

// Source: https://www.shadertoy.com/view/Xt3cDn
uint base_hash(uint p)
{
    p = 1103515245U * ((p >> 1U) ^ p);
    const uint h32 = 1103515245U * (p ^ (p >> 3U));
    return h32 ^ (h32 >> 16);
}

float hash(const float x)
{
    return float(base_hash(asuint(x))) * (1.0 / float(0xFFFFFFFFU));
}

float2 rotate(const float2 vec, const float a) {
    const float2x2 rot = float2x2(cos(a), -sin(a), sin(a), cos(a));
    return float2(dot(vec, rot[0]), dot(vec, rot[1]));
}

// SDF for a n-star polygon with external angle en
// Source: https://iquilezles.org/articles/distfunctions2d/
// p : position, r : radius, n : num sides, m : angle divisor
float lines_sdf(float2 p, const float r, const int n, const float m) // m=[2,n]
{
    // these 4 lines can be precomputed for a given shape
    const float an = PI / float(n);
    const float en = PI / m;
    float2 acs = float2(cos(an), sin(an));
    float2 ecs = float2(cos(en), sin(en)); // ecs=float2(0,1) and simplify, for regular polygon,
    p.x = abs(p.x);

    // reduce to first sector
    const float bn = fmod(atan2(p.x, p.y), 2.f * an) - an;
    p = length(p) * float2(cos(bn), abs(sin(bn)));

    // line sdf
    p -= r * acs;
    p += ecs * clamp(-dot(p, ecs), 0.f, r * acs.y / ecs.y);
    return length(p) * sign(p.x);
}

void Impact_Lines_float(const float2 uv, float time,
                        // Parameters
                        const float weight,
                        const float bias_angle,
                        const float bias_weight,
                        // Outputs
                        out float line_mix_factor,
                        out float line_alpha)
{
    float2 st = uv - 0.5;

    // FIXME: These should be calculated on the CPU
    const float strength = weight;
    const float strength_norm = (1.f - 0.8f) * strength + 0.8f;
    const float bias = bias_weight;
    st += -0.08f * bias * float2(cos(bias_angle), sin(bias_angle));

    // Normalize angle to be between 0 and 1.
    const float angle = atan2(st.y, st.x) + PI;
    const float angle_norm = (0.5 * angle) / PI;

    const float angle_bias = bias * cos(angle - bias_angle) + (1. - bias);
    // Generate a seed for each line based on its angle.
    const float s_rand = 0.5f * hash(floor(angle_norm * SUBDIVISIONS)) + 0.5f;

    // Denote parameters for animation progress
    const float s_time = s_rand * time * 7.f;
    const float p_shape = frac(s_time); // Animation progress
    const float p_iter = floor(s_time); // Animation Iteration

    // We want an iteration-specific hash so that spike radius isn't dependent
    // purely on angle.
    const float p_rand = 0.5f * hash(p_iter) + 0.5f;
    const float s_radius = 1.f - pow(max(0.f, 1.6f * p_shape - 0.6f), 2.0f);

    st *= 0.6f;                       // Scale out space for 'sharper' lines.
    st = rotate(st, s_rand + p_rand); // Rotate by a random amount

    float w = 162.f * s_radius;
    w *= 0.4f * p_rand;              // Add random factor to spikiness.
    w *= angle_bias * strength_norm; // Incorporate angular bias
    w = max(2.f, w);                 // w in [2,SUBDIVISIONS]

    const float d = lines_sdf(st, 0.7f, SUBDIVISIONS, w);
    // Use smoothstep() for anti-aliasing
    line_mix_factor = d > 0.f ? 1.f : 1.f - smoothstep(0.f, 0.001f, abs(d));
    // Alpha increases when line begins to recede
    line_alpha = 1.f - 2.f * p_shape * 0.2f * strength;
}
#endif //IMPACTLINES_INCLUDED
