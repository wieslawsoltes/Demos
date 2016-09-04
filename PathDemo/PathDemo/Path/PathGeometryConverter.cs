﻿// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using TestPATH;

namespace PathDemo
{
    public static class PathGeometryConverter
    {
        public static IList<XPathPoint> ToXPoints(this IList<Point> points)
        {
            var xpoints = new List<XPathPoint>();
            foreach (var point in points)
            {
                xpoints.Add(XPathPoint.Create(point.X, point.Y));
            }
            return xpoints;
        }

        public static XPathGeometry ToXPathGeometry(this PathGeometry pg)
        {
            var xpg = XPathGeometry.Create(
                new List<XPathFigure>(),
                pg.FillRule == FillRule.EvenOdd ? XFillRule.Nonzero : XFillRule.EvenOdd,
                XPathRect.Create(pg.Bounds.X, pg.Bounds.Y, pg.Bounds.Width, pg.Bounds.Height));

            foreach (var pf in pg.Figures)
            {
                //Debug.Print("Figure:");

                xpg.BeginFigure(
                    XPathPoint.Create(pf.StartPoint.X, pf.StartPoint.Y),
                    pf.IsFilled,
                    pf.IsClosed);

                foreach (var segment in pf.Segments)
                {
                    //Debug.Print("  " + segment.GetType());

                    if (segment is ArcSegment)
                    {
                        var arcSegment = segment as ArcSegment;
                        xpg.ArcTo(
                            XPathPoint.Create(arcSegment.Point.X, arcSegment.Point.Y),
                            XPathSize.Create(arcSegment.Size.Width, arcSegment.Size.Height),
                            arcSegment.RotationAngle,
                            arcSegment.IsLargeArc,
                            arcSegment.SweepDirection == SweepDirection.Clockwise ? XSweepDirection.Clockwise : XSweepDirection.Counterclockwise,
                            arcSegment.IsStroked,
                            arcSegment.IsSmoothJoin);
                    }
                    else if (segment is BezierSegment)
                    {
                        var bezierSegment = segment as BezierSegment;
                        xpg.BezierTo(
                            XPathPoint.Create(bezierSegment.Point1.X, bezierSegment.Point1.Y),
                            XPathPoint.Create(bezierSegment.Point2.X, bezierSegment.Point2.Y),
                            XPathPoint.Create(bezierSegment.Point3.X, bezierSegment.Point3.Y),
                            bezierSegment.IsStroked,
                            bezierSegment.IsSmoothJoin);
                    }
                    else if (segment is LineSegment)
                    {
                        var lineSegment = segment as LineSegment;
                        xpg.LineTo(
                            XPathPoint.Create(lineSegment.Point.X, lineSegment.Point.Y),
                            lineSegment.IsStroked,
                            lineSegment.IsSmoothJoin);
                    }
                    else if (segment is PolyBezierSegment)
                    {
                        var polyBezierSegment = segment as PolyBezierSegment;
                        xpg.PolyBezierTo(
                            ToXPoints(polyBezierSegment.Points),
                            polyBezierSegment.IsStroked,
                            polyBezierSegment.IsSmoothJoin);
                    }
                    else if (segment is PolyLineSegment)
                    {
                        var polyLineSegment = segment as PolyLineSegment;
                        xpg.PolyLineTo(
                            ToXPoints(polyLineSegment.Points),
                            polyLineSegment.IsStroked,
                            polyLineSegment.IsSmoothJoin);
                    }
                    else if (segment is PolyQuadraticBezierSegment)
                    {
                        var polyQuadraticSegment = segment as PolyQuadraticBezierSegment;
                        xpg.PolyQuadraticBezierTo(
                            ToXPoints(polyQuadraticSegment.Points),
                            polyQuadraticSegment.IsStroked,
                            polyQuadraticSegment.IsSmoothJoin);
                    }
                    else if (segment is QuadraticBezierSegment)
                    {
                        var qbezierSegment = segment as QuadraticBezierSegment;
                        xpg.QuadraticBezierTo(
                            XPathPoint.Create(qbezierSegment.Point1.X, qbezierSegment.Point1.Y),
                            XPathPoint.Create(qbezierSegment.Point2.X, qbezierSegment.Point2.Y),
                            qbezierSegment.IsStroked,
                            qbezierSegment.IsSmoothJoin);
                    }
                    else
                    {
                        throw new NotSupportedException("Not supported segment type: " + segment.GetType());
                    }
                }
            }

            return xpg;
        }

        public static XPathGeometry ToXPathGeometry(this string source)
        {
            var g = Geometry.Parse(source);
            var pg = PathGeometry.CreateFromGeometry(g);
            return ToXPathGeometry(pg);
        }
    }
}
