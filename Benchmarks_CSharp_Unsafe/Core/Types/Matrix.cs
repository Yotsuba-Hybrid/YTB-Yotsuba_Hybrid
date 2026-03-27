using System.Runtime.CompilerServices;

namespace Benchmarks.Core.Types
{
    public struct Matrix
    {
        public float M11, M12, M13, M14;
        public float M21, M22, M23, M24;
        public float M31, M32, M33, M34;
        public float M41, M42, M43, M44;

        public static readonly Matrix Identity = CreateIdentity();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Matrix CreateIdentity()
        {
            Unsafe.SkipInit(out Matrix m);
            m.M11 = 1; m.M12 = 0; m.M13 = 0; m.M14 = 0;
            m.M21 = 0; m.M22 = 1; m.M23 = 0; m.M24 = 0;
            m.M31 = 0; m.M32 = 0; m.M33 = 1; m.M34 = 0;
            m.M41 = 0; m.M42 = 0; m.M43 = 0; m.M44 = 1;
            return m;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix CreateTranslation(float x, float y, float z)
        {
            Unsafe.SkipInit(out Matrix m);
            m.M11 = 1; m.M12 = 0; m.M13 = 0; m.M14 = 0;
            m.M21 = 0; m.M22 = 1; m.M23 = 0; m.M24 = 0;
            m.M31 = 0; m.M32 = 0; m.M33 = 1; m.M34 = 0;
            m.M41 = x; m.M42 = y; m.M43 = z; m.M44 = 1;
            return m;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix CreateScale(float scale)
        {
            Unsafe.SkipInit(out Matrix m);
            m.M11 = scale; m.M12 = 0; m.M13 = 0; m.M14 = 0;
            m.M21 = 0; m.M22 = scale; m.M23 = 0; m.M24 = 0;
            m.M31 = 0; m.M32 = 0; m.M33 = scale; m.M34 = 0;
            m.M41 = 0; m.M42 = 0; m.M43 = 0; m.M44 = 1;
            return m;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix CreateRotationZ(float radians)
        {
            float cos = MathF.Cos(radians);
            float sin = MathF.Sin(radians);
            Unsafe.SkipInit(out Matrix m);
            m.M11 = cos; m.M12 = sin; m.M13 = 0; m.M14 = 0;
            m.M21 = -sin; m.M22 = cos; m.M23 = 0; m.M24 = 0;
            m.M31 = 0; m.M32 = 0; m.M33 = 1; m.M34 = 0;
            m.M41 = 0; m.M42 = 0; m.M43 = 0; m.M44 = 1;
            return m;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix operator *(Matrix a, Matrix b)
        {
            Unsafe.SkipInit(out Matrix r);
            r.M11 = a.M11 * b.M11 + a.M12 * b.M21 + a.M13 * b.M31 + a.M14 * b.M41;
            r.M12 = a.M11 * b.M12 + a.M12 * b.M22 + a.M13 * b.M32 + a.M14 * b.M42;
            r.M13 = a.M11 * b.M13 + a.M12 * b.M23 + a.M13 * b.M33 + a.M14 * b.M43;
            r.M14 = a.M11 * b.M14 + a.M12 * b.M24 + a.M13 * b.M34 + a.M14 * b.M44;
            r.M21 = a.M21 * b.M11 + a.M22 * b.M21 + a.M23 * b.M31 + a.M24 * b.M41;
            r.M22 = a.M21 * b.M12 + a.M22 * b.M22 + a.M23 * b.M32 + a.M24 * b.M42;
            r.M23 = a.M21 * b.M13 + a.M22 * b.M23 + a.M23 * b.M33 + a.M24 * b.M43;
            r.M24 = a.M21 * b.M14 + a.M22 * b.M24 + a.M23 * b.M34 + a.M24 * b.M44;
            r.M31 = a.M31 * b.M11 + a.M32 * b.M21 + a.M33 * b.M31 + a.M34 * b.M41;
            r.M32 = a.M31 * b.M12 + a.M32 * b.M22 + a.M33 * b.M32 + a.M34 * b.M42;
            r.M33 = a.M31 * b.M13 + a.M32 * b.M23 + a.M33 * b.M33 + a.M34 * b.M43;
            r.M34 = a.M31 * b.M14 + a.M32 * b.M24 + a.M33 * b.M34 + a.M34 * b.M44;
            r.M41 = a.M41 * b.M11 + a.M42 * b.M21 + a.M43 * b.M31 + a.M44 * b.M41;
            r.M42 = a.M41 * b.M12 + a.M42 * b.M22 + a.M43 * b.M32 + a.M44 * b.M42;
            r.M43 = a.M41 * b.M13 + a.M42 * b.M23 + a.M43 * b.M33 + a.M44 * b.M43;
            r.M44 = a.M41 * b.M14 + a.M42 * b.M24 + a.M43 * b.M34 + a.M44 * b.M44;
            return r;
        }
    }
}
