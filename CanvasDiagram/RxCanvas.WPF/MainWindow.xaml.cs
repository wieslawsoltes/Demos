using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace RxCanvas.WPF
{
    public static class ArgbColorExtensions
    {
        public static Color ToColor(this ArgbColor color)
        {
            return Color.FromArgb(color.A, color.R, color.G, color.B);
        }
    }

    public class MyCanvas : Canvas, IDrawableShape
    {
        private readonly CanvasShape _canvasShape;

        public MyCanvas(CanvasShape canvasShape, bool enableInput)
        {
            _canvasShape = canvasShape;

            if (enableInput)
            {
                _canvasShape.Downs = Observable.FromEventPattern<MouseButtonEventArgs>(
                     this,
                     "PreviewMouseLeftButtonDown").Select(e =>
                     {
                         var p = e.EventArgs.GetPosition(this);
                         return new Vector2(
                             _canvasShape.EnableSnap ? _canvasShape.Snap(p.X, _canvasShape.SnapX) : p.X,
                             _canvasShape.EnableSnap ? _canvasShape.Snap(p.Y, _canvasShape.SnapY) : p.Y);
                     });

                _canvasShape.Ups = Observable.FromEventPattern<MouseButtonEventArgs>(
                    this,
                    "PreviewMouseLeftButtonUp").Select(e =>
                    {
                        var p = e.EventArgs.GetPosition(this);
                        return new Vector2(
                            _canvasShape.EnableSnap ? _canvasShape.Snap(p.X, _canvasShape.SnapX) : p.X,
                            _canvasShape.EnableSnap ? _canvasShape.Snap(p.Y, _canvasShape.SnapY) : p.Y);
                    });

                _canvasShape.Moves = Observable.FromEventPattern<MouseEventArgs>(
                    this,
                    "PreviewMouseMove").Select(e =>
                    {
                        var p = e.EventArgs.GetPosition(this);
                        return new Vector2(
                            _canvasShape.EnableSnap ? _canvasShape.Snap(p.X, _canvasShape.SnapX) : p.X,
                            _canvasShape.EnableSnap ? _canvasShape.Snap(p.Y, _canvasShape.SnapY) : p.Y);
                    });

                _canvasShape.IsCaptured = () => Mouse.Captured == this;
                _canvasShape.Capture = () => this.CaptureMouse();
                _canvasShape.ReleaseCapture = () => this.ReleaseMouseCapture(); 
            }

            _canvasShape.Native = this;

            this.Width = _canvasShape.Width;
            this.Height = _canvasShape.Height;
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

                    var pen = new Pen(brush, lineShape.StrokeThickness);
                    pen.Brush = brush;
                    pen.Thickness = lineShape.StrokeThickness;
                    pen.StartLineCap = PenLineCap.Flat;
                    pen.EndLineCap = PenLineCap.Square;
                    pen.Freeze();

                    var point0 = new Point(lineShape.Point1.X, lineShape.Point1.Y);
                    var point1 = new Point(lineShape.Point2.X, lineShape.Point2.Y);

                    dc.DrawLine(pen, point0, point1);
                }
            }
        }
    }

    public partial class MainWindow : Window
    {
        private CanvasView _canvasView;

        public MainWindow()
        {
            InitializeComponent();

            _canvasView = new CanvasView();

            _canvasView.BackgroundCanvas = new CanvasShape();
            Layout.Children.Add(new MyCanvas(_canvasView.BackgroundCanvas, false));

            _canvasView.DrawingCanvas = new CanvasShape();
            Layout.Children.Add(new MyCanvas(_canvasView.DrawingCanvas, true));

            _canvasView.SelectionEditor = new SelectionEditor(_canvasView.DrawingCanvas)
            {
                IsEnabled = false
            };

            _canvasView.LineEditor = new LineEditor(_canvasView.DrawingCanvas)
            {
                IsEnabled = true
            };

            DataContext = _canvasView;
        }
    }
}
