using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YotsubaEngine.ActionFiles.XML_SpriteSheet_Files
{
    /// <summary>
    /// Almacena metadatos de sprites usados para empaquetado de sprite sheets.
    /// <para>Stores sprite metadata used for sprite sheet packing.</para>
    /// </summary>
    public class SpriteInfo
    {
        /// <summary>
        /// Obtiene o establece el nombre del sprite.
        /// <para>Gets or sets the sprite name.</para>
        /// </summary>
        public string Name;      // "Chara - BlueIdle00019"

        /// <summary>
        /// Obtiene o establece la ruta de la imagen origen.
        /// <para>Gets or sets the source image path.</para>
        /// </summary>
        public string Path;      // "C:/Images/..."

        /// <summary>
        /// Obtiene o establece el ancho del sprite en píxeles.
        /// <para>Gets or sets the sprite width in pixels.</para>
        /// </summary>
        public int Width;        // 512

        /// <summary>
        /// Obtiene o establece la altura del sprite en píxeles.
        /// <para>Gets or sets the sprite height in pixels.</para>
        /// </summary>
        public int Height;       // 512

        /// <summary>
        /// Obtiene o establece la posición X empacada.
        /// <para>Gets or sets the packed X position.</para>
        /// </summary>
        public int X;            // Se llena tras el cálculo

        /// <summary>
        /// Obtiene o establece la posición Y empacada.
        /// <para>Gets or sets the packed Y position.</para>
        /// </summary>
        public int Y;            // Se llena tras el cálculo
    }
}
