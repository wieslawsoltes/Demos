// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using ArcDemo.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace ArcDemo.Controls
{
    public class ArcElement : FrameworkElement
    {
        private Brush _brushHelpers = new SolidColorBrush(Colors.DarkGray);
        private Pen _penHelpers = new Pen(Brushes.DarkGray, 2.0) { DashStyle = DashStyles.Dash };
        private Pen _penGeometry = new Pen(Brushes.Black, 2.0);
        private double _ellipseRadius = 4.0;

        public ArcState State { get; set; }
        public XArc WorkingArc { get; set; }
        public IList<XArc> Arcs { get; set; }

        protected override void OnRender(DrawingContext dc)
        {
            if (WorkingArc != null)
            {
                Draw(dc, WorkingArc, State, true, false);
            }

            foreach (var arc in Arcs)
            {
                Draw(dc, arc, ArcState.Point4, false, false);
            }
        }

        private void Draw(DrawingContext dc, XArc arc, ArcState state, bool helpersVisible, bool debugVisible)
        {
            switch (state)
            {
                case ArcState.Point1:
                    {
                    }
                    break;
                case ArcState.Point2:
                    {
                        DrawPoint2(dc, arc, helpersVisible, debugVisible);
                    }
                    break;
                case ArcState.Point3:
                    {
                        DrawPoint3(dc, arc, helpersVisible, debugVisible);
                    }
                    break;
                case ArcState.Point4:
                    {
                        DrawPoint4(dc, arc, helpersVisible, debugVisible);
                    }
                    break;
            }
        }

        private void DrawPoint2(DrawingContext dc, XArc arc, bool helpersVisible, bool debugVisible)
        {
            var a = WpfArc.FromXArc(arc, 0, 0);

            if (debugVisible)
            {
                dc.DrawEllipse(_brushHelpers, null, a.P1, _ellipseRadius, _ellipseRadius);
                dc.DrawEllipse(_brushHelpers, null, a.P2, _ellipseRadius, _ellipseRadius);
                dc.DrawEllipse(_brushHelpers, null, a.Center, _ellipseRadius, _ellipseRadius);
                dc.DrawRectangle(null, _penHelpers, a.Rect);
            }

            if (helpersVisible)
            {
                dc.DrawEllipse(null, _penHelpers, a.Center, a.Radius.Width, a.Radius.Height);
            }
        }

        private void DrawPoint3(DrawingContext dc, XArc arc, bool helpersVisible, bool debugVisible)
        {
            var a = WpfArc.FromXArc(arc, 0, 0);

            if (debugVisible)
            {
                dc.DrawEllipse(_brushHelpers, null, a.P1, _ellipseRadius, _ellipseRadius);
                dc.DrawEllipse(_brushHelpers, null, a.P2, _ellipseRadius, _ellipseRadius);
                dc.DrawEllipse(_brushHelpers, null, a.P3, _ellipseRadius, _ellipseRadius);
                dc.DrawEllipse(_brushHelpers, null, a.Center, _ellipseRadius, _ellipseRadius);
                dc.DrawRectangle(null, _penHelpers, a.Rect);
                dc.DrawLine(_penHelpers, a.Center, a.P3);
            }

            if (helpersVisible)
            {
                dc.DrawEllipse(null, _penHelpers, a.Center, a.Radius.Width, a.Radius.Height);

                if (!debugVisible)
                {
                    dc.DrawLine(_penHelpers, a.Center, a.Start);
                }
            }
        }

        private void DrawPoint4(DrawingContext dc, XArc arc, bool helpersVisible, bool debugVisible)
        {
            var a = WpfArc.FromXArc(arc, 0, 0);
            var pg = FromWpfArc(a);

            if (debugVisible)
            {
                dc.DrawEllipse(_brushHelpers, null, a.P1, _ellipseRadius, _ellipseRadius);
                dc.DrawEllipse(_brushHelpers, null, a.P2, _ellipseRadius, _ellipseRadius);
                dc.DrawEllipse(_brushHelpers, null, a.P3, _ellipseRadius, _ellipseRadius);
                dc.DrawEllipse(_brushHelpers, null, a.P4, _ellipseRadius, _ellipseRadius);
                dc.DrawEllipse(_brushHelpers, null, a.Center, _ellipseRadius, _ellipseRadius);
                dc.DrawRectangle(null, _penHelpers, a.Rect);
                dc.DrawLine(_penHelpers, a.Center, a.P3);
                dc.DrawLine(_penHelpers, a.Center, a.P4);
                dc.DrawEllipse(_brushHelpers, null, a.Start, _ellipseRadius, _ellipseRadius);
                dc.DrawEllipse(_brushHelpers, null, a.End, _ellipseRadius, _ellipseRadius);
            }

            if (helpersVisible)
            {
                dc.DrawEllipse(null, _penHelpers, a.Center, a.Radius.Width, a.Radius.Height);

                if (!debugVisible)
                {
                    dc.DrawLine(_penHelpers, a.Center, a.Start);
                    dc.DrawLine(_penHelpers, a.Center, a.End);
                }
            }

            dc.DrawGeometry(null, _penGeometry, pg);
        }
        
        private PathGeometry FromWpfArc(WpfArc arc)
        {
            var pf = new PathFigure() { StartPoint = arc.Start, IsFilled = false };
            var segment = new ArcSegment(arc.End, arc.Radius, 0.0, arc.IsLargeArc, SweepDirection.Clockwise, true);
            pf.Segments.Add(segment);
            var pg = new PathGeometry();
            pg.Figures.Add(pf);
            return pg;
        }
    }
}
