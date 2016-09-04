#region References

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes; 

#endregion

namespace CustomArcDemo
{
    #region CustomArc

    public class CustomArc : FrameworkElement
    {
        #region Properties

        public float RadiusX
        {
            get { return (float)GetValue(RadiusXProperty); }
            set { SetValue(RadiusXProperty, value); }
        }

        public static readonly DependencyProperty RadiusXProperty =
            DependencyProperty.Register("RadiusX", typeof(float), typeof(CustomArc),
            new FrameworkPropertyMetadata(10f, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

        public float RadiusY
        {
            get { return (float)GetValue(RadiusYProperty); }
            set { SetValue(RadiusYProperty, value); }
        }

        public static readonly DependencyProperty RadiusYProperty =
            DependencyProperty.Register("RadiusY", typeof(float), typeof(CustomArc),
            new FrameworkPropertyMetadata(10f, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

        public Point P0
        {
            get { return (Point)GetValue(P0Property); }
            set { SetValue(P0Property, value); }
        }

        public static readonly DependencyProperty P0Property =
            DependencyProperty.Register("P0", typeof(Point), typeof(CustomArc),
            new FrameworkPropertyMetadata(new Point(0, 0), FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

        public Point P1
        {
            get { return (Point)GetValue(P1Property); }
            set { SetValue(P1Property, value); }
        }

        public static readonly DependencyProperty P1Property =
            DependencyProperty.Register("P1", typeof(Point), typeof(CustomArc),
            new FrameworkPropertyMetadata(new Point(10, 0), FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

        public bool IsLargeArc
        {
            get { return (bool)GetValue(IsLargeArcProperty); }
            set { SetValue(IsLargeArcProperty, value); }
        }

        public static readonly DependencyProperty IsLargeArcProperty =
            DependencyProperty.Register("IsLargeArc", typeof(bool), typeof(CustomArc),
            new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

        public float RotationAngle
        {
            get { return (float)GetValue(RotationAngleProperty); }
            set { SetValue(RotationAngleProperty, value); }
        }

        public static readonly DependencyProperty RotationAngleProperty =
            DependencyProperty.Register("RotationAngle", typeof(float), typeof(CustomArc),
            new FrameworkPropertyMetadata(0f, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

        public bool ShowDebug
        {
            get { return (bool)GetValue(ShowDebugProperty); }
            set { SetValue(ShowDebugProperty, value); }
        }

        public static readonly DependencyProperty ShowDebugProperty =
            DependencyProperty.Register("ShowDebug", typeof(bool), typeof(CustomArc),
            new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

        #endregion

        #region Fields

        private Brush brush;
        private Pen pen;
        private Pen dpen;
        private Size size;
        private PathGeometry pg;
        private PathFigure pf;
        private ArcSegment arc;

        #endregion

        #region Constructor

        public CustomArc()
        {
            Initialize();
        }

        public void Initialize()
        {
            brush = new SolidColorBrush(Colors.Red);
            brush.Freeze();
            pen = new Pen(Brushes.Red, 2.0);
            pen.Freeze();
            dpen = new Pen(Brushes.Yellow, 2.0);
            dpen.Freeze();
            size = new Size();
            pg = new PathGeometry();
            pf = new PathFigure();
            arc = new ArcSegment();
            pf.Segments.Add(arc);
            pg.Figures.Add(pf);
        }

        #endregion

        #region OnRender

        protected override void OnRender(DrawingContext drawingContext)
        {
            Draw(drawingContext);
        }

        private void Draw(DrawingContext dc)
        {
            Point p0 = P0;
            Point p1 = P1;
            float radiusX = RadiusX;
            float radiusY = RadiusY;
            bool isLargeArc = IsLargeArc;
            float rotationAngle = RotationAngle;

            size.Width = radiusX;
            size.Height = radiusY;
            pf.IsFilled = false;
            pf.StartPoint = p0;
            arc.Point = p1;
            arc.Size = size;
            arc.RotationAngle = rotationAngle;
            arc.IsLargeArc = isLargeArc;
            arc.SweepDirection = SweepDirection.Counterclockwise;
            arc.IsStroked = true;

            if (ShowDebug == true)
            {
                dc.DrawRectangle(null, dpen, pg.Bounds);
                dc.DrawLine(dpen, p0, p1);
            }

            dc.DrawGeometry(brush, pen, pg);
        }

        #endregion
    }

    #endregion

    #region MainWindow

    public partial class MainWindow : Window
    {
        #region Constructor

        public MainWindow()
        {
            InitializeComponent();

            InitializeThumbs();
        }

        #endregion

        #region Events

        private void t0_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            MoveP0(e.HorizontalChange, e.VerticalChange);
        }

        private void t1_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            MoveP1(e.HorizontalChange, e.VerticalChange);
        }

        private void ta_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            SetAngle(Mouse.GetPosition(canvas));
        }

        private void trX_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            SetRadiusX(e.HorizontalChange, e.VerticalChange);
        }

        private void trY_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            SetRadiusY(e.HorizontalChange, e.VerticalChange);
        }

        #endregion

        #region Thumb Utils

        private double GetX(Thumb thumb)
        {
             return Canvas.GetLeft(thumb);
        }

        private double GetY(Thumb thumb)
        {
            return Canvas.GetTop(thumb);
        }

        private void SetX(Thumb thumb, double x)
        {
            Canvas.SetLeft(thumb, x);
        }

        private void SetY(Thumb thumb, double y)
        {
            Canvas.SetTop(thumb, y);
        }

        #endregion

        #region Arc Manipulation

        private void InitializeThumbs()
        {
            SetStartThumbPosition(0.0, 0.0, arc.P0.X, arc.P0.Y);
            SetEndThumbPosition(0.0, 0.0, arc.P1.X, arc.P1.Y);
            SetRotationThumbPosition(arc.P0.X, arc.P0.Y, arc.RotationAngle);
            SetRadiusThumbPosition(0.0, 0.0, arc.P1.X, arc.P1.Y);
        }

        private void SetStartThumbPosition(double dx, double dy, double x, double y)
        {
            SetX(t0, x + dx);
            SetY(t0, y + dy);
        }

        private void SetEndThumbPosition(double dx, double dy, double x, double y)
        {
            SetX(t1, x + dx);
            SetY(t1, y + dy);
        }

        private void SetRotationThumbPosition(double x0, double y0, double angle)
        {
            double radius = 50.0;
            double t = Math.Round(angle, 1) * (Math.PI / 180.0);
            double x = x0 + radius * Math.Cos(t);
            double y = y0 + radius * Math.Sin(t);
            SetX(ta, x);
            SetY(ta, y);
        }

        private void SetRadiusThumbPosition(double dx, double dy, double x, double y)
        {
            SetX(trX, x + dx + arc.RadiusX);
            SetY(trX, y + dy);
            SetX(trY, x + dx);
            SetY(trY, y + dy + arc.RadiusY);
        }

        private void MoveP0(double dx, double dy)
        {
            double x = GetX(t0);
            double y = GetY(t0);
            SetStartThumbPosition(dx, dy, x, y);
            SetRotationThumbPosition(arc.P0.X, arc.P0.Y, arc.RotationAngle);
            arc.P0 = new Point(arc.P0.X + dx, arc.P0.Y + dy);
        }

        private void MoveP1(double dx, double dy)
        {
            double x = GetX(t1);
            double y = GetY(t1);
            SetEndThumbPosition(dx, dy, x, y);
            SetRadiusThumbPosition(dx, dy, x, y);
            arc.P1 = new Point(arc.P1.X + dx, arc.P1.Y + dy);
        }

        private void SetAngle(Point p)
        {
            double x0 = GetX(t0);
            double y0 = GetY(t0);
            double angle = Math.Atan2(p.Y - y0, p.X - x0) * 180.0 / Math.PI;
            SetRotationThumbPosition(x0, y0, angle);
            arc.RotationAngle = (float)Math.Round(angle, 1);
        }

        private void SetRadiusX(double dx, double dy)
        {
            double x = GetX(trX);
            double y = GetY(trX);
            double x1 = GetX(t1);
            double y1 = GetY(t1);
            double rX = Math.Max(x + dx, x1);
            SetX(trX, rX);
            double dX = x1 - rX;
            arc.RadiusX = (float)Math.Round(Math.Sqrt(dX * dX), 0);
        }

        private void SetRadiusY(double dx, double dy)
        {
            double x = GetX(trY);
            double y = GetY(trY);
            double x1 = GetX(t1);
            double y1 = GetY(t1);
            double rY = Math.Max(y + dy, y1);
            SetY(trY, rY);
            double dY = y1 - rY;
            arc.RadiusY = (float)Math.Round(Math.Sqrt(dY * dY), 0);
        }

        #endregion
    } 

    #endregion
}
