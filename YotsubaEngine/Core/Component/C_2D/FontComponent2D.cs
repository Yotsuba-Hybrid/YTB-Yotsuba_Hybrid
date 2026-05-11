using Microsoft.Xna.Framework.Graphics;
using YotsubaEngine.Attributes;
using YotsubaEngine.Core.System.YotsubaEngineCore;

namespace YotsubaEngine.Core.Component.C_2D
{
    /// <summary>
    /// Componente para escribir texto en 2D.
    /// <para>Component used to render 2D text.</para>
    /// </summary>
    [UIComponent("Texto 2D", nameof(FontComponent2D))]
    public struct FontComponent2D()
    {
        /// <summary>
        /// Texto a mostrar.
        /// </summary>
        [UIComponentValue("Texto", nameof(Texto), "Texto a mostrar en pantalla.", "Cualquier cadena es válida.")]
        public string Texto { get; set; }

        /// <summary>
        /// Fuente para dibujar el texto.
        /// </summary>
        [UIComponentValue("Fuente", nameof(Font), "Nombre del recurso de fuente registrado.", "Fuente no registrada en el FontRegistry.")]
        public string Font { get; set; }

        /// <summary>
        /// Indica si el elemento es visible.
        /// </summary>
        [UIComponentValue("Visible", nameof(IsVisible), "Indica si el texto se renderiza.", "Valor true/false.")]
        public bool IsVisible { get; set; } = true;
    }
}
