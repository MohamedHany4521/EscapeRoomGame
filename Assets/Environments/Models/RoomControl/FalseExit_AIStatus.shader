// False Exit: self-contained top-strip shader. Only Unity's URP Core.hlsl is required.
Shader "False Exit/Screens/AI Status"
{
    Properties
    {
        [Header(AI Status)]
        [Enum(Offline,0,Booting,1,Online,2,Lockdown,3)] _Status ("Status", Float) = 0
        _BootProgress ("Boot Progress", Range(0,1)) = 0
        _Power ("Display Power", Range(0,1)) = 1
        [Header(Layout)]
        _ScreenAspect ("Screen Width Divided By Height", Range(2,12)) = 6
        _TextScale ("Text Size", Range(1.5,5)) = 4.2
        [Header(Colors)]
        _Brightness ("Brightness", Range(0,5)) = 1.4
        [HDR] _MainColor ("Booting And Online Cyan", Color) = (0.028,0.56,0.66,1)
        [HDR] _WarningColor ("Offline Amber", Color) = (0.95,0.40,0.045,1)
        [HDR] _LockdownColor ("Lockdown Red", Color) = (0.95,0.035,0.045,1)
        _BackgroundColor ("Dark Blue Background", Color) = (0.002,0.012,0.019,1)
        [Header(CRT Effects)]
        _ScanlineStrength ("Scanline Strength", Range(0,1)) = 0.16
        _ScanlineDensity ("Scanline Count", Range(32,400)) = 128
        _NoiseStrength ("Noise Strength", Range(0,0.1)) = 0.003
        _FlickerStrength ("Flicker Strength", Range(0,0.2)) = 0.012
        _PulseStrength ("Warning Pulse Strength", Range(0,0.6)) = 0.18
        _Vignette ("Edge Darkening", Range(0,1)) = 0.2
        _Curvature ("CRT Curvature", Range(0,0.1)) = 0.006
        _GlitchStrength ("Horizontal Glitch", Range(0,1)) = 0
        _AnimationSpeed ("Animation Speed", Range(0,3)) = 1
        _Seed ("Animation Seed", Float) = 0
        [Header(UV Mapping)]
        _UVScaleOffset ("UV Scale XY Offset ZW", Vector) = (1,1,0,0)
        [Toggle] _FlipY ("Flip Vertically", Float) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" "RenderPipeline"="UniversalPipeline" }
        Pass
        {
            Name "AIStatusForward"
            Tags { "LightMode"="SRPDefaultUnlit" }
            Cull Back
            ZWrite On
            ZTest LEqual
            HLSLPROGRAM
            #pragma target 3.5
            #pragma vertex ScreenVertex
            #pragma fragment ScreenFragment
            #pragma multi_compile_instancing
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
            float4 _MainColor, _WarningColor, _LockdownColor, _BackgroundColor;
            float4 _UVScaleOffset;
            float _Status, _BootProgress, _Power, _ScreenAspect, _TextScale;
            float _Brightness, _ScanlineStrength, _ScanlineDensity, _NoiseStrength;
            float _FlickerStrength, _PulseStrength, _Vignette, _Curvature;
            float _GlitchStrength, _AnimationSpeed, _Seed, _FlipY;
            CBUFFER_END

            // Original 5x7 dot font, embedded so this shader has no project include dependencies.
            uint2 FE_TopGlyphBits(uint code) { switch(code) {
            case 32u: return uint2(0u,0u); //  
            case 46u: return uint2(0u,4224u); // .
            case 65u: return uint2(1033774u,17969u); // A
            case 66u: return uint2(509487u,15921u); // B
            case 67u: return uint2(33854u,30753u); // C
            case 68u: return uint2(575023u,15921u); // D
            case 69u: return uint2(492607u,31777u); // E
            case 70u: return uint2(492607u,1057u); // F
            case 71u: return uint2(951358u,31281u); // G
            case 73u: return uint2(135327u,31876u); // I
            case 75u: return uint2(103729u,17701u); // K
            case 76u: return uint2(33825u,31777u); // L
            case 78u: return uint2(841329u,17969u); // N
            case 79u: return uint2(575022u,14897u); // O
            case 84u: return uint2(135327u,4228u); // T
            case 87u: return uint2(706097u,10933u); // W
            default: return uint2(0u,0u); }}
            uint FE_TopStatusChar(int status,int index) { switch(status) {
            case 0: switch(index) {
            case 0: return 65u;
            case 1: return 73u;
            case 2: return 32u;
            case 3: return 79u;
            case 4: return 70u;
            case 5: return 70u;
            case 6: return 76u;
            case 7: return 73u;
            case 8: return 78u;
            case 9: return 69u;
            default: return 32u; }
            case 1: switch(index) {
            case 0: return 66u;
            case 1: return 79u;
            case 2: return 79u;
            case 3: return 84u;
            case 4: return 73u;
            case 5: return 78u;
            case 6: return 71u;
            case 7: return 46u;
            case 8: return 46u;
            case 9: return 46u;
            default: return 32u; }
            case 2: switch(index) {
            case 0: return 65u;
            case 1: return 73u;
            case 2: return 32u;
            case 3: return 79u;
            case 4: return 78u;
            case 5: return 76u;
            case 6: return 73u;
            case 7: return 78u;
            case 8: return 69u;
            default: return 32u; }
            case 3: switch(index) {
            case 0: return 76u;
            case 1: return 79u;
            case 2: return 67u;
            case 3: return 75u;
            case 4: return 68u;
            case 5: return 79u;
            case 6: return 87u;
            case 7: return 78u;
            default: return 32u; }
            default: return 32u; }}

            float FE_TopHash(float2 p)
            {
                float3 p3 = frac(float3(p.x, p.y, p.x) * 0.1031);
                p3 += dot(p3, p3.yzx + 33.33);
                return frac((p3.x + p3.y) * p3.z);
            }
            float FE_TopCover(float d, float aa)
            {
                return 1.0 - smoothstep(-aa, aa, d);
            }
            float FE_TopLine(float2 p, float2 a, float2 b, float thickness, float aa)
            {
                float2 delta = b - a;
                float h = saturate(dot(p-a,delta) / max(dot(delta,delta),0.001));
                return FE_TopCover(length(p - a - h*delta) - thickness*0.5, aa);
            }
            float FE_TopBoxDistance(float2 p, float2 center, float2 halfSize)
            {
                float2 q = abs(p-center)-halfSize;
                return length(max(q,0.0))+min(max(q.x,q.y),0.0);
            }
            float FE_TopGlyph(float2 p, float2 origin, float size, uint code, float aa)
            {
                float2 q = (p-origin)/size;
                if (any(q < 0.0) || q.x >= 5.0 || q.y >= 7.0) return 0.0;
                uint bitIndex = (uint)floor(q.y)*5u+(uint)floor(q.x);
                uint2 bits = FE_TopGlyphBits(code);
                uint packed = bitIndex < 20u ? bits.x : bits.y;
                uint shift = bitIndex < 20u ? bitIndex : bitIndex-20u;
                float lit = (float)((packed >> shift) & 1u);
                float2 dotUV = abs(frac(q)-0.5);
                return lit*FE_TopCover(max(dotUV.x,dotUV.y)-0.37,max(aa/size,0.015));
            }
            float FE_TopText(float2 p, float2 origin, float size, int status, int count, float aa)
            {
                float2 q=p-origin;
                if (any(q < 0.0) || q.x >= (float)count*6.0*size || q.y >= 7.0*size) return 0.0;
                int index=(int)floor(q.x/(6.0*size));
                return FE_TopGlyph(p,origin+float2((float)index*6.0*size,0.0),size,FE_TopStatusChar(status,index),aa);
            }
            float FE_TopIcon(float2 p, float2 c, int status, float t, float aa)
            {
                float2 q=p-c;
                if (status == 0)
                {
                    float shape=max(FE_TopLine(q,float2(0,-14),float2(-14,11),1.5,aa),
                                    FE_TopLine(q,float2(-14,11),float2(14,11),1.5,aa));
                    shape=max(shape,FE_TopLine(q,float2(14,11),float2(0,-14),1.5,aa));
                    shape=max(shape,FE_TopLine(q,float2(0,-6),float2(0,1),2.1,aa));
                    return max(shape,FE_TopCover(length(q-float2(0,6))-1.4,aa));
                }
                if (status == 1)
                {
                    float angle=frac(atan2(q.y,q.x)/6.2831853-t*0.7);
                    float ring=FE_TopCover(abs(length(q)-10.0)-1.5,aa);
                    return ring*lerp(0.12,1.0,step(0.67,angle));
                }
                if (status == 2)
                {
                    float ring=FE_TopCover(abs(length(q)-10.0)-0.8,aa)*0.4;
                    return max(ring,FE_TopCover(length(q)-5.2,aa));
                }
                float body=FE_TopCover(abs(FE_TopBoxDistance(q,float2(0,4),float2(8,7)))-0.9,aa);
                float shackle=FE_TopCover(abs(length(q-float2(0,-3))-5.0)-1.1,aa)*step(q.y,-3.0);
                float key=FE_TopCover(length(q-float2(0,3))-1.8,aa);
                key=max(key,FE_TopLine(q,float2(0,3),float2(0,7),1.6,aa));
                return max(max(body,shackle),key);
            }
            float3 FE_RenderAIStatus(float2 meshUV, float seconds)
            {
                float t=seconds*max(_AnimationSpeed,0.0)+_Seed;
                float2 uv=meshUV*_UVScaleOffset.xy+_UVScaleOffset.zw;
                uv.y=lerp(uv.y,1.0-uv.y,step(0.5,_FlipY));
                float2 q=uv*2.0-1.0;
                uv+=(uv-0.5)*dot(q,q)*_Curvature;
                float frame=floor(t*12.0);
                float glitch=step(0.92,FE_TopHash(float2(frame,7.0)));
                uv.x+=(FE_TopHash(float2(floor(uv.y*16.0),frame))-0.5)*glitch*_GlitchStrength*0.05;
                float width=64.0*clamp(_ScreenAspect,2.0,12.0);
                float2 p=float2(uv.x,1.0-uv.y)*float2(width,64.0);
                float aa=max(0.12,0.6*max(length(ddx(p)),length(ddy(p))));
                int status=(int)clamp(floor(_Status+0.5),0.0,3.0);
                int count=status == 2 ? 9 : (status == 3 ? 8 : 10);
                float size=min(max(_TextScale,0.1),(width-70.0)/((float)count*6.0-1.0));
                float textWidth=((float)count*6.0-1.0)*size;
                float groupWidth=textWidth+42.0;
                float left=(width-groupWidth)*0.5;
                bool hasBar=status == 1 || status == 2;
                float top=hasBar ? 10.0 : (64.0-7.0*size)*0.5;
                float2 textOrigin=float2(left+42.0,top);
                float2 iconCenter=float2(left+14.0,top+3.5*size);
                float3 tint=status == 0 ? _WarningColor.rgb : (status == 3 ? _LockdownColor.rgb : _MainColor.rgb);
                float pulse=1.0;
                if (status == 0 || status == 3)
                    pulse=1.0-_PulseStrength*(0.5+0.5*sin(t*(status == 3 ? 5.0 : 2.5)));
                float ui=FE_TopText(p,textOrigin,size,status,count,aa);
                ui=max(ui,FE_TopIcon(p,iconCenter,status,t,aa));
                float3 color=_BackgroundColor.rgb*(0.9+0.2*(1.0-saturate(length(uv-0.5))))+tint*ui*pulse;
                if (hasBar)
                {
                    float barWidth=width-56.0;
                    float2 barCenter=float2(width*0.5,54.0);
                    float border=FE_TopCover(abs(FE_TopBoxDistance(p,barCenter,float2(barWidth*0.5,3.5)))-0.5,aa);
                    color+=tint*border*0.25;
                    float2 local=p-float2(30.0,52.0);
                    float innerWidth=barWidth-4.0;
                    if (all(local >= 0.0) && local.x <= innerWidth && local.y <= 4.0)
                    {
                        float cellWidth=innerWidth/24.0;
                        float index=floor(local.x/cellWidth);
                        float cellX=frac(local.x/cellWidth)*cellWidth;
                        float shape=FE_TopCover(abs(cellX-cellWidth*0.5)-(cellWidth*0.5-0.6),aa);
                        float progress=status == 2 ? 1.0 : saturate(_BootProgress);
                        float active=1.0-step(ceil(progress*24.0),index);
                        color+=tint*shape*lerp(0.055,0.8,active);
                    }
                }
                float scanPhase=uv.y*_ScanlineDensity;
                float scan=1.0-_ScanlineStrength*saturate(1.0-fwidth(scanPhase))*(0.5+0.5*cos(scanPhase*6.2831853));
                float noise=FE_TopHash(floor(uv*float2(1536.0,256.0))+floor(t*20.0))-0.5;
                float flicker=1.0-_FlickerStrength*(0.5+0.25*sin(t*71.0)+0.25*sin(t*43.0));
                float vignette=1.0-_Vignette*saturate(dot(q,q)*0.5);
                color=max(0.0,color*scan+noise*_NoiseStrength)*max(_Brightness,0.0)*flicker*vignette;
                float inside=step(0.0,uv.x)*step(0.0,uv.y)*step(uv.x,1.0)*step(uv.y,1.0);
                return lerp(_BackgroundColor.rgb*0.12,color,saturate(_Power)*inside);
            }

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_OUTPUT_STEREO
            };
            Varyings ScreenVertex(Attributes input)
            {
                Varyings output=(Varyings)0;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
                output.positionCS=TransformObjectToHClip(input.positionOS.xyz);
                output.uv=input.uv;
                return output;
            }
            half4 ScreenFragment(Varyings input) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
                return half4(FE_RenderAIStatus(input.uv,_Time.y),1.0);
            }
            ENDHLSL
        }
    }
    FallBack Off
}
