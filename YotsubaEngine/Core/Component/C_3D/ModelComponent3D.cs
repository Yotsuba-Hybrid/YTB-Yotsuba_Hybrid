using Microsoft.Xna.Framework.Graphics;


namespace YotsubaEngine.Core.Component.C_3D
{
    /// <summary>
    /// Component that holds a 3D model for rendering.
    /// Componente que contiene un modelo 3D para renderizado.
    /// </summary>
    public struct ModelComponent3D(Model model)
    {
        /// <summary>
        /// 3D model asset.
        /// Modelo 3d
        /// </summary>
        public Model Model { get; set; } = model;

        /// <summary>
        /// Indicates whether the model should be rendered.
        /// Flag que marca si el modelo se renderizara o no
        /// </summary>
        public bool IsVisible { get; set; } = true;

    }
}
