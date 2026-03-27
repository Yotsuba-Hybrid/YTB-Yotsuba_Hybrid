using System.Runtime.CompilerServices;

namespace Benchmarks.Core.Types
{
    public struct Vector3
    {
        public float X;
        public float Y;
        public float Z;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector3(float x, float y, float z)
        {
            Unsafe.SkipInit(out this);
            X = x;
            Y = y;
            Z = z;
        }

        public static readonly Vector3 Zero = new(0, 0, 0);
        public static readonly Vector3 One = new(1, 1, 1);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 operator +(Vector3 a, Vector3 b)
        {
            Unsafe.SkipInit(out Vector3 r);
            r.X = a.X + b.X;
            r.Y = a.Y + b.Y;
            r.Z = a.Z + b.Z;
            return r;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 operator -(Vector3 a, Vector3 b)
        {
            Unsafe.SkipInit(out Vector3 r);
            r.X = a.X - b.X;
            r.Y = a.Y - b.Y;
            r.Z = a.Z - b.Z;
            return r;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 operator *(Vector3 a, float s)
        {
            Unsafe.SkipInit(out Vector3 r);
            r.X = a.X * s;
            r.Y = a.Y * s;
            r.Z = a.Z * s;
            return r;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Vector3 a, Vector3 b) => a.X == b.X && a.Y == b.Y && a.Z == b.Z;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Vector3 a, Vector3 b) => !(a == b);

        public override bool Equals(object obj) => obj is Vector3 v && this == v;
        public override int GetHashCode() => HashCode.Combine(X, Y, Z);
    }
}
