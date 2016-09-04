using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using DemoDraw.Core;
using DemoDraw.Core.Renderer;
using DemoDraw.Core.Shapes;
using DemoDraw.Core.Style;

namespace DemoDraw.Editor
{
    public class WpfRenderer : ShapeRenderer
    {
        class StyleCache : XDisposable
        {
            public Brush Stroke;
            public Pen StrokePen;
            public Brush Fill;

            protected override void Dispose(bool disposing) { }

            public static StyleCache Create(XStyle style)
            {
                var cache = new StyleCache();
                cache.Stroke = WpfRenderer.ToBrush(style.Stroke);
                cache.Stroke.Freeze();
                cache.StrokePen = new Pen(cache.Stroke, style.StrokeThickness);
                cache.StrokePen.Freeze();
                cache.Fill = WpfRenderer.ToBrush(style.Fill);
                cache.Fill.Freeze();
                return cache;
            }
        }

        IDictionary<XStyle, StyleCache> StylesCache;

        StyleCache CacheXStyle(XStyle style)
        {
            StyleCache cache;

            if (!StylesCache.TryGetValue(style, out cache))
            {
                cache = StyleCache.Create(style);
                StylesCache[style] = cache;
            }

            return cache;
        }

        public static Point ToPoint(XPoint point, double dx, double dy)
        {
            return new Point(point.X + dx, point.Y + dy);
        }

        public static Rect ToRect(XPoint tl, XPoint br, double dx, double dy)
        {
            double tlx = Math.Min(tl.X, br.X);
            double tly = Math.Min(tl.Y, br.Y);
            double brx = Math.Max(tl.X, br.X);
            double bry = Math.Max(tl.Y, br.Y);
            double width = Math.Abs(brx - tlx);
            double height = Math.Abs(bry - tly);
            return new Rect(tlx, tly, width, height);
        }

        public static Brush ToBrush(XColor color)
        {
            return new SolidColorBrush(Color.FromArgb(color.A, color.R, color.G, color.B));
        }

        public WpfRenderer()
        {
            StylesCache = new Dictionary<XStyle, StyleCache>();
        }

        public override void DrawLine(XPoint start, XPoint end, XStyle style, double dx, double dy)
        {
            var dc = Context as DrawingContext;
            var cache = CacheXStyle(style);
            dc.DrawLine(cache.StrokePen, ToPoint(start, dx, dy), ToPoint(end, dx, dy));
        }

        public override void DrawRectangle(XPoint tl, XPoint br, XStyle style, double dx, double dy)
        {
            var dc = Context as DrawingContext;
            var cache = CacheXStyle(style);
            var rect = ToRect(tl, br, dx, dy);
            dc.DrawRectangle(cache.Fill, cache.StrokePen, rect);
        }

        public override void DrawEllipse(XPoint tl, XPoint br, XStyle style, double dx, double dy)
        {
            var dc = Context as DrawingContext;
            var cache = CacheXStyle(style);
            var rect = ToRect(tl, br, dx, dy);
            var radiusX = rect.Width / 2.0;
            var radiusY = rect.Height / 2.0;
            var center = new Point(rect.Left + radiusX, rect.Top + radiusY);
            dc.DrawEllipse(cache.Fill, cache.StrokePen, center, radiusX, radiusY);
        }

        protected override void Dispose(bool disposing)
        {
            foreach (var cache in StylesCache)
            {
                cache.Value.Dispose();
            }
        }

        public static WpfRenderer Create()
        {
            return new WpfRenderer();
        }
    }
}
