
using Microsoft.Xna.Framework.Graphics;
using YotsubaEngine.Attributes;

namespace YotsubaEngine.Core.Component.C_AGNOSTIC
{
    /// <summary>
    /// Componente que almacena un efecto de shader y su estado activo.
    /// <para>Component that stores a shader effect and its active state.</para>
    /// </summary>
    [UIComponent("Shader", nameof(ShaderComponent))]
    public struct ShaderComponent(Effect effect)
    {
        /// <summary>
        /// Efecto de shader asignado en runtime (cargado desde ShaderPath).
        /// </summary>
        public Effect Effect { get; set; } = effect;

        /// <summary>
        /// Ruta al archivo del shader compilado. Bridge de serialización para inicializar Effect.
        /// </summary>
        [UIComponentValue("Ruta del sombreador", "ShaderPath", "Ruta relativa al .fx compilado.", "El archivo de shader no existe.")]
        public string ShaderPath { get; set; }

        /// <summary>
        /// Indica si el shader está activo.
        /// </summary>
        [UIComponentValue("Activo", nameof(IsActive), "Si el shader se aplica al renderizar.", "Valor true/false.")]
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Parámetros adicionales (formato libre). Bridge de serialización.
        /// </summary>
        [UIComponentValue("Parámetros", "params", "Parámetros del shader en formato libre.", "Cualquier cadena es válida.")]
        public string Params { get; set; }
    }
}
