using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace CanvasDiagram
{
    public struct Point2
    {
        public double X { get; private set; }

        public double Y { get; private set; }

        public Point2(double x, double y)
        {
            X = x;
            Y = y;
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

    public enum LineCap
    {
        Flat = 0,
        Square = 1,
        Round = 2,
        Triangle = 3
    }

    public interface IBounds
    {
        void Update();
        bool IsVisible();
        void Show();
        void Hide();
        bool Contains(double x, double y);
        void Move(double dx, double dy);
    }

    public abstract class BaseShape
    {
        public IBounds Bounds { get; set; }
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

    public class PolygonShape : BaseShape
    {
        public PointShape[] Points { get; set; }

        public ArgbColor Stroke { get; set; }

        public double StrokeThickness { get; set; }

        public LineCap StartLineCap { get; set; }

        public LineCap EndLineCap { get; set; }

        public PolygonShape()
        {
            Stroke = new ArgbColor(0xFF, 0x00, 0xBF, 0xFF);
            StrokeThickness = 2.0;
            StartLineCap = LineCap.Round;
            EndLineCap = LineCap.Round;
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

    public class CanvasShape : BaseShape
    {
        public double Width { get; set; }

        public double Height { get; set; }

        public ArgbColor Background { get; set; }

        public IList<BaseShape> Children { get; set; }

        public bool EnableSnap { get; set; }

        public double SnapX { get; set; }

        public double SnapY { get; set; }

        public Func<bool> IsCaptured { get; set; }

        public Action Capture { get; set; }

        public Action ReleaseCapture { get; set; }

        public Action InvalidateShape { get; set; }

        public IObservable<Point2> Downs { get; set; }

        public IObservable<Point2> Ups { get; set; }

        public IObservable<Point2> Moves { get; set; }

        public CanvasShape()
        {
            Width = 600.0;
            Height = 600.0;
            Background = new ArgbColor(0x00, 0xFF, 0xFF, 0xFF);
            Children = new ObservableCollection<BaseShape>();
            SnapX = 15.0;
            SnapY = 15.0;
            EnableSnap = true;
        }

        public double Snap(double val, double snap)
        {
            double r = val % snap;
            return r >= snap / 2.0 ? val + snap - r : val - r;
        }
    }

    public class LineBounds : IBounds
    {
        private enum HitResult { None, Point1, Point2, Line };
        private HitResult _hitResult;
        private LineShape _lineShape;
        private CanvasShape _canvasShape;
        private PolygonShape _linePolygon;
        private PolygonShape _point1Polygon;
        private PolygonShape _point2Polygon;
        private bool _isVisible;

        public LineBounds(CanvasShape canvasShape, LineShape line)
        {
            _lineShape = line;
            _canvasShape = canvasShape;
            _hitResult = HitResult.None;
            _linePolygon = CreatePolygon(4);
            _point1Polygon = CreatePolygon(4);
            _point2Polygon = CreatePolygon(4);
        }

        private PolygonShape CreatePolygon(int count)
        {
            var points = new PointShape[count];

            for (int i = 0; i < count; i++)
            {
                points[i] = new PointShape(0, 0);
            }

            var polygon = new PolygonShape
            {
                Points = points
            };

            return polygon;
        }

        private void UpdatePoints(PointShape center, PolygonShape polygonShape, double thickness)
        {
            var x = center.X - (thickness / 2.0);
            var y = center.Y - (thickness / 2.0);

            var width = thickness;
            var height = thickness;

            polygonShape.Points[0].X = x;
            polygonShape.Points[0].Y = y;

            polygonShape.Points[1].X = x + width;
            polygonShape.Points[1].Y = y;

            polygonShape.Points[2].X = x + width;
            polygonShape.Points[2].Y = y + height;

            polygonShape.Points[3].X = x;
            polygonShape.Points[3].Y = y + height;
        }

        private double Angle(PointShape point0, PointShape point1)
        {
            return Math.Atan2(point0.Y - point1.Y, point0.X - point1.X);
        }

        private void RotatePoint(PointShape point, double radians, double centerX, double centerY)
        {
            var x = (point.X - centerX) * Math.Cos(radians) - (point.Y - centerY) * Math.Sin(radians) + centerX;
            var y = (point.X - centerX) * Math.Sin(radians) + (point.Y - centerY) * Math.Cos(radians) + centerY;
            point.X = x;
            point.Y = y;
        }

        private void RotatePoints(PointShape center, PolygonShape polygonShape, double radians)
        {
            RotatePoint(polygonShape.Points[0], radians, center.X, center.Y);
            RotatePoint(polygonShape.Points[1], radians, center.X, center.Y);
            RotatePoint(polygonShape.Points[2], radians, center.X, center.Y);
            RotatePoint(polygonShape.Points[3], radians, center.X, center.Y);
        }

        public void Update()
        {
            UpdatePoints(_lineShape.Point1, _point1Polygon, _lineShape.StrokeThickness);
            UpdatePoints(_lineShape.Point2, _point2Polygon, _lineShape.StrokeThickness);

            var radians = Angle(_lineShape.Point1, _lineShape.Point2);

            RotatePoints(_lineShape.Point1, _point1Polygon, radians);
            RotatePoints(_lineShape.Point2, _point2Polygon, radians);

            _linePolygon.Points[0].X = _point1Polygon.Points[1].X;
            _linePolygon.Points[0].Y = _point1Polygon.Points[1].Y;
            _linePolygon.Points[1].X = _point1Polygon.Points[2].X;
            _linePolygon.Points[1].Y = _point1Polygon.Points[2].Y;
            _linePolygon.Points[2].X = _point2Polygon.Points[3].X;
            _linePolygon.Points[2].Y = _point2Polygon.Points[3].Y;
            _linePolygon.Points[3].X = _point2Polygon.Points[0].X;
            _linePolygon.Points[3].Y = _point2Polygon.Points[0].Y;
        }

        public bool IsVisible()
        {
            return _isVisible;
        }

        public void Show()
        {
            if (!_isVisible)
            {
                _canvasShape.Children.Add(_linePolygon);
#if true
                _canvasShape.Children.Add(_point1Polygon);
                _canvasShape.Children.Add(_point2Polygon);
#endif
                _isVisible = true;
            }
        }

        public void Hide()
        {
            if (_isVisible)
            {
                _canvasShape.Children.Remove(_linePolygon);
#if true
                _canvasShape.Children.Remove(_point1Polygon);
                _canvasShape.Children.Remove(_point2Polygon);
#endif
                _isVisible = false;
            }
        }

        public bool Contains(double x, double y)
        {
            if (_point1Polygon.Contains(x, y))
            {
                _hitResult = HitResult.Point1;
                return true;
            }
            else if (_point2Polygon.Contains(x, y))
            {
                _hitResult = HitResult.Point2;
                return true;
            }
            else if (_linePolygon.Contains(x, y))
            {
                _hitResult = HitResult.Line;
                return true;
            }
            _hitResult = HitResult.None;
            return false;
        }

        public void Move(double dx, double dy)
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

        private void MovePoint1(double dx, double dy)
        {
            double x1 = _lineShape.Point1.X - dx;
            double y1 = _lineShape.Point1.Y - dy;
            _lineShape.Point1.X = _canvasShape.EnableSnap ? _canvasShape.Snap(x1, _canvasShape.SnapX) : x1;
            _lineShape.Point1.Y = _canvasShape.EnableSnap ? _canvasShape.Snap(y1, _canvasShape.SnapY) : y1;
            _lineShape.Point1 = _lineShape.Point1;
        }

        private void MovePoint2(double dx, double dy)
        {
            double x2 = _lineShape.Point2.X - dx;
            double y2 = _lineShape.Point2.Y - dy;
            _lineShape.Point2.X = _canvasShape.EnableSnap ? _canvasShape.Snap(x2, _canvasShape.SnapX) : x2;
            _lineShape.Point2.Y = _canvasShape.EnableSnap ? _canvasShape.Snap(y2, _canvasShape.SnapY) : y2;
            _lineShape.Point2 = _lineShape.Point2;
        }

        private void MoveLine(double dx, double dy)
        {
            double x1 = _lineShape.Point1.X - dx;
            double y1 = _lineShape.Point1.Y - dy;
            double x2 = _lineShape.Point2.X - dx;
            double y2 = _lineShape.Point2.Y - dy;
            _lineShape.Point1.X = _canvasShape.EnableSnap ? _canvasShape.Snap(x1, _canvasShape.SnapX) : x1;
            _lineShape.Point1.Y = _canvasShape.EnableSnap ? _canvasShape.Snap(y1, _canvasShape.SnapY) : y1;
            _lineShape.Point2.X = _canvasShape.EnableSnap ? _canvasShape.Snap(x2, _canvasShape.SnapX) : x2;
            _lineShape.Point2.Y = _canvasShape.EnableSnap ? _canvasShape.Snap(y2, _canvasShape.SnapY) : y2;
            _lineShape.Point1 = _lineShape.Point1;
            _lineShape.Point2 = _lineShape.Point2;
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
        private Point2 _original;
        private Point2 _start;
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

        private BaseShape HitTest(IList<BaseShape> children, double x, double y)
        {
            return children.Where(c => c.Bounds != null && c.Bounds.Contains(x, y)).FirstOrDefault();
        }

        private bool IsState(State state)
        {
            return (_state & state) == state;
        }

        private void Down(Point2 p)
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

            _selected = HitTest(_drawingCanvas.Children, p.X, p.Y);
            if (_selected != null)
            {
                ShowSelected();
                InitMove(p);
                _drawingCanvas.Capture?.Invoke();
                render = true;
            }

            if (render)
            {
                _drawingCanvas.InvalidateShape?.Invoke();
                _boundsCanvas.InvalidateShape?.Invoke();
            }
        }

        private void Up(Point2 p)
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

        private void Drag(Point2 p)
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
                var result = HitTest(_drawingCanvas.Children, p.X, p.Y);

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
                    _drawingCanvas.InvalidateShape?.Invoke();
                    _boundsCanvas.InvalidateShape?.Invoke();
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

        private void InitMove(Point2 p)
        {
            _original = p;
            _start = p;
            _state |= State.Move;
        }

        private void FinishMove(Point2 p)
        {
            _state &= ~State.Move;
        }

        private void Move(Point2 p)
        {
            if (_selected != null)
            {
                double dx = _start.X - p.X;
                double dy = _start.Y - p.Y;
                _start = p;
                _selected.Bounds.Move(dx, dy);
                _selected.Bounds.Update();
                _drawingCanvas.InvalidateShape?.Invoke();
                _boundsCanvas.InvalidateShape?.Invoke();
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
                _drawingCanvas.InvalidateShape?.Invoke();
                _boundsCanvas.InvalidateShape?.Invoke();
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
                    _drawingCanvas.InvalidateShape?.Invoke();
                    _boundsCanvas.InvalidateShape?.Invoke();
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
                    _lineShape.Bounds = new LineBounds(_boundsCanvas, _lineShape);
                    _lineShape.Bounds.Update();
                    _lineShape.Bounds.Show();
                    _drawingCanvas.Capture?.Invoke();
                    _drawingCanvas.InvalidateShape?.Invoke();
                    _boundsCanvas.InvalidateShape?.Invoke();
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
                    _drawingCanvas.InvalidateShape?.Invoke();
                    _boundsCanvas.InvalidateShape?.Invoke();
                }
            });
        }

        public void Dispose()
        {
            _downs.Dispose();
            _drags.Dispose();
        }
    }

    public class CanvasViewModel
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
            DrawingCanvas.InvalidateShape?.Invoke();
            BoundsCanvas.InvalidateShape?.Invoke();
        }

        public void Render()
        {
            BackgroundCanvas.InvalidateShape?.Invoke();
            DrawingCanvas.InvalidateShape?.Invoke();
            BoundsCanvas.InvalidateShape?.Invoke();
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

            DrawingCanvas.InvalidateShape?.Invoke();
            BoundsCanvas.InvalidateShape?.Invoke();
        }
    }

    public class PanAndZoomBorder : Border
    {
        private bool _initialize = true;
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
                if (value != null && value != Child)
                {
                    _child = value;
                    if (_initialize)
                    {
                        var group = new TransformGroup();
                        var st = new ScaleTransform();
                        group.Children.Add(st);
                        var tt = new TranslateTransform();
                        group.Children.Add(tt);
                        _child.RenderTransform = group;
                        _child.RenderTransformOrigin = new Point(0.0, 0.0);
                        MouseWheel += Border_MouseWheel;
                        MouseRightButtonDown += Border_MouseRightButtonDown;
                        MouseRightButtonUp += Border_MouseRightButtonUp;
                        MouseMove += Border_MouseMove;
                        PreviewMouseDown += Border_PreviewMouseDown;
                        _initialize = false;
                    }
                }
                base.Child = value;
            }
        }

        public void Reset()
        {
            if (_initialize == false && _child != null)
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
            if (_initialize == false && _child != null)
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
            if (_initialize == false && _child != null)
            {
                var tt = GetTranslateTransform(_child);
                _start = e.GetPosition(this);
                _origin = new Point(tt.X, tt.Y);
                Cursor = Cursors.Hand;
                _child.CaptureMouse();
            }
        }

        private void Border_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_initialize == false && _child != null)
            {
                _child.ReleaseMouseCapture();
                Cursor = Cursors.Arrow;
            }
        }

        private void Border_MouseMove(object sender, MouseEventArgs e)
        {
            if (_initialize == false && _child != null && _child.IsMouseCaptured)
            {
                var tt = GetTranslateTransform(_child);
                Vector v = _start - e.GetPosition(this);
                tt.X = _origin.X - v.X;
                tt.Y = _origin.Y - v.Y;
            }
        }

        private void Border_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Middle && e.ClickCount == 2 && _initialize == false && _child != null)
            {
                Reset();
            }
        }
    }

    public class DrawingContextRenderer
    {
        private PenLineCap ToPenLineCap(LineCap lineCap)
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

        private Color ToColor(ArgbColor color)
        {
            return Color.FromArgb(color.A, color.R, color.G, color.B);
        }

        public void DrawLine(DrawingContext dc, LineShape lineShape)
        {
            var brush = new SolidColorBrush(ToColor(lineShape.Stroke));
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

        public void DrawPolygon(DrawingContext dc, PolygonShape polygonShape)
        {
            var brush = new SolidColorBrush(ToColor(polygonShape.Stroke));
            brush.Freeze();

            var pen = new Pen(brush, polygonShape.StrokeThickness)
            {
                Brush = brush,
                Thickness = polygonShape.StrokeThickness,
                StartLineCap = ToPenLineCap(polygonShape.StartLineCap),
                EndLineCap = ToPenLineCap(polygonShape.EndLineCap)
            };
            pen.Freeze();

            var points = polygonShape.Points;
            var length = polygonShape.Points.Length;

            for (int i = 0; i < length; i++)
            {
                var index0 = i;
                var index1 = (i == length - 1) ? 0 : i + 1;
                var point0 = new Point(points[index0].X, points[index0].Y);
                var point1 = new Point(points[index1].X, points[index1].Y);

                dc.DrawLine(pen, point0, point1);
            }
        }

        public void DrawBackground(DrawingContext dc, CanvasShape canvasShape)
        {
            var brush = new SolidColorBrush(ToColor(canvasShape.Background));
            brush.Freeze();

            var rectangle = new Rect(0, 0, canvasShape.Width, canvasShape.Height);

            dc.DrawRectangle(brush, null, rectangle);
        }

        public void DrawCanvasShape(DrawingContext dc, CanvasShape canvasShape)
        {
            foreach (var shape in canvasShape.Children)
            {
                if (shape is LineShape lineShape)
                {
                    DrawLine(dc, lineShape);
                }
                else if (shape is PolygonShape polygonShape)
                {
                    DrawPolygon(dc, polygonShape);
                }
            }
        }
    }

    public class RenderCanvas : Canvas
    {
        private readonly CanvasShape _canvasShape;
        private readonly DrawingContextRenderer _drawingContextRenderer;

        public RenderCanvas(CanvasShape canvasShape)
        {
            _canvasShape = canvasShape;
            _canvasShape.InvalidateShape = () => this.InvalidateVisual();

            _drawingContextRenderer = new DrawingContextRenderer();

            Width = _canvasShape.Width;
            Height = _canvasShape.Height;
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            _drawingContextRenderer.DrawBackground(dc, _canvasShape);
            _drawingContextRenderer.DrawCanvasShape(dc, _canvasShape);
        }
    }

    public partial class MainWindow : Window
    {
        private CanvasViewModel _canvasViewModel;

        public MainWindow()
        {
            InitializeComponent();

            InitializeViewModel();

            PreviewKeyDown += MainWindow_PreviewKeyDown;

            DataContext = _canvasViewModel;
        }

        private void InitializeViewModel()
        {
            _canvasViewModel = new CanvasViewModel
            {
                BackgroundCanvas = new CanvasShape(),
                DrawingCanvas = new CanvasShape(),
                BoundsCanvas = new CanvasShape()
            };

            var backgroundRenderCanvas = new RenderCanvas(_canvasViewModel.BackgroundCanvas);
            var drawingRenderCanvas = new RenderCanvas(_canvasViewModel.DrawingCanvas);
            var boundsRenderCanvas = new RenderCanvas(_canvasViewModel.BoundsCanvas);

            layout.Children.Add(backgroundRenderCanvas);
            layout.Children.Add(drawingRenderCanvas);
            layout.Children.Add(boundsRenderCanvas);

            CreateGrid(
                _canvasViewModel.BackgroundCanvas,
                _canvasViewModel.BackgroundCanvas.Width,
                _canvasViewModel.BackgroundCanvas.Height,
                30,
                0, 0);

            ObserveInput(_canvasViewModel.DrawingCanvas, boundsRenderCanvas);

            _canvasViewModel.SelectionEditor = new SelectionEditor(_canvasViewModel.DrawingCanvas, _canvasViewModel.BoundsCanvas)
            {
                IsEnabled = false
            };

            _canvasViewModel.LineEditor = new LineEditor(_canvasViewModel.DrawingCanvas, _canvasViewModel.BoundsCanvas)
            {
                IsEnabled = true
            };
        }

        private void ObserveInput(CanvasShape canvasShape, UIElement target)
        {
            canvasShape.Downs = Observable.FromEventPattern<MouseButtonEventArgs>(
                 target,
                 "PreviewMouseLeftButtonDown").Select(e =>
                 {
                     var p = e.EventArgs.GetPosition(target);
                     return new Point2(
                         canvasShape.EnableSnap ? canvasShape.Snap(p.X, canvasShape.SnapX) : p.X,
                         canvasShape.EnableSnap ? canvasShape.Snap(p.Y, canvasShape.SnapY) : p.Y);
                 });

            canvasShape.Ups = Observable.FromEventPattern<MouseButtonEventArgs>(
                target,
                "PreviewMouseLeftButtonUp").Select(e =>
                {
                    var p = e.EventArgs.GetPosition(target);
                    return new Point2(
                        canvasShape.EnableSnap ? canvasShape.Snap(p.X, canvasShape.SnapX) : p.X,
                        canvasShape.EnableSnap ? canvasShape.Snap(p.Y, canvasShape.SnapY) : p.Y);
                });

            canvasShape.Moves = Observable.FromEventPattern<MouseEventArgs>(
                target,
                "PreviewMouseMove").Select(e =>
                {
                    var p = e.EventArgs.GetPosition(target);
                    return new Point2(
                        canvasShape.EnableSnap ? canvasShape.Snap(p.X, canvasShape.SnapX) : p.X,
                        canvasShape.EnableSnap ? canvasShape.Snap(p.Y, canvasShape.SnapY) : p.Y);
                });

            canvasShape.IsCaptured = () => Mouse.Captured == target;

            canvasShape.Capture = () => target.CaptureMouse();

            canvasShape.ReleaseCapture = () => target.ReleaseMouseCapture();
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
                    _canvasViewModel.LineEditor.IsEnabled = false;
                    _canvasViewModel.SelectionEditor.IsEnabled = true;
                    break;
                case Key.L:
                    _canvasViewModel.LineEditor.IsEnabled = true;
                    _canvasViewModel.SelectionEditor.IsEnabled = false;
                    break;
                case Key.G:
                    _canvasViewModel.ToggleSnap();
                    break;
                case Key.Delete:
                    _canvasViewModel.Delete();
                    break;
            }
        }
    }
}
