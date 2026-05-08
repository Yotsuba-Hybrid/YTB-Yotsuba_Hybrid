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
        private Model model;

        /// <summary>
        /// Modelo 3D.
        /// <para>3D model asset.</para>
        /// </summary>
        public Model Model
        {
            readonly get => model;
            set
            {
                model = value;
                BoneTransforms = new Matrix[model.Bones.Count];
                Model.CopyAbsoluteBoneTransformsTo(BoneTransforms);
                RadiusSphere = model.CalculateBoundingSphere().Radius;
            }
        }

        public RasterizerState RasterizerState { get; set; } = null;
        public Matrix[] BoneTransforms { get; private set; }

        /// <summary>
        /// Indica si el modelo debe renderizarse.
        /// <para>Indicates whether the model should be rendered.</para>
        /// </summary>
        public bool IsVisible { get; set; } = true;

        public float RadiusSphere { get; set; }

        public Vector3 SphereOffset { get; set; } = Vector3.Zero;

        /// <summary>
        /// La herramienta nativa de MonoGame para preguntarle a la GPU.
        /// </summary>
        internal OcclusionQuery OcclusionQuery { get; set; }

        /// <summary>
        /// Bandera para saber si la GPU está procesando nuestra pregunta actual.
        /// </summary>  
        internal bool IsQueryActive { get; set; }

        /// <summary>
        /// El resultado del frame anterior. Si es true, una pared nos está tapando.
        /// Asumimos que es visible (false) por defecto.
        /// </summary>
        internal bool IsOccluded { get; set; }
        public ModelComponent3D(Model model)
        {
            Model = model;
        }

        
        public readonly void SetBoneTransform(string boneName, Matrix newTransform)
        {
            int boneIndex = Model.Bones[boneName].Index;
            BoneTransforms[boneIndex] = newTransform;
        }

        public readonly BoundingSphere GetWorldBoundingSphere(Matrix world)
        {
            var sphere = Model.CalculateBoundingSphere();
            sphere.Center += SphereOffset;
            sphere.Radius = RadiusSphere;
            return sphere.Transform(world);
        }
    }

    public static class YTBRasterizerStates
    {

        /// <summary>
        /// El normal (Culling activado)
        /// </summary>
        public static readonly RasterizerState CullBack = new RasterizerState { CullMode = CullMode.CullCounterClockwiseFace };

        /// <summary>
        /// Para hojas, césped, capas (Sin Culling)
        /// </summary>
        public static readonly RasterizerState CullNone = new RasterizerState { CullMode = CullMode.None };

        /// <summary>
        /// Raro de usar, pero útil para invertir mallas o ver interiores
        /// </summary>
        public static readonly RasterizerState CullFront = new RasterizerState { CullMode = CullMode.CullClockwiseFace };

        public static readonly BlendState NoColorWrite = new BlendState
        {
            ColorWriteChannels = ColorWriteChannels.None
        };

    }

    public static class ModelComponent3DExtensions
    {
        public static BoundingSphere CalculateBoundingSphere(this Model model)
        {
            Matrix[] boneTransforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(boneTransforms);

            BoundingSphere sphere = new BoundingSphere(Vector3.Zero, 0);

            foreach (var mesh in model.Meshes)
            {
                BoundingSphere transformedSphere =
                    mesh.BoundingSphere.Transform(boneTransforms[mesh.ParentBone.Index]);

                if (sphere.Radius == 0)
                    sphere = transformedSphere;
                else
                    sphere = BoundingSphere.CreateMerged(sphere, transformedSphere);
            }

            return sphere;
        }

        public static BoundingSphere CalculateBoundingSphere(this ModelComponent3D modelComponent3d)
        {
            Model model = modelComponent3d.Model;
            Matrix[] boneTransforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(boneTransforms);

            BoundingSphere sphere = new BoundingSphere(Vector3.Zero, 0);

            foreach (var mesh in model.Meshes)
            {
                BoundingSphere transformedSphere =
                    mesh.BoundingSphere.Transform(boneTransforms[mesh.ParentBone.Index]);

                if (sphere.Radius == 0)
                    sphere = transformedSphere;
                else
                    sphere = BoundingSphere.CreateMerged(sphere, transformedSphere);
            }

            return sphere;
        }

    }
}
