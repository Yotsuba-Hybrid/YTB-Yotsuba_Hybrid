
using Microsoft.Xna.Framework.Graphics;

namespace YotsubaEngine.Core.Component.C_AGNOSTIC
{
    /// <summary>
    /// Componente que almacena un efecto de shader y su estado activo.
    /// <para>Component that stores a shader effect and its active state.</para>
    /// </summary>
    /// <param name="effect">Efecto de shader a aplicar.<para>Shader effect to apply.</para></param>
    /// <param name="IsActive">Indica si el shader está activo.<para>Indicates whether the shader is active.</para></param>
    public struct ShaderComponent(Effect effect, bool IsActive)
    {
        /// <summary>
        /// Efecto de shader asignado.
        /// <para>Assigned shader effect.</para>
        /// </summary>
        public Effect Effect { get; set; } = effect;

        /// <summary>
        /// Indica si el shader está activo.
        /// <para>Indicates whether the shader is active.</para>
        /// </summary>
        public bool IsActive { get; set; } = true;
    }
}
