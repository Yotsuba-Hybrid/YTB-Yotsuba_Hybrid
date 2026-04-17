
using System.Collections.Generic;
using YotsubaEngine.HighestPerformanceTypes;

namespace YotsubaEngine.Core.Component.C_3D
{
    /// <summary>
    /// Lista de objetos 3D asociados a una entidad.
    /// <para>List of 3D objects associated with an entity.</para>
    /// </summary>
    public struct YTBModelComponent3D
    {
        /// <summary>
        /// Colección de identificadores de objetos 3D.
        /// <para>Collection of 3D object identifiers.</para>
        /// </summary>
        public string Tag { get; set; }

        /// <summary>
        /// Indica si los objetos deben renderizarse.
        /// <para>Indicates whether the objects should be rendered.</para>
        /// </summary>
        public bool IsVisible { get; set; }
    }
}
