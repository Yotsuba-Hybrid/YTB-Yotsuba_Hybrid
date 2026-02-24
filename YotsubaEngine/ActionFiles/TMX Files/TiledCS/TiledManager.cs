using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using YotsubaEngine.ActionFiles.YTB_Files;
using YotsubaEngine.Core.Component.C_2D;
using YotsubaEngine.Core.System.YotsubaEngineUI;
using YotsubaEngine.Graphics;
using YotsubaEngine.HighestPerformanceTypes;

namespace YotsubaEngine.ActionFiles.TMX_Files.TiledCS
{
    /// <summary>
    /// Gestor encargado de la lectura, parseo y conversión de archivos de mapas de Tiled (.tmx)
    /// a componentes utilizables por el motor (TileMapComponent2D).
    /// <para>Manager responsible for reading and converting Tiled (.tmx) maps into engine components.</para>
    /// </summary>
    public static class TiledManager
    {

        /// <summary>
        /// Metodo busca todos los archivos tmx de tiled en la carpeta content.
        /// <para>Searches for all .tmx files in the content folder.</para>
        /// </summary>
        /// <returns>Rutas relativas de archivos .tmx. <para>Relative .tmx file paths.</para></returns>
        public static string[] GetAllTMXFiles()
        {
            string _contentPath = YTBFileToGameData.ContentManager.RootDirectory;

            YTB<string> tmxFiles = new();
            if (Directory.Exists(_contentPath))
            {
                var xmlFiles = Directory.GetFiles(_contentPath, "*.tmx", SearchOption.AllDirectories);
                foreach (var file in xmlFiles)
                {
                    string relativePath = Path.GetRelativePath(_contentPath, file).Replace("\\", "/");
                    tmxFiles.Add(relativePath);
                }
            }
            return tmxFiles.ToArray();
        }


        /// <summary>
        /// Reads a raw .tmx file and deserializes XML into intermediate objects.
        /// Lee un archivo .tmx crudo y deserializa su estructura XML en objetos intermedios.
        /// </summary>
        /// <param name="tmxPath">Path to the .tmx file. Ruta del archivo .tmx en el disco.</param>
        /// <returns>Tilesets, layers, and map info. Lista de tilesets, capas e info del mapa.</returns>
        private static (YTB<TiledTileSet>, YTB<TiledLayer>, Map) ReadTMXFile(string tmxPath)
        {
            string _contentPath = YTBFileToGameData.ContentManager.RootDirectory;
            // Carga el documento XML en memoria

            string relative = tmxPath.TrimStart('\\', '/');
            if (relative.StartsWith(_contentPath + "/", StringComparison.OrdinalIgnoreCase) ||
                relative.StartsWith(_contentPath + "\\", StringComparison.OrdinalIgnoreCase))
            {
                relative = relative.Substring(_contentPath.Length + 1);
            }
            string path = Path.Combine(_contentPath, relative);

            XmlDocument document = new XmlDocument();

            // Load the TMX file using TitleContainer for cross-platform compatibility
            try
            {
                using (Stream stream = TitleContainer.OpenStream(path))
                {
                    document.Load(stream);
                }
            }
            catch (FileNotFoundException)
            {
                // Fallback: Try with normalized path for Android
                string androidPath = relative.Replace('\\', '/');
                try
                {
                    using var stream = TitleContainer.OpenStream(androidPath);
                    using var reader = new StreamReader(stream);
                    document.LoadXml(reader.ReadToEnd());
                }
                catch (Exception ex)
                {
                    EngineUISystem.SendLog($"Error loading TMX file '{path}': {ex.Message}");
                    throw;
                }
            }

            // --- 1. Lectura de TileSets ---
            XmlNodeList listOfTilesets = document.GetElementsByTagName("tileset");
            YTB<TiledTileSet> tileSets = new YTB<TiledTileSet>();

            foreach (XmlNode node in listOfTilesets)
            {
                TiledTileSet tileSet = new TiledTileSet();
                // Extraer atributos principales del tileset
                tileSet.FirstGid = Convert.ToInt32(node.Attributes["firstgid"].Value);
                tileSet.Name = node.Attributes["name"].Value;
                tileSet.TileWidth = Convert.ToInt32(node.Attributes["tilewidth"].Value);
                tileSet.TileHeight = Convert.ToInt32(node.Attributes["tileheight"].Value);
                tileSet.TileCount = Convert.ToInt32(node.Attributes["tilecount"].Value);
                tileSet.Columns = Convert.ToInt32(node.Attributes["columns"].Value);

                // Procesar la imagen asociada al tileset
                TiledTileSetImage tileSetImage = new TiledTileSetImage();
                XmlNode imgNode = node.SelectSingleNode("image");
                tileSetImage.Source = imgNode.Attributes["source"].Value;
                tileSetImage.Width = Convert.ToInt32(imgNode.Attributes["width"].Value);
                tileSetImage.Height = Convert.ToInt32(imgNode.Attributes["height"].Value);
                tileSet.Image = tileSetImage;

                // Parsear colisiones nativas de Tiled (objectgroups dentro de <tile>)
                tileSet.TileCollisions = new Dictionary<int, YTB<Rectangle>>();
                XmlNodeList tileNodes = node.SelectNodes("tile");
                if (tileNodes != null)
                {
                    foreach (XmlNode tileNode in tileNodes)
                    {
                        int localTileId = Convert.ToInt32(tileNode.Attributes["id"].Value);
                        XmlNode objGroup = tileNode.SelectSingleNode("objectgroup");
                        if (objGroup == null) continue;

                        YTB<Rectangle> collisionRects = new YTB<Rectangle>();
                        XmlNodeList objects = objGroup.SelectNodes("object");
                        if (objects != null)
                        {
                            foreach (XmlNode obj in objects)
                            {
                                int ox = obj.Attributes["x"] != null ? (int)Math.Round(Convert.ToDouble(obj.Attributes["x"].Value)) : 0;
                                int oy = obj.Attributes["y"] != null ? (int)Math.Round(Convert.ToDouble(obj.Attributes["y"].Value)) : 0;
                                int ow = obj.Attributes["width"] != null ? (int)Math.Round(Convert.ToDouble(obj.Attributes["width"].Value)) : 0;
                                int oh = obj.Attributes["height"] != null ? (int)Math.Round(Convert.ToDouble(obj.Attributes["height"].Value)) : 0;
                                collisionRects.Add(new Rectangle(ox, oy, ow, oh));
                            }
                        }

                        if (collisionRects.Count > 0)
                            tileSet.TileCollisions[localTileId] = collisionRects;
                    }
                }

                tileSets.Add(tileSet);
            }

            // --- 2. Lectura de Capas (Layers) ---
            XmlNodeList listOfLayers = document.GetElementsByTagName("layer");
            YTB<TiledLayer> tiledLayers = new YTB<TiledLayer>();

            foreach (XmlNode node in listOfLayers)
            {
                TiledLayer layer = new TiledLayer();
                layer.Id = Convert.ToInt32(node.Attributes["id"].Value);
                layer.Name = node.Attributes["name"].Value;
                layer.Width = Convert.ToInt32(node.Attributes["width"].Value);
                layer.Height = Convert.ToInt32(node.Attributes["height"].Value);

                // Procesar la data CSV de la capa
                XmlNode dataNode = node.SelectSingleNode("data");
                YTB<int> guids = new YTB<int>();

                // Separar por comas y convertir a int
                foreach (var item in dataNode.InnerText.Split(',').ToArray())
                {
                    // Limpieza básica por si hay saltos de línea en el XML
                    if (int.TryParse(item, out int gid))
                    {
                        guids.Add(gid);
                    }
                }

                layer.Data = new TiledLayerData()
                {
                    Guids = guids.ToArray()
                };

                tiledLayers.Add(layer);
            }

            // --- 3. Lectura de Propiedades del Mapa ---
            XmlNode mapNode = document.SelectSingleNode("map");
            Map map = new()
            {
                Width = Convert.ToInt32(mapNode.Attributes["width"].Value),
                Height = Convert.ToInt32(mapNode.Attributes["height"].Value),
                TileHeight = Convert.ToInt32(mapNode.Attributes["tilewidth"].Value),
                TileWidth = Convert.ToInt32(mapNode.Attributes["tileheight"].Value)
            };

            return (tileSets, tiledLayers, map);
        }
        /// <summary>
        /// Genera un componente ECS completo a partir de un archivo .tmx.
        /// <para>Builds a full ECS component from a .tmx file.</para>
        /// </summary>
        /// <param name="tmxPath">Ruta al archivo del mapa. <para>Map file path.</para></param>
        /// <returns>El componente TileMapComponent2D listo para usar. <para>Ready-to-use TileMapComponent2D.</para></returns>
        public static TileMapComponent2D GenerateTilemapComponent(string tmxPath)
        {

            //referencia al content manager
            var Content = YTBFileToGameData.ContentManager;

            // Paso 0: Obtener los datos crudos parseados del XML
            var (tiledTileSets, tiledLayersRaw, mapInfo) = ReadTMXFile(tmxPath);

            // Inicializar el componente con las dimensiones del mapa
            TileMapComponent2D tilemapComponent = new(mapInfo.Width, mapInfo.Height, mapInfo.TileWidth, mapInfo.TileHeight);

            // --- FASE 1: PROCESAMIENTO DE TILESETS (LA PALETA) ---
            // Recorremos cada definición de tileset para cargar su textura y recortar los tiles
            foreach (ref TiledTileSet tilesetData in tiledTileSets.AsSpan())
            {
                // Construcción de la ruta absoluta o relativa para el ContentManager
                string dir = Path.GetDirectoryName(tmxPath);
                string texturePath = Path.Combine(dir, tilesetData.Image.Source);

                // MonoGame carga los assets sin extensión, la removemos
                texturePath = Path.ChangeExtension(texturePath, null);

                Texture2D texture = Content.Load<Texture2D>(texturePath);

                // Iteramos sobre cada tile individual dentro de la textura (hoja de sprites)
                for (int i = 0; i < tilesetData.TileCount; i++)
                {
                    // El GID (Global ID) es único para todo el mapa
                    int currentGid = tilesetData.FirstGid + i;

                    // Cálculo de coordenadas (Fila y Columna) dentro de la imagen
                    int col = i % tilesetData.Columns;
                    int row = i / tilesetData.Columns;

                    int x = col * tilesetData.TileWidth;
                    int y = row * tilesetData.TileHeight;

                    Rectangle sourceRect = new Rectangle(x, y, tilesetData.TileWidth, tilesetData.TileHeight);

                    // Creación de la región gráfica
                    TextureRegion tRegion = new TextureRegion(texture, sourceRect);
                    TileRegion tileRegion = new TileRegion(tRegion);

                    // Almacenamos el tile en el diccionario maestro
                    tilemapComponent.Tiles.Add(currentGid, tileRegion);

                    // Colisiones nativas de Tiled: si el tile tiene objectgroup con formas de colisión
                    if (tilesetData.TileCollisions != null && tilesetData.TileCollisions.TryGetValue(i, out var tileCollisionRects))
                    {
                        List<Rectangle> rects = new List<Rectangle>(tileCollisionRects.Count);
                        foreach (ref var rect in tileCollisionRects.AsSpan())
                            rects.Add(rect);
                        tilemapComponent.Collisions[currentGid] = rects;
                    }
                }
            }

            // --- FASE 2: CONSTRUCCIÓN DEL MAPA (CAPAS) ---
            // Transferimos la data numérica de las capas al componente
            foreach (ref TiledLayer layerRaw in tiledLayersRaw.AsSpan())
            {
                TileLayer finalLayer = new TileLayer
                {
                    Name = layerRaw.Name,
                    IsVisible = true,
                    // Copiamos el array de enteros (GIDs) que define qué tile va en cada celda
                    Data = layerRaw.Data.Guids
                };

                tilemapComponent.TileLayers.Add(finalLayer);
            }

            return tilemapComponent;
        }
    }

    /// <summary>
    /// Estructura intermedia que representa las propiedades globales del mapa leído.
    /// <para>Intermediate structure describing map-wide properties.</para>
    /// </summary>
    public struct Map
    {
        /// <summary>
        /// Ancho del mapa en cantidad de tiles.
        /// <para>Map width in tiles.</para>
        /// </summary>
        public int Width { get; set; }
        /// <summary>
        /// Alto del mapa en cantidad de tiles.
        /// <para>Map height in tiles.</para>
        /// </summary>
        public int Height { get; set; }
        /// <summary>
        /// Ancho de un tile individual en píxeles.
        /// <para>Tile width in pixels.</para>
        /// </summary>
        public int TileWidth { get; set; }
        /// <summary>
        /// Alto de un tile individual en píxeles.
        /// <para>Tile height in pixels.</para>
        /// </summary>
        public int TileHeight { get; set; }
    }

    /// <summary>
    /// Representación intermedia de un Tileset (conjunto de tiles) leído del XML.
    /// <para>Intermediate representation of a tileset read from XML.</para>
    /// </summary>
    public struct TiledTileSet
    {
        /// <summary>
        /// Primer ID Global asignado a este tileset.
        /// <para>First global ID assigned to this tileset.</para>
        /// </summary>
        public int FirstGid { get; set; }
        /// <summary>
        /// Nombre del tileset.
        /// <para>Tileset name.</para>
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Ancho de cada tile en píxeles.
        /// <para>Tile width in pixels.</para>
        /// </summary>
        public int TileWidth { get; set; }
        /// <summary>
        /// Alto de cada tile en píxeles.
        /// <para>Tile height in pixels.</para>
        /// </summary>
        public int TileHeight { get; set; }
        /// <summary>
        /// Cantidad total de tiles en este set.
        /// <para>Total tile count.</para>
        /// </summary>
        public int TileCount { get; set; }
        /// <summary>
        /// Cantidad de columnas en la imagen de origen.
        /// <para>Column count in the source image.</para>
        /// </summary>
        public int Columns { get; set; }
        /// <summary>
        /// Ruta relativa de la imagen de origen.
        /// <para>Relative source image path.</para>
        /// </summary>
        public string ImageSource { get; set; }
        /// <summary>
        /// Ancho de la imagen de origen.
        /// <para>Source image width.</para>
        /// </summary>
        public int ImageWidth { get; set; }
        /// <summary>
        /// Alto de la imagen de origen.
        /// <para>Source image height.</para>
        /// </summary>
        public int ImageHeight { get; set; }
        /// <summary>
        /// Datos detallados de la imagen asociada.
        /// <para>Detailed image metadata.</para>
        /// </summary>
        public TiledTileSetImage Image { get; set; }
        /// <summary>
        /// Colisiones nativas de Tiled definidas por tile (objectgroups dentro de tile).
        /// Clave: ID local del tile. Valor: lista de rectángulos de colisión.
        /// <para>Native Tiled per-tile collisions (objectgroups inside tile elements).
        /// Key: local tile ID. Value: list of collision rectangles.</para>
        /// </summary>
        public Dictionary<int, YTB<Rectangle>> TileCollisions { get; set; }
    }

    /// <summary>
    /// Representa la imagen fuente de un tileset.
    /// <para>Represents the source image of a tileset.</para>
    /// </summary>
    public struct TiledTileSetImage
    {
        /// <summary>
        /// Ruta del archivo de imagen.
        /// <para>Image file path.</para>
        /// </summary>
        public string Source { get; set; }
        /// <summary>
        /// Ancho total de la imagen.
        /// <para>Total image width.</para>
        /// </summary>
        public int Width { get; set; }
        /// <summary>
        /// Alto total de la imagen.
        /// <para>Total image height.</para>
        /// </summary>
        public int Height { get; set; }
    }

    /// <summary>
    /// Representación intermedia de una capa del mapa (Layer).
    /// <para>Intermediate representation of a map layer.</para>
    /// </summary>
    public struct TiledLayer
    {
        /// <summary>
        /// Identificador de la capa.
        /// <para>Layer identifier.</para>
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Nombre de la capa.
        /// <para>Layer name.</para>
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Ancho de la capa en tiles.
        /// <para>Layer width in tiles.</para>
        /// </summary>
        public int Width { get; set; }
        /// <summary>
        /// Alto de la capa en tiles.
        /// <para>Layer height in tiles.</para>
        /// </summary>
        public int Height { get; set; }
        /// <summary>
        /// Contenedor de los datos numéricos (GIDs) de la capa.
        /// <para>Container for numeric layer data (GIDs).</para>
        /// </summary>
        public TiledLayerData Data { get; set; }
    }

    /// <summary>
    /// Contenedor para el array de IDs de los tiles.
    /// <para>Container for tile ID arrays.</para>
    /// </summary>
    public struct TiledLayerData
    {
        /// <summary>
        /// Arreglo de Global IDs que definen el mapa.
        /// <para>Global ID array for the map.</para>
        /// </summary>
        public int[] Guids { get; set; }
    }
}
