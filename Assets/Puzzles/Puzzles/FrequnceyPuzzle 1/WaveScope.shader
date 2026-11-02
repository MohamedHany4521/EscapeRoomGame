Shader "Custom/WaveScope"
{
 Properties
    {
        _BG ("Background", Color) = (0.05,0.18,0.08,1)
        _ColA ("Player Wave", Color) = (1,0.55,0.1,1)
        _ColB ("Target Wave", Color) = (0.6,1,0.8,1)
        _AmpA ("Amp A", Range(0,0.45)) = 0.2
        _FreqA ("Freq A", Range(0.5,10)) = 3
        _PhaseA ("Phase A", Range(0,1)) = 0
        _AmpB ("Amp B", Range(0,0.45)) = 0.3
        _FreqB ("Freq B", Range(0.5,10)) = 4
        _PhaseB ("Phase B", Range(0,1)) = 0.25
        _Thick ("Thickness", Range(0.001,0.03)) = 0.006
        _Glow ("Glow Falloff", Range(5,150)) = 60
        _Intensity ("Intensity", Range(0,5)) = 0.9
        _Scan ("Scanlines", Range(0,0.3)) = 0.1
        _Aspect ("Screen Aspect (W/H)", Float) = 1.3
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            float4 _BG, _ColA, _ColB;
            float _AmpA,_FreqA,_PhaseA,_AmpB,_FreqB,_PhaseB;
            float _Thick,_Glow,_Intensity,_Scan,_Aspect;

            struct v2f { float4 pos:SV_POSITION; float2 uv:TEXCOORD0; };

            v2f vert(appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord.xy;
                return o;
            }

            float Line(float2 uv, float amp, float freq, float ph)
            {
                float d = 1;
                [unroll] for (int k = -12; k <= 12; k++)
                {
                    float x = uv.x + k * 0.003;
                    float y = 0.5 + amp * sin((x * freq + ph) * 6.28318);
                    float2 p = float2((x - uv.x) * _Aspect, uv.y - y);
                    d = min(d, length(p));
                }
                return smoothstep(_Thick, 0, d) + exp(-d * _Glow) * 0.15;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float lA = Line(i.uv, _AmpA, _FreqA, _PhaseA);
                float lB = Line(i.uv, _AmpB, _FreqB, _PhaseB);
                float3 col = _BG.rgb + (lA * _ColA.rgb + lB * _ColB.rgb) * _Intensity;
                col = min(col, 1.5);
                col *= 1 - _Scan * (0.5 + 0.5 * sin(i.uv.y * 400));
                return float4(col, 1);
            }
            ENDCG
        }
    }

}
