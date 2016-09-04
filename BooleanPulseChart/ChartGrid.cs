// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace BooleanPulseChart
{
    public class ChartGrid : DrawingVisual
    {
        public ChartGrid(Size frame, Size cell, double thickness, Brush stroke)
        {
            double halfThickness = thickness / 2.0;
            XSnappingGuidelines = new DoubleCollection(new[] { halfThickness, halfThickness });
            YSnappingGuidelines = new DoubleCollection(new[] { halfThickness, halfThickness });

            using (var dc = this.RenderOpen())
            {
                var frameGeometry = new StreamGeometry();
                using (var ctx = frameGeometry.Open())
                {
                    ctx.BeginFigure(new Point(0, 0), false, false);
                    ctx.LineTo(new Point(frame.Width, 0), true, false);
                    ctx.LineTo(new Point(frame.Width, frame.Height), true, false);
                    ctx.LineTo(new Point(0, frame.Height), true, false);
                    ctx.LineTo(new Point(0, 0), true, false);
                }
                frameGeometry.Freeze();

                var linesGeometry = new StreamGeometry();
                using (var ctx = linesGeometry.Open())
                {
                    for (double y = cell.Height; y < frame.Height; y += cell.Height)
                    {
                        ctx.BeginFigure(new Point(0, y), false, false);
                        ctx.LineTo(new Point(frame.Width, y), true, false);
                    }

                    for (double x = cell.Width; x < frame.Width; x += cell.Width)
                    {
                        ctx.BeginFigure(new Point(x, 0), false, false);
                        ctx.LineTo(new Point(x, frame.Height), true, false);
                    }
                }
                linesGeometry.Freeze();

                var framePen = new Pen(stroke, thickness);
                framePen.Freeze();

                var linesPen = new Pen(stroke, thickness)
                {
                    DashStyle = new DashStyle(new double [] { 1, 2 }, 0)
                };
                linesPen.Freeze();

                dc.DrawGeometry(null, framePen, frameGeometry);
                dc.DrawGeometry(null, linesPen, linesGeometry);
            }
        }
    }
}
