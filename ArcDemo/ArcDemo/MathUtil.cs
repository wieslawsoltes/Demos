using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfApplication1
{
    public static class MathUtil
    {
        public static double Distance(Point p1, Point p2)
        {
            double dX = p2.X - p1.X;
            double dY = p2.Y - p1.Y;
            return Math.Sqrt(dX * dX + dY * dY);
        }

        public static Point PointOnCircle(double R, double cX, double cY, double pX, double pY)
        {
            double vX = pX - cX;
            double vY = pY - cY;
            double magV = Math.Sqrt(vX * vX + vY * vY);
            double aX = cX + vX / magV * R;
            double aY = cY + vY / magV * R;
            return new Point(aX, aY);
        }
    }
}
