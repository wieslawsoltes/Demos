
#region References

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#endregion

namespace ConvexHull
{
    #region Point2

    public class Point2 : IComparable<Point2>
    {
        public double X { get; set; }
        public double Y { get; set; }
        public bool IsInside { get; set; }

        public Point2(double x, double y)
        {
            X = x;
            Y = y;
        }

        public static bool operator <(Point2 p1, Point2 p2)
        {
            return p1.X < p2.X || (p1.X == p2.X && p1.Y < p2.Y);
        }

        public static bool operator >(Point2 p1, Point2 p2)
        {
            return p1.X > p2.X || (p1.X == p2.X && p1.Y > p2.Y);
        }

        public int CompareTo(Point2 other)
        {
            return (this > other) ? -1 : ((this < other) ? 1 : 0);
        }
    }

    #endregion
}
