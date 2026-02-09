#if OPENGL
    #define SV_POSITION POSITION
    #define VS_SHADERMODEL vs_3_0
    #define PS_SHADERMODEL ps_3_0
#else
    #define VS_SHADERMODEL vs_4_0_level_9_1
    #define PS_SHADERMODEL ps_4_0_level_9_1
#endif

sampler2D TextureSampler : register(s0);

float4 PixelShaderFunction(float4 position : SV_Position, float4 color : COLOR0, float2 texCoord : TEXCOORD0) : COLOR0
{
    float4 textureColor = tex2D(TextureSampler, texCoord) * color;
    
    // Convert to grayscale using luminosity method
    float gray = dot(textureColor.rgb, float3(0.299, 0.587, 0.114));
    
    return float4(gray, gray, gray, textureColor.a);
}

technique Grayscale
{
    pass Pass1
    {
        PixelShader = compile PS_SHADERMODEL PixelShaderFunction();
    }
}
