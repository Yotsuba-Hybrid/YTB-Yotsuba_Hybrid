using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YotsubaEngine.ActionFiles.XML_SpriteSheet_Files
{
    /// <summary>
    /// Stores sprite metadata used for sprite sheet packing.
    /// Almacena metadatos de sprites usados para empaquetado de sprite sheets.
    /// </summary>
    public class SpriteInfo
    {
        /// <summary>
        /// Gets or sets the sprite name.
        /// Obtiene o establece el nombre del sprite.
        /// </summary>
        public string Name;      // "Chara - BlueIdle00019"

        /// <summary>
        /// Gets or sets the source image path.
        /// Obtiene o establece la ruta de la imagen origen.
        /// </summary>
        public string Path;      // "C:/Images/..."

        /// <summary>
        /// Gets or sets the sprite width in pixels.
        /// Obtiene o establece el ancho del sprite en píxeles.
        /// </summary>
        public int Width;        // 512

        /// <summary>
        /// Gets or sets the sprite height in pixels.
        /// Obtiene o establece la altura del sprite en píxeles.
        /// </summary>
        public int Height;       // 512

        /// <summary>
        /// Gets or sets the packed X position.
        /// Obtiene o establece la posición X empacada.
        /// </summary>
        public int X;            // Se llena tras el cálculo

        /// <summary>
        /// Gets or sets the packed Y position.
        /// Obtiene o establece la posición Y empacada.
        /// </summary>
        public int Y;            // Se llena tras el cálculo
    }
}
