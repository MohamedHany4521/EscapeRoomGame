#ifndef FALSE_EXIT_FUSES_INCLUDED
#define FALSE_EXIT_FUSES_INCLUDED
#include "FalseExitScreenCommon.hlsl"

float3 FE_RenderFuses(float2 meshUV, float seconds)
{
    float t = seconds * _AnimationSpeed + _Seed;
    float2 uv = FE_ScreenUV(meshUV, t);
    float2 p = float2(uv.x, 1.0 - uv.y) * float2(256.0, 320.0);
    float aa = FE_AA(p);
    float3 color = FE_Background(uv);
    // Right-align the FUSES header with a 16-pixel margin.
    float headerX = 256.0 - 16.0 - FE_TextWidth(5, 2.6);
    color += _MainColor.rgb * FE_Text(p, float2(headerX, 22.0), 2.6, FE_LABEL_FUSES, 5, aa);
    color += _MainColor.rgb * FE_Line(p, float2(16.0, 50.0), float2(240.0, 50.0), 0.8, aa) * 0.3;
    float3 present = step(0.5, float3(_Fuse1Present, _Fuse2Present, _Fuse3Present));
    float3 charge = saturate(float3(_Fuse1Charge, _Fuse2Charge, _Fuse3Charge));
    float3 online = present * step(max(_OnlineThreshold, 0.01), charge);
    int count = (int)(online.x + online.y + online.z);

    // Only one row is evaluated per fragment. Headers/footers skip this branch.
    if (p.y >= 70.0 && p.y < 260.0)
    {
        int row = min(2, (int)floor((p.y - 70.0) / 65.0));
        float installed = row == 0 ? present.x : (row == 1 ? present.y : present.z);
        float level = row == 0 ? charge.x : (row == 1 ? charge.y : charge.z);
        float y = 76.0 + (float)row * 65.0;
        float label = FE_Text(p, float2(16.0, y + 13.0), 2.4, FE_LABEL_F1 + row, 2, aa);
        color += _MainColor.rgb * label * lerp(0.3, 0.9, installed);
        float2 origin = float2(62.0, y);
        float2 size = float2(178.0, 43.0);
        float border = FE_BoxOutline(p, origin, size, 1.5, aa);
        color += _MainColor.rgb * border * lerp(0.24, 0.85, installed);
        if (installed > 0.5)
        {
            float2 inner = origin + 4.0;
            float2 innerSize = size - 8.0;
            color += _MainColor.rgb * FE_Segments(p, inner, innerSize, 5, 1.0, aa) * 0.09;
            color += _MainColor.rgb * FE_Segments(p, inner, innerSize, 5, level, aa) * 0.86;
        }
        else
        {
            float2 textOrigin = origin + float2((size.x - FE_TextWidth(7, 2.5)) * 0.5, 12.5);
            color += _WarningColor.rgb * FE_Text(p, textOrigin, 2.5, FE_LABEL_NO_FUSE, 7, aa);
        }
    }

    float size = 2.8;
    float2 footer = float2((256.0 - FE_TextWidth(10, size)) * 0.5, 282.0);
    float text = FE_Digit(p, footer, size, count, aa) +
        FE_Text(p, footer + float2(6.0 * size, 0.0), size, FE_LABEL_OF3_ONLINE, 9, aa);
    color += (count == 3 ? _MainColor.rgb : _WarningColor.rgb) * text;
    return FE_Finish(color, uv, t);
}

#endif
