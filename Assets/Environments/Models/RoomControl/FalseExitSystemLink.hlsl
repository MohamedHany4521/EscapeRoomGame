#ifndef FALSE_EXIT_SYSTEM_LINK_INCLUDED
#define FALSE_EXIT_SYSTEM_LINK_INCLUDED
#include "FalseExitScreenCommon.hlsl"

float3 FE_LinkNode(float2 p, float2 node, float2 center, float connected, float time, float aa)
{
    float3 tint = connected > 0.5 ? _MainColor.rgb : _WarningColor.rgb;
    float2 delta = center - node;
    float len = length(delta);
    float along = dot(p - node, delta / len);
    float dashed = step(0.4, frac(along / 15.0 - time * 0.25));
    float lineMask = FE_Line(p, node, center, 1.2, aa) * lerp(dashed, 1.0, connected);
    float3 result = tint * lineMask * 0.65;
    // Tiny packet pulses follow connected cables; missing cables remain dashed.
    float flow = frac(time * _FlowSpeed + node.x * 0.007 + node.y * 0.003);
    result += tint * FE_Circle(p, lerp(node, center, flow), 2.2, aa) * connected * 0.7;
    float square = FE_BoxOutline(p, node - 10.0, float2(20.0, 20.0), 1.5, aa);
    result = lerp(result, FE_Background(float2(0.5, 0.5)), FE_Box(p, node - 9.0, float2(18.0, 18.0), aa));
    result += tint * square;
    if (connected > 0.5)
        result += tint * FE_Box(p, node - 3.5, float2(7.0, 7.0), aa) * 0.85;
    else
        result += tint * max(FE_Line(p, node - 5.0, node + 5.0, 1.4, aa),
                             FE_Line(p, node + float2(-5.0,5.0), node + float2(5.0,-5.0), 1.4, aa));
    return result;
}

float3 FE_RenderSystemLink(float2 meshUV, float seconds)
{
    float t = seconds * _AnimationSpeed + _Seed;
    float2 uv = FE_ScreenUV(meshUV, t);
    float2 p = float2(uv.x, 1.0 - uv.y) * float2(320.0, 320.0);
    float aa = FE_AA(p);
    float3 color = FE_Background(uv) + FE_Title(p, 320.0, FE_LABEL_SYSTEM_LINK, 11, aa);
    float4 links = step(0.5, float4(_LinkPWR, _LinkNET, _LinkCPU, _LinkCORE));
    int count = (int)(links.x + links.y + links.z + links.w);
    float2 center = float2(160.0, 168.0);
    color += FE_LinkNode(p, float2(40.0,108.0), center, links.x, t, aa);
    color += FE_LinkNode(p, float2(280.0,108.0), center, links.y, t, aa);
    color += FE_LinkNode(p, float2(40.0,226.0), center, links.z, t, aa);
    color += FE_LinkNode(p, float2(280.0,226.0), center, links.w, t, aa);

    color += lerp(_WarningColor.rgb, _MainColor.rgb, links.x) * FE_Text(p, float2(19.0,76.0), 2.4, FE_LABEL_PWR, 3, aa);
    color += lerp(_WarningColor.rgb, _MainColor.rgb, links.y) * FE_Text(p, float2(259.0,76.0), 2.4, FE_LABEL_NET, 3, aa);
    color += lerp(_WarningColor.rgb, _MainColor.rgb, links.z) * FE_Text(p, float2(19.0,244.0), 2.4, FE_LABEL_CPU, 3, aa);
    color += lerp(_WarningColor.rgb, _MainColor.rgb, links.w) * FE_Text(p, float2(252.0,244.0), 2.4, FE_LABEL_CORE, 4, aa);

    float hex = FE_HexDistance((p - center).yx, 26.0);
    color = lerp(color, FE_Background(uv), FE_Cover(hex, aa));
    color += _MainColor.rgb * FE_Cover(abs(hex) - 1.0, aa) * 0.9;
    float pulse = 0.55 + 0.15 * sin(t * 2.0);
    color += _MainColor.rgb * FE_Circle(p, center, 8.0, aa) * pulse;

    float size = 2.8;
    float2 footer = float2((320.0 - FE_TextWidth(10, size)) * 0.5, 282.0);
    float text = FE_Digit(p, footer, size, count, aa) +
        FE_Text(p, footer + float2(6.0 * size, 0.0), size, FE_LABEL_OF4_LINKED, 9, aa);
    color += (count == 4 ? _MainColor.rgb : _WarningColor.rgb) * text;
    return FE_Finish(color, uv, t);
}

#endif
