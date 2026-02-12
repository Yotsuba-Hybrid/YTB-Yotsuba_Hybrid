
using System.Collections.Generic;
using YotsubaEngine.HighestPerformanceTypes;

namespace YotsubaEngine.Core.Component.C_3D
{
    /// <summary>
    /// Lista de objetos 3D asociados a una entidad.
    /// <para>List of 3D objects associated with an entity.</para>
    /// </summary>
    public struct ListObject3D
    {

        /// <summary>
        /// Colección de identificadores de objetos 3D.
        /// <para>Collection of 3D object identifiers.</para>
        /// </summary>
        public YTB<int> Object3Ds { get; set; }

        /// <summary>
        /// Indica si los objetos deben renderizarse.
        /// <para>Indicates whether the objects should be rendered.</para>
        /// </summary>
        public bool IsVisible { get; set; }

        /// <summary>
        /// Inicializa la lista de objetos 3D.
        /// <para>Initializes the 3D object list.</para>
        /// </summary>
        public ListObject3D()
        {
            Object3Ds = new();
        }
    }
}
