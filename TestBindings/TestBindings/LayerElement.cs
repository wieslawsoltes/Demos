// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using Test2d;

namespace TestBindings
{
    public class Test
    {
        public ImmutableArray<Value> Values { get; set; }
        public ImmutableArray<XPoint> Points { get; set; }
        public ImmutableArray<XBinding> Bindings { get; set; }
    }

    public class LayerElement : ObservableFrameworkElement
    {
        private Test _test;
        private string _elapsed = string.Empty;

        public Test Test
        {
            get { return _test; }
            set { Update(ref _test, value); }
        }

        public string Elapsed
        {
            get { return _elapsed; }
            set { Update(ref _elapsed, value); }
        }
        
        private void Drawline(DrawingContext dc, Pen pen, XPoint p1, XPoint p2)
        {
            double x1 = p1.X;
            double y1 = p1.Y;
            double x2 = p2.X;
            double y2 = p2.Y;
            dc.DrawLine(pen, new Point(x1, y1), new Point(x2, y2));
        }
        
        private void DrawPolyline(DrawingContext dc, Pen pen, IList<XPoint> points)
        {
            for (int i = 0, j = points.Count - 1; i < points.Count; j = i++)
            {
                Drawline(dc, pen, points[i], points[j]);
            }
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            var brush = new SolidColorBrush(Colors.Red);
            brush.Freeze();
            
            var pen = new Pen(brush, 1.0);
            pen.Freeze();

            var sw = Stopwatch.StartNew();

            DrawPolyline(drawingContext, pen, Test.Points);

            foreach (var point in Test.Points)
            {
                double x = point.X;
                double y = point.Y;
                drawingContext.DrawEllipse(brush, null, new Point(x, y), 2, 2);
            }

            sw.Stop();
            
            Elapsed = sw.Elapsed.TotalMilliseconds + "ms";
            //Trace.WriteLine(Elapsed);
        }
    }
}
