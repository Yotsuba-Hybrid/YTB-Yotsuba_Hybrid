#if OPENGL
    #define SV_POSITION POSITION
    #define VS_SHADERMODEL vs_3_0
    #define PS_SHADERMODEL ps_3_0
#else
    #define VS_SHADERMODEL vs_4_0_level_9_1
    #define PS_SHADERMODEL ps_4_0_level_9_1
#endif

sampler2D TextureSampler : register(s0);

// Brightness: -1.0 to 1.0 (default 0.0)
float Brightness = 0.0;

// Contrast: 0.0 to 2.0 (default 1.0)
float Contrast = 1.0;

float4 PixelShaderFunction(float4 position : SV_Position, float4 color : COLOR0, float2 texCoord : TEXCOORD0) : COLOR0
{
    float4 textureColor = tex2D(TextureSampler, texCoord) * color;
    
    // Apply brightness
    float3 brightColor = textureColor.rgb + Brightness;
    
    // Apply contrast (contrast formula: (color - 0.5) * contrast + 0.5)
    float3 contrastColor = (brightColor - 0.5) * Contrast + 0.5;
    
    // Clamp to valid range
    contrastColor = saturate(contrastColor);
    
    return float4(contrastColor, textureColor.a);
}

technique BrightnessContrast
{
    pass Pass1
    {
        PixelShader = compile PS_SHADERMODEL PixelShaderFunction();
    }
}
