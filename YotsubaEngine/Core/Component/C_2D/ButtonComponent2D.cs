
using Microsoft.Xna.Framework;
using System;
using YotsubaEngine.Attributes;

namespace YotsubaEngine.Core.Component.C_2D
{
    /// <summary>
    /// Describe un componente de interacción de botón 2D.
    /// <para>Describes a 2D button interaction component.</para>
    /// </summary>
    [UIComponent("Botón 2D", nameof(ButtonComponent2D))]
    public struct ButtonComponent2D
    {
        /// <summary>
        /// Indica si el botón está activo y puede ejecutar acciones.
        /// </summary>
        [UIComponentValue("Activo", nameof(IsActive), "Si el botón puede ejecutar su acción.", "Valor true/false.")]
        public bool IsActive { get; set; }

        /// <summary>
        /// Área efectiva (clickeable) del botón.
        /// </summary>
        [UIComponentValue("Área efectiva", nameof(EffectiveArea), "Rectángulo en píxeles donde el botón es clickeable.", "Formato: X,Y,Width,Height (4 enteros).")]
        public Rectangle EffectiveArea { get; set; }

        /// <summary>
        /// Descripción humana del propósito del botón.
        /// </summary>
        [UIComponentValue("Descripción", nameof(Description), "Descripción del botón (informativa).", "Cualquier cadena es válida.")]
        public string Description { get; set; }

        /// <summary>
        /// Operación que realizará el botón al ser presionado (no serializado).
        /// </summary>
        public Action Action { get; set; }

    }
}
