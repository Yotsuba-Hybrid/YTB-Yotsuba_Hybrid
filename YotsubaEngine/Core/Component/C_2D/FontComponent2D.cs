using Microsoft.Xna.Framework.Graphics;
using YotsubaEngine.Core.System.YotsubaEngineCore;

namespace YotsubaEngine.Core.Component.C_2D
{
    /// <summary>
    /// Component used to render 2D text.
    /// Componente para escribit texto en 2D.
    /// </summary>
    public struct FontComponent2D()
    {
        /// <summary>
        /// Text to display.
        /// Texto a mostrar.
        /// </summary>
        public string Texto { get; set; }

        /// <summary>
        /// Font asset used to draw the text.
        /// Fuente para dibujar el texto.
        /// </summary>
        public string Font { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the element is visible.
        /// </summary>
        public bool IsVisible { get; set; } = true;
    }
}
