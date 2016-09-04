// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace ArcDemo.Model
{
    public struct WpfArc
    {
        public Point P1;
        public Point P2;
        public Point P3;
        public Point P4;
        public Rect Rect;
        public Point Center;
        public Point Start;
        public Point End;
        public Size Radius;
        public bool IsLargeArc;
        public double Angle;

        public static WpfArc FromXArc(XArc arc, double dx, double dy)
        {
            var p1 = new Point(arc.Point1.X, arc.Point1.Y);
            var p2 = new Point(arc.Point2.X, arc.Point2.Y);
            var p3 = new Point(arc.Point3.X, arc.Point3.Y);
            var p4 = new Point(arc.Point4.X, arc.Point4.Y);
            var rect = new Rect(p1, p2);
            var center = new Point(rect.X + rect.Width / 2.0, rect.Y + rect.Height / 2.0);
            double offsetX = center.X - rect.X;
            double offsetY = center.Y - rect.Y;

            double minLenght = Math.Max(offsetX, offsetY);

            double length1 = MathHelper.Distance(center, p3);
            double p3x = p3.X + (p3.X - center.X) / length1 * minLenght;
            double p3y = p3.Y + (p3.Y - center.Y) / length1 * minLenght;

            double length2 = MathHelper.Distance(center, p4);
            double p4x = p4.X + (p4.X - center.X) / length2 * minLenght;
            double p4y = p4.Y + (p4.Y - center.Y) / length2 * minLenght;

            p3.X = p3x;
            p3.Y = p3y;
            p4.X = p4x;
            p4.Y = p4y;

            var p3i = MathHelper.FindEllipseSegmentIntersections(rect, center, p3, true);
            var p4i = MathHelper.FindEllipseSegmentIntersections(rect, center, p4, true);
            Point start;
            Point end;

            if (p3i.Count == 1)
                start = p3i.FirstOrDefault();
            else
                start = new Point(p3.X, p3.Y);

            if (p4i.Count == 1)
                end = p4i.FirstOrDefault();
            else
                end = new Point(p4.X, p4.Y);

            double angle = MathHelper.AngleLineSegments(center, start, center, end);
            bool isLargeArc = angle > 180.0;

            double helperLenght = 60.0;

            double lengthStart = MathHelper.Distance(center, start);
            double p3hx = start.X + (start.X - center.X) / lengthStart * helperLenght;
            double p3hy = start.Y + (start.Y - center.Y) / lengthStart * helperLenght;

            double lengthEnd = MathHelper.Distance(center, end);
            double p4hx = end.X + (end.X - center.X) / lengthEnd * helperLenght;
            double p4hy = end.Y + (end.Y - center.Y) / lengthEnd * helperLenght;

            p3.X = p3hx;
            p3.Y = p3hy;
            p4.X = p4hx;
            p4.Y = p4hy;

            return new WpfArc()
            {
                P1 = p1,
                P2 = p2,
                P3 = p3,
                P4 = p4,
                Rect = rect,
                Center = center,
                Start = start,
                End = end,
                Radius = new Size(offsetX, offsetY),
                IsLargeArc = isLargeArc,
                Angle = angle
            };
        }
    }
}
