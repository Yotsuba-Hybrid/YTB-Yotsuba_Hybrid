#if OPENGL
    #define SV_POSITION POSITION
    #define VS_SHADERMODEL vs_3_0
    #define PS_SHADERMODEL ps_3_0
#else
    #define VS_SHADERMODEL vs_4_0_level_9_1
    #define PS_SHADERMODEL ps_4_0_level_9_1
#endif

sampler2D TextureSampler : register(s0);

// Saturation parameter: 0.0 = grayscale, 1.0 = normal, >1.0 = oversaturated
float Saturation = 1.0;

float4 PixelShaderFunction(float4 position : SV_Position, float4 color : COLOR0, float2 texCoord : TEXCOORD0) : COLOR0
{
    float4 textureColor = tex2D(TextureSampler, texCoord) * color;
    
    // Calculate luminance
    float luminance = dot(textureColor.rgb, float3(0.299, 0.587, 0.114));
    float3 grayscale = float3(luminance, luminance, luminance);
    
    // Interpolate between grayscale and original color based on saturation
    float3 saturatedColor = lerp(grayscale, textureColor.rgb, Saturation);
    
    return float4(saturatedColor, textureColor.a);
}

technique Saturate
{
    pass Pass1
    {
        PixelShader = compile PS_SHADERMODEL PixelShaderFunction();
    }
}
