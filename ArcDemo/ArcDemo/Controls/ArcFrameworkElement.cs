// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Collections.Specialized;

namespace WpfApplication1
{
    public class ArcFrameworkElement : FrameworkElement
    {
        public Cloud Cloud
        {
            get { return (Cloud)GetValue(CloudProperty); }
            set { SetValue(CloudProperty, value); }
        }

        public static readonly DependencyProperty CloudProperty =
            DependencyProperty.Register("Cloud", 
            typeof(Cloud), 
            typeof(ArcFrameworkElement), 
            new FrameworkPropertyMetadata(new Cloud(), 
                FrameworkPropertyMetadataOptions.AffectsRender | 
                FrameworkPropertyMetadataOptions.AffectsMeasure |
                FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

        private Brush brush;
        private Pen pen;

        public ArcFrameworkElement()
        {
            brush = Brushes.Transparent;
            brush.Freeze();

            pen = new Pen(Brushes.Red, 1.0)
            {
                StartLineCap = PenLineCap.Round,
                EndLineCap = PenLineCap.Round
            };
            pen.Freeze();
        }

        private void DrawArcs(DrawingContext dc, IEnumerable<Arc> arcs)
        {
            var pg = new PathGeometry();

            foreach (var arc in arcs)
            {
                var pf = new PathFigure();

                pg.Figures.Add(pf);
                pf.StartPoint = arc.Start;

                var segment = new ArcSegment(
                    arc.End, 
                    arc.Radius, 
                    0.0, 
                    false, 
                    SweepDirection.Clockwise, 
                    true);

                pf.Segments.Add(segment);
            }

            dc.DrawGeometry(brush, pen, pg);
        }

        private IEnumerable<Arc> GetArcs(IEnumerable<Point> points)
        {
            if (points.Count() < 2)
            {
                return null;
            }

            var arcs = new List<Arc>();
            var start = points.First();

            foreach (var end in points)
            {
                double distance = MathUtil.Distance(start, end);

                var arc = new Arc()
                {
                    Start = new Point(start.X, start.Y),
                    End = new Point(end.X, end.Y),
                    Radius = new Size(distance / 2.0, distance / 2.0)
                };

                arcs.Add(arc);
                start = end;
            }

            return arcs;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            if (Cloud.Points != null)
            {
                (Cloud.Points as ObservableCollection<Point>).CollectionChanged +=
                    (s, e) =>
                    {
                        this.InvalidateVisual();
                    };

                var arcs = GetArcs(Cloud.Points);
                if (arcs != null)
                {
                    DrawArcs(drawingContext, arcs);
                }
            }
        }
    }
}
