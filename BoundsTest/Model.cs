using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace BoundsTest
{
    public class XPoint : IPoint
    {
        public double X { get; set; }
        public double Y { get; set; }
        public XPoint(double x, double y)
        {
            X = x;
            Y = y;
        }
    }

    public class XPolygon : IPolygon
    {
        public IPoint[] Points { get; set; }
        public ILine[] Lines { get; set; }

        public bool Contains(IPoint point)
        {
            return Contains(point.X, point.Y);
        }

        public bool Contains(double x, double y)
        {
            bool contains = false;
            for (int i = 0, j = Points.Length - 1; i < Points.Length; j = i++)
            {
                if (((Points[i].Y > y) != (Points[j].Y > y))
                    && (x < (Points[j].X - Points[i].X) * (y - Points[i].Y) / (Points[j].Y - Points[i].Y) + Points[i].X))
                {
                    contains = !contains;
                }
            }
            return contains;
        }
    }

    public class XCanvas : ICanvas
    {
        public object Native { get; set; }
    }

    public class WpfLine : ILine
    {
        public object Native { get; set; }
        public IBounds Bounds { get; set; }

        public void Move(IPoint p1, IPoint p2)
        {
            var line = (Native as Line);
            line.X1 = p1.X;
            line.Y1 = p1.Y;
            line.X2 = p2.X;
            line.Y2 = p2.Y;
        }
    }

    public class WpfEllipse : IEllipse
    {
        public object Native { get; set; }
        public IBounds Bounds { get; set; }
        public double X
        {
            get { return (Native as FrameworkElement).GetX(); }
            set { (Native as FrameworkElement).SetX(value); }
        }
        public double Y
        {
            get { return (Native as FrameworkElement).GetY(); }
            set { (Native as FrameworkElement).SetY(value); }
        }
        public double Width
        {
            get { return (Native as FrameworkElement).GetWidth(); }
            set { (Native as FrameworkElement).SetWidth(value); }
        }
        public double Height
        {
            get { return (Native as FrameworkElement).GetHeight(); }
            set { (Native as FrameworkElement).SetHeight(value); }
        }
    }

    public class WpfEllipseBounds : IBounds
    {
        private IEllipse _ellipse;
        private double _offset;
        private ICanvas _canvas;
        private IPolygon _polygon;
        private bool _isVisible;

        public WpfEllipseBounds(ICanvas canvas, IEllipse ellipse, double offset)
        {
            _ellipse = ellipse;
            _offset = offset;
            _canvas = canvas;

            _polygon = new XPolygon()
            {
                Points = new XPoint[4],
                Lines = new WpfLine[4]
            };

            for (int i = 0; i < 4; i++)
            {
                _polygon.Points[i] = new XPoint(0.0, 0.0);
                _polygon.Lines[i] = new WpfLine()
                {
                    Native = new Line() { Stroke = Brushes.DeepSkyBlue, StrokeThickness = 2.0, Opacity = 0.6 }
                };
            }
        }

        public void Update()
        {
            var ps = _polygon.Points;
            var ls = _polygon.Lines;
            var offset = _offset;

            double x = _ellipse.X;
            double y = _ellipse.Y;
            double width = _ellipse.Width;
            double height = _ellipse.Height;

            // top-left
            ps[0].X = x - offset;
            ps[0].Y = y - offset;
            // top-right
            ps[1].X = (x + width) + offset;
            ps[1].Y = y - offset;
            // botton-right
            ps[2].X = (x + width) + offset;
            ps[2].Y = (y + height) + offset;
            // bottom-left
            ps[3].X = x - offset;
            ps[3].Y = (y + height) + offset;

            ls[0].Move(ps[0], ps[1]);
            ls[1].Move(ps[1], ps[2]);
            ls[2].Move(ps[2], ps[3]);
            ls[3].Move(ps[3], ps[0]);
        }

        public bool IsVisible()
        {
            return _isVisible;
        }

        public void Show()
        {
            if (!_isVisible)
            {
                foreach (var line in _polygon.Lines)
                {
                    (_canvas.Native as Canvas).Children.Add(line.Native as UIElement);
                }
                _isVisible = true;
            }
        }

        public void Hide()
        {
            if (_isVisible)
            {
                foreach (var line in _polygon.Lines)
                {
                    (_canvas.Native as Canvas).Children.Remove(line.Native as UIElement);
                }
                _isVisible = false;
            }
        }

        public bool Contains(double x, double y)
        {
            return _polygon.Contains(x, y);
        }
    }
}
