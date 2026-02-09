using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using YotsubaEngine.Core.Component.C_2D;
using YotsubaEngine.Core.Component.C_AGNOSTIC;
using YotsubaEngine.Core.System.YotsubaEngineCore;

namespace YotsubaEngine.Graphics;
public class Graphics3D
{
    private BasicEffect _basicEffect;

    public Graphics3D()
    {
        _basicEffect = new BasicEffect(YTBGlobalState.GraphicsDevice);
        // ESTO ES CLAVE: Le decimos que use los colores de los vértices
        // en lugar de buscar una textura.
        _basicEffect.VertexColorEnabled = true;
        // Desactivar iluminación si quieres colores planos "cartoon" o tipo UI
        _basicEffect.LightingEnabled = false;
    }

    /// <summary>
    /// Metodo para dibujar un cubo 3D en el espacio con colores sólidos.
    /// </summary>
    /// <param name="center"></param>
    /// <param name="size"></param>
    /// <param name="color"></param>
    /// <param name="view"></param>
    /// <param name="projection"></param>
    public void DrawBox(Vector3 center, Vector3 size, Color color, Matrix view, Matrix projection)
    {
        // 1. Calcular las dimensiones medias para encontrar las esquinas
        float x = size.X / 2f;
        float y = size.Y / 2f;
        float z = size.Z / 2f;

        // 2. Definir las 8 esquinas del cubo
        // Nomenclatura: T=Top, B=Bottom, L=Left, R=Right, F=Front, K=Back
        Vector3 TLF = center + new Vector3(-x, y, z);
        Vector3 TRF = center + new Vector3(x, y, z);
        Vector3 BLF = center + new Vector3(-x, -y, z);
        Vector3 BRF = center + new Vector3(x, -y, z);

        Vector3 TLK = center + new Vector3(-x, y, -z);
        Vector3 TRK = center + new Vector3(x, y, -z);
        Vector3 BLK = center + new Vector3(-x, -y, -z);
        Vector3 BRK = center + new Vector3(x, -y, -z);

        // 3. Colores "Falsos" para simular iluminación (Fake Shading)
        // Esto ayuda a ver el volumen sin calcular luces reales
        Color topColor = color;
        Color frontColor = new Color(color.ToVector3() * 0.8f); // 80% brillo
        Color sideColor = new Color(color.ToVector3() * 0.6f);  // 60% brillo

        // 4. Construir la lista de triángulos (6 caras * 2 triángulos * 3 vértices = 36 vértices)
        // Usamos PrimitiveType.TriangleList
        VertexPositionColor[] vertices = new VertexPositionColor[36];
        int i = 0;

        // CARA FRONTAL (Z positivo) - Color Frontal
        vertices[i++] = new VertexPositionColor(TLF, frontColor);
        vertices[i++] = new VertexPositionColor(TRF, frontColor);
        vertices[i++] = new VertexPositionColor(BLF, frontColor);

        vertices[i++] = new VertexPositionColor(BLF, frontColor);
        vertices[i++] = new VertexPositionColor(TRF, frontColor);
        vertices[i++] = new VertexPositionColor(BRF, frontColor);

        // CARA TRASERA (Z negativo) - Color Frontal
        vertices[i++] = new VertexPositionColor(TRK, frontColor);
        vertices[i++] = new VertexPositionColor(TLK, frontColor);
        vertices[i++] = new VertexPositionColor(BLK, frontColor);

        vertices[i++] = new VertexPositionColor(BLK, frontColor);
        vertices[i++] = new VertexPositionColor(BRK, frontColor);
        vertices[i++] = new VertexPositionColor(TRK, frontColor);

        // CARA SUPERIOR (Y positivo) - Color Original (Más brillante)
        vertices[i++] = new VertexPositionColor(TLK, topColor);
        vertices[i++] = new VertexPositionColor(TRK, topColor);
        vertices[i++] = new VertexPositionColor(TLF, topColor);

        vertices[i++] = new VertexPositionColor(TLF, topColor);
        vertices[i++] = new VertexPositionColor(TRK, topColor);
        vertices[i++] = new VertexPositionColor(TRF, topColor);

        // CARA INFERIOR (Y negativo) - Color Lateral (Más oscuro)
        vertices[i++] = new VertexPositionColor(BLK, sideColor);
        vertices[i++] = new VertexPositionColor(BLF, sideColor);
        vertices[i++] = new VertexPositionColor(BRF, sideColor);

        vertices[i++] = new VertexPositionColor(BRF, sideColor);
        vertices[i++] = new VertexPositionColor(BRK, sideColor);
        vertices[i++] = new VertexPositionColor(BLK, sideColor);

        // CARA IZQUIERDA (X negativo) - Color Lateral
        vertices[i++] = new VertexPositionColor(TLK, sideColor);
        vertices[i++] = new VertexPositionColor(TLF, sideColor);
        vertices[i++] = new VertexPositionColor(BLK, sideColor);

        vertices[i++] = new VertexPositionColor(BLK, sideColor);
        vertices[i++] = new VertexPositionColor(TLF, sideColor);
        vertices[i++] = new VertexPositionColor(BLF, sideColor);

        // CARA DERECHA (X positivo) - Color Lateral
        vertices[i++] = new VertexPositionColor(TRF, sideColor);
        vertices[i++] = new VertexPositionColor(TRK, sideColor);
        vertices[i++] = new VertexPositionColor(BRK, sideColor);

        vertices[i++] = new VertexPositionColor(BRK, sideColor);
        vertices[i++] = new VertexPositionColor(BRF, sideColor);
        vertices[i++] = new VertexPositionColor(TRF, sideColor);

        // 5. Configurar Effect y Dibujar
        _basicEffect.World = Matrix.Identity;
        _basicEffect.View = view;
        _basicEffect.Projection = projection;
        _basicEffect.VertexColorEnabled = true;

        foreach (EffectPass pass in _basicEffect.CurrentTechnique.Passes)
        {
            pass.Apply();
            // Nota: PrimitiveCount es el número de triángulos (36 vértices / 3 = 12 triángulos)
            YTBGlobalState.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, vertices, 0, 12);
        }
    }

    /// <summary>
    /// Dibuja un Rectangulo en 3D con colores sólidos en el suelo (plano XZ).
    /// </summary>
    /// <param name="center"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="color"></param>
    /// <param name="view"></param>
    /// <param name="projection"></param>
    public void Draw3DRectangle(Vector3 center, float width, float height, Color color, Matrix view, Matrix projection, float rotation = 0f)
    {
        
        float halfWidth = width * 0.5f;
        float halfHeight = height * 0.5f;

        VertexPositionColor[] vertices = new VertexPositionColor[4];
        vertices[0] = new VertexPositionColor(new Vector3(center.X - halfWidth, center.Y, center.Z + halfHeight), color);
        vertices[1] = new VertexPositionColor(new Vector3(center.X + halfWidth, center.Y, center.Z + halfHeight), color);
        vertices[2] = new VertexPositionColor(new Vector3(center.X - halfWidth, center.Y, center.Z - halfHeight), color);
        vertices[3] = new VertexPositionColor(new Vector3(center.X + halfWidth, center.Y, center.Z - halfHeight), color);

        float yaw = MathHelper.ToRadians(rotation);
        Matrix world = Matrix.CreateRotationY(yaw)
                       * Matrix.CreateTranslation(center);


        // 2. Configurar el efecto (Cámara y Transformaciones)
        _basicEffect.World = world; // O usa una matriz de mundo si quieres rotarlo
        _basicEffect.View = view;
        _basicEffect.Projection = projection;

        // 3. Dibujar
        // TriangleStrip es perfecto para un rectángulo: usa 4 vértices para hacer 2 triángulos automáticamente.
        foreach (EffectPass pass in _basicEffect.CurrentTechnique.Passes)
        {
            pass.Apply();
            YTBGlobalState.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices, 0, 2);
        }
    }

    /// <summary>
    /// Dibuja un cuadrado en 3D con colores sólidos en el suelo (plano XZ).
    /// </summary>
    /// <param name="center"></param>
    /// <param name="size"></param>
    /// <param name="color"></param>
    /// <param name="view"></param>
    /// <param name="projection"></param>
    public void DrawSquare(Vector3 center, float size, Color color, Matrix view, Matrix projection, float rotation = 0f) 
        => Draw3DRectangle(center, size, size, color, view, projection, rotation);



    /// <summary>
    /// Dibuja un sprite en dimencion 2.5D
    /// </summary>
    /// <param name="center"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="color"></param>
    /// <param name="view"></param>
    /// <param name="projection"></param>
    public void DrawSprite2_5D(Texture2D texture2D, Vector3 center, float width, float height, Color color, Matrix view, Matrix projection, float rotation = 0f)
    {

        float halfWidth = width * 0.5f;
        float halfHeight = height * 0.5f;

        VertexPositionColorTexture[] vertices = new VertexPositionColorTexture[4];
        
        vertices[0] = new VertexPositionColorTexture(
           new Vector3(-halfWidth, 0, 0),
           color,
           new Vector2(0, 1));

        vertices[1] = new VertexPositionColorTexture(
            new Vector3(halfWidth, 0, 0),
            color,
            new Vector2(1, 1));

        vertices[2] = new VertexPositionColorTexture(
            new Vector3(-halfWidth, height, 0),
            color,
            new Vector2(0, 0));

        vertices[3] = new VertexPositionColorTexture(
            new Vector3(halfWidth, height, 0),
            color,
            new Vector2(1, 0));

        float yaw = MathHelper.ToRadians(rotation);
        Matrix world = Matrix.CreateRotationY(yaw)
                       * Matrix.CreateTranslation(center);


        // 2. Configurar el efecto (Cámara y Transformaciones)
        _basicEffect.World = world; // O usa una matriz de mundo si quieres rotarlo
        _basicEffect.View = view;
        _basicEffect.Projection = projection;
        _basicEffect.TextureEnabled = true;
        _basicEffect.VertexColorEnabled = false;
        _basicEffect.LightingEnabled = false;
        _basicEffect.Texture = texture2D;

        var previousBlendState = YTBGlobalState.GraphicsDevice.BlendState;
        var previousDepthState = YTBGlobalState.GraphicsDevice.DepthStencilState;
        YTBGlobalState.GraphicsDevice.BlendState = BlendState.AlphaBlend;
        YTBGlobalState.GraphicsDevice.DepthStencilState = DepthStencilState.DepthRead;
        // 3. Dibujar
        // TriangleStrip es perfecto para un rectángulo: usa 4 vértices para hacer 2 triángulos automáticamente.
        foreach (EffectPass pass in _basicEffect.CurrentTechnique.Passes)
        {
            pass.Apply();
            YTBGlobalState.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices, 0, 2);
        }
        YTBGlobalState.GraphicsDevice.BlendState = previousBlendState;
        YTBGlobalState.GraphicsDevice.DepthStencilState = previousDepthState;
    }

    /// <summary>
    /// Dibuja un sprite en dimencion 2.5D
    /// </summary>
    /// <param name="center"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="color"></param>
    /// <param name="view"></param>
    /// <param name="projection"></param>
    public void DrawSprite2_5D(SpriteComponent2D texture2D, Vector3 center, Color color, Matrix view, Matrix projection, float rotation = 0f)
    {

        float width = texture2D.SourceRectangle.Width;
        float height = texture2D.SourceRectangle.Height;

        float x = texture2D.SourceRectangle.X;
        float y = texture2D.SourceRectangle.Y;

        float halfWidth = width * 0.5f;
        float halfHeight = height * 0.5f;

        VertexPositionColorTexture[] vertices = new VertexPositionColorTexture[4];

        // Calcula las UVs correctas para la zona de textura
        float u0 = x / (float)texture2D.Texture.Width;
        float v0 = y / (float)texture2D.Texture.Height;
        float u1 = (x + width) / (float)texture2D.Texture.Width;
        float v1 = (y + height) / (float)texture2D.Texture.Height;

        vertices[0] = new VertexPositionColorTexture(
        new Vector3(-halfWidth, 0, 0),
        color,
        new Vector2(u0, v1)); // esquina inferior izquierda

        vertices[1] = new VertexPositionColorTexture(
            new Vector3(halfWidth, 0, 0),
            color,
            new Vector2(u1, v1)); // esquina inferior derecha


        vertices[2] = new VertexPositionColorTexture(
        new Vector3(-halfWidth, height, 0),
        color,
        new Vector2(u0, v0)); // esquina superior izquierda

        vertices[3] = new VertexPositionColorTexture(
            new Vector3(halfWidth, height, 0),
            color,
            new Vector2(u1, v0)); // esquina superior derecha

        float yaw = MathHelper.ToRadians(rotation);
        Matrix world = Matrix.CreateRotationY(yaw)
                       * Matrix.CreateTranslation(center);


        // 2. Configurar el efecto (Cámara y Transformaciones)
        _basicEffect.World = world; // O usa una matriz de mundo si quieres rotarlo
        _basicEffect.View = view;
        _basicEffect.Projection = projection;
        _basicEffect.TextureEnabled = true;
        _basicEffect.VertexColorEnabled = false;
        _basicEffect.LightingEnabled = false;
        _basicEffect.Texture = texture2D.Texture;

        var previousBlendState = YTBGlobalState.GraphicsDevice.BlendState;
        var previousDepthState = YTBGlobalState.GraphicsDevice.DepthStencilState;
        var previousSamplerState = YTBGlobalState.GraphicsDevice.SamplerStates[0];
        YTBGlobalState.GraphicsDevice.BlendState = BlendState.AlphaBlend;
        YTBGlobalState.GraphicsDevice.DepthStencilState = DepthStencilState.DepthRead;
        YTBGlobalState.GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp; // Pixel art nítido
        // 3. Dibujar
        // TriangleStrip es perfecto para un rectángulo: usa 4 vértices para hacer 2 triángulos automáticamente.
        foreach (EffectPass pass in _basicEffect.CurrentTechnique.Passes)
        {
            pass.Apply();
            YTBGlobalState.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices, 0, 2);
        }
        YTBGlobalState.GraphicsDevice.BlendState = previousBlendState;
        YTBGlobalState.GraphicsDevice.DepthStencilState = previousDepthState;
        YTBGlobalState.GraphicsDevice.SamplerStates[0] = previousSamplerState;
    }
}