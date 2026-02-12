using Microsoft.Xna.Framework.Graphics;
using YotsubaEngine.Core.System.YotsubaEngineCore;

namespace YotsubaEngine.Core.Component.C_2D
{
    /// <summary>
    /// Componente para escribir texto en 2D.
    /// <para>Component used to render 2D text.</para>
    /// </summary>
    public struct FontComponent2D()
    {
        /// <summary>
        /// Texto a mostrar.
        /// <para>Text to display.</para>
        /// </summary>
        public string Texto { get; set; }

        /// <summary>
        /// Fuente para dibujar el texto.
        /// <para>Font asset used to draw the text.</para>
        /// </summary>
        public string Font { get; set; }

        /// <summary>
        /// Indica si el elemento es visible.
        /// <para>Gets or sets a value indicating whether the element is visible.</para>
        /// </summary>
        public bool IsVisible { get; set; } = true;
    }
}
