using Microsoft.Xna.Framework;
using System.Collections.Generic;
using YotsubaEngine.Graphics;
using YotsubaEngine.HighestPerformanceTypes;

namespace YotsubaEngine.Core.Component.C_2D
{
    /// <summary>
    /// Represents a 2D tile map component including regions, collisions, and layers.
    /// Componente que representa un mapa de tiles en 2D, incluyendo sus regiones,
    /// colisiones, tamaño y capas.
    /// </summary>
    public struct TileMapComponent2D
    {
        /// <summary>
        /// Collection of tile regions indexed by unique identifiers.
        /// Colección de regiones de tiles indexadas por un identificador entero único.
        /// Cada región define un área dentro de una textura.
        /// </summary>
        public Dictionary<int, TileRegion> Tiles;

        /// <summary>
        /// Stores collision rectangles associated with each tile.
        /// Diccionario que almacena áreas de colisión asociadas a cada tile,
        /// permitiendo gestión independiente del aspecto gráfico.
        /// </summary>
        public Dictionary<int, Rectangle> Collisions;

        /// <summary>
        /// Total map width in tiles.
        /// Ancho total del mapa en cantidad de tiles.
        /// </summary>
        public int Width;

        /// <summary>
        /// Total map height in tiles.
        /// Alto total del mapa en cantidad de tiles.
        /// </summary>
        public int Height;

        /// <summary>
        /// Width of each tile in pixels.
        /// Ancho de cada tile en píxeles.
        /// </summary>
        public int TileWidth;

        /// <summary>
        /// Height of each tile in pixels.
        /// Alto de cada tile en píxeles.
        /// </summary>
        public int TileHeight;

        /// <summary>
        /// Collection of map layers with tile index data.
        /// Colección de capas del mapa, donde cada capa contiene un arreglo de índices de tiles.
        /// Usa una estructura optimizada YTB para máximo rendimiento.
        /// </summary>
        public YTB<TileLayer> TileLayers = new();

        /// <summary>
        /// Initializes the tile map with dimensions and tile sizes.
        /// Constructor que inicializa el mapa de tiles con dimensiones y tamaños específicos.
        /// </summary>
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
    /// Represents a region within a texture for a specific tile.
    /// Representa una región dentro de una textura que corresponde a un tile específico.
    /// Contiene la posición y dimensiones dentro del atlas de sprites.
    /// </summary>
    public struct TileRegion
    {
        /// <summary>
        /// Texture region describing the tile location in the atlas.
        /// Región de textura que indica de dónde se toma el tile en el atlas.
        /// </summary>
        public TextureRegion TextureRegion;

        /// <summary>
        /// Creates a tile region from an existing texture region.
        /// Crea una región de tile a partir de una región de textura existente.
        /// </summary>
        public TileRegion(TextureRegion region)
        {
            TextureRegion = region;
        }
    }

    /// <summary>
    /// Represents a layer within the tile map.
    /// Representa una capa dentro del mapa de tiles.
    /// Cada capa tiene un nombre, visibilidad y un arreglo de índices de tiles.
    /// </summary>
    public struct TileLayer
    {
        /// <summary>
        /// Layer name (e.g. "Background", "Objects", "Collisions").
        /// Nombre de la capa (ej. "Fondo", "Objetos", "Colisiones").
        /// </summary>
        public string Name;

        /// <summary>
        /// Indicates whether the layer is visible when rendering.
        /// Indica si la capa es visible o no durante el renderizado.
        /// </summary>
        public bool IsVisible;

        /// <summary>
        /// Tile data indices referencing the Tiles dictionary.
        /// Datos de los tiles de la capa, almacenados como índices que referencian al diccionario de Tiles.
        /// </summary>
        public int[] Data;
    }
}
