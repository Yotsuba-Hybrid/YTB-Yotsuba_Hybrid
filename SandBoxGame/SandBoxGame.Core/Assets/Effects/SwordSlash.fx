#if OPENGL
#define SV_POSITION POSITION
#define SV_TARGET COLOR0
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#define SAMPLE_TEXTURE(tex, samp, uv) tex2D(samp, uv)
#else
#define SV_TARGET SV_Target0
#define VS_SHADERMODEL vs_6_0
#define PS_SHADERMODEL ps_6_0
#define SAMPLE_TEXTURE(tex, samp, uv) tex.Sample(samp, uv)
#endif

// =============================
// CONFIG
// =============================
#define USE_NOISE 1

// =============================
// MATRICES
// =============================
float4x4 WorldViewProjection;

// =============================
// PARAMS
// =============================
float Time;
float4 TintColor;
float GlowIntensity;

// =============================
// TEXTURA
// =============================
Texture2D SlashTexture : register(t0);
sampler TextureSampler : register(s0);

// =============================
// INPUT / OUTPUT
// =============================
struct VS_INPUT
{
    float4 Position : POSITION0;
    float4 Color : COLOR0;
    float2 TexCoord : TEXCOORD0;
};

struct VS_OUTPUT
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
    float2 TexCoord : TEXCOORD0;
};

// =============================
// VERTEX SHADER
// =============================
VS_OUTPUT VSMain(VS_INPUT input)
{
    VS_OUTPUT output;

    output.Position = mul(input.Position, WorldViewProjection);
    output.Color = input.Color;
    output.TexCoord = input.TexCoord;

    return output;
}

// =============================
// NOISE (sin intrinsics viejos)
// =============================
float Hash(float2 p)
{
    return frac(sin(dot(p, float2(127.1, 311.7))) * 43758.5453);
}

float Noise(float2 p)
{
    float2 i = floor(p);
    float2 f = frac(p);

    float a = Hash(i);
    float b = Hash(i + float2(1, 0));
    float c = Hash(i + float2(0, 1));
    float d = Hash(i + float2(1, 1));

    float2 u = f * f * (3.0 - 2.0 * f);

    return lerp(a, b, u.x) +
           (c - a) * u.y * (1.0 - u.x) +
           (d - b) * u.x * u.y;
}

// =============================
// PIXEL SHADER
// =============================
float4 PSMain(VS_OUTPUT input) : SV_TARGET
{
    float2 uv = input.TexCoord;

    // gradiente tipo arco
    float edge = smoothstep(0.0, 0.2, uv.y) * smoothstep(1.0, 0.8, uv.y);
    float glow = pow(edge, 1.5);

#if USE_NOISE
    float n = Noise(uv * 8 + Time * 6);
    glow *= lerp(0.8, 1.4, n);
#endif

    float4 tex = SAMPLE_TEXTURE(SlashTexture, TextureSampler, uv);
    float4 finalColor = input.Color * TintColor;
    finalColor.rgb *= glow * GlowIntensity;
    finalColor.a *= glow;

    return finalColor * tex;
}

// =============================
// TECHNIQUE (Macros aplicadas)
// =============================
technique Basic
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL VSMain();
        PixelShader = compile PS_SHADERMODEL PSMain();
    }
}