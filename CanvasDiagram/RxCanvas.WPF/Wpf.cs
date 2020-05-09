using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Autofac;
using MathUtil;

namespace RxCanvas.WPF
{
    internal static class WpfExtensions
    {
        public static Color ToNativeColor(this ArgbColor color)
        {
            return Color.FromArgb(color.A, color.R, color.G, color.B);
        }
    }

    public class WpfLine : LineShape
    {
        private SolidColorBrush _strokeBrush;
        private Line _nline;
        private LineShape _xline;

        public WpfLine(LineShape line)
        {
            _xline = line;

            _strokeBrush = new SolidColorBrush(_xline.Stroke.ToNativeColor());
            _strokeBrush.Freeze();

            _nline = new Line()
            {
                X1 = _xline.Point1.X,
                Y1 = _xline.Point1.Y,
                X2 = _xline.Point2.X,
                Y2 = _xline.Point2.Y,
                Stroke = _strokeBrush,
                StrokeThickness = line.StrokeThickness,
                StrokeStartLineCap = PenLineCap.Flat,
                StrokeEndLineCap = PenLineCap.Square
            };

            Native = _nline;
        }

        public PointShape Point1
        {
            get { return _xline.Point1; }
            set
            {
                _xline.Point1 = value;
                _nline.X1 = _xline.Point1.X;
                _nline.Y1 = _xline.Point1.Y;
            }
        }

        public PointShape Point2
        {
            get { return _xline.Point2; }
            set
            {
                _xline.Point2 = value;
                _nline.X2 = _xline.Point2.X;
                _nline.Y2 = _xline.Point2.Y;
            }
        }

        public ArgbColor Stroke
        {
            get { return _xline.Stroke; }
            set
            {
                _xline.Stroke = value;
                _strokeBrush = new SolidColorBrush(_xline.Stroke.ToNativeColor());
                _strokeBrush.Freeze();
                _nline.Stroke = _strokeBrush;
            }
        }

        public double StrokeThickness
        {
            get { return _xline.StrokeThickness; }
            set
            {
                _xline.StrokeThickness = value;
                _nline.StrokeThickness = value;
            }
        }
    }
    
    public class MyCanvas : Canvas
    {
        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);
        }
    }

    public class WpfCanvas : CanvasShape
    {
        private SolidColorBrush _backgroundBrush;
        private CanvasShape _canvasShape;
        private MyCanvas _myCanvas;

        public WpfCanvas(CanvasShape canvasShape)
        {
            _canvasShape = canvasShape;

            _backgroundBrush = new SolidColorBrush(_canvasShape.Background.ToNativeColor());
            _backgroundBrush.Freeze();

            _myCanvas = new MyCanvas()
            {
                Width = canvasShape.Width,
                Height = canvasShape.Height,
                Background = _backgroundBrush
            };

            Downs = Observable.FromEventPattern<MouseButtonEventArgs>(
                _myCanvas,
                "PreviewMouseLeftButtonDown").Select(e =>
                {
                    var p = e.EventArgs.GetPosition(_myCanvas);
                    return new Vector2(
                        _canvasShape.EnableSnap ? Snap(p.X, _canvasShape.SnapX) : p.X,
                        _canvasShape.EnableSnap ? Snap(p.Y, _canvasShape.SnapY) : p.Y);
                });

            Ups = Observable.FromEventPattern<MouseButtonEventArgs>(
                _myCanvas,
                "PreviewMouseLeftButtonUp").Select(e =>
                {
                    var p = e.EventArgs.GetPosition(_myCanvas);
                    return new Vector2(
                        _canvasShape.EnableSnap ? Snap(p.X, _canvasShape.SnapX) : p.X,
                        _canvasShape.EnableSnap ? Snap(p.Y, _canvasShape.SnapY) : p.Y);
                });

            Moves = Observable.FromEventPattern<MouseEventArgs>(
                _myCanvas,
                "PreviewMouseMove").Select(e =>
                {
                    var p = e.EventArgs.GetPosition(_myCanvas);
                    return new Vector2(
                        _canvasShape.EnableSnap ? Snap(p.X, _canvasShape.SnapX) : p.X,
                        _canvasShape.EnableSnap ? Snap(p.Y, _canvasShape.SnapY) : p.Y);
                });

            Native = _myCanvas;
        }

        public IList<NativeShape> Children
        {
            get { return _canvasShape.Children; }
            set { _canvasShape.Children = value; }
        }

        public double Width
        {
            get { return _myCanvas.Width; }
            set { _myCanvas.Width = value; }
        }

        public double Height
        {
            get { return _myCanvas.Height; }
            set { _myCanvas.Height = value; }
        }

        public ArgbColor Background
        {
            get { return _canvasShape.Background; }
            set
            {
                _canvasShape.Background = value;
                if (_canvasShape.Background == null)
                {
                    _backgroundBrush = null;
                }
                else
                {
                    _backgroundBrush = new SolidColorBrush(_canvasShape.Background.ToNativeColor());
                    _backgroundBrush.Freeze();
                }
                _myCanvas.Background = _backgroundBrush;
            }
        }

        public bool EnableSnap
        {
            get { return _canvasShape.EnableSnap; }
            set { _canvasShape.EnableSnap = value; }
        }

        public double SnapX
        {
            get { return _canvasShape.SnapX; }
            set { _canvasShape.SnapX = value; }
        }

        public double SnapY
        {
            get { return _canvasShape.SnapY; }
            set { _canvasShape.SnapY = value; }
        }

        public bool IsCaptured
        {
            get { return Mouse.Captured == _myCanvas; }
            set { _myCanvas.CaptureMouse(); }
        }

        public void Capture()
        {
            _myCanvas.CaptureMouse();
        }

        public void ReleaseCapture()
        {
            _myCanvas.ReleaseMouseCapture();
        }

        public void Add(NativeShape value)
        {
            if (value.Native != null)
            {
                _myCanvas.Children.Add(value.Native as UIElement);
            }

            Children.Add(value);
        }

        public void Remove(NativeShape value)
        {
            if (value.Native != null)
            {
                _myCanvas.Children.Remove(value.Native as UIElement);
            }

            Children.Remove(value);
        }

        public void Clear()
        {
            _myCanvas.Children.Clear();
            Children.Clear();
        }

        public void Render(NativeShape context)
        {
        }
    }

    public class WpfConverter : INativeConverter
    {
        public LineShape Convert(LineShape line)
        {
            return new WpfLine(line);
        }

        public CanvasShape Convert(CanvasShape canvas)
        {
            return new WpfCanvas(canvas);
        }
    }

    public class WpfModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register<INativeConverter>(c => new WpfConverter()).SingleInstance();
        }
    }
}
