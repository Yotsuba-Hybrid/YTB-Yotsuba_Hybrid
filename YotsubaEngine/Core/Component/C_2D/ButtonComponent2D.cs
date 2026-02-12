
using Microsoft.Xna.Framework;
using System;

namespace YotsubaEngine.Core.Component.C_2D
{
    /// <summary>
    /// Describe un componente de interacción de botón 2D.
    /// <para>Describes a 2D button interaction component.</para>
    /// </summary>
    public struct ButtonComponent2D
    {
        /// <summary>
        /// Indica si el botón está activo y puede ejecutar acciones.
        /// <para>Indicates whether the button is active and can trigger actions.</para>
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Área efectiva del botón.
        /// <para>Defines the effective clickable area of the button.</para>
        /// </summary>
        public Rectangle EffectiveArea { get; set; }

        /// <summary>
        /// Operación que realizará el botón al ser presionado.
        /// <para>Action executed when the button is pressed.</para>
        /// </summary>
        public Action Action { get; set; }
        
    }
}
