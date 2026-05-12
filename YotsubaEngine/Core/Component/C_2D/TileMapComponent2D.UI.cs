#if YTB
using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Hexa.NET.ImGui;
using YotsubaEngine.Core.System.YotsubaEngineUI.UI;

namespace YotsubaEngine.Core.Component.C_2D
{
    public partial struct TileMapComponent2D
    {
        /// <summary>
        /// Controles extra del TileMapComponent2D: botón "Aplicar dimensiones al transform" que
        /// lee el TMX seleccionado y escribe Size en el TransformComponent hermano.
        /// </summary>
        public static void RenderExtraControls(IUIRenderContext ctx)
        {
            var entity = ctx.CurrentEntity;
            if (entity == null) return;

            var tilemapProp = ctx.Component.Propiedades.FirstOrDefault(p => p.Item1 == "TileMapPath");
            if (tilemapProp == null || string.IsNullOrWhiteSpace(tilemapProp.Item2)) return;

            if (ImGui.Button("Aplicar dimensiones al componente de transformación"))
            {
                ApplyTileMapDimensionsToTransform(ctx, tilemapProp.Item2);
            }
        }

        private static void ApplyTileMapDimensionsToTransform(IUIRenderContext ctx, string tmxPath)
        {
            var entity = ctx.CurrentEntity;
            if (entity == null) return;

            var transformComp = entity.Components.FirstOrDefault(c => c.ComponentName == "TransformComponent");
            if (transformComp == null)
            {
                Console.WriteLine("Error: No se encontró TransformComponent en la entidad.");
                return;
            }

            try
            {
                string sanitizedPath = tmxPath.TrimStart('\\', '/');
                string fullPath = Path.GetFullPath(Path.Combine(ctx.ContentPath, sanitizedPath));
                string contentFullPath = Path.GetFullPath(ctx.ContentPath);

                if (!fullPath.StartsWith(contentFullPath, StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine($"Error: Ruta de archivo TMX inválida: {tmxPath}");
                    return;
                }

                if (!File.Exists(fullPath))
                {
                    Console.WriteLine($"Error: Archivo TMX no encontrado: {fullPath}");
                    return;
                }

                var doc = XDocument.Load(fullPath);
                var mapElement = doc.Root;
                if (mapElement == null) return;

                if (!int.TryParse(mapElement.Attribute("width")?.Value, out int width) || width <= 0) return;
                if (!int.TryParse(mapElement.Attribute("height")?.Value, out int height) || height <= 0) return;
                if (!int.TryParse(mapElement.Attribute("tilewidth")?.Value, out int tileWidth) || tileWidth <= 0) return;
                if (!int.TryParse(mapElement.Attribute("tileheight")?.Value, out int tileHeight) || tileHeight <= 0) return;

                int totalWidth = width * tileWidth;
                int totalHeight = height * tileHeight;

                // Actualizamos el Size del Transform directamente sobre su lista de propiedades.
                var sizeProp = transformComp.Propiedades.FirstOrDefault(p => p.Item1 == "Size");
                if (sizeProp != null)
                {
                    int index = transformComp.Propiedades.IndexOf(sizeProp);
                    transformComp.Propiedades[index] = new Tuple<string, string>("Size", $"{totalWidth},{totalHeight},0");
                }

                Console.WriteLine($"Dimensiones del tilemap aplicadas: {totalWidth}x{totalHeight}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error aplicando dimensiones del tilemap: {ex.Message}");
            }
        }
    }
}
#endif
