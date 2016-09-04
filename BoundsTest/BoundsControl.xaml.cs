using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BoundsTest
{
    public partial class BoundsControl : UserControl
    {
        private Point _start;
        private bool _autoHideBounds;
        private ICanvas _boundsCanvas;
        private IList<IEllipse> _ellipses;
        private IEllipse _hitEllipse;
        private IEllipse _hoverEllipse;

        public BoundsControl()
        {
            InitializeComponent();
            InitEvents();
            InitModel();
            Test(totalEllipses: 100);
        }

        private void InitEvents()
        {
            // mouse downs
            layout.PreviewMouseLeftButtonDown += (sender, e) =>
            {
                if (drawingCanvas.IsMouseCaptured)
                {
                    return;
                }

                if (_hitEllipse != null && _autoHideBounds)
                {
                    _hitEllipse.Bounds.Hide();
                }

                if (_hoverEllipse != null && _autoHideBounds)
                {
                    _hoverEllipse.Bounds.Hide();
                }

                var p = e.GetPosition(drawingCanvas);
                _hitEllipse = HitTest(p.X, p.Y);
                if (_hitEllipse != null)
                {
                    if (_autoHideBounds)
                    {
                        _hitEllipse.Bounds.Show();
                    }
                    _start = p;
                    drawingCanvas.CaptureMouse();
                }
            };

            // mouse ups
            layout.PreviewMouseLeftButtonUp += (sender, e) =>
            {
                if (drawingCanvas.IsMouseCaptured)
                {
                    drawingCanvas.ReleaseMouseCapture();
                }
            };

            // mouse moves
            layout.PreviewMouseMove += (sender, e) =>
            {
                if (drawingCanvas.IsMouseCaptured)
                {
                    var p = e.GetPosition(drawingCanvas);
                    double dx = _start.X - p.X;
                    double dy = _start.Y - p.Y;
                    _start = p;
                    (_hitEllipse.Native as FrameworkElement).MoveDelta(dx, dy);
                   _hitEllipse.Bounds.Update();
                }

                if (!drawingCanvas.IsMouseCaptured)
                {
                    var p = e.GetPosition(drawingCanvas);
                    var result = HitTest(p.X, p.Y);

                    if (_autoHideBounds)
                    {
                        if(_hoverEllipse != null && (_hitEllipse != _hoverEllipse))
                        {
                            _hoverEllipse.Bounds.Hide();
                        }

                        if (result != null)
                        {
                            _hoverEllipse = result;
                            _hoverEllipse.Bounds.Show();
                        }
                    }

                    if (result != null)
                    {
                        Cursor = Cursors.SizeAll;
                    }
                    else
                    {
                        Cursor = Cursors.Arrow;
                    }
                }
            };
        }

        private void InitModel()
        {
            _autoHideBounds = true;
            _boundsCanvas = new XCanvas() { Native = boundsCanvas };
            _ellipses = new List<IEllipse>();
        }

        private void Test(int totalEllipses)
        {
            var rnd = new Random(DateTime.Now.Millisecond);
            double min = 50.0;
            double max = 550.0;
            for (int i = 0; i < totalEllipses; i++)
            {
                double x = (rnd.NextDouble() * (max - min)) + min;
                double y = (rnd.NextDouble() * (max - min)) + min;
                AddEllipse(x, y, 10.0, 10.0, 5.0);
            }
        }

        private void AddEllipse(double x, double y, double width, double height, double boundsOffset)
        {
            var ellipse = new WpfEllipse();

            ellipse.Bounds = new WpfEllipseBounds(_boundsCanvas, ellipse, boundsOffset);
            ellipse.Native = new Ellipse() { Width = width, Height = height, Fill = Brushes.Black, Stroke = Brushes.Black, StrokeThickness = 2.0 };
            
            (ellipse.Native as FrameworkElement).Move(x, y);
            drawingCanvas.Children.Add(ellipse.Native as UIElement);

            ellipse.Bounds.Update();
            _ellipses.Add(ellipse);

            if (!_autoHideBounds)
            {
                ellipse.Bounds.Show();
            }
        }

        private IEllipse HitTest(double x, double y)
        {
            foreach (var ellipse in _ellipses)
            {
                if (ellipse.Bounds.Contains(x, y))
                {
                    return ellipse;
                }
            }
            return null;
        }
    }
}
