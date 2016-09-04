using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace TestDemo
{
    public class GuideCanvas : Canvas
    {
        private IDictionary<BasicStyle, BasicStyleCache> _cache;

        public GuideEditor Editor { get; private set; }
  
        private BasicStyleCache FromCache(BasicStyle style)
        {
            BasicStyleCache value;
            if (_cache.TryGetValue(style, out value))
            {
                return value;
            }
            value = new BasicStyleCache(style);
            _cache.Add(style, value);
            return value;
        }
        
        public GuideCanvas()
        {
            _cache = new Dictionary<BasicStyle, BasicStyleCache>();
            
            Editor = GuideEditor.Create();
            Editor.InvalidateVisual = () => this.InvalidateVisual();
            Editor.CaptureMouse = () => this.CaptureMouse();
            Editor.ReleaseMouseCapture = () => this.ReleaseMouseCapture();

            PreviewMouseLeftButtonDown += 
                (s, e) =>
                {
                    var p = e.GetPosition(this);
                    Editor.LeftDown(p.X, p.Y);
                };
            
            PreviewMouseRightButtonDown += 
                (s, e) =>
                {
                    var p = e.GetPosition(this);
                    Editor.RightDown(p.X, p.Y);
                };

            PreviewMouseMove += 
                (s, e) =>
                {
                    var p = e.GetPosition(this);
                    Editor.Move(p.X, p.Y);
                };
        }

        private static void DrawPoint(DrawingContext dc, BasicStyleCache cache, GuidePoint point, double radius)
        {
            dc.DrawEllipse(cache.FillBrush, cache.StrokePen, new Point(point.X, point.Y), radius, radius);
        }

        private static void DrawLine(DrawingContext dc, BasicStyleCache cache, GuidePoint point0, GuidePoint point1)
        {
            var gs = new GuidelineSet(
                new double [] { point0.X + cache.HalfThickness, point1.X + cache.HalfThickness },
                new double [] { point0.Y + cache.HalfThickness, point1.Y + cache.HalfThickness });
            gs.Freeze();
            dc.PushGuidelineSet(gs);
            dc.DrawLine(cache.StrokePen, new Point(point0.X, point0.Y), new Point(point1.X, point1.Y));
            dc.Pop();
        }

        private static void DrawHorizontalGuide(DrawingContext dc, BasicStyleCache cache, GuidePoint point, double width)
        {
            var point0 = new GuidePoint(0, point.Y);
            var point1 = new GuidePoint(width, point.Y);
            DrawLine(dc, cache, point0, point1);
        }

        private static void DrawVerticalGuide(DrawingContext dc, BasicStyleCache cache, GuidePoint point, double height)
        {
            var point0 = new GuidePoint(point.X, 0);
            var point1 = new GuidePoint(point.X, height);
            DrawLine(dc, cache, point0, point1);
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);
            
            foreach (var line in Editor.Lines)
            {
                DrawLine(dc, FromCache(Editor.LineStyle), line.Point0, line.Point1);
            }
            
            if (Editor.IsCaptured || Editor.HaveSnapPoint)
            {
                DrawHorizontalGuide(dc, FromCache(Editor.HaveSnapPoint ? Editor.SnapGuideStyle : Editor.GuideStyle), Editor.GuidePosition, this.ActualWidth);
                DrawVerticalGuide(dc, FromCache(Editor.HaveSnapPoint ? Editor.SnapGuideStyle : Editor.GuideStyle), Editor.GuidePosition, this.ActualHeight);
            }
            
            if (Editor.IsCaptured)
            {
                DrawLine(dc, FromCache(Editor.NewLineStyle), Editor.Point0, Editor.Point1);
            }
            
            if (Editor.HaveSnapPoint)
            {
                DrawPoint(dc, FromCache(Editor.GuideStyle), Editor.SnapPoint, Editor.SnapPointRadius);
            }
        }
    }
}
