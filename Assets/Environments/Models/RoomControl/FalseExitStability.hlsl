#ifndef FALSE_EXIT_STABILITY_INCLUDED
#define FALSE_EXIT_STABILITY_INCLUDED
#include "FalseExitScreenCommon.hlsl"

float FE_Wave(float x, float time, float instability)
{
    float phase = x * 4.7 - time * _WaveSpeed;
    float clean = sin(phase * 6.2831853) * 0.66;
    float sampleX = x * 35.0 - time * _WaveSpeed * 7.0;
    float cell = floor(sampleX);
    float a = FE_Hash(float2(cell, 41.0 + _Seed)) * 2.0 - 1.0;
    float b = FE_Hash(float2(cell + 1.0, 41.0 + _Seed)) * 2.0 - 1.0;
    float jagged = lerp(a, b, frac(sampleX));
    return lerp(clean, jagged, instability);
}

float3 FE_RenderStability(float2 meshUV, float seconds)
{
    float t = seconds * _AnimationSpeed + _Seed;
    float2 uv = FE_ScreenUV(meshUV, t);
    float2 p = float2(uv.x, 1.0 - uv.y) * float2(256.0, 320.0);
    float aa = FE_AA(p);
    float3 color = FE_Background(uv) + FE_Title(p, 256.0, FE_LABEL_STABILITY, 9, aa);
    float stability = saturate(_Stability);
    float stable = step(max(0.01, _StableThreshold), stability);
    float3 waveColor = lerp(_WarningColor.rgb, _MainColor.rgb, stable);
    float instability = saturate((1.0 - stability) * 1.6) * (1.0 - stable);
    float2 graphOrigin = float2(16.0, 66.0);
    float2 graphSize = float2(224.0, 85.0);
    color += _MainColor.rgb * FE_BoxOutline(p, graphOrigin, graphSize, 0.9, aa) * 0.25;
    if (all(p >= graphOrigin) && all(p <= graphOrigin + graphSize))
    {
        float2 local = (p - graphOrigin) / graphSize;
        float2 gridStep = graphSize / float2(8.0, 4.0);
        float2 nearest = abs(frac((p - graphOrigin) / gridStep + 0.5) - 0.5) * gridStep;
        float grid = FE_Cover(min(nearest.x, nearest.y) - 0.3, aa);
        color += _MainColor.rgb * grid * 0.065;
        float height = FE_Wave(local.x, t, instability) * 33.0;
        float nextHeight = FE_Wave(local.x + 0.001, t, instability) * 33.0;
        float slope = (nextHeight - height) / (graphSize.x * 0.001);
        float distance = abs(p.y - (graphOrigin.y + graphSize.y * 0.5 - height)) / sqrt(1.0 + slope * slope);
        color += waveColor * FE_Cover(distance - 0.65, aa) * 0.95;
    }

    if (p.y >= 170.0 && p.y < 262.0)
    {
        int row = min(2, (int)floor((p.y - 170.0) / 31.0));
        float value = saturate(row == 0 ? _Signal : (row == 1 ? _SystemPower : _Sync));
        int label = row == 0 ? FE_LABEL_SIG : (row == 1 ? FE_LABEL_PWR : FE_LABEL_SYN);
        float y = 173.0 + (float)row * 31.0;
        float3 tint = value >= 0.66 ? _MainColor.rgb : _WarningColor.rgb;
        color += tint * FE_Text(p, float2(16.0, y), 2.3, label, 3, aa);
        float2 barOrigin = float2(64.0, y - 0.5);
        float2 barSize = float2(176.0, 18.0);
        color += _MainColor.rgb * FE_Segments(p, barOrigin, barSize, 12, 1.0, aa) * 0.10;
        color += tint * FE_Segments(p, barOrigin, barSize, 12, value, aa) * 0.9;
    }

    int percentage = (int)floor(stability * 100.0 + 0.5);
    int digits = percentage >= 100 ? 3 : (percentage >= 10 ? 2 : 1);
    int suffixLength = stable > 0.5 ? 8 : 10;
    int suffix = stable > 0.5 ? FE_LABEL_PCT_STABLE : FE_LABEL_PCT_UNSTABLE;
    float size = 2.45;
    float2 footer = float2((256.0 - FE_TextWidth(digits + suffixLength, size)) * 0.5, 282.0);
    float text = 0.0;
    if (digits == 3)
    {
        text += FE_Digit(p, footer, size, percentage / 100, aa);
        text += FE_Digit(p, footer + float2(6.0 * size, 0.0), size, (percentage / 10) % 10, aa);
    }
    else if (digits == 2)
        text += FE_Digit(p, footer, size, percentage / 10, aa);
    text += FE_Digit(p, footer + float2((float)(digits - 1) * 6.0 * size, 0.0), size, percentage % 10, aa);
    text += FE_Text(p, footer + float2((float)digits * 6.0 * size, 0.0), size, suffix, suffixLength, aa);
    color += waveColor * text;
    return FE_Finish(color, uv, t);
}

#endif
