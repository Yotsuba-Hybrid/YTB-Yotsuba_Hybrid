// ==========================================================
// Macros para compatibilidad multiplataforma (DirectX / OpenGL)
// ==========================================================
#if OPENGL
    #define SV_POSITION POSITION
    #define SV_TARGET COLOR0    
    #define VS_SHADERMODEL vs_3_0
    #define PS_SHADERMODEL ps_3_0
#else
    #define SV_TARGET SV_Target0  
    #define VS_SHADERMODEL vs_6_0
    #define PS_SHADERMODEL ps_6_0
#endif

// ==========================================================
// 1. Parámetros Globales (Los que envías desde C#)
// Global Parameters (The ones you send from C#)
// ==========================================================
// Tienen que llamarse EXACTAMENTE igual que en tu C#: Parameters["World"]
float4x4 World;
float4x4 View;
float4x4 Projection;

// Textura del modelo (opcional, pero recomendada)
Texture2D ModelTexture;

// Usar 'sampler' en lugar de 'sampler2D' para compatibilidad con Vulkan
sampler TextureSampler = sampler_state
{
    Texture = <ModelTexture>;
    AddressU = Wrap;
    AddressV = Wrap;
};

// ==========================================================
// 2. Estructuras de Datos
// Data Structures
// ==========================================================
// Lo que entra al shader desde el modelo 3D (Vertices crudos)
struct VertexInput
{
    float4 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;
};

// Lo que sale del Vertex Shader hacia el Pixel Shader
struct VertexOutput
{
    float4 Position : SV_POSITION;
    float2 TexCoord : TEXCOORD0;
};

// ==========================================================
// 3. Vertex Shader (Donde ocurre la magia de la cámara)
// Vertex Shader (Where the camera magic happens)
// ==========================================================
VertexOutput MainVS(in VertexInput input)
{
    VertexOutput output;

    // EL ORDEN MATEMÁTICO ES VITAL AQUÍ:
    // 1. Multiplicamos la posición local por el Mundo (hueso + posición de entidad)
    float4 worldPosition = mul(input.Position, World);
    
    // 2. Multiplicamos la posición del mundo por la Vista (la posición de tu cámara)
    float4 viewPosition = mul(worldPosition, View);
    
    // 3. Multiplicamos la posición de vista por la Proyección (el lente/FOV de tu cámara)
    output.Position = mul(viewPosition, Projection);

    // Pasamos las coordenadas UV de la textura intactas
    output.TexCoord = input.TexCoord;

    return output;
}

// ==========================================================
// 4. Pixel Shader (Color)
// ==========================================================
float4 MainPS(VertexOutput input) : SV_TARGET
{
    // Si no usas textura, puedes retornar un color fijo ej: return float4(1, 0, 0, 1);
    return ModelTexture.Sample(TextureSampler, input.TexCoord);
}

// ==========================================================
// 5. Definición de la Técnica
// Technique Definition
// ==========================================================
technique BasicShader
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL MainVS();
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
}