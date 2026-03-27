using Benchmarks.Core.Components;
using Benchmarks.Core.Entity;
using Benchmarks.Core.Scene;
using Benchmarks.Core.Systems.Contract;
using Benchmarks.Core.Types;

namespace Benchmarks.Core.Systems
{
    /// <summary>
    /// Sistema encargado de dibujar los tilemaps en pantalla.
    /// Simula la iteracion de tiles del engine.
    /// </summary>
    public class TileMapSystem2D : ISystem
    {
        public int TilesDrawn { get; private set; }

        public override void InitializeSystem(EntityManager @entities)
        {
            EntityManager = @entities;
        }

        public void UpdateSystem(GameTime gameTime, Rectangle cameraWorldBounds)
        {
            TilesDrawn = 0;
            if (EntityManager == null) return;

            Span<TransformComponent> transforms = GetTransformComponentsAsSpan();

            foreach (ref Yotsuba entity in GetEntitiesAsSpan())
            {
                if (!entity.HasComponent(YTBComponent.TileMap)) continue;

                ref TileMapComponent2D tilemap = ref EntityManager.TileMapComponent2Ds[entity.Id];
                ref readonly TransformComponent transform = ref transforms[entity.Id];

                float originOffsetX = transform.Size.X * 0.5f * transform.Scale;
                float originOffsetY = transform.Size.Y * 0.5f * transform.Scale;

                foreach (ref TileLayer layer in tilemap.TileLayers.AsSpan())
                {
                    for (int i = 0; i < layer.Data.Length; i++)
                    {
                        int gid = layer.Data[i];
                        if (gid == 0) continue;

                        int tileX = i % tilemap.Width;
                        int tileY = i / tilemap.Width;

                        float worldX = transform.Scale * (tileX * tilemap.TileWidth) + transform.Position.X - originOffsetX;
                        float worldY = transform.Scale * (tileY * tilemap.TileHeight) + transform.Position.Y - originOffsetY;

                        Rectangle tileRect = new Rectangle(
                            (int)worldX,
                            (int)worldY,
                            (int)(tilemap.TileWidth * transform.Scale),
                            (int)(tilemap.TileHeight * transform.Scale)
                        );

                        if (cameraWorldBounds.Intersects(tileRect))
                        {
                            TilesDrawn++;
                        }
                    }
                }
            }
        }

        public override void Dispose() { }
    }
}
