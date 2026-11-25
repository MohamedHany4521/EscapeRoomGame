// Generated from SourceTools/build_assets.py. Original 5 x 7 pixel font.
#ifndef FALSE_EXIT_PIXEL_FONT_INCLUDED
#define FALSE_EXIT_PIXEL_FONT_INCLUDED
uint2 FE_GlyphBits(uint code)
{
    switch (code)
    {
        case 32u: return uint2(0u, 0u); //  
        case 65u: return uint2(1033774u, 17969u); // A
        case 66u: return uint2(509487u, 15921u); // B
        case 67u: return uint2(33854u, 30753u); // C
        case 68u: return uint2(575023u, 15921u); // D
        case 69u: return uint2(492607u, 31777u); // E
        case 70u: return uint2(492607u, 1057u); // F
        case 71u: return uint2(951358u, 31281u); // G
        case 72u: return uint2(1033777u, 17969u); // H
        case 73u: return uint2(135327u, 31876u); // I
        case 74u: return uint2(270620u, 6441u); // J
        case 75u: return uint2(103729u, 17701u); // K
        case 76u: return uint2(33825u, 31777u); // L
        case 77u: return uint2(710513u, 17969u); // M
        case 78u: return uint2(841329u, 17969u); // N
        case 79u: return uint2(575022u, 14897u); // O
        case 80u: return uint2(509487u, 1057u); // P
        case 81u: return uint2(575022u, 22837u); // Q
        case 82u: return uint2(509487u, 17701u); // R
        case 83u: return uint2(459838u, 15888u); // S
        case 84u: return uint2(135327u, 4228u); // T
        case 85u: return uint2(575025u, 14897u); // U
        case 86u: return uint2(575025u, 4433u); // V
        case 87u: return uint2(706097u, 10933u); // W
        case 88u: return uint2(141873u, 17962u); // X
        case 89u: return uint2(141873u, 4228u); // Y
        case 90u: return uint2(139807u, 31778u); // Z
        case 48u: return uint2(714286u, 14899u); // 0
        case 49u: return uint2(135364u, 14468u); // 1
        case 50u: return uint2(279086u, 31812u); // 2
        case 51u: return uint2(475663u, 15888u); // 3
        case 52u: return uint2(305544u, 8479u); // 4
        case 53u: return uint2(492607u, 15888u); // 5
        case 54u: return uint2(492590u, 14897u); // 6
        case 55u: return uint2(139807u, 2114u); // 7
        case 56u: return uint2(476718u, 14897u); // 8
        case 57u: return uint2(1001006u, 14864u); // 9
        case 47u: return uint2(139792u, 1058u); // /
        case 37u: return uint2(139891u, 26402u); // %
        case 45u: return uint2(1015808u, 0u); // -
        default: return uint2(0u, 0u);
    }
}

#define FE_LABEL_FUSES 0
#define FE_LABEL_SYSTEM_LINK 1
#define FE_LABEL_STABILITY 2
#define FE_LABEL_F1 3
#define FE_LABEL_F2 4
#define FE_LABEL_F3 5
#define FE_LABEL_NO_FUSE 6
#define FE_LABEL_OF3_ONLINE 7
#define FE_LABEL_OF4_LINKED 8
#define FE_LABEL_PWR 9
#define FE_LABEL_NET 10
#define FE_LABEL_CPU 11
#define FE_LABEL_CORE 12
#define FE_LABEL_SIG 13
#define FE_LABEL_SYN 14
#define FE_LABEL_PCT_STABLE 15
#define FE_LABEL_PCT_UNSTABLE 16

uint FE_LabelChar(int label, int index)
{
    switch (label)
    {
        case 0:
            switch (index)
            {
                case 0: return 70u;
                case 1: return 85u;
                case 2: return 83u;
                case 3: return 69u;
                case 4: return 83u;
                default: return 32u;
            }
        case 1:
            switch (index)
            {
                case 0: return 83u;
                case 1: return 89u;
                case 2: return 83u;
                case 3: return 84u;
                case 4: return 69u;
                case 5: return 77u;
                case 6: return 32u;
                case 7: return 76u;
                case 8: return 73u;
                case 9: return 78u;
                case 10: return 75u;
                default: return 32u;
            }
        case 2:
            switch (index)
            {
                case 0: return 83u;
                case 1: return 84u;
                case 2: return 65u;
                case 3: return 66u;
                case 4: return 73u;
                case 5: return 76u;
                case 6: return 73u;
                case 7: return 84u;
                case 8: return 89u;
                default: return 32u;
            }
        case 3:
            switch (index)
            {
                case 0: return 70u;
                case 1: return 49u;
                default: return 32u;
            }
        case 4:
            switch (index)
            {
                case 0: return 70u;
                case 1: return 50u;
                default: return 32u;
            }
        case 5:
            switch (index)
            {
                case 0: return 70u;
                case 1: return 51u;
                default: return 32u;
            }
        case 6:
            switch (index)
            {
                case 0: return 78u;
                case 1: return 79u;
                case 2: return 32u;
                case 3: return 70u;
                case 4: return 85u;
                case 5: return 83u;
                case 6: return 69u;
                default: return 32u;
            }
        case 7:
            switch (index)
            {
                case 0: return 47u;
                case 1: return 51u;
                case 2: return 32u;
                case 3: return 79u;
                case 4: return 78u;
                case 5: return 76u;
                case 6: return 73u;
                case 7: return 78u;
                case 8: return 69u;
                default: return 32u;
            }
        case 8:
            switch (index)
            {
                case 0: return 47u;
                case 1: return 52u;
                case 2: return 32u;
                case 3: return 76u;
                case 4: return 73u;
                case 5: return 78u;
                case 6: return 75u;
                case 7: return 69u;
                case 8: return 68u;
                default: return 32u;
            }
        case 9:
            switch (index)
            {
                case 0: return 80u;
                case 1: return 87u;
                case 2: return 82u;
                default: return 32u;
            }
        case 10:
            switch (index)
            {
                case 0: return 78u;
                case 1: return 69u;
                case 2: return 84u;
                default: return 32u;
            }
        case 11:
            switch (index)
            {
                case 0: return 67u;
                case 1: return 80u;
                case 2: return 85u;
                default: return 32u;
            }
        case 12:
            switch (index)
            {
                case 0: return 67u;
                case 1: return 79u;
                case 2: return 82u;
                case 3: return 69u;
                default: return 32u;
            }
        case 13:
            switch (index)
            {
                case 0: return 83u;
                case 1: return 73u;
                case 2: return 71u;
                default: return 32u;
            }
        case 14:
            switch (index)
            {
                case 0: return 83u;
                case 1: return 89u;
                case 2: return 78u;
                default: return 32u;
            }
        case 15:
            switch (index)
            {
                case 0: return 37u;
                case 1: return 32u;
                case 2: return 83u;
                case 3: return 84u;
                case 4: return 65u;
                case 5: return 66u;
                case 6: return 76u;
                case 7: return 69u;
                default: return 32u;
            }
        case 16:
            switch (index)
            {
                case 0: return 37u;
                case 1: return 32u;
                case 2: return 85u;
                case 3: return 78u;
                case 4: return 83u;
                case 5: return 84u;
                case 6: return 65u;
                case 7: return 66u;
                case 8: return 76u;
                case 9: return 69u;
                default: return 32u;
            }
        default: return 32u;
    }
}

// p is in top-left-origin logical screen pixels. aa is computed before branching.
float FE_Glyph(float2 p, float2 origin, float size, uint code, float aa)
{
    float2 q = (p - origin) / size;
    if (any(q < 0.0) || q.x >= 5.0 || q.y >= 7.0) return 0.0;
    uint bitIndex = (uint)floor(q.y) * 5u + (uint)floor(q.x);
    uint2 bits = FE_GlyphBits(code);
    uint packed = bitIndex < 20u ? bits.x : bits.y;
    uint shift = bitIndex < 20u ? bitIndex : bitIndex - 20u;
    float enabled = (float)((packed >> shift) & 1u);
    float2 cell = abs(frac(q) - 0.5);
    float d = max(cell.x, cell.y) - 0.36;
    float softness = max(aa / size, 0.015);
    return enabled * (1.0 - smoothstep(-softness, softness, d));
}

float FE_Text(float2 p, float2 origin, float size, int label, int count, float aa)
{
    float2 q = p - origin;
    if (any(q < 0.0) || q.x >= (float)count * 6.0 * size || q.y >= 7.0 * size) return 0.0;
    int index = (int)floor(q.x / (6.0 * size));
    return FE_Glyph(p, origin + float2((float)index * 6.0 * size, 0.0),
                    size, FE_LabelChar(label, index), aa);
}

float FE_Digit(float2 p, float2 origin, float size, int digit, float aa)
{
    return FE_Glyph(p, origin, size, (uint)clamp(digit, 0, 9) + 48u, aa);
}

float FE_TextWidth(int count, float size)
{
    return ((float)count * 6.0 - 1.0) * size;
}

#endif
