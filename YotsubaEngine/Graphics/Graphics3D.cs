using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using YotsubaEngine.Core.Component.C_2D;
using YotsubaEngine.Core.Component.C_AGNOSTIC;
using YotsubaEngine.Core.YotsubaGame;

namespace YotsubaEngine.Graphics;
/// <summary>
/// Utilidades para dibujar primitivas 3D básicas.
/// Usa DynamicVertexBuffer pre-allocados en lugar de DrawUserPrimitives
/// para evitar pinning de memoria managed que causa ExecutionEngineException
/// bajo el debugger de Visual Studio.
/// <para>Utilities for drawing basic 3D primitives.
/// Uses pre-allocated DynamicVertexBuffers instead of DrawUserPrimitives
/// to avoid managed memory pinning that causes ExecutionEngineException
/// under the Visual Studio debugger.</para>
/// </summary>
public class Graphics3D : IDisposable
{
    private BasicEffect _basicEffect;

    // Pre-allocated vertex buffers para evitar DrawUserPrimitives
    private DynamicVertexBuffer _boxVertexBuffer;
    private DynamicVertexBuffer _rectVertexBuffer;
    private DynamicVertexBuffer _spriteVertexBuffer;

    // Arrays reutilizables para llenar los buffers sin allocar cada frame
    private readonly VertexPositionColor[] _boxVertices = new VertexPositionColor[36];
    private readonly VertexPositionColor[] _rectVertices = new VertexPositionColor[4];
    private readonly VertexPositionColorTexture[] _spriteVertices = new VertexPositionColorTexture[4];

    private bool _disposed;

    /// <summary>
    /// Inicializa el renderizador 3D básico.
    /// <para>Initializes the basic 3D renderer.</para>
    /// </summary>
    public Graphics3D()
    {
        var gd = YTBGlobalState.GraphicsDevice;

        _basicEffect = new BasicEffect(gd);
        // ESTO ES CLAVE: Le decimos que use los colores de los vértices
        // en lugar de buscar una textura.
        _basicEffect.VertexColorEnabled = true;
        // Desactivar iluminación si quieres colores planos "cartoon" o tipo UI
        _basicEffect.LightingEnabled = false;

        // Pre-allocar los vertex buffers (una sola vez)
        _boxVertexBuffer = new DynamicVertexBuffer(gd, typeof(VertexPositionColor), 36, BufferUsage.WriteOnly);
        _rectVertexBuffer = new DynamicVertexBuffer(gd, typeof(VertexPositionColor), 4, BufferUsage.WriteOnly);
        _spriteVertexBuffer = new DynamicVertexBuffer(gd, typeof(VertexPositionColorTexture), 4, BufferUsage.WriteOnly);
    }

    /// <summary>
    /// Dibuja un cubo 3D en el espacio con colores sólidos.
    /// <para>Draws a 3D box in space with solid colors.</para>
    /// </summary>
    /// <param name="center">Centro del cubo. <para>Box center.</para></param>
    /// <param name="size">Tamaño del cubo. <para>Box size.</para></param>
    /// <param name="color">Color base. <para>Base color.</para></param>
    /// <param name="view">Matriz de vista. <para>View matrix.</para></param>
    /// <param name="projection">Matriz de proyección. <para>Projection matrix.</para></param>
    public void DrawBox(Vector3 center, Vector3 size, Color color, Matrix view, Matrix projection, float rotationX = 0f, float rotationY = 0)
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
        int i = 0;

        // CARA FRONTAL (Z positivo) - Color Frontal
        _boxVertices[i++] = new VertexPositionColor(TLF, frontColor);
        _boxVertices[i++] = new VertexPositionColor(TRF, frontColor);
        _boxVertices[i++] = new VertexPositionColor(BLF, frontColor);

        _boxVertices[i++] = new VertexPositionColor(BLF, frontColor);
        _boxVertices[i++] = new VertexPositionColor(TRF, frontColor);
        _boxVertices[i++] = new VertexPositionColor(BRF, frontColor);

        // CARA TRASERA (Z negativo) - Color Frontal
        _boxVertices[i++] = new VertexPositionColor(TRK, frontColor);
        _boxVertices[i++] = new VertexPositionColor(TLK, frontColor);
        _boxVertices[i++] = new VertexPositionColor(BLK, frontColor);

        _boxVertices[i++] = new VertexPositionColor(BLK, frontColor);
        _boxVertices[i++] = new VertexPositionColor(BRK, frontColor);
        _boxVertices[i++] = new VertexPositionColor(TRK, frontColor);

        // CARA SUPERIOR (Y positivo) - Color Original (Más brillante)
        _boxVertices[i++] = new VertexPositionColor(TLK, topColor);
        _boxVertices[i++] = new VertexPositionColor(TRK, topColor);
        _boxVertices[i++] = new VertexPositionColor(TLF, topColor);

        _boxVertices[i++] = new VertexPositionColor(TLF, topColor);
        _boxVertices[i++] = new VertexPositionColor(TRK, topColor);
        _boxVertices[i++] = new VertexPositionColor(TRF, topColor);

        // CARA INFERIOR (Y negativo) - Color Lateral (Más oscuro)
        _boxVertices[i++] = new VertexPositionColor(BLK, sideColor);
        _boxVertices[i++] = new VertexPositionColor(BLF, sideColor);
        _boxVertices[i++] = new VertexPositionColor(BRF, sideColor);

        _boxVertices[i++] = new VertexPositionColor(BRF, sideColor);
        _boxVertices[i++] = new VertexPositionColor(BRK, sideColor);
        _boxVertices[i++] = new VertexPositionColor(BLK, sideColor);

        // CARA IZQUIERDA (X negativo) - Color Lateral
        _boxVertices[i++] = new VertexPositionColor(TLK, sideColor);
        _boxVertices[i++] = new VertexPositionColor(TLF, sideColor);
        _boxVertices[i++] = new VertexPositionColor(BLK, sideColor);

        _boxVertices[i++] = new VertexPositionColor(BLK, sideColor);
        _boxVertices[i++] = new VertexPositionColor(TLF, sideColor);
        _boxVertices[i++] = new VertexPositionColor(BLF, sideColor);

        // CARA DERECHA (X positivo) - Color Lateral
        _boxVertices[i++] = new VertexPositionColor(TRF, sideColor);
        _boxVertices[i++] = new VertexPositionColor(TRK, sideColor);
        _boxVertices[i++] = new VertexPositionColor(BRK, sideColor);

        _boxVertices[i++] = new VertexPositionColor(BRK, sideColor);
        _boxVertices[i++] = new VertexPositionColor(BRF, sideColor);
        _boxVertices[i++] = new VertexPositionColor(TRF, sideColor);

        // 5. Configurar Effect y Dibujar
        float yaw = MathHelper.ToRadians(rotationX);
        float xaw = MathHelper.ToRadians(rotationY);
        Matrix world = Matrix.CreateRotationY(yaw)
                       * Matrix.CreateRotationX(xaw);

        _basicEffect.World = world;
        _basicEffect.View = view;
        _basicEffect.Projection = projection;
        _basicEffect.VertexColorEnabled = true;

        // Subir datos al GPU buffer y dibujar desde ahí (evita pinning managed)
        _boxVertexBuffer.SetData(_boxVertices, 0, 36, SetDataOptions.Discard);
        YTBGlobalState.GraphicsDevice.SetVertexBuffer(_boxVertexBuffer);

        foreach (EffectPass pass in _basicEffect.CurrentTechnique.Passes)
        {
            pass.Apply();
            YTBGlobalState.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, 12);
        }

        YTBGlobalState.GraphicsDevice.SetVertexBuffer(null);
    }

    /// <summary>
    /// Dibuja un rectángulo en 3D con colores sólidos en el suelo (plano XZ).
    /// <para>Draws a 3D rectangle with solid colors on the ground (XZ plane).</para>
    /// </summary>
    /// <param name="center">Centro del rectángulo. <para>Rectangle center.</para></param>
    /// <param name="width">Ancho del rectángulo. <para>Rectangle width.</para></param>
    /// <param name="height">Alto del rectángulo. <para>Rectangle height.</para></param>
    /// <param name="color">Color base. <para>Base color.</para></param>
    /// <param name="view">Matriz de vista. <para>View matrix.</para></param>
    /// <param name="projection">Matriz de proyección. <para>Projection matrix.</para></param>
    /// <param name="rotation">Rotación en grados. <para>Rotation in degrees.</para></param>
    public void Draw3DRectangle(Vector3 center, float width, float height, Color color, Matrix view, Matrix projection, float rotation = 0f)
    {

        float halfWidth = width * 0.5f;
        float halfHeight = height * 0.5f;

        _rectVertices[0] = new VertexPositionColor(new Vector3(center.X - halfWidth, center.Y, center.Z + halfHeight), color);
        _rectVertices[1] = new VertexPositionColor(new Vector3(center.X + halfWidth, center.Y, center.Z + halfHeight), color);
        _rectVertices[2] = new VertexPositionColor(new Vector3(center.X - halfWidth, center.Y, center.Z - halfHeight), color);
        _rectVertices[3] = new VertexPositionColor(new Vector3(center.X + halfWidth, center.Y, center.Z - halfHeight), color);

        float yaw = MathHelper.ToRadians(rotation);
        Matrix world = Matrix.CreateRotationY(yaw)
                       * Matrix.CreateTranslation(center);


        // 2. Configurar el efecto (Cámara y Transformaciones)
        _basicEffect.World = world; // O usa una matriz de mundo si quieres rotarlo
        _basicEffect.View = view;
        _basicEffect.Projection = projection;

        // 3. Dibujar con DynamicVertexBuffer
        _rectVertexBuffer.SetData(_rectVertices, 0, 4, SetDataOptions.Discard);
        YTBGlobalState.GraphicsDevice.SetVertexBuffer(_rectVertexBuffer);

        foreach (EffectPass pass in _basicEffect.CurrentTechnique.Passes)
        {
            pass.Apply();
            YTBGlobalState.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
        }

        YTBGlobalState.GraphicsDevice.SetVertexBuffer(null);
    }

    /// <summary>
    /// Dibuja un cuadrado en 3D con colores sólidos en el suelo (plano XZ).
    /// <para>Draws a 3D square with solid colors on the ground (XZ plane).</para>
    /// </summary>
    /// <param name="center">Centro del cuadrado. <para>Square center.</para></param>
    /// <param name="size">Tamaño del cuadrado. <para>Square size.</para></param>
    /// <param name="color">Color base. <para>Base color.</para></param>
    /// <param name="view">Matriz de vista. <para>View matrix.</para></param>
    /// <param name="projection">Matriz de proyección. <para>Projection matrix.</para></param>
    /// <param name="rotation">Rotación en grados. <para>Rotation in degrees.</para></param>
    public void DrawSquare(Vector3 center, float size, Color color, Matrix view, Matrix projection, float rotation = 0f)
        => Draw3DRectangle(center, size, size, color, view, projection, rotation);



    /// <summary>
    /// Dibuja un sprite en dimensión 2.5D.
    /// <para>Draws a sprite in 2.5D space.</para>
    /// </summary>
    /// <param name="texture2D">Textura del sprite. <para>Sprite texture.</para></param>
    /// <param name="center">Centro del sprite. <para>Sprite center.</para></param>
    /// <param name="width">Ancho del sprite. <para>Sprite width.</para></param>
    /// <param name="height">Alto del sprite. <para>Sprite height.</para></param>
    /// <param name="color">Color del sprite. <para>Sprite color.</para></param>
    /// <param name="view">Matriz de vista. <para>View matrix.</para></param>
    /// <param name="projection">Matriz de proyección. <para>Projection matrix.</para></param>
    /// <param name="rotation">Rotación en grados. <para>Rotation in degrees.</para></param>
    public void DrawSprite2_5D(Texture2D texture2D, Vector3 center, float width, float height, Color color, Matrix view, Matrix projection, float rotation = 0f)
    {

        float halfWidth = width * 0.5f;
        float halfHeight = height * 0.5f;

        _spriteVertices[0] = new VertexPositionColorTexture(
           new Vector3(-halfWidth, 0, 0),
           color,
           new Vector2(0, 1));

        _spriteVertices[1] = new VertexPositionColorTexture(
            new Vector3(halfWidth, 0, 0),
            color,
            new Vector2(1, 1));

        _spriteVertices[2] = new VertexPositionColorTexture(
            new Vector3(-halfWidth, height, 0),
            color,
            new Vector2(0, 0));

        _spriteVertices[3] = new VertexPositionColorTexture(
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

        // 3. Dibujar con DynamicVertexBuffer
        _spriteVertexBuffer.SetData(_spriteVertices, 0, 4, SetDataOptions.Discard);
        YTBGlobalState.GraphicsDevice.SetVertexBuffer(_spriteVertexBuffer);

        foreach (EffectPass pass in _basicEffect.CurrentTechnique.Passes)
        {
            pass.Apply();
            YTBGlobalState.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
        }

        YTBGlobalState.GraphicsDevice.SetVertexBuffer(null);
        YTBGlobalState.GraphicsDevice.BlendState = previousBlendState;
        YTBGlobalState.GraphicsDevice.DepthStencilState = previousDepthState;
    }

    /// <summary>
    /// Dibuja un sprite en dimensión 2.5D.
    /// <para>Draws a sprite in 2.5D space.</para>
    /// </summary>
    /// <param name="texture2D">Componente de sprite. <para>Sprite component.</para></param>
    /// <param name="center">Centro del sprite. <para>Sprite center.</para></param>
    /// <param name="color">Color del sprite. <para>Sprite color.</para></param>
    /// <param name="view">Matriz de vista. <para>View matrix.</para></param>
    /// <param name="projection">Matriz de proyección. <para>Projection matrix.</para></param>
    /// <param name="rotation">Rotación en grados. <para>Rotation in degrees.</para></param>
    public void DrawSprite2_5D(ref SpriteComponent2D texture2D, Vector3 center, Color color, Matrix view, Matrix projection, float rotation = 0f)
    {

        float width = texture2D.SourceRectangle.Width;
        float height = texture2D.SourceRectangle.Height;

        float x = texture2D.SourceRectangle.X;
        float y = texture2D.SourceRectangle.Y;

        float halfWidth = width * 0.5f;
        float halfHeight = height * 0.5f;

        // Calcula las UVs correctas para la zona de textura
        float u0 = x / (float)texture2D.Texture.Width;
        float v0 = y / (float)texture2D.Texture.Height;
        float u1 = (x + width) / (float)texture2D.Texture.Width;
        float v1 = (y + height) / (float)texture2D.Texture.Height;

        _spriteVertices[0] = new VertexPositionColorTexture(
        new Vector3(-halfWidth, 0, 0),
        color,
        new Vector2(u0, v1)); // esquina inferior izquierda

        _spriteVertices[1] = new VertexPositionColorTexture(
            new Vector3(halfWidth, 0, 0),
            color,
            new Vector2(u1, v1)); // esquina inferior derecha


        _spriteVertices[2] = new VertexPositionColorTexture(
        new Vector3(-halfWidth, height, 0),
        color,
        new Vector2(u0, v0)); // esquina superior izquierda

        _spriteVertices[3] = new VertexPositionColorTexture(
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

        // 3. Dibujar con DynamicVertexBuffer
        _spriteVertexBuffer.SetData(_spriteVertices, 0, 4, SetDataOptions.Discard);
        YTBGlobalState.GraphicsDevice.SetVertexBuffer(_spriteVertexBuffer);

        foreach (EffectPass pass in _basicEffect.CurrentTechnique.Passes)
        {
            pass.Apply();
            YTBGlobalState.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
        }

        YTBGlobalState.GraphicsDevice.SetVertexBuffer(null);
        YTBGlobalState.GraphicsDevice.BlendState = previousBlendState;
        YTBGlobalState.GraphicsDevice.DepthStencilState = previousDepthState;
        YTBGlobalState.GraphicsDevice.SamplerStates[0] = previousSamplerState;
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;

        _boxVertexBuffer?.Dispose();
        _rectVertexBuffer?.Dispose();
        _spriteVertexBuffer?.Dispose();
        _basicEffect?.Dispose();
    }
}
