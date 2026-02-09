
using Microsoft.Xna.Framework;
using System;

namespace YotsubaEngine.Core.Component.C_2D
{
    /// <summary>
    /// Describes a 2D button interaction component.
    /// Describe un componente de interacción de botón 2D.
    /// </summary>
    public struct ButtonComponent2D
    {
        /// <summary>
        /// Indicates whether the button is active and can trigger actions.
        /// Propiedad que define si el botón estará activo o no, que haga una acción, o que no haga nada.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Defines the effective clickable area of the button.
        /// Area efectiva del botón
        /// </summary>
        public Rectangle EffectiveArea { get; set; }

        /// <summary>
        /// Action executed when the button is pressed.
        /// Operación que realizara el botón al ser presionado
        /// </summary>
        public Action Action { get; set; }
        
    }
}
