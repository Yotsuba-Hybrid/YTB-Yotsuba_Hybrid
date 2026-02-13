using Microsoft.Xna.Framework;
using System.Collections.Generic;
using YotsubaEngine.Graphics;
using YotsubaEngine.HighestPerformanceTypes;

namespace YotsubaEngine.Core.Component.C_2D
{
    /// <summary>
    /// Componente que representa un mapa de tiles en 2D, incluyendo regiones, colisiones, tamaño y capas.
    /// <para>Represents a 2D tile map component including regions, collisions, and layers.</para>
    /// </summary>
    public struct TileMapComponent2D
    {
        /// <summary>
        /// Colección de regiones de tiles indexadas por un identificador entero único. Cada región define un área dentro de una textura.
        /// <para>Collection of tile regions indexed by unique identifiers. Each region defines an area within a texture.</para>
        /// </summary>
        public Dictionary<int, TileRegion> Tiles;

        /// <summary>
        /// Diccionario que almacena áreas de colisión nativas de Tiled por GID. Cada tile puede tener múltiples rectángulos de colisión.
        /// <para>Stores native Tiled collision areas per GID. Each tile can have multiple collision rectangles.</para>
        /// </summary>
        public Dictionary<int, List<Rectangle>> Collisions;

        /// <summary>
        /// Ancho total del mapa en cantidad de tiles.
        /// <para>Total map width in tiles.</para>
        /// </summary>
        public int Width;

        /// <summary>
        /// Alto total del mapa en cantidad de tiles.
        /// <para>Total map height in tiles.</para>
        /// </summary>
        public int Height;

        /// <summary>
        /// Ancho de cada tile en píxeles.
        /// <para>Width of each tile in pixels.</para>
        /// </summary>
        public int TileWidth;

        /// <summary>
        /// Alto de cada tile en píxeles.
        /// <para>Height of each tile in pixels.</para>
        /// </summary>
        public int TileHeight;

        /// <summary>
        /// Colección de capas del mapa, donde cada capa contiene un arreglo de índices de tiles. Usa una estructura optimizada YTB para máximo rendimiento.
        /// <para>Collection of map layers with tile index data. Uses an optimized YTB structure for maximum performance.</para>
        /// </summary>
        public YTB<TileLayer> TileLayers = new();

        /// <summary>
        /// Inicializa el mapa de tiles con dimensiones y tamaños específicos.
        /// <para>Initializes the tile map with dimensions and tile sizes.</para>
        /// </summary>
        /// <param name="width">Ancho total del mapa en tiles.<para>Total map width in tiles.</para></param>
        /// <param name="height">Alto total del mapa en tiles.<para>Total map height in tiles.</para></param>
        /// <param name="tileWidth">Ancho de cada tile en píxeles.<para>Width of each tile in pixels.</para></param>
        /// <param name="tileHeight">Alto de cada tile en píxeles.<para>Height of each tile in pixels.</para></param>
        public TileMapComponent2D(int width, int height, int tileWidth, int tileHeight)
        {
            Tiles = new();
            Collisions = new();
            Width = width;
            Height = height;
            TileWidth = tileWidth;
            TileHeight = tileHeight;
        }
    }

    /// <summary>
    /// Representa una región dentro de una textura que corresponde a un tile específico. Contiene la posición y dimensiones dentro del atlas de sprites.
    /// <para>Represents a region within a texture for a specific tile. Contains the position and dimensions inside the sprite atlas.</para>
    /// </summary>
    public struct TileRegion
    {
        /// <summary>
        /// Región de textura que indica de dónde se toma el tile en el atlas.
        /// <para>Texture region describing the tile location in the atlas.</para>
        /// </summary>
        public TextureRegion TextureRegion;

        /// <summary>
        /// Crea una región de tile a partir de una región de textura existente.
        /// <para>Creates a tile region from an existing texture region.</para>
        /// </summary>
        /// <param name="region">Región de textura base.<para>Base texture region.</para></param>
        public TileRegion(TextureRegion region)
        {
            TextureRegion = region;
        }
    }

    /// <summary>
    /// Representa una capa dentro del mapa de tiles. Cada capa tiene un nombre, visibilidad y un arreglo de índices de tiles.
    /// <para>Represents a layer within the tile map. Each layer has a name, visibility, and an array of tile indices.</para>
    /// </summary>
    public struct TileLayer
    {
        /// <summary>
        /// Nombre de la capa (ej. "Fondo", "Objetos", "Colisiones").
        /// <para>Layer name (e.g. "Background", "Objects", "Collisions").</para>
        /// </summary>
        public string Name;

        /// <summary>
        /// Indica si la capa es visible o no durante el renderizado.
        /// <para>Indicates whether the layer is visible when rendering.</para>
        /// </summary>
        public bool IsVisible;

        /// <summary>
        /// Datos de los tiles de la capa, almacenados como índices que referencian al diccionario de Tiles.
        /// <para>Tile data indices referencing the Tiles dictionary.</para>
        /// </summary>
        public int[] Data;
    }
}
