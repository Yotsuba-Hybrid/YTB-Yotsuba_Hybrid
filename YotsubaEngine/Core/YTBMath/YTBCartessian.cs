using Microsoft.Xna.Framework;


namespace YotsubaEngine.Core.YTBMath
{
    /// <summary>
    /// Cartesian math helpers for 2D and 3D vectors.
    /// Ayudantes matemáticos cartesianos para vectores 2D y 3D.
    /// </summary>
    public struct YTBCartessian
    {
        /// <summary>
        /// Helper matematico para calcular el punto medio entre dos puntos en un plano cartesiano
        /// </summary>
        /// <param name="pointA"></param>
        /// <param name="PointB"></param>
        /// <returns></returns>
        public static Vector2 MiddlePoint(Vector2 pointA, Vector2 PointB)
        {
            Vector2 middlePoint = new Vector2(
                (pointA.X + PointB.X) * 0.5f,
                (pointA.Y + PointB.Y) * 0.5f);

            return middlePoint;
        }

        /// <summary>
        /// Convertir vector3 a vector2 descartando la componente Z
        /// </summary>
        /// <param name="vector3"></param>
        /// <returns></returns>
        public static Vector2 Vector3ToVector2(Vector3 vector3)
        {
            return new Vector2(vector3.X, vector3.Y);
        }
    }
}
