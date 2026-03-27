namespace Benchmarks.Core.Systems

open Benchmarks.Core.Components
open Benchmarks.Core.Entity
open Benchmarks.Core.Scene
open Benchmarks.Core.Systems.Contract
open Benchmarks.Core.Types

type TileMapSystem2D() =
    inherit ISystem()

    member val TilesDrawn = 0 with get, set

    override this.InitializeSystem(entities: EntityManager) =
        this.EntityManager <- entities

    member this.UpdateSystem(gameTime: GameTime, cameraWorldBounds: Rectangle) =
        this.TilesDrawn <- 0
        if isNull this.EntityManager then ()
        else
        let transforms = this.GetTransformComponentsAsSpan()
        let entities = this.GetEntitiesAsSpan()
        for i = 0 to entities.Length - 1 do
            let entity = &entities.[i]
            if entity.HasComponent(YTBComponent.TileMap) then
                let tilemap = &this.EntityManager.TileMapComponent2Ds.[entity.Id]
                let transform = &transforms.[entity.Id]
                let originOffsetX = transform.Size.X * 0.5f * transform.Scale
                let originOffsetY = transform.Size.Y * 0.5f * transform.Scale

                let layers = tilemap.TileLayers.AsSpan()
                for li = 0 to layers.Length - 1 do
                    let layer = &layers.[li]
                    for j = 0 to layer.Data.Length - 1 do
                        let gid = layer.Data.[j]
                        if gid <> 0 then
                            let tileX = j % tilemap.Width
                            let tileY = j / tilemap.Width
                            let worldX = transform.Scale * float32 (tileX * tilemap.TileWidth) + transform.Position.X - originOffsetX
                            let worldY = transform.Scale * float32 (tileY * tilemap.TileHeight) + transform.Position.Y - originOffsetY
                            let tileRect = Rectangle(
                                int worldX, int worldY,
                                int (float32 tilemap.TileWidth * transform.Scale),
                                int (float32 tilemap.TileHeight * transform.Scale))
                            if cameraWorldBounds.Intersects(tileRect) then
                                this.TilesDrawn <- this.TilesDrawn + 1

    override _.Dispose() = ()
