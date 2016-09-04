
#region References

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#endregion

namespace ConvexHull
{
    #region MonotoneChain

    public static class MonotoneChain
    {
        // Implementation of Andrew's monotone chain 2D convex hull algorithm.
        // http://en.wikibooks.org/wiki/Algorithm_Implementation/Geometry/Convex_hull/Monotone_chain

        // Asymptotic complexity O(n log n).

        // 2D cross product of OA and OB vectors, i.e. z-component of their 3D cross product.
        // Returns a positive value, if OAB makes a counter-clockwise turn,
        // negative for clockwise turn, and zero if the points are collinear.
        public static double Cross(Point2 p1, Point2 p2, Point2 p3)
        {
            return (p2.X - p1.X) * (p3.Y - p1.Y) - (p2.Y - p1.Y) * (p3.X - p1.X);
        }

        // Returns a list of points on the convex hull in counter-clockwise order.
        // Note: the last point in the returned list is the same as the first one.
        public static void ConvexHull(List<Point2> points, out Point2[] hull, out int k)
        {
            int n = points.Count;
            int i, t;

            k = 0;
            hull = new Point2[2 * n];

            // sort points lexicographically
            points.Sort();

            // lower hull
            for (i = 0; i < n; i++)
            {
                while (k >= 2 && Cross(hull[k - 2], hull[k - 1], points[i]) <= 0)
                    k--;

                hull[k++] = points[i];
            }

            // upper hull
            for (i = n - 2, t = k + 1; i >= 0; i--)
            {
                while (k >= t && Cross(hull[k - 2], hull[k - 1], points[i]) <= 0)
                    k--;

                hull[k++] = points[i];
            }
        }

        // Returns a list of points on the convex hull in counter-clockwise order.
        // Note: the last point in the returned list is the same as the first one.
        public static void ConvexHullArray(List<Point2> points, out Point2[] hull, out int k, bool parallel)
        {
            Point2[] array = points.ToArray();

            int n = array.Length;
            int i, t;

            k = 0;
            hull = new Point2[2 * n];

            // sort points lexicographically
            if (parallel)
                Sort.ParallelQuickSort<Point2>(array);
            else
                Array.Sort(array);

            // lower hull
            for (i = 0; i < n; i++)
            {
                while (k >= 2 && Cross(hull[k - 2], hull[k - 1], array[i]) <= 0)
                    k--;

                hull[k++] = array[i];
            }

            // upper hull
            for (i = n - 2, t = k + 1; i >= 0; i--)
            {
                while (k >= t && Cross(hull[k - 2], hull[k - 1], array[i]) <= 0)
                    k--;

                hull[k++] = array[i];
            }
        }
    }

    #endregion
}
