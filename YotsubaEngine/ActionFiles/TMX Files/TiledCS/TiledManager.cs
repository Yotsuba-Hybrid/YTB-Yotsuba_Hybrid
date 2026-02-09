using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
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
    /// Manager responsible for reading and converting Tiled (.tmx) maps into engine components.
    /// Gestor encargado de la lectura, parseo y conversión de archivos de mapas de Tiled (.tmx)
    /// a componentes utilizables por el motor (TileMapComponent2D).
    /// </summary>
    public static class TiledManager
    {

        /// <summary>
        /// Searches for all .tmx files in the content folder.
        /// Metodo busca todos los archivos tmx de tiled en la carpeta content
        /// </summary>
        /// <returns>Relative .tmx file paths. Rutas relativas de archivos .tmx.</returns>
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
        /// Builds a full ECS component from a .tmx file.
        /// Genera un componente ECS completo a partir de un archivo .tmx.
        /// Carga las texturas necesarias y mapea los tiles a sus regiones correspondientes.
        /// </summary>
        /// <param name="tmxPath">Map file path. Ruta al archivo del mapa.</param>
        /// <returns>Ready-to-use TileMapComponent2D. El componente TileMapComponent2D listo para usar.</returns>
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
            foreach (TiledTileSet tilesetData in tiledTileSets)
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

                    // Lógica simple de colisiones basada en el nombre del Tileset
                    if (!tilesetData.Name.Contains("Collision", StringComparison.OrdinalIgnoreCase))
                    {
                        // Si no es una capa de colisión explicita, agregamos colisión por defecto (ajustar según lógica de juego)
                        tilemapComponent.Collisions.TryAdd(currentGid, sourceRect);
                    }
                }
            }

            // --- FASE 2: CONSTRUCCIÓN DEL MAPA (CAPAS) ---
            // Transferimos la data numérica de las capas al componente
            foreach (TiledLayer layerRaw in tiledLayersRaw)
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
    /// Intermediate structure describing map-wide properties.
    /// Estructura intermedia que representa las propiedades globales del mapa leído.
    /// </summary>
    public struct Map
    {
        /// <summary> Map width in tiles. Ancho del mapa en cantidad de tiles. </summary>
        public int Width { get; set; }
        /// <summary> Map height in tiles. Alto del mapa en cantidad de tiles. </summary>
        public int Height { get; set; }
        /// <summary> Tile width in pixels. Ancho de un tile individual en píxeles. </summary>
        public int TileWidth { get; set; }
        /// <summary> Tile height in pixels. Alto de un tile individual en píxeles. </summary>
        public int TileHeight { get; set; }
    }

    /// <summary>
    /// Intermediate representation of a tileset read from XML.
    /// Representación intermedia de un Tileset (conjunto de tiles) leído del XML.
    /// </summary>
    public struct TiledTileSet
    {
        /// <summary> First global ID assigned to this tileset. Primer ID Global asignado a este tileset. </summary>
        public int FirstGid { get; set; }
        /// <summary> Tileset name. Nombre del tileset. </summary>
        public string Name { get; set; }
        /// <summary> Tile width in pixels. Ancho de cada tile en píxeles. </summary>
        public int TileWidth { get; set; }
        /// <summary> Tile height in pixels. Alto de cada tile en píxeles. </summary>
        public int TileHeight { get; set; }
        /// <summary> Total tile count. Cantidad total de tiles en este set. </summary>
        public int TileCount { get; set; }
        /// <summary> Column count in the source image. Cantidad de columnas en la imagen de origen. </summary>
        public int Columns { get; set; }
        /// <summary> Relative source image path. Ruta relativa de la imagen de origen. </summary>
        public string ImageSource { get; set; }
        /// <summary>
        /// Source image width.
        /// Ancho de la imagen de origen.
        /// </summary>
        public int ImageWidth { get; set; }
        /// <summary>
        /// Source image height.
        /// Alto de la imagen de origen.
        /// </summary>
        public int ImageHeight { get; set; }
        /// <summary> Detailed image metadata. Datos detallados de la imagen asociada. </summary>
        public TiledTileSetImage Image { get; set; }
    }

    /// <summary>
    /// Represents the source image of a tileset.
    /// Representa la imagen fuente de un tileset.
    /// </summary>
    public struct TiledTileSetImage
    {
        /// <summary> Image file path. Ruta del archivo de imagen. </summary>
        public string Source { get; set; }
        /// <summary> Total image width. Ancho total de la imagen. </summary>
        public int Width { get; set; }
        /// <summary> Total image height. Alto total de la imagen. </summary>
        public int Height { get; set; }
    }

    /// <summary>
    /// Intermediate representation of a map layer.
    /// Representación intermedia de una capa del mapa (Layer).
    /// </summary>
    public struct TiledLayer
    {
        /// <summary>
        /// Layer identifier.
        /// Identificador de la capa.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Layer name.
        /// Nombre de la capa.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Layer width in tiles.
        /// Ancho de la capa en tiles.
        /// </summary>
        public int Width { get; set; }
        /// <summary>
        /// Layer height in tiles.
        /// Alto de la capa en tiles.
        /// </summary>
        public int Height { get; set; }
        /// <summary> Container for numeric layer data (GIDs). Contenedor de los datos numéricos (GIDs) de la capa. </summary>
        public TiledLayerData Data { get; set; }
    }

    /// <summary>
    /// Container for tile ID arrays.
    /// Contenedor para el array de IDs de los tiles.
    /// </summary>
    public struct TiledLayerData
    {
        /// <summary> Global ID array for the map. Arreglo de Global IDs que definen el mapa. </summary>
        public int[] Guids { get; set; }
    }
}
