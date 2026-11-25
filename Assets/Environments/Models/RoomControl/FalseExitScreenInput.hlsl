#ifndef FALSE_EXIT_SCREEN_INPUT_INCLUDED
#define FALSE_EXIT_SCREEN_INPUT_INCLUDED

// All material data stays in one CBUFFER for URP's SRP Batcher layout.
CBUFFER_START(UnityPerMaterial)
float4 _MainColor;
float4 _WarningColor;
float4 _BackgroundColor;
float4 _UVScaleOffset;
float _Power;
float _Brightness;
float _ScanlineStrength;
float _ScanlineDensity;
float _NoiseStrength;
float _FlickerStrength;
float _Vignette;
float _Curvature;
float _GlitchStrength;
float _AnimationSpeed;
float _Seed;
float _FlipY;

#if defined(FE_SCREEN_FUSES)
float _Fuse1Present;
float _Fuse2Present;
float _Fuse3Present;
float _Fuse1Charge;
float _Fuse2Charge;
float _Fuse3Charge;
float _OnlineThreshold;
#elif defined(FE_SCREEN_SYSTEMLINK)
float _LinkPWR;
float _LinkNET;
float _LinkCPU;
float _LinkCORE;
float _FlowSpeed;
#elif defined(FE_SCREEN_STABILITY)
float _Stability;
float _Signal;
float _SystemPower;
float _Sync;
float _StableThreshold;
float _WaveSpeed;
#endif
CBUFFER_END

#endif
