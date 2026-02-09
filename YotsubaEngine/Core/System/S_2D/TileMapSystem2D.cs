using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using YotsubaEngine.Core.Component.C_2D;
using YotsubaEngine.Core.Component.C_AGNOSTIC;
using YotsubaEngine.Core.Entity;
using YotsubaEngine.Core.System.YotsubaEngineCore;
using YotsubaEngine.Core.System.YotsubaEngineUI;
using YotsubaEngine.Core.YotsubaGame;
using YotsubaEngine.Events.YTBEvents.EngineEvents;
using YotsubaEngine.Exceptions;

namespace YotsubaEngine.Core.System.S_2D
{
    /// <summary>
    /// System that manages tile map rendering and updates.
    /// Sistema que gestiona el renderizado y la actualización de tilemaps.
    /// </summary>
    public class TileMapSystem2D
    {

#if YTB
        public static bool IsGameActive = false;
#endif

        /// <summary>
        /// Referencia al EventManager para manejar eventos.
        /// </summary>
        private EventManager EventManager { get; set; }

        /// <summary>
        /// Referencia al EntityManager para manejar entidades y componentes.
        /// </summary>
        private EntityManager EntityManager { get; set; }

        public void InitializeSystem(EntityManager entities)
        {

#if YTB
            if (GameWontRun.GameWontRunByException) return;
#endif
            EventManager = EventManager.Instance;
            EntityManager = entities;

#if YTB
            EventManager.Instance.Subscribe<OnHiddeORShowGameUI>(OnHiddeORShowGameUIFunc);
            EventManager.Instance.Subscribe<OnShowGameUIHiddeEngineEditor>(OnHiddeORShowGameUIFunc);
#endif
        }



#if YTB
        private void OnHiddeORShowGameUIFunc(OnShowGameUIHiddeEngineEditor editor)
        {
            IsGameActive = editor.ShowGameUI;
        }
        private void OnHiddeORShowGameUIFunc(OnHiddeORShowGameUI uI)
        {
            IsGameActive = !IsGameActive;
        }
#endif

        public void SharedEntityForEachUpdate(Yotsuba Entidad, GameTime time)
        {
        }

        public void SharedEntityInitialize(Yotsuba Entidad)
        {
        }

        public void UpdateSystem(GameTime gameTime, SpriteBatch spriteBatch)
        {

#if YTB
            // FEATURE: Allow tilemap rendering in Editor mode for real-time scene preview
            // The previous early return prevented tilemap rendering in Editor mode.
            // Now tilemaps render continuously regardless of Editor/Game mode.
            if (GameWontRun.GameWontRunByException) return;
            bool canRenderUIElements = IsGameActive || !OperatingSystem.IsWindows();
#endif


            EntityManager entityManager = EntityManager;
            var cameraEntity = EntityManager.Camera;
            Matrix viewMatrix = Matrix.Identity;
            if (cameraEntity != null)
            {
                ref var transform = ref EntityManager.TransformComponents[cameraEntity.EntityToFollow];
                var viewport = spriteBatch.GraphicsDevice.Viewport;
                Vector2 screenCenter = new Vector2(viewport.Width / 2f, viewport.Height / 2f);

                // --- Unificar lógica con RenderSystem2D ---
                float currentZoom =
#if YTB
                    canRenderUIElements ? YTBGlobalState.CameraZoom : RenderSystem2D.EDITOR_SCALE_CAMERA;
#else
                    YTBGlobalState.CameraZoom;
                                    if (currentZoom <= 0.001f) currentZoom = 0.001f;
#endif

                Vector2 offset =
#if YTB
                  canRenderUIElements ? YTBGlobalState.OffsetCamera : new Vector2(RenderSystem2D.EDITOR_OFFSET_CAMERA_X, RenderSystem2D.EDITOR_OFFSET_CAMERA_Y);
#else
                    YTBGlobalState.OffsetCamera;
#endif
                // Pasamos el screenCenter a tu método actualizado
                viewMatrix = cameraEntity.Get2DViewMatrix(
                    new Vector2(transform.Position.X, transform.Position.Y) + offset,
                    screenCenter,
                    currentZoom
                );

                // --- FIN DEL CAMBIO ---
            }

            spriteBatch.Begin(
                transformMatrix: viewMatrix,
                sortMode: SpriteSortMode.Deferred,
                blendState: BlendState.NonPremultiplied,
                samplerState: SamplerState.PointClamp,
                depthStencilState: DepthStencilState.Default,
                rasterizerState: RasterizerState.CullCounterClockwise
            );

            foreach (var entity in EntityManager.YotsubaEntities)
            {
                if (entity.HasComponent(YTBComponent.TileMap) && entity.HasComponent(YTBComponent.Transform))
                {
                    ref TileMapComponent2D tilemap = ref EntityManager.TileMapComponent2Ds[entity.Id];
                    ref TransformComponent transform = ref EntityManager.TransformComponents[entity.Id];
                    DrawTileMap(spriteBatch, ref tilemap, ref transform);
                }
            }
            spriteBatch.End();
        }

        public void DrawTileMap(SpriteBatch spriteBatch, ref TileMapComponent2D map, ref TransformComponent transform)
        {
            // 1. Recorremos las capas en orden (primero el suelo, luego objetos, etc.)
            foreach (ref var layer in map.TileLayers.AsSpan())
            {
                if (!layer.IsVisible) continue; // Si la capa está oculta, la saltamos
                if (layer.Name.Contains("collision", StringComparison.OrdinalIgnoreCase))
                {
                    layer.IsVisible = false;
                    continue;
                }
                // 2. Recorremos el array de datos (la rejilla)
                for (int i = 0; i < layer.Data.Length; i++)
                {
                    int gid = layer.Data[i]; // Obtenemos el ID del tile en esta celda

                    // GID 0 significa "vacío" en Tiled, no dibujamos nada
                    if (gid == 0) continue;

                    // 3. Intentamos obtener la región visual del diccionario
                    if (map.Tiles.TryGetValue(gid, out TileRegion tileRegion))
                    {
                        // 4. Matemáticas: Convertir índice lineal (0, 1, 2...) a Coordenadas X, Y
                        int columna = i % map.Width;
                        int fila = i / map.Width;

                        // Posición en píxeles en el mundo
                        int x = columna * map.TileWidth;
                        int y = fila * map.TileHeight;

                        // 5. Dibujar usando la TextureRegion que ya tienes lista
                        // Usar la sobrecarga correcta de SpriteBatch.Draw
                        spriteBatch.Draw(
                            tileRegion.TextureRegion.Texture,
                            new Vector2(transform.Scale * x + transform.Position.X, transform.Scale * y + transform.Position.Y), // Posición en pantalla
                            tileRegion.TextureRegion.SourceRectangle, // Recorte de la hoja de sprites
                            transform.Color,                         // Tinte
                            transform.Rotation,                      // Rotación
                            new Vector2(transform.Size.X * 0.5f, transform.Size.Y * 0.5f),                            // Origen (Centro)
                            transform.Scale,                         // Escala
                            transform.SpriteEffects,                 // Efectos de sprite
                            transform.Position.Z                    // Profundidad de capa
                        );
                    }
                }
            }

        }
    }
}