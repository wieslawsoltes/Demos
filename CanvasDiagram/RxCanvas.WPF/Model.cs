﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using Autofac;
using MathUtil;

namespace RxCanvas
{
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

    public interface IEditor
    {
        string Name { get; set; }
        bool IsEnabled { get; set; }
        string Key { get; set; }
        string Modifiers { get; set; }
    }

    public interface INativeConverter
    {
        LineShape Convert(LineShape line);
        CanvasShape Convert(CanvasShape canvas);
    }

    public static class ModelExtenstion
    {
        private static readonly char[] Separators = new char[] { ',' };

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
                Separators[0],
                point.Y.ToString(NumberFormat));
        }

        public static PointShape FromText(this string str)
        {
            string[] values = str.Split(Separators);
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

    public abstract class NativeShape
    {
        public object Native { get; set; }
        public IBounds Bounds { get; set; }
    }

    public class PolygonShape : NativeShape
    {
        public PointShape[] Points { get; set; }
        public LineShape[] Lines { get; set; }

        public bool Contains(PointShape point)
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

    public class PointShape : NativeShape
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

    public class LineShape : NativeShape
    {
        public PointShape Point1 { get; set; }
        public PointShape Point2 { get; set; }
        public ArgbColor Stroke { get; set; }
        public double StrokeThickness { get; set; }
    }

    public class CanvasShape : NativeShape
    {
        public IObservable<Vector2> Downs { get; set; }
        public IObservable<Vector2> Ups { get; set; }
        public IObservable<Vector2> Moves { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public ArgbColor Background { get; set; }
        public bool EnableSnap { get; set; }
        public double SnapX { get; set; }
        public double SnapY { get; set; }
        public bool IsCaptured { get; set; }
        public IList<NativeShape> Children { get; set; }

        public double Snap(double val, double snap)
        {
            double r = val % snap;
            return r >= snap / 2.0 ? val + snap - r : val - r;
        }

        public CanvasShape()
        {
            Children = new ObservableCollection<NativeShape>();
        }

        public void Capture()
        {
            IsCaptured = true;
        }

        public void ReleaseCapture()
        {
            IsCaptured = false;
        }

        public void Add(NativeShape value)
        {
            Children.Add(value);
        }

        public void Remove(NativeShape value)
        {
            Children.Remove(value);
        }

        public void Clear()
        {
            Children.Clear();
        }

        public void Render(NativeShape context)
        {
        }
    }

    public class LineBounds : IBounds
    {
        private LineShape _line;
        private double _size;
        private double _offset;
        private CanvasShape _canvas;
        private PolygonShape _polygonLine;
        private PolygonShape _polygonPoint1;
        private PolygonShape _polygonPoint2;
        private bool _isVisible;

        private enum HitResult { None, Point1, Point2, Line };
        private HitResult _hitResult;
        private Vector2[] _vertices;

        public LineBounds(INativeConverter nativeConverter, CanvasFactory canvasFactory, CanvasShape canvas, LineShape line, double size, double offset)
        {
            _line = line;
            _size = size;
            _offset = offset;
            _canvas = canvas;

            _hitResult = HitResult.None;

            InitBounds(nativeConverter, canvasFactory);
        }

        private void InitBounds(
            INativeConverter nativeConverter,
            CanvasFactory canvasFactory)
        {
            _polygonPoint1 = HitTestHelper.CreateBoundsPolygon(nativeConverter, canvasFactory, 4);
            _polygonPoint2 = HitTestHelper.CreateBoundsPolygon(nativeConverter, canvasFactory, 4);
            _polygonLine = HitTestHelper.CreateBoundsPolygon(nativeConverter, canvasFactory, 4);
            _vertices = new Vector2[4];
        }

        private void UpdatePoint1Bounds()
        {
            var ps = _polygonPoint1.Points;
            var ls = _polygonPoint1.Lines;
            HitTestHelper.UpdatePointBounds(_line.Point1, ps, ls, _size, _offset);
        }

        private void UpdatePoint2Bounds()
        {
            var ps = _polygonPoint2.Points;
            var ls = _polygonPoint2.Lines;
            HitTestHelper.UpdatePointBounds(_line.Point2, ps, ls, _size, _offset);
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
                    _canvas.Add(line);
                }
#if true
                foreach (var line in _polygonPoint1.Lines)
                {
                    _canvas.Add(line);
                }
                foreach (var line in _polygonPoint2.Lines)
                {
                    _canvas.Add(line);
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
                    _canvas.Remove(line);
                }
#if true
                foreach (var line in _polygonPoint1.Lines)
                {
                    _canvas.Remove(line);
                }
                foreach (var line in _polygonPoint2.Lines)
                {
                    _canvas.Remove(line);
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
            _line.Point1.X = _canvas.EnableSnap ? _canvas.Snap(x1, _canvas.SnapX) : x1;
            _line.Point1.Y = _canvas.EnableSnap ? _canvas.Snap(y1, _canvas.SnapY) : y1;
            _line.Point1 = _line.Point1;
        }

        private void MovePoint2(double dx, double dy)
        {
            double x2 = _line.Point2.X - dx;
            double y2 = _line.Point2.Y - dy;
            _line.Point2.X = _canvas.EnableSnap ? _canvas.Snap(x2, _canvas.SnapX) : x2;
            _line.Point2.Y = _canvas.EnableSnap ? _canvas.Snap(y2, _canvas.SnapY) : y2;
            _line.Point2 = _line.Point2;
        }

        private void MoveLine(double dx, double dy)
        {
            double x1 = _line.Point1.X - dx;
            double y1 = _line.Point1.Y - dy;
            double x2 = _line.Point2.X - dx;
            double y2 = _line.Point2.Y - dy;
            _line.Point1.X = _canvas.EnableSnap ? _canvas.Snap(x1, _canvas.SnapX) : x1;
            _line.Point1.Y = _canvas.EnableSnap ? _canvas.Snap(y1, _canvas.SnapY) : y1;
            _line.Point2.X = _canvas.EnableSnap ? _canvas.Snap(x2, _canvas.SnapX) : x2;
            _line.Point2.Y = _canvas.EnableSnap ? _canvas.Snap(y2, _canvas.SnapY) : y2;
            _line.Point1 = _line.Point1;
            _line.Point2 = _line.Point2;
        }
    }

    public class BoundsFactory
    {
        private readonly INativeConverter _nativeConverter;
        private readonly CanvasFactory _canvasFactory;

        public BoundsFactory(INativeConverter nativeConverter, CanvasFactory canvasFactory)
        {
            _nativeConverter = nativeConverter;
            _canvasFactory = canvasFactory;
        }

        public IBounds Create(CanvasShape canvas, LineShape line)
        {
            return new LineBounds
                (_nativeConverter,
                _canvasFactory,
                canvas,
                line,
                line.StrokeThickness,
                0.0);
        }
    }

    internal static class HitTestHelper
    {
        public static NativeShape HitTest(IList<NativeShape> children, double x, double y)
        {
            return children.Where(c => c.Bounds != null && c.Bounds.Contains(x, y)).FirstOrDefault();
        }

        public static MonotoneChain ConvexHull = new MonotoneChain();

        public const int PointBoundVertexCount = 4;

        public static double Min(double val1, double val2, double val3, double val4)
        {
            return Math.Min(Math.Min(val1, val2), Math.Min(val3, val4));
        }

        public static double Max(double val1, double val2, double val3, double val4)
        {
            return Math.Max(Math.Max(val1, val2), Math.Max(val3, val4));
        }

        public static PolygonShape CreateBoundsPolygon(INativeConverter nativeConverter, CanvasFactory canvasFactory, int points)
        {
            var polygon = canvasFactory.CreatePolygon();
            polygon.Points = new PointShape[points];
            polygon.Lines = new LineShape[points];

            for (int i = 0; i < points; i++)
            {
                polygon.Points[i] = canvasFactory.CreatePoint();

                var _xline = canvasFactory.CreateLine();
                _xline.Stroke = canvasFactory.CreateColor();
                _xline.Stroke.A = 0xFF;
                _xline.Stroke.R = 0x00;
                _xline.Stroke.G = 0xBF;
                _xline.Stroke.B = 0xFF;
                _xline.StrokeThickness = 2.0;
                var _nline = nativeConverter.Convert(_xline);
                polygon.Lines[i] = _nline;
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

            HitTestHelper.UpdateRectangleBounds(ps, ls, offset, x, y, width, height);
        }

        public static void UpdateRectangleBounds(PointShape[] ps, LineShape[] ls, double offset, double x, double y, double width, double height)
        {
            Debug.Assert(ps != null);
            Debug.Assert(ls != null);
            Debug.Assert(ps.Length == PointBoundVertexCount);
            Debug.Assert(ls.Length == PointBoundVertexCount);

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

            HitTestHelper.MoveLine(ls[0], ps[0], ps[1]);
            HitTestHelper.MoveLine(ls[1], ps[1], ps[2]);
            HitTestHelper.MoveLine(ls[2], ps[2], ps[3]);
            HitTestHelper.MoveLine(ls[3], ps[3], ps[0]);
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

    public class SelectionEditor : IEditor, IDisposable
    {
        [Flags]
        public enum State
        {
            None = 0,
            Hover = 1,
            Selected = 2,
            Move = 4,
            HoverSelected = Hover | Selected,
            HoverMove = Hover | Move,
            SelectedMove = Selected | Move
        }

        public string Name { get; set; }

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

        public string Key { get; set; }
        public string Modifiers { get; set; }

        private CanvasShape _canvas;
        private Vector2 _original;
        private Vector2 _start;
        private NativeShape _selected;
        private NativeShape _hover;
        private State _state = State.None;
        private IDisposable _downs;
        private IDisposable _ups;
        private IDisposable _drag;

        public SelectionEditor(CanvasShape canvas)
        {
            _canvas = canvas;

            Name = "Single Selection";
            Key = "H";
            Modifiers = "";

            var drags = Observable.Merge(_canvas.Downs, _canvas.Ups, _canvas.Moves);

            _downs = _canvas.Downs.Where(_ => IsEnabled).Subscribe(p => Down(p));
            _ups = _canvas.Ups.Where(_ => IsEnabled).Subscribe(p => Up(p));
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

            _selected = HitTestHelper.HitTest(_canvas.Children, p.X, p.Y);
            if (_selected != null)
            {
                ShowSelected();
                InitMove(p);
                _canvas.Capture();
                render = true;
            }

            if (render)
            {
                _canvas.Render(null);
            }
        }

        private void Up(Vector2 p)
        {
            if (_canvas.IsCaptured)
            {
                if (IsState(State.Move))
                {
                    FinishMove(p);
                    _canvas.ReleaseCapture();
                }
            }
        }

        private void Drag(Vector2 p)
        {
            if (_canvas.IsCaptured)
            {
                if (IsState(State.Move))
                {
                    Move(p);
                }
            }
            else
            {
                bool render = false;
                var result = HitTestHelper.HitTest(_canvas.Children, p.X, p.Y);

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
                    _canvas.Render(null);
                }
            }
        }

        private void ShowHover()
        {
            _hover.Bounds.Show();
            _state |= State.Hover;
            Debug.WriteLine("_state: {0}", _state);
        }

        private void HideHover()
        {
            _hover.Bounds.Hide();
            _hover = null;
            _state = _state & ~State.Hover;
            Debug.WriteLine("_state: {0}", _state);
        }

        private void ShowSelected()
        {
            _selected.Bounds.Show();
            _state |= State.Selected;
            Debug.WriteLine("_state: {0}", _state);
        }

        private void HideSelected()
        {
            _selected.Bounds.Hide();
            _selected = null;
            _state = _state & ~State.Selected;
            Debug.WriteLine("_state: {0}", _state);
        }

        private void InitMove(Vector2 p)
        {
            // TODO: Create history snapshot but do not push undo.
            _original = p;
            _start = p;
            _state |= State.Move;
            Debug.WriteLine("_state: {0}", _state);
        }

        private void FinishMove(Vector2 p)
        {
            if (p.X == _original.X && p.Y == _original.Y)
            {
                // TODO: Do not push history undo.
            }
            else
            {
                // TODO: Push history undo.
            }
            _state = _state & ~State.Move;
            Debug.WriteLine("_state: {0}", _state);
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
                _canvas.Render(null);
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
            Debug.WriteLine("_state: {0}", _state);

            if (render)
            {
                _canvas.Render(null);
            }
        }

        public void Dispose()
        {
            _downs.Dispose();
            _ups.Dispose();
            _drag.Dispose();
        }
    }

    public class LineEditor : IEditor, IDisposable
    {
        public enum State { None, Start, End }

        public string Name { get; set; }
        public bool IsEnabled { get; set; }
        public string Key { get; set; }
        public string Modifiers { get; set; }

        private CanvasShape _canvas;
        private LineShape _xline;
        private LineShape _nline;
        private State _state = State.None;
        private IDisposable _downs;
        private IDisposable _drags;

        public LineEditor(
            INativeConverter nativeConverter,
            CanvasFactory canvasFactory,
            BoundsFactory boundsFactory,
            CanvasShape canvas)
        {
            _canvas = canvas;

            Name = "Line";
            Key = "L";
            Modifiers = "";

            var moves = _canvas.Moves.Where(_ => _canvas.IsCaptured);
            var drags = Observable.Merge(_canvas.Downs, _canvas.Ups, moves);

            _downs = _canvas.Downs.Where(_ => IsEnabled).Subscribe(p =>
            {
                if (_canvas.IsCaptured)
                {
                    _nline.Bounds.Hide();
                    _canvas.Render(null);
                    _state = State.None;
                    _canvas.ReleaseCapture();
                }
                else
                {
                    _xline = canvasFactory.CreateLine();
                    _xline.Point1.X = p.X;
                    _xline.Point1.Y = p.Y;
                    _xline.Point2.X = p.X;
                    _xline.Point2.Y = p.Y;
                    _nline = nativeConverter.Convert(_xline);

                    _canvas.Add(_nline);
                    _nline.Bounds = boundsFactory.Create(_canvas, _nline);
                    _nline.Bounds.Update();
                    _nline.Bounds.Show();
                    _canvas.Capture();
                    _canvas.Render(null);
                    _state = State.End;
                }
            });

            _drags = drags.Where(_ => IsEnabled).Subscribe(p =>
            {
                if (_state == State.End)
                {
                    _xline.Point2.X = p.X;
                    _xline.Point2.Y = p.Y;
                    _nline.Point2 = _xline.Point2;
                    _nline.Bounds.Update();
                    _canvas.Render(null);
                }
            });
        }

        public void Dispose()
        {
            _downs.Dispose();
            _drags.Dispose();
        }
    }

    public class CanvasFactory
    {
        public ArgbColor CreateColor()
        {
            return new ArgbColor(0x00, 0x00, 0x00, 0x00);
        }

        public PointShape CreatePoint()
        {
            return new PointShape(0.0, 0.0);
        }

        public PolygonShape CreatePolygon()
        {
            return new PolygonShape();
        }

        public LineShape CreateLine()
        {
            var line = new LineShape()
            {
                Point1 = new PointShape(0.0, 0.0),
                Point2 = new PointShape(0.0, 0.0),
                Stroke = new ArgbColor(0xFF, 0x00, 0x00, 0x00),
                StrokeThickness = 30.0,
            };
            return line;
        }

        public CanvasShape CreateCanvas()
        {
            return new CanvasShape()
            {
                Width = 600.0,
                Height = 600.0,
                Background = new ArgbColor(0x00, 0xFF, 0xFF, 0xFF),
                SnapX = 15.0,
                SnapY = 15.0,
                EnableSnap = true
            };
        }
    }

    public class Bootstrapper
    {
        public IContainer Build(Assembly[] assembly)
        {
            var builder = new ContainerBuilder();

            builder.RegisterAssemblyTypes(assembly).As<IEditor>().InstancePerLifetimeScope();

            builder.Register<CanvasFactory>(c => new CanvasFactory()).SingleInstance();

            builder.Register<BoundsFactory>(c =>
            {
                var nativeConverter = c.Resolve<INativeConverter>();
                var canvasFactory = c.Resolve<CanvasFactory>();
                return new BoundsFactory(nativeConverter, canvasFactory);
            }).InstancePerLifetimeScope();

            builder.Register<CanvasShape>(c =>
            {
                var nativeConverter = c.Resolve<INativeConverter>();
                var canvasFactory = c.Resolve<CanvasFactory>();
                var xcanvas = canvasFactory.CreateCanvas();
                return nativeConverter.Convert(xcanvas);
            }).InstancePerLifetimeScope();

            builder.RegisterAssemblyModules(assembly);

            return builder.Build();
        }
    }

    public class DrawingView
    {
        private readonly IList<ILifetimeScope> _scopes;

        public IList<CanvasShape> Layers { get; set; }

        public IList<IEditor> Editors { get; set; }

        public DrawingView(Assembly[] assembly)
        {
            var bootstrapper = new Bootstrapper();
            var container = bootstrapper.Build(assembly);

            _scopes = new List<ILifetimeScope>
            {
                container.BeginLifetimeScope(),
                container.BeginLifetimeScope()
            };

            Layers = new List<CanvasShape>();

            for (int i = 0; i < _scopes.Count; i++)
            {
                Layers.Add(_scopes[i].Resolve<CanvasShape>());
            }
        }

        public void Initialize()
        {
            var scope = _scopes.LastOrDefault();

            Editors = scope.Resolve<IList<IEditor>>();
            Editors.Where(e => e.Name == "Line").FirstOrDefault().IsEnabled = true;
        }

        public void Enable(IEditor editor)
        {
            for (int i = 0; i < Editors.Count; i++)
            {
                Editors[i].IsEnabled = false;
            }

            if (editor != null)
            {
                editor.IsEnabled = true;
            }
        }

        public void ToggleSnap()
        {
            var drawingCanvas = Layers.LastOrDefault();
            drawingCanvas.EnableSnap = drawingCanvas.EnableSnap ? false : true;
        }


        public void Clear()
        {
            var drawingCanvas = Layers.LastOrDefault();
            drawingCanvas.Clear();
            drawingCanvas.Render(null);
        }

        public void Render()
        {
            for (int i = 0; i < Layers.Count; i++)
            {
                Layers[i].Render(null);
            }
        }

        public void CreateGrid(double width, double height, double size, double originX, double originY)
        {
            var scope = _scopes.FirstOrDefault();
            var backgroundCanvas = scope.Resolve<CanvasShape>();
            var nativeConverter = scope.Resolve<INativeConverter>();
            var canvasFactory = scope.Resolve<CanvasFactory>();

            double thickness = 2.0;

            var stroke = canvasFactory.CreateColor();
            stroke.A = 0xFF;
            stroke.R = 0xE8;
            stroke.G = 0xE8;
            stroke.B = 0xE8;

            for (double y = size; y < height; y += size)
            {
                var xline = canvasFactory.CreateLine();
                xline.Point1.X = originX;
                xline.Point1.Y = y;
                xline.Point2.X = width;
                xline.Point2.Y = y;
                xline.Stroke = stroke;
                xline.StrokeThickness = thickness;
                var nline = nativeConverter.Convert(xline);
                backgroundCanvas.Add(nline);
            }

            for (double x = size; x < width; x += size)
            {
                var xline = canvasFactory.CreateLine();
                xline.Point1.X = x;
                xline.Point1.Y = originY;
                xline.Point2.X = x;
                xline.Point2.Y = height;
                xline.Stroke = stroke;
                xline.StrokeThickness = thickness;
                var nline = nativeConverter.Convert(xline);
                backgroundCanvas.Add(nline);
            }
        }

        public CanvasShape ToModel()
        {
            var scope = _scopes.LastOrDefault();
            return scope.Resolve<CanvasShape>();
        }

        public NativeShape ToNative(CanvasShape xcanvas)
        {
            var scope = _scopes.LastOrDefault();
            var nativeConverter = scope.Resolve<INativeConverter>();
            var nativeCanvas = scope.Resolve<CanvasShape>();
            var boundsFactory = scope.Resolve<BoundsFactory>();

            nativeCanvas.Clear();

            var natives = ToNatives(nativeConverter, boundsFactory, nativeCanvas, xcanvas.Children);

            foreach (var native in natives)
            {
                nativeCanvas.Add(native);
            }

            return nativeCanvas;
        }

        private IList<NativeShape> ToNatives(INativeConverter nativeConverter, BoundsFactory boundsFactory, CanvasShape nativeCanvas, IList<NativeShape> xchildren)
        {
            var natives = new List<NativeShape>();

            foreach (var child in xchildren)
            {
                if (child is LineShape)
                {
                    var native = nativeConverter.Convert(child as LineShape);
                    natives.Add(native);
                    native.Bounds = boundsFactory.Create(nativeCanvas, native);
                    if (native.Bounds != null)
                    {
                        native.Bounds.Update();
                    }
                }
            }
            return natives;
        }

        public void Delete()
        {
            var scope = _scopes.LastOrDefault();
            var drawingCanvas = scope.Resolve<CanvasShape>();

            var selected = drawingCanvas.Children.Where(c => c.Bounds != null && c.Bounds.IsVisible()).ToList();

            foreach (var child in selected)
            {
                child.Bounds.Hide();
            }

            foreach (var child in selected)
            {
                drawingCanvas.Children.Remove(child);
            }

            drawingCanvas.Render(null);
        }
    }
}