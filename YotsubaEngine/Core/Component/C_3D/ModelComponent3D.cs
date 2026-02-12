using Microsoft.Xna.Framework.Graphics;


namespace YotsubaEngine.Core.Component.C_3D
{
    /// <summary>
    /// Componente que contiene un modelo 3D para renderizado.
    /// <para>Component that holds a 3D model for rendering.</para>
    /// </summary>
    /// <param name="model">Modelo 3D a renderizar.<para>3D model to render.</para></param>
    public struct ModelComponent3D(Model model)
    {
        /// <summary>
        /// Modelo 3D.
        /// <para>3D model asset.</para>
        /// </summary>
        public Model Model { get; set; } = model;

        /// <summary>
        /// Indica si el modelo debe renderizarse.
        /// <para>Indicates whether the model should be rendered.</para>
        /// </summary>
        public bool IsVisible { get; set; } = true;

    }
}
