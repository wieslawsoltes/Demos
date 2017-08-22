using System.Collections.Generic;
using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace DrawCanvasDemo
{
    public class DrawCanvas : Canvas
    {
        private double _width;
        private double _height;
        private SolidColorBrush _brush;
        private IList<EllipseShape> _ellipses;
        private IDictionary<EllipseShape, EllipseGeometry> _ellipsesCache;

        public DrawCanvas()
        {
            _width = 10;
            _height = 10;
            _brush = new SolidColorBrush(Color.Parse("red"));
            _ellipses = new List<EllipseShape>();
            _ellipsesCache = new Dictionary<EllipseShape, EllipseGeometry>();

            this.PointerMoved += (sender, args) =>
            {
                var p = args.GetPosition(this);

                var ellipse = new EllipseShape(p.X - (_width / 2), p.Y - (_height / 2), _width, _height);
                _ellipses.Add(ellipse);

                var geometry = new EllipseGeometry(new Rect(ellipse.X, ellipse.Y, ellipse.Width, ellipse.Height));
                _ellipsesCache.Add(ellipse, geometry);

                Debug.WriteLine(_ellipses.Count);
                this.InvalidateVisual();
            };
        }

        public override void Render(DrawingContext context)
        {
            base.Render(context);

            foreach (var ellipse in _ellipses)
            {
                var geometry = _ellipsesCache[ellipse];
                context.DrawGeometry(_brush, null, geometry);
            }
        }
    }
}
