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
    /// Sistema que gestiona el renderizado y la actualización de tilemaps.
    /// <para>System that manages tile map rendering and updates.</para>
    /// </summary>
    public class TileMapSystem2D
    {

//-:cnd:noEmit
#if YTB
        /// <summary>
        /// Indica si la vista de juego está activa en modo editor.
        /// <para>Indicates whether the game view is active in editor mode.</para>
        /// </summary>
        public static bool IsGameActive = false;
#endif
//+:cnd:noEmit

        /// <summary>
        /// Referencia al EventManager para manejar eventos.
        /// <para>Event manager reference for handling events.</para>
        /// </summary>
        private EventManager EventManager { get; set; }

        /// <summary>
        /// Referencia al EntityManager para manejar entidades y componentes.
        /// <para>Entity manager reference for entities and components.</para>
        /// </summary>
        private EntityManager EntityManager { get; set; }

        /// <summary>
        /// Inicializa el sistema de tilemaps.
        /// <para>Initializes the tile map system.</para>
        /// </summary>
        /// <param name="entities">Administrador de entidades. <para>Entity manager.</para></param>
        public void InitializeSystem(EntityManager entities)
        {

//-:cnd:noEmit
#if YTB
            if (GameWontRun.GameWontRunByException) return;
#endif
//+:cnd:noEmit
            EventManager = EventManager.Instance;
            EntityManager = entities;

//-:cnd:noEmit
#if YTB
            EventManager.Instance.Subscribe<OnHiddeORShowGameUI>(OnHiddeORShowGameUIFunc);
            EventManager.Instance.Subscribe<OnShowGameUIHiddeEngineEditor>(OnHiddeORShowGameUIFunc);
#endif
//+:cnd:noEmit
        }



//-:cnd:noEmit
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
//+:cnd:noEmit

        /// <summary>
        /// Hook de actualización compartida por entidad (sin uso).
        /// <para>Shared per-entity update hook (unused).</para>
        /// </summary>
        /// <param name="Entidad">Instancia de entidad. <para>Entity instance.</para></param>
        /// <param name="time">Tiempo de juego. <para>Game time.</para></param>
        public void SharedEntityForEachUpdate(Yotsuba Entidad, GameTime time)
        {
        }

        /// <summary>
        /// Hook de inicialización compartida por entidad (sin uso).
        /// <para>Shared per-entity initialization hook (unused).</para>
        /// </summary>
        /// <param name="Entidad">Instancia de entidad. <para>Entity instance.</para></param>
        public void SharedEntityInitialize(Yotsuba Entidad)
        {
        }

        /// <summary>
        /// Actualiza y dibuja los tilemaps del frame actual.
        /// <para>Updates and draws tilemaps for the current frame.</para>
        /// </summary>
        /// <param name="gameTime">Tiempo de juego. <para>Game time.</para></param>
        /// <param name="spriteBatch">Sprite batch para dibujar. <para>Sprite batch for drawing.</para></param>
        public void UpdateSystem(GameTime gameTime, SpriteBatch spriteBatch)
        {

//-:cnd:noEmit
#if YTB
            // FEATURE: Allow tilemap rendering in Editor mode for real-time scene preview
            // The previous early return prevented tilemap rendering in Editor mode.
            // Now tilemaps render continuously regardless of Editor/Game mode.
            if (GameWontRun.GameWontRunByException) return;
            bool canRenderUIElements = IsGameActive || !OperatingSystem.IsWindows();
#endif
//+:cnd:noEmit


            EntityManager entityManager = EntityManager;

//-:cnd:noEmit
#if YTB
            if (entityManager == null) return;
#endif
//+:cnd:noEmit
            var cameraEntity = EntityManager.Camera;
            Matrix viewMatrix = Matrix.Identity;
            if (cameraEntity != null)
            {
                ref var transform = ref EntityManager.TransformComponents[cameraEntity.EntityToFollow];
                var viewport = spriteBatch.GraphicsDevice.Viewport;
                Vector2 screenCenter = new Vector2(viewport.Width / 2f, viewport.Height / 2f);

                // --- Unificar lógica con RenderSystem2D ---
                float currentZoom =
//-:cnd:noEmit
#if YTB
                    canRenderUIElements ? YTBGlobalState.CameraZoom : RenderSystem2D.EDITOR_SCALE_CAMERA;
#else
                    YTBGlobalState.CameraZoom;
                                    if (currentZoom <= 0.001f) currentZoom = 0.001f;
#endif
//+:cnd:noEmit

                Vector2 offset =
//-:cnd:noEmit
#if YTB
                  canRenderUIElements ? YTBGlobalState.OffsetCamera : new Vector2(RenderSystem2D.EDITOR_OFFSET_CAMERA_X, RenderSystem2D.EDITOR_OFFSET_CAMERA_Y);
#else
                    YTBGlobalState.OffsetCamera;
#endif
//+:cnd:noEmit
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

        /// <summary>
        /// Dibuja un tilemap usando la configuración actual.
        /// <para>Draws a tilemap using the current configuration.</para>
        /// </summary>
        /// <param name="spriteBatch">Sprite batch para dibujar. <para>Sprite batch for drawing.</para></param>
        /// <param name="map">Tilemap a renderizar. <para>Tilemap to render.</para></param>
        /// <param name="transform">Transformación del tilemap. <para>Tilemap transform.</para></param>
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
