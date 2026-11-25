// False Exit - procedural URP screen. Requires the adjacent HLSL includes.
Shader "False Exit/Screens/System Link"
{
    Properties
    {
        [Header(Cable Puzzle)]
        [Toggle] _LinkPWR ("PWR Connected", Float) = 1
        [Toggle] _LinkNET ("NET Connected", Float) = 1
        [Toggle] _LinkCPU ("CPU Connected", Float) = 1
        [Toggle] _LinkCORE ("CORE Connected", Float) = 0
        _FlowSpeed ("Packet Flow Speed", Range(0,2)) = 0.35

        [Header(Display)]
        _Power ("Screen Power", Range(0,1)) = 1
        _Brightness ("Screen Brightness", Range(0,5)) = 1.4
        [HDR] _MainColor ("Online Cyan", Color) = (0.028,0.56,0.66,1)
        [HDR] _WarningColor ("Fault Amber", Color) = (0.95,0.40,0.045,1)
        _BackgroundColor ("Dark Blue Background", Color) = (0.002,0.012,0.019,1)
        [Header(CRT Effects)]
        _ScanlineStrength ("Scanline Strength", Range(0,1)) = 0.16
        _ScanlineDensity ("Scanline Count", Range(80,800)) = 320
        _NoiseStrength ("Noise Strength", Range(0,0.1)) = 0.008
        _FlickerStrength ("Flicker Strength", Range(0,0.2)) = 0.012
        _Vignette ("Edge Darkening", Range(0,1)) = 0.28
        _Curvature ("CRT Curvature", Range(0,0.15)) = 0.012
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
            Name "ScreenForward"
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
            #define FE_SCREEN_SYSTEMLINK 1
            #include "FalseExitScreenInput.hlsl"
            #include "FalseExitSystemLink.hlsl"

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
                Varyings output = (Varyings)0;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = input.uv;
                return output;
            }
            half4 ScreenFragment(Varyings input) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
                return half4(FE_RenderSystemLink(input.uv, _Time.y), 1.0);
            }
            ENDHLSL
        }
    }
    FallBack Off
}
