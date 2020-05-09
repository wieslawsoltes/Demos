using System;

namespace RxCanvas.WPF
{
    public struct Vector2 : IComparable<Vector2>
    {
        public static Vector2 One { get { return new Vector2(1.0); } }
        public static Vector2 Zero { get { return new Vector2(0.0); } }
        public static Vector2 UnitX { get { return new Vector2(1.0, 0.0); } }
        public static Vector2 UnitY { get { return new Vector2(0.0, 1.0); } }

        public double X { get; private set; }
        public double Y { get; private set; }

        public Vector2(double value)
            : this()
        {
            this.X = value;
            this.Y = value;
        }

        public Vector2(double x, double y)
            : this()
        {
            this.X = x;
            this.Y = y;
        }

        public override string ToString()
        {
            return string.Concat(X, System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator, Y);
        }

        public static bool operator <(Vector2 v1, Vector2 v2)
        {
            return v1.X < v2.X || (v1.X == v2.X && v1.Y < v2.Y);
        }

        public static bool operator >(Vector2 v1, Vector2 v2)
        {
            return v1.X > v2.X || (v1.X == v2.X && v1.Y > v2.Y);
        }

        public int CompareTo(Vector2 v)
        {
            return (this > v) ? -1 : ((this < v) ? 1 : 0);
        }

        public bool Equals(Vector2 v)
        {
            return this.X == v.X && this.Y == v.Y;
        }

        public override bool Equals(object obj)
        {
            if (obj is Vector2)
            {
                return this.Equals((Vector2)obj);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return this.X.GetHashCode() + this.Y.GetHashCode();
        }

        public Vector2 Negate()
        {
            return new Vector2(-this.X, -this.Y);
        }

        public Vector2 Perpendicular()
        {
            return new Vector2(-this.Y, this.X);
        }

        public Vector2 Subtract(Vector2 v)
        {
            return new Vector2(this.X - v.X, this.Y - v.Y);
        }

        public Vector2 Add(Vector2 v)
        {
            return new Vector2(this.X + v.X, this.Y + v.Y);
        }

        public Vector2 Multiply(double scalar)
        {
            return new Vector2(
                this.X * scalar,
                this.Y * scalar);
        }

        public Vector2 Multiply(Vector2 v)
        {
            return new Vector2(
                this.X * v.X,
                this.Y * v.Y);
        }

        public Vector2 Divide(double scalar)
        {
            double value = 1.0 / scalar;
            return new Vector2(
                this.X * value,
                this.Y * value);
        }

        public Vector2 Divide(Vector2 v)
        {
            return new Vector2(
                this.X / v.X,
                this.Y / v.Y);
        }

        public double Dot(Vector2 v)
        {
            return (this.X * v.X) + (this.Y * v.Y);
        }

        public double Angle(Vector2 v)
        {
            return Math.Acos(this.Dot(v));
        }

        public double Cross(Vector2 v)
        {
            return (this.X * v.Y) - (this.Y * v.X);
        }

        public static bool operator ==(Vector2 v1, Vector2 v2)
        {
            return v1.X == v2.X && v1.Y == v2.Y;
        }

        public static bool operator !=(Vector2 v1, Vector2 v2)
        {
            return v1.X != v2.X || v1.Y != v2.Y;
        }

        public static Vector2 operator -(Vector2 v)
        {
            return new Vector2(-v.X, -v.Y);
        }

        public static Vector2 operator -(Vector2 v1, Vector2 v2)
        {
            return new Vector2(v1.X - v2.X, v1.Y - v2.Y);
        }

        public static Vector2 operator +(Vector2 v1, Vector2 v2)
        {
            return new Vector2(v1.X + v2.X, v1.Y + v2.Y);
        }

        public static Vector2 operator *(Vector2 v, double scalar)
        {
            return new Vector2(
                v.X * scalar,
                v.Y * scalar);
        }

        public static Vector2 operator *(double scalar, Vector2 v)
        {
            return new Vector2(
                v.X * scalar,
                v.Y * scalar);
        }

        public static Vector2 operator *(Vector2 v1, Vector2 v2)
        {
            return new Vector2(
                v1.X * v2.X,
                v1.Y * v2.Y);
        }

        public static Vector2 operator /(Vector2 v, double scalar)
        {
            double value = 1.0 / scalar;
            return new Vector2(
                v.X * value,
                v.Y * value);
        }

        public static Vector2 operator /(Vector2 v1, Vector2 v2)
        {
            return new Vector2(
                v1.X / v2.X,
                v1.Y / v2.Y);
        }

        public double Magnitude()
        {
            return Math.Sqrt(this.X * this.X + this.Y * this.Y);
        }

        public double MagnitudeSquared()
        {
            return this.X * this.X + this.Y * this.Y;
        }

        public double Length()
        {
            return Math.Sqrt(this.X * this.X + this.Y * this.Y);
        }

        public double LengthSquared()
        {
            return this.X * this.X + this.Y * this.Y;
        }

        public Vector2 Normalize()
        {
            return this / this.Length();
        }

        //public double Component(Vector2 v)
        //{
        //    return this.Dot(v.Normalize());
        //}

        public Vector2 Project(Vector2 v)
        {
            return v * (this.Dot(v) / v.Dot(v));
        }

        //public Vector2 Project(Vector2 v)
        //{
        //    return v.Normalize() * this.Component(v);
        //}

        public Vector2 Reflect(Vector2 normal)
        {
            double dot = this.Dot(normal);
            return new Vector2(
                this.X - 2.0 * dot * normal.X,
                this.Y - 2.0 * dot * normal.Y);
        }

        public Vector2 Min(Vector2 v)
        {
            return new Vector2(
                this.X < v.X ? this.X : v.X,
                this.Y < v.Y ? this.Y : v.Y);
        }

        public Vector2 Max(Vector2 v)
        {
            return new Vector2(
                this.X > v.X ? this.X : v.X,
                this.Y > v.Y ? this.Y : v.Y);
        }

        public Vector2 Lerp(Vector2 v, double amount)
        {
            return this + (v - this) * amount;
        }

        public Vector2 Slerp(Vector2 v, double amount)
        {
            double dot = Clamp(this.Dot(v), -1.0, 1.0);
            double theta = Math.Acos(dot) * amount;
            Vector2 relative = (v - this * dot).Normalize();
            return ((this * Math.Cos(theta)) + (relative * Math.Sin(theta)));
        }

        public Vector2 Nlerp(Vector2 v, double amount)
        {
            return this.Lerp(v, amount).Normalize();
        }

        public double Distance(Vector2 v)
        {
            double dx = this.X - v.X;
            double dy = this.Y - v.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        public Vector2 Middle(Vector2 v)
        {
            return new Vector2(
                (this.X + v.X) / 2.0,
                (this.Y + v.Y) / 2.0);
        }

        public Vector2 NearestPointOnLine(Vector2 a, Vector2 b)
        {
            return (this - a).Project(b - a) + a;
        }

        public const double RadiansToDegrees = 180.0 / Math.PI;
        public const double DegreesToRadians = Math.PI / 180.0;

        public static double Clamp(double value, double min, double max)
        {
            return value > max ? max : value < min ? min : value;
        }
    }

    public class MonotoneChain
    {
        // Implementation of Andrew's monotone chain 2D convex hull algorithm.
        // http://en.wikibooks.org/wiki/Algorithm_Implementation/Geometry/Convex_hull/Monotone_chain
        // Asymptotic complexity O(n log n).

        // 2D cross product of OA and OB vectors, i.e. z-component of their 3D cross product.
        // Returns a positive value, if OAB makes a counter-clockwise turn,
        // negative for clockwise turn, and zero if the vertices are collinear.
        public double Cross(Vector2 p1, Vector2 p2, Vector2 p3)
        {
            return (p2.X - p1.X) * (p3.Y - p1.Y) - (p2.Y - p1.Y) * (p3.X - p1.X);
        }

        // Returns a list of vertices on the convex hull in counter-clockwise order.
        // Note: the last vertice in the returned list is the same as the first one.
        public void ConvexHull(Vector2[] vertices, out Vector2[] hull, out int k)
        {
            int n = vertices.Length;
            int i, t;

            k = 0;
            hull = new Vector2[2 * n];

            // sort vertices lexicographically
            Array.Sort(vertices);

            // lower hull
            for (i = 0; i < n; i++)
            {
                while (k >= 2 && Cross(hull[k - 2], hull[k - 1], vertices[i]) <= 0)
                    k--;

                hull[k++] = vertices[i];
            }

            // upper hull
            for (i = n - 2, t = k + 1; i >= 0; i--)
            {
                while (k >= t && Cross(hull[k - 2], hull[k - 1], vertices[i]) <= 0)
                    k--;

                hull[k++] = vertices[i];
            }
        }
    }
}
