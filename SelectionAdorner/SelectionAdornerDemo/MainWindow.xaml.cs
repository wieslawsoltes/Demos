
namespace SelectionAdornerDemo
{
    #region References

    using System;
    using System.Collections.Generic;
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

    #endregion

    #region MainWindow

    public partial class MainWindow : Window
    {
        #region Fields

        private Point selectionOrigin;
        private SelectionAdorner adorner = null;

        #endregion

        #region Constructor

        public MainWindow()
        {
            InitializeComponent();
        } 

        #endregion

        #region Selection Adorner

        private void CreateAdorner(Canvas canvas, Point origin, Point point)
        {
            var layer = AdornerLayer.GetAdornerLayer(canvas);

            adorner = new SelectionAdorner(canvas);
            adorner.Zoom = 1.0;
            adorner.SelectionOrigin = new Point(origin.X, origin.Y);

            adorner.SelectionRect = new Rect(origin, point);

            adorner.SnapsToDevicePixels = false;
            RenderOptions.SetEdgeMode(adorner, EdgeMode.Aliased);

            layer.Add(adorner);
            adorner.InvalidateVisual();
        }

        private void RemoveAdorner(Canvas canvas)
        {
            var layer = AdornerLayer.GetAdornerLayer(canvas);

            layer.Remove(adorner);

            adorner = null;
        }

        private void UpdateAdorner(Point point)
        {
            var origin = adorner.SelectionOrigin;
            double width = Math.Abs(point.X - origin.X);
            double height = Math.Abs(point.Y - origin.Y);

            adorner.SelectionRect = new Rect(point, origin);
            adorner.InvalidateVisual();
        }

        #endregion

        #region HitTest

        public IEnumerable<FrameworkElement> HitTest(Canvas canvas, ref Rect rect)
        {
            var selectedElements = new List<DependencyObject>();

            var rectangle = new RectangleGeometry(rect, 0.0, 0.0);

            var hitTestParams = new GeometryHitTestParameters(rectangle);

            var resultCallback = new HitTestResultCallback(result => HitTestResultBehavior.Continue);

            var filterCallback = new HitTestFilterCallback(
                element =>
                {
                    if (VisualTreeHelper.GetParent(element) == canvas)
                    {
                        selectedElements.Add(element);
                    }
                    return HitTestFilterBehavior.Continue;
                });

            VisualTreeHelper.HitTest(canvas, filterCallback, resultCallback, hitTestParams);

            return selectedElements.Cast<FrameworkElement>();
        }

        #endregion

        #region Canvas Events

        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var canvas = sender as Canvas;
            var point = e.GetPosition(canvas);

            selectionOrigin = point;

            canvas.CaptureMouse();
        }

        private void Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var canvas = sender as Canvas;

            if (canvas.IsMouseCaptured)
            {
                canvas.ReleaseMouseCapture();

                if (adorner != null)
                {
                    var rect = adorner.SelectionRect;

                    var elements = HitTest(canvas, ref rect);

                    if (elements != null)
                    {
                        foreach (var element in elements)
                        {
                            // TODO: Handle selected elements
                        }
                    }

                    RemoveAdorner(canvas);
                }
            }
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            var canvas = sender as Canvas;

            var point = e.GetPosition(canvas);

            if (canvas.IsMouseCaptured)
            {
                if (adorner == null)
                {
                    CreateAdorner(canvas, selectionOrigin, point);
                }

                UpdateAdorner(point);
            }
        } 

        #endregion
    } 

    #endregion
}
