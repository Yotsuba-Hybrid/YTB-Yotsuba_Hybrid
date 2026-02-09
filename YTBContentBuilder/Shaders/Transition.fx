#if OPENGL
    #define SV_POSITION POSITION
    #define VS_SHADERMODEL vs_3_0
    #define PS_SHADERMODEL ps_3_0
#else
    #define VS_SHADERMODEL vs_4_0_level_9_1
    #define PS_SHADERMODEL ps_4_0_level_9_1
#endif

sampler2D TextureSampler : register(s0);
sampler2D TransitionTextureSampler : register(s1);

// Progress: 0.0 = fully scene A, 1.0 = fully scene B
float Progress = 0.0;

// Type of transition: 0 = fade, 1 = dissolve
float TransitionType = 0.0;

// Threshold for dissolve effect
float DissolveThreshold = 0.5;

float4 PixelShaderFunction(float4 position : SV_Position, float4 color : COLOR0, float2 texCoord : TEXCOORD0) : COLOR0
{
    float4 textureColor = tex2D(TextureSampler, texCoord) * color;
    
    if (TransitionType < 0.5)
    {
        // Fade transition
        textureColor.a *= (1.0 - Progress);
    }
    else
    {
        // Dissolve transition using a noise texture
        float4 transitionColor = tex2D(TransitionTextureSampler, texCoord);
        float dissolveValue = transitionColor.r;
        
        // Discard pixels based on dissolve progress
        if (dissolveValue < Progress)
        {
            textureColor.a = 0.0;
        }
    }
    
    return textureColor;
}

technique Transition
{
    pass Pass1
    {
        PixelShader = compile PS_SHADERMODEL PixelShaderFunction();
    }
}
