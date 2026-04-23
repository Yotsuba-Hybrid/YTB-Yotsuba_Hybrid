using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Xna.Framework;

namespace YotsubaEngine.HighestPerformanceTypes
{
    public struct Point3 : IEquatable<Point3>
    {
        public int X, Y, Z;

        public Point3(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Point3(Vector3 vector3)
        {
            X = (int)vector3.X;
            Y = (int)vector3.Y;
            Z = (int)vector3.Z;
        }

        
        public bool Equals(Point3 other) => other.X == X && other.Y == Y && other.Z == Z;
        public override bool Equals([NotNullWhen(true)] object obj) => obj is Point3 other && Equals(other);
        public override readonly int GetHashCode() => HashCode.Combine(X, Y, Z);
        public static bool operator ==(Point3 a, Point3 b) => a.Equals(b);
        public static bool operator !=(Point3 a, Point3 b) => !a.Equals(b);
    }
}
