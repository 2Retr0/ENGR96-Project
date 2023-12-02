//UNITY_SHADER_NO_UPGRADE
#ifndef QUANTIZE_INCLUDED
#define QUANTIZE_INCLUDED

#define PI 3.14159265358979323846
#define NUM_COLORS 32.f

// Source: https://www.shadertoy.com/view/4ddGWr
float quantize(float inp, float period)
{
    return floor((inp+period/2.)/period)*period;
}

void Quantize_float(const float3 in_color, out float3 out_color)
{
    const float quantizationPeriod = 1.f / (NUM_COLORS - 1.f);

    float rq = quantize(in_color.r, quantizationPeriod);
    float gq = quantize(in_color.g, quantizationPeriod);
    float bq = quantize(in_color.b, quantizationPeriod);

    out_color = float3(rq, gq, bq);
}

#endif //QUANTIZE_INCLUDED
