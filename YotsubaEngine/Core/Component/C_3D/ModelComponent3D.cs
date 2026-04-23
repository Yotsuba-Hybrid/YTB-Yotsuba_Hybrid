using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace YotsubaEngine.Core.Component.C_3D
{
    /// <summary>
    /// Componente que contiene un modelo 3D para renderizado.
    /// <para>Component that holds a 3D model for rendering.</para>
    /// </summary>
    /// <param name="model">Modelo 3D a renderizar.<para>3D model to render.</para></param>
    public struct ModelComponent3D
    {

        /// <summary>
        /// Modelo 3D.
        /// <para>3D model asset.</para>
        /// </summary>
        public Model Model { get; set; }

        public Matrix[] BoneTransforms { get; set; }

        public ModelComponent3D(Model model)
        {
            Model = model;
            BoneTransforms = new Matrix[model.Bones.Count];

            // Copiamos la postura original (Bind Pose) del asset a nuestra copia local
            model.CopyAbsoluteBoneTransformsTo(BoneTransforms);
        }

        /// <summary>
        /// Indica si el modelo debe renderizarse.
        /// <para>Indicates whether the model should be rendered.</para>
        /// </summary>
        public bool IsVisible { get; set; } = true;

        public float RadiusSphere { get; set; }

        public Vector3 SphereOffset { get; set; } = Vector3.Zero;

        public void SetBoneTransform(string boneName, Matrix newTransform)
        {
            int boneIndex = Model.Bones[boneName].Index;
            BoneTransforms[boneIndex] = newTransform;
        }

    }
}
