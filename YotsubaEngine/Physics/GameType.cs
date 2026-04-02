using System;
using System.Collections.Generic;
using System.Text;

namespace YotsubaEngine.Physics
{
    /// <summary>
    /// Tipo de juego para determinar el comportamiento de la física.
    /// <para>Game type used to determine physics behavior.</para>
    /// </summary>
    public enum GameType
    {
        /// <summary>
        /// Para videojuegos con vista de arriba hacia abajo (top-down).
        /// <para>Top-down game behavior.</para>
        /// </summary>
        TopDown,
        /// <summary>
        /// Para videojuegos con vista lateral, plataformas, etc.
        /// <para>Side-scroller/platformer behavior.</para>
        /// </summary>
        Platform
    }
}
