using Microsoft.Xna.Framework;


namespace YotsubaEngine.YTBMath
{
    /// <summary>
    /// Ayudantes matemáticos cartesianos para vectores 2D y 3D.
    /// <para>Cartesian math helpers for 2D and 3D vectors.</para>
    /// </summary>
    public struct YTBCartessian
    {
        /// <summary>
        /// Ayudante matemático para calcular el punto medio entre dos puntos en un plano cartesiano.
        /// <para>Math helper to calculate the midpoint between two points in a Cartesian plane.</para>
        /// </summary>
        /// <param name="pointA">Primer punto.<para>First point.</para></param>
        /// <param name="PointB">Segundo punto.<para>Second point.</para></param>
        /// <returns>Punto medio calculado.<para>Calculated midpoint.</para></returns>
        public static Vector2 MiddlePoint(Vector2 pointA, Vector2 PointB)
        {
            Vector2 middlePoint = new Vector2(
                (pointA.X + PointB.X) * 0.5f,
                (pointA.Y + PointB.Y) * 0.5f);

            return middlePoint;
        }

        /// <summary>
        /// Convierte un Vector3 a Vector2 descartando la componente Z.
        /// <para>Converts a Vector3 to Vector2 by discarding the Z component.</para>
        /// </summary>
        /// <param name="vector3">Vector 3D de entrada.<para>Input 3D vector.</para></param>
        /// <returns>Vector 2D resultante.<para>Resulting 2D vector.</para></returns>
        public static Vector2 Vector3ToVector2(Vector3 vector3)
        {
            return new Vector2(vector3.X, vector3.Y);
        }
    }
}
