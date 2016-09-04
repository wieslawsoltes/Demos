// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace ArcDemo.Model
{
    public class MathHelper
    {
        public static double AngleLineSegments(Point line1Start, Point line1End, Point line2Start, Point line2End)
        {
            double angle1 = Math.Atan2(line1Start.Y - line1End.Y, line1Start.X - line1End.X);
            double angle2 = Math.Atan2(line2Start.Y - line2End.Y, line2Start.X - line2End.X);
            double result = (angle2 - angle1) * 180 / Math.PI;
            if (result < 0)
                result += 360;
            return result;
        }

        public static double Distance(Point p1, Point p2)
        {
            double dx = p1.X - p2.X;
            double dy = p1.Y - p2.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        public static IList<Point> FindEllipseSegmentIntersections(Rect rect, Point p1, Point p2, bool onlySegment)
        {
            if ((rect.Width == 0) || (rect.Height == 0) || ((p1.X == p2.X) && (p1.Y == p2.Y)))
                return new Point[] { };

            if (rect.Width < 0)
            {
                rect.X = rect.Right;
                rect.Width = -rect.Width;
            }

            if (rect.Height < 0)
            {
                rect.Y = rect.Bottom;
                rect.Height = -rect.Height;
            }

            double cx = rect.Left + rect.Width / 2.0;
            double cy = rect.Top + rect.Height / 2.0;

            rect.X -= cx;
            rect.Y -= cy;

            p1.X -= cx;
            p1.Y -= cy;
            p2.X -= cx;
            p2.Y -= cy;

            double a = rect.Width / 2.0;
            double b = rect.Height / 2.0;

            double A = (p2.X - p1.X) * (p2.X - p1.X) / a / a + (p2.Y - p1.Y) * (p2.Y - p1.Y) / b / b;
            double B = 2 * p1.X * (p2.X - p1.X) / a / a + 2 * p1.Y * (p2.Y - p1.Y) / b / b;
            double C = p1.X * p1.X / a / a + p1.Y * p1.Y / b / b - 1;

            var solutions = new List<double>();

            double discriminant = B * B - 4 * A * C;
            if (discriminant == 0)
            {
                solutions.Add(-B / 2 / A);
            }
            else if (discriminant > 0)
            {
                solutions.Add((-B + Math.Sqrt(discriminant)) / 2 / A);
                solutions.Add((-B - Math.Sqrt(discriminant)) / 2 / A);
            }

            var points = new List<Point>();

            foreach (var t in solutions)
            {
                if (!onlySegment || ((t >= 0f) && (t <= 1f)))
                {
                    double x = p1.X + (p2.X - p1.X) * t + cx;
                    double y = p1.Y + (p2.Y - p1.Y) * t + cy;
                    points.Add(new Point(x, y));
                }
            }

            return points;
        }

        public static Point RotatePoint(Point pointToRotate, Point centerPoint, double angleInDegrees)
        {
            double angleInRadians = angleInDegrees * (Math.PI / 180.0);
            double cosTheta = Math.Cos(angleInRadians);
            double sinTheta = Math.Sin(angleInRadians);
            return new Point
            {
                X = (cosTheta * (pointToRotate.X - centerPoint.X) - sinTheta * (pointToRotate.Y - centerPoint.Y) + centerPoint.X),
                Y = (sinTheta * (pointToRotate.X - centerPoint.X) + cosTheta * (pointToRotate.Y - centerPoint.Y) + centerPoint.Y)
            };
        }
    }
}
