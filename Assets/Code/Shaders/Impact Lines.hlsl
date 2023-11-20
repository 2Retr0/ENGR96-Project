//UNITY_SHADER_NO_UPGRADE
#ifndef IMPACTLINES_INCLUDED
#define IMPACTLINES_INCLUDED

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

    p.x = abs(p.x);

    // reduce to first sector
    float bn = fmod(atan2(p.x, p.y), 2. * an) - an;
    p = length(p) * float2(cos(bn), abs(sin(bn)));

    // line sdf
    p -= r * acs;
    p += ecs * clamp(-dot(p, ecs), 0., r * acs.y / ecs.y);
    return length(p) * sign(p.x);
}

void Impact_Lines_float(float2 uv, float2 fragCoord, float2 resolution, float time, out float alpha)
{
    float2 p = 2 * uv - 1;
    p.x /= resolution.y / resolution.x;

    float d = lines_sdf(p, 0.7, 5, 3.5);

    alpha = d >= 0.f ? 1.f : 0.f;
}
#endif //IMPACTLINES_INCLUDED
