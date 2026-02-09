#if OPENGL
    #define SV_POSITION POSITION
    #define VS_SHADERMODEL vs_3_0
    #define PS_SHADERMODEL ps_3_0
#else
    #define VS_SHADERMODEL vs_4_0_level_9_1
    #define PS_SHADERMODEL ps_4_0_level_9_1
#endif

sampler2D TextureSampler : register(s0);

// Color tint (RGB), intensity in alpha channel
float4 TintColor = float4(1.0, 1.0, 1.0, 0.0);

float4 PixelShaderFunction(float4 position : SV_Position, float4 color : COLOR0, float2 texCoord : TEXCOORD0) : COLOR0
{
    float4 textureColor = tex2D(TextureSampler, texCoord) * color;
    
    // Apply tint by mixing original color with tint color
    float3 tintedColor = lerp(textureColor.rgb, textureColor.rgb * TintColor.rgb, TintColor.a);
    
    return float4(tintedColor, textureColor.a);
}

technique ColorTint
{
    pass Pass1
    {
        PixelShader = compile PS_SHADERMODEL PixelShaderFunction();
    }
}
