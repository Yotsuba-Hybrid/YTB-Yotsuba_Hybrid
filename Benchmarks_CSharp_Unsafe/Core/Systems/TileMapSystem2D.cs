using System.Runtime.CompilerServices;
using Benchmarks.Core.Components;
using Benchmarks.Core.Entity;
using Benchmarks.Core.Scene;
using Benchmarks.Core.Types;

namespace Benchmarks.Core.Systems
{
    public unsafe struct TileMapSystem2D
    {
        private nint _emPtr;
        public int TilesDrawn;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void InitializeSystem(EntityManager* entities) => _emPtr = (nint)entities;

        public void UpdateSystem(GameTime gameTime, Rectangle cameraWorldBounds)
        {
            TilesDrawn = 0;
            EntityManager* em = (EntityManager*)_emPtr;
            if (em == null) return;

            TransformComponent* transforms = em->TransformComponents.Ptr;
            Yotsuba* entities = em->YotsubaEntities.Ptr;
            int count = em->YotsubaEntities.Count;

            for (int i = 0; i < count; i++)
            {
                ref Yotsuba entity = ref Unsafe.Add(ref Unsafe.AsRef<Yotsuba>(entities), i);
                if (!entity.HasComponent(YTBComponent.TileMap)) continue;

                ref TileMapComponent2D tilemap = ref em->TileMapComponent2Ds[entity.Id];
                ref TransformComponent t = ref Unsafe.Add(ref Unsafe.AsRef<TransformComponent>(transforms), entity.Id);

                float ofsX = t.Size.X * 0.5f * t.Scale;
                float ofsY = t.Size.Y * 0.5f * t.Scale;

                TileLayer* layers = tilemap.TileLayers.Ptr;
                int layerCount = tilemap.TileLayers.Count;

                for (int li = 0; li < layerCount; li++)
                {
                    ref TileLayer layer = ref Unsafe.Add(ref Unsafe.AsRef<TileLayer>(layers), li);
                    int* data = layer.Data;
                    for (int idx = 0; idx < layer.DataLength; idx++)
                    {
                        int gid = Unsafe.Add(ref Unsafe.AsRef<int>(data), idx);
                        if (gid == 0) continue;

                        int tileX = idx % tilemap.Width;
                        int tileY = idx / tilemap.Width;
                        float worldX = t.Scale * (tileX * tilemap.TileWidth) + t.Position.X - ofsX;
                        float worldY = t.Scale * (tileY * tilemap.TileHeight) + t.Position.Y - ofsY;

                        Unsafe.SkipInit(out Rectangle tileRect);
                        tileRect.X = (int)worldX; tileRect.Y = (int)worldY;
                        tileRect.Width = (int)(tilemap.TileWidth * t.Scale);
                        tileRect.Height = (int)(tilemap.TileHeight * t.Scale);

                        if (cameraWorldBounds.Intersects(tileRect))
                            TilesDrawn++;
                    }
                }
            }
        }

        public void Dispose() { }
    }
}
