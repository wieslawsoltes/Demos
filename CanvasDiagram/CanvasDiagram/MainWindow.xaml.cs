using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace CanvasDiagram
{
    public struct Vector2 : IComparable<Vector2>
    {
        public static Vector2 One { get { return new Vector2(1.0); } }
        public static Vector2 Zero { get { return new Vector2(0.0); } }
        public static Vector2 UnitX { get { return new Vector2(1.0, 0.0); } }
        public static Vector2 UnitY { get { return new Vector2(0.0, 1.0); } }

        public double X { get; private set; }
        public double Y { get; private set; }

        public Vector2(double value)
            : this()
        {
            this.X = value;
            this.Y = value;
        }

        public Vector2(double x, double y)
            : this()
        {
            this.X = x;
            this.Y = y;
        }

        public override string ToString()
        {
            return string.Concat(X, System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator, Y);
        }

        public static bool operator <(Vector2 v1, Vector2 v2)
        {
            return v1.X < v2.X || (v1.X == v2.X && v1.Y < v2.Y);
        }

        public static bool operator >(Vector2 v1, Vector2 v2)
        {
            return v1.X > v2.X || (v1.X == v2.X && v1.Y > v2.Y);
        }

        public int CompareTo(Vector2 v)
        {
            return (this > v) ? -1 : ((this < v) ? 1 : 0);
        }

        public bool Equals(Vector2 v)
        {
            return this.X == v.X && this.Y == v.Y;
        }

        public override bool Equals(object obj)
        {
            if (obj is Vector2)
            {
                return this.Equals((Vector2)obj);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return this.X.GetHashCode() + this.Y.GetHashCode();
        }

        public Vector2 Negate()
        {
            return new Vector2(-this.X, -this.Y);
        }

        public Vector2 Perpendicular()
        {
            return new Vector2(-this.Y, this.X);
        }

        public Vector2 Subtract(Vector2 v)
        {
            return new Vector2(this.X - v.X, this.Y - v.Y);
        }

        public Vector2 Add(Vector2 v)
        {
            return new Vector2(this.X + v.X, this.Y + v.Y);
        }

        public Vector2 Multiply(double scalar)
        {
            return new Vector2(
                this.X * scalar,
                this.Y * scalar);
        }

        public Vector2 Multiply(Vector2 v)
        {
            return new Vector2(
                this.X * v.X,
                this.Y * v.Y);
        }

        public Vector2 Divide(double scalar)
        {
            double value = 1.0 / scalar;
            return new Vector2(
                this.X * value,
                this.Y * value);
        }

        public Vector2 Divide(Vector2 v)
        {
            return new Vector2(
                this.X / v.X,
                this.Y / v.Y);
        }

        public double Dot(Vector2 v)
        {
            return (this.X * v.X) + (this.Y * v.Y);
        }

        public double Angle(Vector2 v)
        {
            return Math.Acos(this.Dot(v));
        }

        public double Cross(Vector2 v)
        {
            return (this.X * v.Y) - (this.Y * v.X);
        }

        public static bool operator ==(Vector2 v1, Vector2 v2)
        {
            return v1.X == v2.X && v1.Y == v2.Y;
        }

        public static bool operator !=(Vector2 v1, Vector2 v2)
        {
            return v1.X != v2.X || v1.Y != v2.Y;
        }

        public static Vector2 operator -(Vector2 v)
        {
            return new Vector2(-v.X, -v.Y);
        }

        public static Vector2 operator -(Vector2 v1, Vector2 v2)
        {
            return new Vector2(v1.X - v2.X, v1.Y - v2.Y);
        }

        public static Vector2 operator +(Vector2 v1, Vector2 v2)
        {
            return new Vector2(v1.X + v2.X, v1.Y + v2.Y);
        }

        public static Vector2 operator *(Vector2 v, double scalar)
        {
            return new Vector2(
                v.X * scalar,
                v.Y * scalar);
        }

        public static Vector2 operator *(double scalar, Vector2 v)
        {
            return new Vector2(
                v.X * scalar,
                v.Y * scalar);
        }

        public static Vector2 operator *(Vector2 v1, Vector2 v2)
        {
            return new Vector2(
                v1.X * v2.X,
                v1.Y * v2.Y);
        }

        public static Vector2 operator /(Vector2 v, double scalar)
        {
            double value = 1.0 / scalar;
            return new Vector2(
                v.X * value,
                v.Y * value);
        }

        public static Vector2 operator /(Vector2 v1, Vector2 v2)
        {
            return new Vector2(
                v1.X / v2.X,
                v1.Y / v2.Y);
        }

        public double Magnitude()
        {
            return Math.Sqrt(this.X * this.X + this.Y * this.Y);
        }

        public double MagnitudeSquared()
        {
            return this.X * this.X + this.Y * this.Y;
        }

        public double Length()
        {
            return Math.Sqrt(this.X * this.X + this.Y * this.Y);
        }

        public double LengthSquared()
        {
            return this.X * this.X + this.Y * this.Y;
        }

        public Vector2 Normalize()
        {
            return this / this.Length();
        }

        //public double Component(Vector2 v)
        //{
        //    return this.Dot(v.Normalize());
        //}

        public Vector2 Project(Vector2 v)
        {
            return v * (this.Dot(v) / v.Dot(v));
        }

        //public Vector2 Project(Vector2 v)
        //{
        //    return v.Normalize() * this.Component(v);
        //}

        public Vector2 Reflect(Vector2 normal)
        {
            double dot = this.Dot(normal);
            return new Vector2(
                this.X - 2.0 * dot * normal.X,
                this.Y - 2.0 * dot * normal.Y);
        }

        public Vector2 Min(Vector2 v)
        {
            return new Vector2(
                this.X < v.X ? this.X : v.X,
                this.Y < v.Y ? this.Y : v.Y);
        }

        public Vector2 Max(Vector2 v)
        {
            return new Vector2(
                this.X > v.X ? this.X : v.X,
                this.Y > v.Y ? this.Y : v.Y);
        }

        public Vector2 Lerp(Vector2 v, double amount)
        {
            return this + (v - this) * amount;
        }

        public Vector2 Slerp(Vector2 v, double amount)
        {
            double dot = Clamp(this.Dot(v), -1.0, 1.0);
            double theta = Math.Acos(dot) * amount;
            Vector2 relative = (v - this * dot).Normalize();
            return ((this * Math.Cos(theta)) + (relative * Math.Sin(theta)));
        }

        public Vector2 Nlerp(Vector2 v, double amount)
        {
            return this.Lerp(v, amount).Normalize();
        }

        public double Distance(Vector2 v)
        {
            double dx = this.X - v.X;
            double dy = this.Y - v.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        public Vector2 Middle(Vector2 v)
        {
            return new Vector2(
                (this.X + v.X) / 2.0,
                (this.Y + v.Y) / 2.0);
        }

        public Vector2 NearestPointOnLine(Vector2 a, Vector2 b)
        {
            return (this - a).Project(b - a) + a;
        }

        public const double RadiansToDegrees = 180.0 / Math.PI;
        public const double DegreesToRadians = Math.PI / 180.0;

        public static double Clamp(double value, double min, double max)
        {
            return value > max ? max : value < min ? min : value;
        }
    }

    public class MonotoneChain
    {
        // Implementation of Andrew's monotone chain 2D convex hull algorithm.
        // http://en.wikibooks.org/wiki/Algorithm_Implementation/Geometry/Convex_hull/Monotone_chain
        // Asymptotic complexity O(n log n).

        // 2D cross product of OA and OB vectors, i.e. z-component of their 3D cross product.
        // Returns a positive value, if OAB makes a counter-clockwise turn,
        // negative for clockwise turn, and zero if the vertices are collinear.
        public double Cross(Vector2 p1, Vector2 p2, Vector2 p3)
        {
            return (p2.X - p1.X) * (p3.Y - p1.Y) - (p2.Y - p1.Y) * (p3.X - p1.X);
        }

        // Returns a list of vertices on the convex hull in counter-clockwise order.
        // Note: the last vertice in the returned list is the same as the first one.
        public void ConvexHull(Vector2[] vertices, out Vector2[] hull, out int k)
        {
            int n = vertices.Length;
            int i, t;

            k = 0;
            hull = new Vector2[2 * n];

            // sort vertices lexicographically
            Array.Sort(vertices);

            // lower hull
            for (i = 0; i < n; i++)
            {
                while (k >= 2 && Cross(hull[k - 2], hull[k - 1], vertices[i]) <= 0)
                    k--;

                hull[k++] = vertices[i];
            }

            // upper hull
            for (i = n - 2, t = k + 1; i >= 0; i--)
            {
                while (k >= t && Cross(hull[k - 2], hull[k - 1], vertices[i]) <= 0)
                    k--;

                hull[k++] = vertices[i];
            }
        }
    }

    public interface IBounds
    {
        Vector2[] GetVertices();
        void Update();
        bool IsVisible();
        void Show();
        void Hide();
        bool Contains(double x, double y);
        void MoveContaining(double dx, double dy);
        void MoveAll(double dx, double dy);
    }

    public static class Extenstions
    {
        private static readonly char[] s_separators = new char[] { ',' };

        public static ArgbColor FromHtml(this string str)
        {
            return new ArgbColor(byte.Parse(str.Substring(1, 2), NumberStyles.HexNumber),
                byte.Parse(str.Substring(3, 2), NumberStyles.HexNumber),
                byte.Parse(str.Substring(5, 2), NumberStyles.HexNumber),
                byte.Parse(str.Substring(7, 2), NumberStyles.HexNumber));
        }

        public static string ToHtml(this ArgbColor color)
        {
            return string.Concat('#',
                color.A.ToString("X2"),
                color.R.ToString("X2"),
                color.G.ToString("X2"),
                color.B.ToString("X2"));
        }

        private static readonly NumberFormatInfo NumberFormat = new CultureInfo("en-GB").NumberFormat;

        public static string ToText(this PointShape point)
        {

            return string.Concat(
                point.X.ToString(NumberFormat),
                s_separators[0],
                point.Y.ToString(NumberFormat));
        }

        public static PointShape FromText(this string str)
        {
            string[] values = str.Split(s_separators);
            return new PointShape(
                double.Parse(values[0], NumberFormat),
                double.Parse(values[1], NumberFormat));
        }
    }

    public class ArgbColor
    {
        public byte A { get; set; }
        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }

        public ArgbColor(byte a, byte r, byte g, byte b)
        {
            A = a;
            R = r;
            G = g;
            B = b;
        }
    }

    public interface IDrawableShape
    {
        void InvalidateShape();
    }

    public abstract class BaseShape
    {
        public IBounds Bounds { get; set; }
    }

    public class PolygonShape : BaseShape
    {
        public PointShape[] Points { get; set; }

        public LineShape[] Lines { get; set; }

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

    public class PointShape : BaseShape
    {
        public double X { get; set; }

        public double Y { get; set; }

        public PointShape(double x, double y)
        {
            X = x;
            Y = y;
        }

        public static bool operator <(PointShape p1, PointShape p2)
        {
            return p1.X < p2.X || (p1.X == p2.X && p1.Y < p2.Y);
        }

        public static bool operator >(PointShape p1, PointShape p2)
        {
            return p1.X > p2.X || (p1.X == p2.X && p1.Y > p2.Y);
        }

        public int CompareTo(PointShape other)
        {
            return (this > other) ? -1 : ((this < other) ? 1 : 0);
        }
    }

    public enum LineCap
    {
        Flat = 0,
        Square = 1,
        Round = 2,
        Triangle = 3
    }

    public class LineShape : BaseShape
    {
        public PointShape Point1 { get; set; }

        public PointShape Point2 { get; set; }

        public ArgbColor Stroke { get; set; }

        public double StrokeThickness { get; set; }

        public LineCap StartLineCap { get; set; }

        public LineCap EndLineCap { get; set; }

        public LineShape()
        {
            Point1 = new PointShape(0.0, 0.0);
            Point2 = new PointShape(0.0, 0.0);
            Stroke = new ArgbColor(0xFF, 0x00, 0x00, 0x00);
            StrokeThickness = 30.0;
            StartLineCap = LineCap.Square;
            EndLineCap = LineCap.Square;
        }
    }

    public class CanvasShape : BaseShape, IDrawableShape
    {
        public IDrawableShape Native { get; set; }

        public IObservable<Vector2> Downs { get; set; }

        public IObservable<Vector2> Ups { get; set; }

        public IObservable<Vector2> Moves { get; set; }

        public double Width { get; set; }

        public double Height { get; set; }

        public ArgbColor Background { get; set; }

        public bool EnableSnap { get; set; }

        public double SnapX { get; set; }

        public double SnapY { get; set; }

        public Func<bool> IsCaptured { get; set; }

        public Action Capture { get; set; }

        public Action ReleaseCapture { get; set; }

        public IList<BaseShape> Children { get; set; }

        public CanvasShape()
        {
            Width = 600.0;
            Height = 600.0;
            Background = new ArgbColor(0x00, 0xFF, 0xFF, 0xFF);
            SnapX = 15.0;
            SnapY = 15.0;
            EnableSnap = true;
            Children = new ObservableCollection<BaseShape>();
        }

        public double Snap(double val, double snap)
        {
            double r = val % snap;
            return r >= snap / 2.0 ? val + snap - r : val - r;
        }

        public void InvalidateShape()
        {
            Native?.InvalidateShape();
        }
    }

    public class LineBounds : IBounds
    {
        private enum HitResult { None, Point1, Point2, Line };
        private HitResult _hitResult;
        private LineShape _line;
        private double _offset;
        private CanvasShape _canvasShape;
        private PolygonShape _polygonLine;
        private PolygonShape _polygonPoint1;
        private PolygonShape _polygonPoint2;
        private bool _isVisible;
        private Vector2[] _vertices;

        public LineBounds(CanvasShape canvasShape, LineShape line, double offset)
        {
            _line = line;
            _offset = offset;
            _canvasShape = canvasShape;
            _hitResult = HitResult.None;
            _polygonPoint1 = HitTestHelper.CreatePolygonBounds(4);
            _polygonPoint2 = HitTestHelper.CreatePolygonBounds(4);
            _polygonLine = HitTestHelper.CreatePolygonBounds(4);
            _vertices = new Vector2[4];
        }

        private void UpdatePoint1Bounds()
        {
            var ps = _polygonPoint1.Points;
            var ls = _polygonPoint1.Lines;
            HitTestHelper.UpdatePointBounds(_line.Point1, ps, ls, _line.StrokeThickness, _offset);
        }

        private void UpdatePoint2Bounds()
        {
            var ps = _polygonPoint2.Points;
            var ls = _polygonPoint2.Lines;
            HitTestHelper.UpdatePointBounds(_line.Point2, ps, ls, _line.StrokeThickness, _offset);
        }

        private void UpdateLineBounds()
        {
            var ps = _polygonLine.Points;
            var ls = _polygonLine.Lines;
            var ps1 = _polygonPoint1.Points;
            var ps2 = _polygonPoint2.Points;

            double min1X = HitTestHelper.Min(ps1[0].X, ps1[1].X, ps1[2].X, ps1[3].X);
            double min1Y = HitTestHelper.Min(ps1[0].Y, ps1[1].Y, ps1[2].Y, ps1[3].Y);
            double max1X = HitTestHelper.Max(ps1[0].X, ps1[1].X, ps1[2].X, ps1[3].X);
            double max1Y = HitTestHelper.Max(ps1[0].Y, ps1[1].Y, ps1[2].Y, ps1[3].Y);
            double min2X = HitTestHelper.Min(ps2[0].X, ps2[1].X, ps2[2].X, ps2[3].X);
            double min2Y = HitTestHelper.Min(ps2[0].Y, ps2[1].Y, ps2[2].Y, ps2[3].Y);
            double max2X = HitTestHelper.Max(ps2[0].X, ps2[1].X, ps2[2].X, ps2[3].X);
            double max2Y = HitTestHelper.Max(ps2[0].Y, ps2[1].Y, ps2[2].Y, ps2[3].Y);

            if (Math.Round(_line.Point1.X, 1) == Math.Round(_line.Point2.X, 1))
            {
                ps[0].X = Math.Min(min1X, min2X);
                ps[0].Y = Math.Max(min1Y, min2Y);
                ps[1].X = Math.Min(min1X, min2X);
                ps[1].Y = Math.Min(max1Y, max2Y);
                ps[2].X = Math.Max(max1X, max2X);
                ps[2].Y = Math.Min(max1Y, max2Y);
                ps[3].X = Math.Max(max1X, max2X);
                ps[3].Y = Math.Max(min1Y, min2Y);
            }
            else if (Math.Round(_line.Point1.Y, 1) == Math.Round(_line.Point2.Y, 1))
            {
                ps[0].X = Math.Max(min1X, min2X);
                ps[0].Y = Math.Min(min1Y, min2Y);
                ps[1].X = Math.Min(max1X, max2X);
                ps[1].Y = Math.Min(min1Y, min2Y);
                ps[2].X = Math.Min(max1X, max2X);
                ps[2].Y = Math.Max(max1Y, max2Y);
                ps[3].X = Math.Max(min1X, min2X);
                ps[3].Y = Math.Max(max1Y, max2Y);
            }
            else
            {
                if (((_line.Point2.X > _line.Point1.X) && (_line.Point2.Y < _line.Point1.Y)) ||
                    ((_line.Point2.X < _line.Point1.X) && (_line.Point2.Y > _line.Point1.Y)))
                {
                    ps[0].X = min1X;
                    ps[0].Y = min1Y;
                    ps[1].X = max1X;
                    ps[1].Y = max1Y;
                    ps[2].X = max2X;
                    ps[2].Y = max2Y;
                    ps[3].X = min2X;
                    ps[3].Y = min2Y;
                }
                else
                {
                    ps[0].X = min1X;
                    ps[0].Y = max1Y;
                    ps[1].X = max1X;
                    ps[1].Y = min1Y;
                    ps[2].X = max2X;
                    ps[2].Y = min2Y;
                    ps[3].X = min2X;
                    ps[3].Y = max2Y;
                }
            }

#if true
            HitTestHelper.MoveLine(ls[0], ps[0], ps[1]);
            HitTestHelper.MoveLine(ls[1], ps[1], ps[2]);
            HitTestHelper.MoveLine(ls[2], ps[2], ps[3]);
            HitTestHelper.MoveLine(ls[3], ps[3], ps[0]);
#else
            double tlx = Math.Min(min1X, min2X);
            double tly = Math.Min(min1Y, min2Y);
            double brx = Math.Max(max1X, max2X);
            double bry = Math.Max(max1Y, max2Y);
            ps[0].X = tlx;
            ps[0].Y = tly;
            ps[1].X = brx;
            ps[1].Y = tly;
            ps[2].X = brx;
            ps[2].Y = bry;
            ps[3].X = tlx;
            ps[3].Y = bry;
            HitTestHelper.MoveLine(ls[0], ps[0], ps[1]);
            HitTestHelper.MoveLine(ls[1], ps[1], ps[2]);
            HitTestHelper.MoveLine(ls[2], ps[2], ps[3]);
            HitTestHelper.MoveLine(ls[3], ps[3], ps[0]);
#endif

            UpdateVertices(ps);
        }

        private void UpdateVertices(PointShape[] ps)
        {
            _vertices[0] = new Vector2(ps[0].X, ps[0].Y);
            _vertices[1] = new Vector2(ps[1].X, ps[1].Y);
            _vertices[2] = new Vector2(ps[2].X, ps[2].Y);
            _vertices[3] = new Vector2(ps[3].X, ps[3].Y);
        }

        public Vector2[] GetVertices()
        {
            return _vertices;
        }

        public void Update()
        {
            UpdatePoint1Bounds();
            UpdatePoint2Bounds();
            UpdateLineBounds();
        }

        public bool IsVisible()
        {
            return _isVisible;
        }

        public void Show()
        {
            if (!_isVisible)
            {
                foreach (var line in _polygonLine.Lines)
                {
                    _canvasShape.Children.Add(line);
                }
#if true
                foreach (var line in _polygonPoint1.Lines)
                {
                    _canvasShape.Children.Add(line);
                }
                foreach (var line in _polygonPoint2.Lines)
                {
                    _canvasShape.Children.Add(line);
                }
#endif
                _isVisible = true;
            }
        }

        public void Hide()
        {
            if (_isVisible)
            {
                foreach (var line in _polygonLine.Lines)
                {
                    _canvasShape.Children.Remove(line);
                }
#if true
                foreach (var line in _polygonPoint1.Lines)
                {
                    _canvasShape.Children.Remove(line);
                }
                foreach (var line in _polygonPoint2.Lines)
                {
                    _canvasShape.Children.Remove(line);
                }
#endif
                _isVisible = false;
            }
        }

        public bool Contains(double x, double y)
        {
            if (_polygonPoint1.Contains(x, y))
            {
                _hitResult = HitResult.Point1;
                return true;
            }
            else if (_polygonPoint2.Contains(x, y))
            {
                _hitResult = HitResult.Point2;
                return true;
            }
            else if (_polygonLine.Contains(x, y))
            {
                _hitResult = HitResult.Line;
                return true;
            }
            _hitResult = HitResult.None;
            return false;
        }

        public void MoveContaining(double dx, double dy)
        {
            switch (_hitResult)
            {
                case HitResult.Point1:
                    MovePoint1(dx, dy);
                    break;
                case HitResult.Point2:
                    MovePoint2(dx, dy);
                    break;
                case HitResult.Line:
                    MoveLine(dx, dy);
                    break;
            }
        }

        public void MoveAll(double dx, double dy)
        {
            MoveLine(dx, dy);
        }

        private void MovePoint1(double dx, double dy)
        {
            double x1 = _line.Point1.X - dx;
            double y1 = _line.Point1.Y - dy;
            _line.Point1.X = _canvasShape.EnableSnap ? _canvasShape.Snap(x1, _canvasShape.SnapX) : x1;
            _line.Point1.Y = _canvasShape.EnableSnap ? _canvasShape.Snap(y1, _canvasShape.SnapY) : y1;
            _line.Point1 = _line.Point1;
        }

        private void MovePoint2(double dx, double dy)
        {
            double x2 = _line.Point2.X - dx;
            double y2 = _line.Point2.Y - dy;
            _line.Point2.X = _canvasShape.EnableSnap ? _canvasShape.Snap(x2, _canvasShape.SnapX) : x2;
            _line.Point2.Y = _canvasShape.EnableSnap ? _canvasShape.Snap(y2, _canvasShape.SnapY) : y2;
            _line.Point2 = _line.Point2;
        }

        private void MoveLine(double dx, double dy)
        {
            double x1 = _line.Point1.X - dx;
            double y1 = _line.Point1.Y - dy;
            double x2 = _line.Point2.X - dx;
            double y2 = _line.Point2.Y - dy;
            _line.Point1.X = _canvasShape.EnableSnap ? _canvasShape.Snap(x1, _canvasShape.SnapX) : x1;
            _line.Point1.Y = _canvasShape.EnableSnap ? _canvasShape.Snap(y1, _canvasShape.SnapY) : y1;
            _line.Point2.X = _canvasShape.EnableSnap ? _canvasShape.Snap(x2, _canvasShape.SnapX) : x2;
            _line.Point2.Y = _canvasShape.EnableSnap ? _canvasShape.Snap(y2, _canvasShape.SnapY) : y2;
            _line.Point1 = _line.Point1;
            _line.Point2 = _line.Point2;
        }
    }

    public static class HitTestHelper
    {
        public static MonotoneChain ConvexHull = new MonotoneChain();

        public const int PointBoundVertexCount = 4;

        public static BaseShape HitTest(IList<BaseShape> children, double x, double y)
        {
            return children.Where(c => c.Bounds != null && c.Bounds.Contains(x, y)).FirstOrDefault();
        }

        public static double Min(double val1, double val2, double val3, double val4)
        {
            return Math.Min(Math.Min(val1, val2), Math.Min(val3, val4));
        }

        public static double Max(double val1, double val2, double val3, double val4)
        {
            return Math.Max(Math.Max(val1, val2), Math.Max(val3, val4));
        }

        public static PolygonShape CreatePolygonBounds(int points)
        {
            var polygon = new PolygonShape();
            polygon.Points = new PointShape[points];
            polygon.Lines = new LineShape[points];

            for (int i = 0; i < points; i++)
            {
                polygon.Points[i] = new PointShape(0, 0);

                var lineShape = new LineShape
                {
                    Stroke = new ArgbColor(0xFF, 0x00, 0xBF, 0xFF),
                    StrokeThickness = 2.0
                };

                polygon.Lines[i] = lineShape;
            }

            return polygon;
        }

        public static void UpdatePointBounds(PointShape point, PointShape[] ps, LineShape[] ls, double size, double offset)
        {
            Debug.Assert(point != null);

            double x = point.X - (size / 2.0);
            double y = point.Y - (size / 2.0);
            double width = size;
            double height = size;

            UpdateRectangleBounds(ps, ls, offset, x, y, width, height);
        }

        public static void UpdateRectangleBounds(PointShape[] ps, LineShape[] ls, double offset, double x, double y, double width, double height)
        {
            Debug.Assert(ps != null);
            Debug.Assert(ls != null);
            Debug.Assert(ps.Length == PointBoundVertexCount);
            Debug.Assert(ls.Length == PointBoundVertexCount);

            // Top-Left
            ps[0].X = x - offset;
            ps[0].Y = y - offset;
            // Top-Right
            ps[1].X = (x + width) + offset;
            ps[1].Y = y - offset;
            // Botton-Right
            ps[2].X = (x + width) + offset;
            ps[2].Y = (y + height) + offset;
            // Bottom-Left
            ps[3].X = x - offset;
            ps[3].Y = (y + height) + offset;

            MoveLine(ls[0], ps[0], ps[1]);
            MoveLine(ls[1], ps[1], ps[2]);
            MoveLine(ls[2], ps[2], ps[3]);
            MoveLine(ls[3], ps[3], ps[0]);
        }

        public static void MoveLine(LineShape line, PointShape point1, PointShape point2)
        {
            line.Point1 = point1;
            line.Point2 = point2;
        }

        public static void MoveLine(LineShape line, Vector2 point1, Vector2 point2)
        {
            line.Point1.X = point1.X;
            line.Point1.Y = point1.Y;
            line.Point2.X = point2.X;
            line.Point2.Y = point2.Y;
            line.Point1 = line.Point1;
            line.Point2 = line.Point2;
        }
    }

    public class SelectionEditor : IDisposable
    {
        [Flags]
        private enum State
        {
            None = 0,
            Hover = 1,
            Selected = 2,
            Move = 4,
            HoverSelected = Hover | Selected,
            HoverMove = Hover | Move,
            SelectedMove = Selected | Move
        }
        private State _state = State.None;
        private CanvasShape _drawingCanvas;
        private CanvasShape _boundsCanvas;
        private Vector2 _original;
        private Vector2 _start;
        private BaseShape _selected;
        private BaseShape _hover;
        private IDisposable _downs;
        private IDisposable _ups;
        private IDisposable _drag;
        private bool _isEnabled;

        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                if (_isEnabled)
                {
                    Reset();
                }
                _isEnabled = value;
            }
        }

        public SelectionEditor(CanvasShape drawingCanvas, CanvasShape boundsCanvas)
        {
            _drawingCanvas = drawingCanvas;
            _boundsCanvas = boundsCanvas;

            var drags = Observable.Merge(_drawingCanvas.Downs, _drawingCanvas.Ups, _drawingCanvas.Moves);

            _downs = _drawingCanvas.Downs.Where(_ => IsEnabled).Subscribe(p => Down(p));
            _ups = _drawingCanvas.Ups.Where(_ => IsEnabled).Subscribe(p => Up(p));
            _drag = drags.Where(_ => IsEnabled).Subscribe(p => Drag(p));
        }

        private bool IsState(State state)
        {
            return (_state & state) == state;
        }

        private void Down(Vector2 p)
        {
            bool render = false;

            if (IsState(State.Selected))
            {
                HideSelected();
                render = true;
            }

            if (IsState(State.Hover))
            {
                HideHover();
                render = true;
            }

            _selected = HitTestHelper.HitTest(_drawingCanvas.Children, p.X, p.Y);
            if (_selected != null)
            {
                ShowSelected();
                InitMove(p);
                _drawingCanvas.Capture?.Invoke();
                render = true;
            }

            if (render)
            {
                _drawingCanvas.InvalidateShape();
                _boundsCanvas.InvalidateShape();
            }
        }

        private void Up(Vector2 p)
        {
            if (_drawingCanvas.IsCaptured?.Invoke() == true)
            {
                if (IsState(State.Move))
                {
                    FinishMove(p);
                    _drawingCanvas.ReleaseCapture?.Invoke();
                }
            }
        }

        private void Drag(Vector2 p)
        {
            if (_drawingCanvas.IsCaptured?.Invoke() == true)
            {
                if (IsState(State.Move))
                {
                    Move(p);
                }
            }
            else
            {
                bool render = false;
                var result = HitTestHelper.HitTest(_drawingCanvas.Children, p.X, p.Y);

                if (IsState(State.Hover))
                {
                    if (IsState(State.Selected))
                    {
                        if (_hover != _selected && _hover != result)
                        {
                            HideHover();
                            render = true;
                        }
                        else
                        {
                            return;
                        }
                    }
                    else
                    {
                        if (result != _hover)
                        {
                            HideHover();
                            render = true;
                        }
                        else
                        {
                            return;
                        }
                    }
                }

                if (result != null)
                {
                    if (IsState(State.Selected))
                    {
                        if (result != _selected)
                        {
                            _hover = result;
                            ShowHover();
                            render = true;
                        }
                    }
                    else
                    {
                        _hover = result;
                        ShowHover();
                        render = true;
                    }
                }

                if (render)
                {
                    _drawingCanvas.InvalidateShape();
                    _boundsCanvas.InvalidateShape();
                }
            }
        }

        private void ShowHover()
        {
            _hover.Bounds.Show();
            _state |= State.Hover;
        }

        private void HideHover()
        {
            _hover.Bounds.Hide();
            _hover = null;
            _state &= ~State.Hover;
        }

        private void ShowSelected()
        {
            _selected.Bounds.Show();
            _state |= State.Selected;
        }

        private void HideSelected()
        {
            _selected.Bounds.Hide();
            _selected = null;
            _state &= ~State.Selected;
        }

        private void InitMove(Vector2 p)
        {
            _original = p;
            _start = p;
            _state |= State.Move;
        }

        private void FinishMove(Vector2 p)
        {
            _state &= ~State.Move;
        }

        private void Move(Vector2 p)
        {
            if (_selected != null)
            {
                double dx = _start.X - p.X;
                double dy = _start.Y - p.Y;
                _start = p;
                _selected.Bounds.MoveContaining(dx, dy);
                _selected.Bounds.Update();
                _drawingCanvas.InvalidateShape();
                _boundsCanvas.InvalidateShape();
            }
        }

        private void Reset()
        {
            bool render = false;

            if (_hover != null)
            {
                _hover.Bounds.Hide();
                _hover = null;
                render = true;
            }

            if (_selected != null)
            {
                _selected.Bounds.Hide();
                _selected = null;
                render = true;
            }

            _state = State.None;

            if (render)
            {
                _drawingCanvas.InvalidateShape();
                _boundsCanvas.InvalidateShape();
            }
        }

        public void Dispose()
        {
            _downs.Dispose();
            _ups.Dispose();
            _drag.Dispose();
        }
    }

    public class LineEditor : IDisposable
    {
        private enum State { None, Start, End }
        private State _state = State.None;
        private CanvasShape _drawingCanvas;
        private CanvasShape _boundsCanvas;
        private LineShape _lineShape;
        private IDisposable _downs;
        private IDisposable _drags;

        public bool IsEnabled { get; set; }

        public LineEditor(CanvasShape drawingCanvas, CanvasShape boundsCanvas)
        {
            _drawingCanvas = drawingCanvas;
            _boundsCanvas = boundsCanvas;

            var moves = _drawingCanvas.Moves.Where(_ => _drawingCanvas.IsCaptured?.Invoke() == true);
            var drags = Observable.Merge(_drawingCanvas.Downs, _drawingCanvas.Ups, moves);

            _downs = _drawingCanvas.Downs.Where(_ => IsEnabled).Subscribe(p =>
            {
                if (_drawingCanvas.IsCaptured?.Invoke() == true)
                {
                    _lineShape.Bounds.Hide();
                    _drawingCanvas.InvalidateShape();
                    _boundsCanvas.InvalidateShape();
                    _state = State.None;
                    _drawingCanvas.ReleaseCapture?.Invoke();
                }
                else
                {
                    _lineShape = new LineShape();
                    _lineShape.Point1.X = p.X;
                    _lineShape.Point1.Y = p.Y;
                    _lineShape.Point2.X = p.X;
                    _lineShape.Point2.Y = p.Y;

                    _drawingCanvas.Children.Add(_lineShape);
                    _lineShape.Bounds = new LineBounds(_boundsCanvas, _lineShape, 0.0);
                    _lineShape.Bounds.Update();
                    _lineShape.Bounds.Show();
                    _drawingCanvas.Capture?.Invoke();
                    _drawingCanvas.InvalidateShape();
                    _boundsCanvas.InvalidateShape();
                    _state = State.End;
                }
            });

            _drags = drags.Where(_ => IsEnabled).Subscribe(p =>
            {
                if (_state == State.End)
                {
                    _lineShape.Point2.X = p.X;
                    _lineShape.Point2.Y = p.Y;
                    _lineShape.Bounds.Update();
                    _drawingCanvas.InvalidateShape();
                    _boundsCanvas.InvalidateShape();
                }
            });
        }

        public void Dispose()
        {
            _downs.Dispose();
            _drags.Dispose();
        }
    }

    public class CanvasView
    {
        public CanvasShape BackgroundCanvas { get; set; }

        public CanvasShape DrawingCanvas { get; set; }

        public CanvasShape BoundsCanvas { get; set; }

        public SelectionEditor SelectionEditor { get; set; }

        public LineEditor LineEditor { get; set; }

        public void ToggleSnap()
        {
            DrawingCanvas.EnableSnap = !DrawingCanvas.EnableSnap;
        }

        public void Clear()
        {
            DrawingCanvas.Children.Clear();
            DrawingCanvas.InvalidateShape();
            BoundsCanvas.InvalidateShape();
        }

        public void Render()
        {
            BackgroundCanvas.InvalidateShape();
            DrawingCanvas.InvalidateShape();
            BoundsCanvas.InvalidateShape();
        }

        public void Delete()
        {
            var selectedShapes = DrawingCanvas.Children.Where(c => c.Bounds != null && c.Bounds.IsVisible()).ToList();

            foreach (var child in selectedShapes)
            {
                child.Bounds.Hide();
            }

            foreach (var child in selectedShapes)
            {
                DrawingCanvas.Children.Remove(child);
            }

            DrawingCanvas.InvalidateShape();
            BoundsCanvas.InvalidateShape();
        }

        public void ObserveInput(CanvasShape canvasShape, RenderCanvas target)
        {
            canvasShape.Downs = Observable.FromEventPattern<MouseButtonEventArgs>(
                 target,
                 "PreviewMouseLeftButtonDown").Select(e =>
                 {
                     var p = e.EventArgs.GetPosition(target);
                     return new Vector2(
                         canvasShape.EnableSnap ? canvasShape.Snap(p.X, canvasShape.SnapX) : p.X,
                         canvasShape.EnableSnap ? canvasShape.Snap(p.Y, canvasShape.SnapY) : p.Y);
                 });

            canvasShape.Ups = Observable.FromEventPattern<MouseButtonEventArgs>(
                target,
                "PreviewMouseLeftButtonUp").Select(e =>
                {
                    var p = e.EventArgs.GetPosition(target);
                    return new Vector2(
                        canvasShape.EnableSnap ? canvasShape.Snap(p.X, canvasShape.SnapX) : p.X,
                        canvasShape.EnableSnap ? canvasShape.Snap(p.Y, canvasShape.SnapY) : p.Y);
                });

            canvasShape.Moves = Observable.FromEventPattern<MouseEventArgs>(
                target,
                "PreviewMouseMove").Select(e =>
                {
                    var p = e.EventArgs.GetPosition(target);
                    return new Vector2(
                        canvasShape.EnableSnap ? canvasShape.Snap(p.X, canvasShape.SnapX) : p.X,
                        canvasShape.EnableSnap ? canvasShape.Snap(p.Y, canvasShape.SnapY) : p.Y);
                });

            canvasShape.IsCaptured = () => Mouse.Captured == target;
            canvasShape.Capture = () => target.CaptureMouse();
            canvasShape.ReleaseCapture = () => target.ReleaseMouseCapture();
        }
    }

    public static class ArgbColorExtensions
    {
        public static Color ToColor(this ArgbColor color)
        {
            return Color.FromArgb(color.A, color.R, color.G, color.B);
        }
    }

    public class PanAndZoomBorder : Border
    {
        private bool initialize = true;
        private UIElement _child = null;
        private Point _origin;
        private Point _start;

        private TranslateTransform GetTranslateTransform(UIElement element)
        {
            return (TranslateTransform)((TransformGroup)element.RenderTransform).Children.First(tr => tr is TranslateTransform);
        }

        private ScaleTransform GetScaleTransform(UIElement element)
        {
            return (ScaleTransform)((TransformGroup)element.RenderTransform).Children.First(tr => tr is ScaleTransform);
        }

        public override UIElement Child
        {
            get { return base.Child; }
            set
            {
                if (value != null && value != this.Child)
                {
                    _child = value;
                    if (initialize)
                    {
                        var group = new TransformGroup();
                        var st = new ScaleTransform();
                        group.Children.Add(st);
                        var tt = new TranslateTransform();
                        group.Children.Add(tt);
                        _child.RenderTransform = group;
                        _child.RenderTransformOrigin = new Point(0.0, 0.0);
                        this.MouseWheel += Border_MouseWheel;
                        this.MouseRightButtonDown += Border_MouseRightButtonDown;
                        this.MouseRightButtonUp += Border_MouseRightButtonUp;
                        this.MouseMove += Border_MouseMove;
                        this.PreviewMouseDown += Border_PreviewMouseDown;
                        initialize = false;
                    }
                }
                base.Child = value;
            }
        }

        public void Reset()
        {
            if (initialize == false && _child != null)
            {
                var st = GetScaleTransform(_child);
                st.ScaleX = 1.0;
                st.ScaleY = 1.0;
                var tt = GetTranslateTransform(_child);
                tt.X = 0.0;
                tt.Y = 0.0;
            }
        }

        private void Border_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (initialize == false && _child != null)
            {
                var st = GetScaleTransform(_child);
                var tt = GetTranslateTransform(_child);
                double zoom = e.Delta > 0 ? .2 : -.2;
                if (!(e.Delta > 0) && (st.ScaleX < .4 || st.ScaleY < .4))
                    return;
                Point relative = e.GetPosition(_child);
                double abosuluteX = relative.X * st.ScaleX + tt.X;
                double abosuluteY = relative.Y * st.ScaleY + tt.Y;
                st.ScaleX += zoom;
                st.ScaleY += zoom;
                tt.X = abosuluteX - relative.X * st.ScaleX;
                tt.Y = abosuluteY - relative.Y * st.ScaleY;
            }
        }

        private void Border_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (initialize == false && _child != null)
            {
                var tt = GetTranslateTransform(_child);
                _start = e.GetPosition(this);
                _origin = new Point(tt.X, tt.Y);
                this.Cursor = Cursors.Hand;
                _child.CaptureMouse();
            }
        }

        private void Border_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (initialize == false && _child != null)
            {
                _child.ReleaseMouseCapture();
                this.Cursor = Cursors.Arrow;
            }
        }

        private void Border_MouseMove(object sender, MouseEventArgs e)
        {
            if (initialize == false && _child != null && _child.IsMouseCaptured)
            {
                var tt = GetTranslateTransform(_child);
                Vector v = _start - e.GetPosition(this);
                tt.X = _origin.X - v.X;
                tt.Y = _origin.Y - v.Y;
            }
        }

        private void Border_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Middle && e.ClickCount == 2 && initialize == false && _child != null)
            {
                this.Reset();
            }
        }
    }

    public class RenderCanvas : Canvas, IDrawableShape
    {
        private readonly CanvasShape _canvasShape;

        public RenderCanvas(CanvasShape canvasShape)
        {
            _canvasShape = canvasShape;
            _canvasShape.Native = this;
            this.Width = _canvasShape.Width;
            this.Height = _canvasShape.Height;
        }

        public PenLineCap ToPenLineCap(LineCap lineCap)
        {
            switch (lineCap)
            {
                default:
                case LineCap.Flat:
                    return PenLineCap.Flat;
                case LineCap.Square:
                    return PenLineCap.Square;
                case LineCap.Round:
                    return PenLineCap.Round;
                case LineCap.Triangle:
                    return PenLineCap.Triangle;
            }
        }

        public void InvalidateShape()
        {
            InvalidateVisual();
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            var backgroundBrush = new SolidColorBrush(_canvasShape.Background.ToColor());
            backgroundBrush.Freeze();

            dc.DrawRectangle(backgroundBrush, null, new Rect(0, 0, _canvasShape.Width, _canvasShape.Height));

            foreach (var shape in _canvasShape.Children)
            {
                if (shape is LineShape lineShape)
                {
                    var brush = new SolidColorBrush(lineShape.Stroke.ToColor());
                    brush.Freeze();

                    var pen = new Pen(brush, lineShape.StrokeThickness)
                    {
                        Brush = brush,
                        Thickness = lineShape.StrokeThickness,
                        StartLineCap = ToPenLineCap(lineShape.StartLineCap),
                        EndLineCap = ToPenLineCap(lineShape.EndLineCap)
                    };
                    pen.Freeze();

                    var point0 = new Point(lineShape.Point1.X, lineShape.Point1.Y);
                    var point1 = new Point(lineShape.Point2.X, lineShape.Point2.Y);

                    dc.DrawLine(pen, point0, point1);
                }
            }
        }
    }

    public class ArgbColorValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((ArgbColor)value).ToHtml();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((string)value).FromHtml();
        }
    }

    public class PointShapeValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((PointShape)value).ToText();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((string)value).FromText();
        }
    }

    public partial class MainWindow : Window
    {
        private CanvasView _canvasView;

        public MainWindow()
        {
            InitializeComponent();

            InitializeModel();

            PreviewKeyDown += MainWindow_PreviewKeyDown;

            DataContext = _canvasView;
        }

        private void InitializeModel()
        {
            _canvasView = new CanvasView
            {
                BackgroundCanvas = new CanvasShape(),
                DrawingCanvas = new CanvasShape(),
                BoundsCanvas = new CanvasShape()
            };

            var backgroundRenderCanvas = new RenderCanvas(_canvasView.BackgroundCanvas);
            var drawingRenderCanvas = new RenderCanvas(_canvasView.DrawingCanvas);
            var boundsRenderCanvas = new RenderCanvas(_canvasView.BoundsCanvas);

            layout.Children.Add(backgroundRenderCanvas);
            layout.Children.Add(drawingRenderCanvas);
            layout.Children.Add(boundsRenderCanvas);

            CreateGrid(
                _canvasView.BackgroundCanvas,
                _canvasView.BackgroundCanvas.Width,
                _canvasView.BackgroundCanvas.Height,
                30,
                0, 0);

            _canvasView.ObserveInput(_canvasView.DrawingCanvas, boundsRenderCanvas);

            _canvasView.SelectionEditor = new SelectionEditor(_canvasView.DrawingCanvas, _canvasView.BoundsCanvas)
            {
                IsEnabled = false
            };

            _canvasView.LineEditor = new LineEditor(_canvasView.DrawingCanvas, _canvasView.BoundsCanvas)
            {
                IsEnabled = true
            };
        }

        private void CreateGrid(CanvasShape canvasShape, double width, double height, double size, double originX, double originY)
        {
            var thickness = 2.0;
            var stroke = new ArgbColor(0xFF, 0xE8, 0xE8, 0xE8);

            for (double y = size; y < height; y += size)
            {
                var lineShape = new LineShape();
                lineShape.Point1.X = originX;
                lineShape.Point1.Y = y;
                lineShape.Point2.X = width;
                lineShape.Point2.Y = y;
                lineShape.Stroke = stroke;
                lineShape.StrokeThickness = thickness;
                canvasShape.Children.Add(lineShape);
            }

            for (double x = size; x < width; x += size)
            {
                var lineShape = new LineShape();
                lineShape.Point1.X = x;
                lineShape.Point1.Y = originY;
                lineShape.Point2.X = x;
                lineShape.Point2.Y = height;
                lineShape.Stroke = stroke;
                lineShape.StrokeThickness = thickness;
                canvasShape.Children.Add(lineShape);
            }
        }

        private void MainWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.S:
                    _canvasView.LineEditor.IsEnabled = false;
                    _canvasView.SelectionEditor.IsEnabled = true;
                    break;
                case Key.L:
                    _canvasView.LineEditor.IsEnabled = false;
                    _canvasView.SelectionEditor.IsEnabled = true;
                    break;
                case Key.Delete:
                    _canvasView.Delete();
                    break;
            }
        }
    }
}
