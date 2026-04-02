using System;
using System.Collections.Generic;
using System.Text;

namespace YotsubaEngine.Physics
{
    /// <summary>
    /// Define las capas de colisión disponibles en el sistema mediante banderas de bits (bitmasking).
    /// </summary>
    [Flags]
    public enum CollisionLayer : uint
    {

        /// <summary>
        /// Capa de colisión principal.
        /// </summary>
        Main = 1 << 0,

        /// <summary>
        /// Capa de colisión secundaria.
        /// </summary>
        Secondary = 1 << 1,

        /// <summary>
        /// Capa de colisión genérica 2.
        /// </summary>
        Layer2 = 1 << 2,

        /// <summary>
        /// Capa de colisión genérica 3.
        /// </summary>
        Layer3 = 1 << 3,

        /// <summary>
        /// Capa de colisión genérica 4.
        /// </summary>
        Layer4 = 1 << 4,

        /// <summary>
        /// Capa de colisión genérica 5.
        /// </summary>
        Layer5 = 1 << 5,

        /// <summary>
        /// Capa de colisión genérica 6.
        /// </summary>
        Layer6 = 1 << 6,

        /// <summary>
        /// Capa de colisión genérica 7.
        /// </summary>
        Layer7 = 1 << 7,

        /// <summary>
        /// Capa de colisión genérica 8.
        /// </summary>
        Layer8 = 1 << 8,

        /// <summary>
        /// Capa de colisión genérica 9.
        /// </summary>
        Layer9 = 1 << 9,

        /// <summary>
        /// Capa de colisión genérica 10.
        /// </summary>
        Layer10 = 1 << 10,

        /// <summary>
        /// Capa de colisión genérica 11.
        /// </summary>
        Layer11 = 1 << 11,

        /// <summary>
        /// Capa de colisión genérica 12.
        /// </summary>
        Layer12 = 1 << 12,

        /// <summary>
        /// Capa de colisión genérica 13.
        /// </summary>
        Layer13 = 1 << 13,

        /// <summary>
        /// Capa de colisión genérica 14.
        /// </summary>
        Layer14 = 1 << 14,

        /// <summary>
        /// Capa de colisión genérica 15.
        /// </summary>
        Layer15 = 1 << 15,

        /// <summary>
        /// Capa de colisión genérica 16.
        /// </summary>
        Layer16 = 1 << 16,

        /// <summary>
        /// Capa de colisión genérica 17.
        /// </summary>
        Layer17 = 1 << 17,

        /// <summary>
        /// Capa de colisión genérica 18.
        /// </summary>
        Layer18 = 1 << 18,

        /// <summary>
        /// Capa de colisión genérica 19.
        /// </summary>
        Layer19 = 1 << 19,

        /// <summary>
        /// Capa de colisión genérica 20.
        /// </summary>
        Layer20 = 1 << 20,

        /// <summary>
        /// Capa de colisión genérica 21.
        /// </summary>
        Layer21 = 1 << 21,

        /// <summary>
        /// Capa de colisión genérica 22.
        /// </summary>
        Layer22 = 1 << 22,

        /// <summary>
        /// Capa de colisión genérica 23.
        /// </summary>
        Layer23 = 1 << 23,

        /// <summary>
        /// Capa de colisión genérica 24.
        /// </summary>
        Layer24 = 1 << 24,

        /// <summary>
        /// Capa de colisión genérica 25.
        /// </summary>
        Layer25 = 1 << 25,

        /// <summary>
        /// Capa de colisión genérica 26.
        /// </summary>
        Layer26 = 1 << 26,

        /// <summary>
        /// Capa de colisión genérica 27.
        /// </summary>
        Layer27 = 1 << 27,

        /// <summary>
        /// Capa de colisión genérica 28.
        /// </summary>
        Layer28 = 1 << 28,

        /// <summary>
        /// Capa de colisión genérica 29.
        /// </summary>
        Layer29 = 1 << 29,

        /// <summary>
        /// Capa de colisión genérica 30.
        /// </summary>
        Layer30 = 1 << 30,

        /// <summary>
        /// Máscara que representa la activación de todas las capas de colisión posibles simultáneamente.
        /// </summary>
        All = ~0u
    }
}
