using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using YotsubaEngine.Core.Component.C_2D;
using YotsubaEngine.Core.Component.C_AGNOSTIC;
using YotsubaEngine.Core.Entity;
using YotsubaEngine.Core.System.YotsubaEngineCore;
using YotsubaEngine.Core.System.YotsubaEngineUI.UI;
using YotsubaEngine.Core.YotsubaGame;

namespace YotsubaEngine.Core.System.S_2D
{
#if YTB
    /// <summary>
    /// Sistema que dibuja overlays de debug para visualizar colisiones, grids y otros elementos de ayuda.
    /// Solo se compila y ejecuta en modo DEBUG.
    /// </summary>
    public class DebugDrawSystem
    {
        private Texture2D _pixel; // Textura de 1x1 para dibujar líneas y rectángulos
        private EntityManager _entityManager;
        private FontSystem2D _fontSystem; // Referencia al sistema de fuentes para medir texto
        private const int GRID_SIZE = 100; // Tamaño de cada celda de la cuadrícula en píxeles
        private const int FONT_HANDLE_SIZE = 24; // Tamaño del recuadro de arrastre para texto

        /// <summary>
        /// Inicializa el sistema de debug draw
        /// </summary>
        public void InitializeSystem(EntityManager entityManager, GraphicsDevice graphicsDevice)
        {
            _entityManager = entityManager;
            YTBGame game = (YTBGame)YTBGame.Instance;
            _fontSystem = game.SceneManager.CurrentScene.FontSystem2D;

            // Crear textura de 1x1 píxel blanco para dibujar formas
            _pixel = new Texture2D(graphicsDevice, 1, 1);
            _pixel.SetData(new[] { Color.Gray });
        }

        /// <summary>
        /// Establece la referencia al FontSystem2D para medir texto
        /// </summary>
        public void SetFontSystem(FontSystem2D fontSystem)
        {
            _fontSystem = fontSystem;
        }

        /// <summary>
        /// Dibuja los overlays de debug activos
        /// </summary>
        public void DrawDebugOverlays(SpriteBatch spriteBatch, Matrix viewMatrix, Rectangle cameraWorldBounds)
        {
            Matrix effectiveViewMatrix = viewMatrix;
            Rectangle effectiveCameraBounds = cameraWorldBounds;

            if (effectiveCameraBounds == Rectangle.Empty && _entityManager?.Camera != null)
            {
#if YTB
                bool canRenderUIElements = RenderSystem2D.IsGameActive || !OperatingSystem.IsWindows();
#else
                bool canRenderUIElements = !OperatingSystem.IsWindows();
#endif

                float currentZoom =
#if YTB
                    canRenderUIElements ?
#endif
                    YTBGlobalState.CameraZoom
#if YTB
                    : RenderSystem2D.EDITOR_SCALE_CAMERA
#endif
                    ;

                if (currentZoom <= 0.001f) currentZoom = 0.001f;

                Vector2 offset =
#if YTB
                    canRenderUIElements ?
#endif
                    YTBGlobalState.OffsetCamera
#if YTB
                    : new Vector2(RenderSystem2D.EDITOR_OFFSET_CAMERA_X, RenderSystem2D.EDITOR_OFFSET_CAMERA_Y)
#endif
                    ;

                ref var camTransform = ref _entityManager.TransformComponents[_entityManager.Camera.EntityToFollow];
                var viewport = spriteBatch.GraphicsDevice.Viewport;
                Vector2 screenCenter = new Vector2(viewport.Width / 2f, viewport.Height / 2f);

                effectiveViewMatrix = _entityManager.Camera.Get2DViewMatrix(
                    new Vector2(camTransform.Position.X, camTransform.Position.Y) + offset,
                    screenCenter,
                    currentZoom,
                    0f
                );

                float visibleWorldWidth = viewport.Width / currentZoom;
                float visibleWorldHeight = viewport.Height / currentZoom;

                Vector2 cameraCenter = new Vector2(camTransform.Position.X, camTransform.Position.Y) + offset;
                float camLeft = cameraCenter.X - (visibleWorldWidth / 2f);
                float camTop = cameraCenter.Y - (visibleWorldHeight / 2f);

                int padding = 100;
                effectiveCameraBounds = new Rectangle(
                    (int)(camLeft - padding),
                    (int)(camTop - padding),
                    (int)(visibleWorldWidth + (padding * 2)),
                    (int)(visibleWorldHeight + (padding * 2))
                );
            }

            //// Solo dibujar si el juego está activo
            //if (!RenderSystem2D.IsGameActive)
            //    return;

            // Dibujar con la transformación de cámara
            spriteBatch.Begin(
                transformMatrix: effectiveViewMatrix,
                sortMode: SpriteSortMode.Deferred,
                blendState: BlendState.AlphaBlend,
                samplerState: SamplerState.PointClamp,
                depthStencilState: DepthStencilState.Default,
                rasterizerState: RasterizerState.CullNone
            );

            // Dibujar cuadrícula si está activa
            if (DebugOverlayUI.ShowGrid)
            {
                DrawGrid(spriteBatch, effectiveCameraBounds);
            }

            // Dibujar colisiones de entidades
            if (DebugOverlayUI.ShowEntityCollisions)
            {
                DrawEntityCollisions(spriteBatch);
            }

            // Dibujar colisiones de tilemap
            if (DebugOverlayUI.ShowTilemapCollisions)
            {
                DrawTilemapCollisions(spriteBatch);
            }

            // Dibujar handles de texto (recuadros en el centro)
            if (DebugOverlayUI.ShowFontHandles)
            {
                DrawFontHandles(spriteBatch);
            }

            spriteBatch.End();
        }

        /// <summary>
        /// Dibuja una cuadrícula tipo plano cartesiano con eje 0,0 más grueso
        /// </summary>
        private void DrawGrid(SpriteBatch spriteBatch, Rectangle cameraWorldBounds)
        {
            Color gridColor = new Color(100, 100, 100, 80); // Gris semi-transparente
            Color axisColor = new Color(200, 200, 200, 150); // Blanco más visible para ejes principales

            int startX = (cameraWorldBounds.Left / GRID_SIZE) * GRID_SIZE;
            int startY = (cameraWorldBounds.Top / GRID_SIZE) * GRID_SIZE;
            int endX = cameraWorldBounds.Right + GRID_SIZE;
            int endY = cameraWorldBounds.Bottom + GRID_SIZE;

            // Dibujar líneas verticales
            for (int x = startX; x <= endX; x += GRID_SIZE)
            {
                Color lineColor = (x == 0) ? axisColor : gridColor;
                int thickness = (x == 0) ? 12 : 7;
                DrawLine(spriteBatch, new Vector2(x, startY), new Vector2(x, endY), lineColor, thickness);
            }

            // Dibujar líneas horizontales
            for (int y = startY; y <= endY; y += GRID_SIZE)
            {
                Color lineColor = (y == 0) ? axisColor : gridColor;
                int thickness = (y == 0) ? 12 : 6;
                DrawLine(spriteBatch, new Vector2(startX, y), new Vector2(endX, y), lineColor, thickness);
            }
        }

        /// <summary>
        /// Dibuja los rectángulos de colisión de las entidades con RigidBodyComponent2D
        /// </summary>
        private void DrawEntityCollisions(SpriteBatch spriteBatch)
        {
            foreach (var entity in _entityManager.YotsubaEntities)
            {
                // Solo dibujar entidades con Transform y RigidBody
                if (!entity.HasComponent(YTBComponent.Transform) || !entity.HasComponent(YTBComponent.Rigibody) || entity.HasComponent(YTBComponent.TileMap))
                    continue;

                ref TransformComponent transform = ref _entityManager.TransformComponents[entity.Id];
                ref RigidBodyComponent2D rigidBody = ref _entityManager.Rigidbody2DComponents[entity.Id];

                // Calcular el rectángulo de colisión
                Rectangle collisionRect = new Rectangle(
                    (int)(transform.Position.X + rigidBody.OffSetCollision.X),
                    (int)(transform.Position.Y + rigidBody.OffSetCollision.Y),
                    (int)(transform.Size.X * transform.Scale),
                    (int)(transform.Size.Y * transform.Scale)
                );

                // Dibujar rectángulo semi-transparente verde
                Color fillColor = new Color(0, 255, 0, 50); // Verde transparente
                Color borderColor = new Color(0, 255, 0, 200); // Verde más visible para bordes

                DrawRectangle(spriteBatch, collisionRect, fillColor, borderColor, 2);
            }
        }

        /// <summary>
        /// Dibuja los rectángulos de colisión del tilemap.
        /// Las colisiones del tilemap se determinan por capas (layers) cuyo nombre contenga "Collision".
        /// </summary>
        private void DrawTilemapCollisions(SpriteBatch spriteBatch)
        {
            foreach (var entity in _entityManager.YotsubaEntities)
            {
                // Solo entidades con tilemap
                if (!entity.HasComponent(YTBComponent.TileMap))
                    continue;

                ref TileMapComponent2D tilemap = ref _entityManager.TileMapComponent2Ds[entity.Id];
                ref TransformComponent transform = ref _entityManager.TransformComponents[entity.Id];

                // Calcular el desplazamiento del origen (igual que en TileMapSystem2D.DrawTileMap)
                // El tilemap usa origin = transform.Size * 0.5, lo que desplaza visualmente el dibujo
                // Para que las colisiones coincidan, restamos ese mismo offset escalado
                float originOffsetX = transform.Size.X * 0.5f * transform.Scale;
                float originOffsetY = transform.Size.Y * 0.5f * transform.Scale;

                // Iterar sobre cada capa del tilemap
                foreach (var layer in tilemap.TileLayers)
                {
                    // Solo procesar capas de colisión (nombre contiene "Collision")
                    if (!layer.Name.Contains("Collision", StringComparison.OrdinalIgnoreCase))
                        continue;

                    // Iterar sobre cada tile de la capa de colisión
                    for (int i = 0; i < layer.Data.Length; i++)
                    {
                        int gid = layer.Data[i];
                        
                        // GID = 0 significa tile vacío, no hay colisión
                        if (gid == 0)
                            continue;

                        // Calcular la posición del tile en el mapa
                        int tileX = i % tilemap.Width;
                        int tileY = i / tilemap.Width;

                        // Calcular la posición en píxeles (igual que TileMapSystem2D)
                        // La fórmula es: transform.Scale * x + transform.Position.X - originOffset
                        float worldX = transform.Scale * (tileX * tilemap.TileWidth) + transform.Position.X - originOffsetX;
                        float worldY = transform.Scale * (tileY * tilemap.TileHeight) + transform.Position.Y - originOffsetY;

                        Rectangle tileCollisionRect = new Rectangle(
                            (int)worldX,
                            (int)worldY,
                            (int)(tilemap.TileWidth * transform.Scale),
                            (int)(tilemap.TileHeight * transform.Scale)
                        );

                        // Dibujar rectángulo semi-transparente rojo
                        Color fillColor = new Color(255, 0, 0, 50); // Rojo transparente
                        Color borderColor = new Color(255, 0, 0, 200); // Rojo más visible para bordes

                        DrawRectangle(spriteBatch, tileCollisionRect, fillColor, borderColor, 2);
                    }
                }
            }
        }

        /// <summary>
        /// Dibuja una línea entre dos puntos
        /// </summary>
        private void DrawLine(SpriteBatch spriteBatch, Vector2 start, Vector2 end, Color color, int thickness = 1)
        {
            Vector2 edge = end - start;
            float angle = (float)Math.Atan2(edge.Y, edge.X);
            float length = edge.Length();

            spriteBatch.Draw(
                _pixel,
                new Rectangle((int)start.X, (int)start.Y, (int)length, thickness),
                null,
                color,
                angle,
                Vector2.Zero,
                SpriteEffects.None,
                0.8f
            );
        }

        /// <summary>
        /// Dibuja un rectángulo relleno con borde
        /// </summary>
        private void DrawRectangle(SpriteBatch spriteBatch, Rectangle rect, Color fillColor, Color borderColor, int borderThickness)
        {
            // Dibujar relleno
            spriteBatch.Draw(_pixel, rect, fillColor);

            // Dibujar bordes
            // Top
            spriteBatch.Draw(_pixel, new Rectangle(rect.X, rect.Y, rect.Width, borderThickness), borderColor);
            // Bottom
            spriteBatch.Draw(_pixel, new Rectangle(rect.X, rect.Bottom - borderThickness, rect.Width, borderThickness), borderColor);
            // Left
            spriteBatch.Draw(_pixel, new Rectangle(rect.X, rect.Y, borderThickness, rect.Height), borderColor);
            // Right
            spriteBatch.Draw(_pixel, new Rectangle(rect.Right - borderThickness, rect.Y, borderThickness, rect.Height), borderColor);
        }

        /// <summary>
        /// Dibuja un rectángulo envolvente (bounding box) alrededor de cada entidad de texto.
        /// </summary>
        private void DrawFontHandles(SpriteBatch spriteBatch)
        {
            if (_fontSystem == null) return;

            foreach (var entity in _entityManager.YotsubaEntities)
            {
                // Solo entidades con FontComponent y Transform
                if (!entity.HasComponent(YTBComponent.Font) || !entity.HasComponent(YTBComponent.Transform))
                    continue;

                ref var fontComponent = ref _entityManager.Font2DComponents[entity.Id];
                ref var transform = ref _entityManager.TransformComponents[entity.Id];

                // Obtener la fuente para medir el texto
                if (!_fontSystem.Fuentes.ContainsKey(fontComponent.Font))
                    continue;

                var font = _fontSystem.Fuentes[fontComponent.Font];
                Vector2 textSize = font.MeasureString(fontComponent.Texto);
                Vector2 scaledTextSize = textSize * transform.Scale;

                // Calcular la posición (top-left)
                Vector2 entityPos = new Vector2(transform.Position.X, transform.Position.Y);

                // ---------------------------------------------------------
                // CAMBIO: Crear rectángulo del tamaño total del texto
                // ---------------------------------------------------------
                Rectangle textBounds = new Rectangle(
                    (int)entityPos.X,
                    (int)entityPos.Y,
                    (int)scaledTextSize.X,
                    (int)scaledTextSize.Y
                );

                // Dibujar el rectángulo envolvente
                Color fillColor = new Color(50, 150, 255, 40); // Azul muy transparente para el fondo
                Color borderColor = new Color(50, 200, 255, 180); // Cyan para el borde

                DrawRectangle(spriteBatch, textBounds, fillColor, borderColor, 2);

                // Se ha eliminado el dibujo de la cruz central para limpiar la vista
            }
        }
    }
#endif
}
