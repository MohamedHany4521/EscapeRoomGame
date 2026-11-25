#ifndef FALSE_EXIT_SCREEN_COMMON_INCLUDED
#define FALSE_EXIT_SCREEN_COMMON_INCLUDED
#include "FalseExitPixelFont.hlsl"

float FE_Hash(float2 p)
{
    float3 p3 = frac(float3(p.x, p.y, p.x) * 0.1031);
    p3 += dot(p3, p3.yzx + 33.33);
    return frac((p3.x + p3.y) * p3.z);
}

float FE_Cover(float distance, float aa)
{
    return 1.0 - smoothstep(-aa, aa, distance);
}

float FE_BoxDistance(float2 p, float2 center, float2 halfSize)
{
    float2 q = abs(p - center) - halfSize;
    return length(max(q, 0.0)) + min(max(q.x, q.y), 0.0);
}

float FE_Box(float2 p, float2 origin, float2 size, float aa)
{
    return FE_Cover(FE_BoxDistance(p, origin + size * 0.5, size * 0.5), aa);
}

float FE_BoxOutline(float2 p, float2 origin, float2 size, float thickness, float aa)
{
    return FE_Cover(abs(FE_BoxDistance(p, origin + size * 0.5, size * 0.5)) - thickness * 0.5, aa);
}

float FE_Line(float2 p, float2 a, float2 b, float thickness, float aa)
{
    float2 ab = b - a;
    float h = saturate(dot(p - a, ab) / max(dot(ab, ab), 0.001));
    return FE_Cover(length(p - (a + h * ab)) - thickness * 0.5, aa);
}

float FE_Circle(float2 p, float2 center, float radius, float aa)
{
    return FE_Cover(length(p - center) - radius, aa);
}

// Flat-sided hexagon with top and bottom vertices. Returns signed distance.
float FE_HexDistance(float2 p, float radius)
{
    const float3 k = float3(-0.866025404, 0.5, 0.577350269);
    p = abs(p);
    p -= 2.0 * min(dot(k.xy, p), 0.0) * k.xy;
    p -= float2(clamp(p.x, -k.z * radius, k.z * radius), radius);
    return length(p) * sign(p.y);
}

float2 FE_ScreenUV(float2 meshUV, float time)
{
    float2 uv = meshUV * _UVScaleOffset.xy + _UVScaleOffset.zw;
    uv.y = lerp(uv.y, 1.0 - uv.y, step(0.5, _FlipY));
    float2 q = uv * 2.0 - 1.0;
    uv += (uv - 0.5) * dot(q, q) * _Curvature;
    // Off by default: brief horizontal faults, not constant aggressive shaking.
    float frame = floor(time * 12.0 + _Seed);
    float gate = step(0.92, FE_Hash(float2(frame, 7.0)));
    float stripe = FE_Hash(float2(floor(uv.y * 20.0), frame));
    uv.x += (stripe - 0.5) * gate * _GlitchStrength * 0.05;
    return uv;
}

float FE_AA(float2 p)
{
    float2 dx = ddx(p);
    float2 dy = ddy(p);
    return max(0.12, 0.6 * max(length(dx), length(dy)));
}

float3 FE_Background(float2 uv)
{
    float glow = 1.0 - saturate(length((uv - 0.5) * 1.4));
    return _BackgroundColor.rgb * (0.7 + 0.5 * glow);
}

float3 FE_Title(float2 p, float width, int label, int count, float aa)
{
    float title = FE_Text(p, float2(16.0, 22.0), 2.6, label, count, aa);
    float divider = FE_Line(p, float2(16.0, 50.0), float2(width - 16.0, 50.0), 0.8, aa);
    return _MainColor.rgb * (title + divider * 0.3);
}

float FE_Segments(float2 p, float2 origin, float2 size, int count, float level, float aa)
{
    float2 q = p - origin;
    if (any(q < 0.0) || any(q > size)) return 0.0;
    float cellWidth = size.x / (float)count;
    float index = floor(q.x / cellWidth);
    float2 cellOrigin = origin + float2(index * cellWidth + 1.0, 1.0);
    float shape = FE_Box(p, cellOrigin, float2(cellWidth - 2.0, size.y - 2.0), aa);
    // At zero, no segments light; any nonzero amount lights at least one.
    float active = 1.0 - step(ceil(saturate(level) * (float)count), index);
    return shape * active;
}

float3 FE_Finish(float3 color, float2 uv, float time)
{
    float scanPhase = uv.y * _ScanlineDensity;
    float scanAA = saturate(1.0 - fwidth(scanPhase));
    float scan = 1.0 - _ScanlineStrength * scanAA * (0.5 + 0.5 * cos(scanPhase * 6.2831853));
    float grain = FE_Hash(floor(uv * float2(768.0, 960.0)) + floor(time * 20.0)) - 0.5;
    float flicker = 1.0 - _FlickerStrength *
        (0.5 + 0.25 * sin(time * 71.0) + 0.25 * sin(time * 43.0 + _Seed));
    float2 q = uv * 2.0 - 1.0;
    float vignette = 1.0 - _Vignette * saturate(dot(q, q) * 0.5);
    color = max(0.0, color * scan + grain * _NoiseStrength);
    color *= max(0.0, _Brightness) * flicker * vignette;
    float inside = step(0.0, uv.x) * step(0.0, uv.y) * step(uv.x, 1.0) * step(uv.y, 1.0);
    float3 offColor = _BackgroundColor.rgb * 0.12;
    return lerp(offColor, color, saturate(_Power) * inside);
}

#endif
