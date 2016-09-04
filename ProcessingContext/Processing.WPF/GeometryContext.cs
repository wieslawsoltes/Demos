using Processing.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Processing.WPF
{
    public class GeometryContext : IGeometryContext
    {
        private PathGeometry _path;
        private GeometryGroup _geometries;
        private Transform _currentTransform = Transform.Identity;
        private Stack<Matrix> _matrixes = new Stack<Matrix>();
        private bool _closed;
        private bool _filled;

        private PathFigure _currentFigure;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private PathFigure CurrentFigure
        {
            get
            {
                if (_currentFigure == null)
                {
                    _currentFigure = new PathFigure();
                    _path.Figures.Add(_currentFigure);
                    SetProperties(_currentFigure);
                    _currentFigure.StartPoint = _origin;
                }
                return _currentFigure;
            }
        }

        private Point _origin = new Point(0.0, 0.0);
        public Point Origin
        {
            get { return _origin; }
        }

        public GeometryContext(GeometryGroup group)
        {
            _path = new PathGeometry();
            _geometries = group;
            _geometries.Children.Add(_path);
        }

        private void Append(Matrix matrix)
        {
            var currentTransform = _currentTransform.Value;
            currentTransform.Append(matrix);
            _currentTransform = new MatrixTransform(currentTransform);
        }

        private void Remove(Matrix matrix)
        {
            matrix.Invert();
            Append(matrix);
        }

        private void PushTransform(Transform transform)
        {
            Append(transform.Value);
            _origin = _currentTransform.Transform(new Point(0, 0));
        }

        private void Translate(double x, double y)
        {
            var t = _currentTransform.Transform(new Point(x, y));
            PushTransform(new TranslateTransform(t.X - _origin.X, t.Y - _origin.Y));
        }

        private void SetProperties(PathFigure figure)
        {
            figure.IsClosed = _closed;
            figure.IsFilled = _filled;
        }

        private PathGeometry ExtractPath(Geometry geometry)
        {
            var figures = geometry.GetOutlinedPathGeometry().Figures.Select(f =>
            {
                var figure = f.Clone();
                SetProperties(figure);
                return figure;
            });
            return new PathGeometry(figures);
        }

        public void PushMatrix()
        {
            _matrixes.Push(_currentTransform.Value);
        }

        public void PopMatrix()
        {
            _currentTransform = new MatrixTransform(_matrixes.Pop());
            _origin = _currentTransform.Transform(new Point(0, 0));
        }

        public void Move(double x, double y)
        {
            Translate(x, y);
        }

        public void Rotate(double degree)
        {
            var rotation = new RotateTransform(degree)
            {
                CenterX = _origin.X,
                CenterY = _origin.Y
            };
            this.PushTransform(rotation);
        }

        public void Scale(double x, double y)
        {
            this.PushTransform(new ScaleTransform(x, y)
            {
                CenterX = _origin.X,
                CenterY = _origin.Y
            });
        }

        public void Line(double x, double y, bool isStroked)
        {
            var nextPoint = _currentTransform.Transform(new Point(x, y));
            var line = new LineSegment(nextPoint, isStroked);
            this.CurrentFigure.Segments.Add(line);
            Translate(x, y);
        }

        public void Arc(double x, double y, double width, double height, double angle, bool isStroked)
        {
            var nextPoint = _currentTransform.Transform(new Point(x, y));
            var arc = new ArcSegment(nextPoint, new Size(width, height), angle, false, SweepDirection.Clockwise, isStroked);
            this.CurrentFigure.Segments.Add(arc);
            Translate(x, y);
        }

        public void Bezier(double x, double y, double controlX, double controlY, bool isStroked)
        {
            var nextPoint = _currentTransform.Transform(new Point(x, y));
            var controlPoint = _currentTransform.Transform(new Point(controlX, controlY));
            var bezier = new BezierSegment(_origin, controlPoint, nextPoint, isStroked);
            this.CurrentFigure.Segments.Add(bezier);
            Translate(x, y);
        }

        public void Ellipse(double radiusX, double radiusY)
        {
            var result = ExtractPath(
                new EllipseGeometry(
                    new Point(0, 0), 
                    radiusX, 
                    radiusY, 
                    _currentTransform));
            _geometries.Children.Add(result);
        }

        public void Rectangle(double radiusX, double radiusY, double radiusCornX, double radiusCornY)
        {
            var result = ExtractPath(
                new RectangleGeometry(
                    new Rect(-radiusX, -radiusY, radiusX * 2, radiusY * 2),
                    radiusCornX, 
                    radiusCornY, 
                    _currentTransform));
            _geometries.Children.Add(result);
        }

        private void DrawArrowHead(double size)
        {
            PushMatrix();
            Rotate(135);
            Line(size, 0, true);
            Line(-size, 0, false);
            PopMatrix();

            PushMatrix();
            Rotate(-135);
            Line(size, 0, true);

            Line(-size, 0, false);
            PopMatrix();
        }

        public void Basis(double size)
        {
            var oldFigure = _currentFigure;

            _currentFigure = new PathFigure();
            _currentFigure.IsClosed = false;
            _currentFigure.IsFilled = false;
            _path.Figures.Add(_currentFigure);
            PushMatrix();

            Line(-size, 0, false);
            Line(size, 0, true);
            Ellipse(1, 1);
            Line(size, 0.0, true);
            DrawArrowHead(size / 5);
            Line(-size, 0.0, false);

            Rotate(90);

            Line(-size, 0, false);
            Line(size, 0, true);
            Line(size, 0.0, true);
            DrawArrowHead(size / 10);

            _currentFigure = oldFigure;
            PopMatrix();
        }

        public void Closed()
        {
            _closed = true;
        }

        public void NotClosed()
        {
            _closed = false;
        }

        public void Filled()
        {
            _filled = true;
        }

        public void NotFilled()
        {
            _filled = false;
        }

        public void EndFigure()
        {
            var figure = CurrentFigure;
            SetProperties(figure);
            _currentFigure = null;
        }
    }
}
