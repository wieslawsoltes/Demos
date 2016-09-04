using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace XYControllerDemo
{
    public class ControllerCanvas : Canvas
    {
        #region Properties

        public double X
        {
            get { return Point.X; }
        }

        public double Y
        {
            get { return Point.Y; }
        }

        #endregion

        #region Dependency Properties

        public Point Point
        {
            get { return (Point)GetValue(PointProperty); }
            set 
            {
                if (value.X < 0.0)
                {
                    value.X = 0.0;
                }

                if (value.Y < 0.0)
                {
                    value.Y = 0.0;
                }

                if (value.X > this.ActualWidth)
                {
                    value.X = this.ActualWidth;
                }

                if (value.Y > this.ActualHeight)
                {
                    value.Y = this.ActualHeight;
                }

                SetValue(PointProperty, value);
            }
        }

        public static readonly DependencyProperty PointProperty =
            DependencyProperty.Register("Point", typeof(Point), typeof(ControllerCanvas), new FrameworkPropertyMetadata(new Point(),
                FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

        public Brush ControllerStroke
        {
            get { return (Brush)GetValue(ControllerStrokeProperty); }
            set { SetValue(ControllerStrokeProperty, value); }
        }

        public static readonly DependencyProperty ControllerStrokeProperty =
            DependencyProperty.Register("ControllerStroke", typeof(Brush), typeof(ControllerCanvas), new FrameworkPropertyMetadata(Brushes.Red,
                FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

        public double ControllerStrokeThickness
        {
            get { return (double)GetValue(ControllerStrokeThicknessProperty); }
            set { SetValue(ControllerStrokeThicknessProperty, value); }
        }

        public static readonly DependencyProperty ControllerStrokeThicknessProperty =
            DependencyProperty.Register("ControllerStrokeThickness", typeof(double), typeof(ControllerCanvas), new FrameworkPropertyMetadata(1.0,
                FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

        public Brush GridStroke
        {
            get { return (Brush)GetValue(GridStrokeProperty); }
            set { SetValue(GridStrokeProperty, value); }
        }

        public static readonly DependencyProperty GridStrokeProperty =
            DependencyProperty.Register("GridStroke", typeof(Brush), typeof(ControllerCanvas), new FrameworkPropertyMetadata(Brushes.LightGray,
                FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

        public double GridStrokeThickness
        {
            get { return (double)GetValue(GridStrokeThicknessProperty); }
            set { SetValue(GridStrokeThicknessProperty, value); }
        }

        public static readonly DependencyProperty GridStrokeThicknessProperty =
            DependencyProperty.Register("GridStrokeThickness", typeof(double), typeof(ControllerCanvas), new FrameworkPropertyMetadata(1.0,
                FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

        public bool GridVisible
        {
            get { return (bool)GetValue(GridVisibleProperty); }
            set { SetValue(GridVisibleProperty, value); }
        }

        public static readonly DependencyProperty GridVisibleProperty =
            DependencyProperty.Register("GridVisible", typeof(bool), typeof(ControllerCanvas), new FrameworkPropertyMetadata(false,
                FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

        public Thickness GridMargin
        {
            get { return (Thickness)GetValue(GridMarginProperty); }
            set { SetValue(GridMarginProperty, value); }
        }

        public static readonly DependencyProperty GridMarginProperty =
            DependencyProperty.Register("GridMargin", typeof(Thickness), typeof(ControllerCanvas), new FrameworkPropertyMetadata(new Thickness(0.0, 0.0, 0.0, 0.0),
                FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

        public double GridSize
        {
            get { return (double)GetValue(GridSizeProperty); }
            set { SetValue(GridSizeProperty, value); }
        }

        public static readonly DependencyProperty GridSizeProperty =
            DependencyProperty.Register("GridSize", typeof(double), typeof(ControllerCanvas), new FrameworkPropertyMetadata(30.0,
                FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

        #endregion

        #region Drawing Context

        Pen penGrid = null;
        Pen penController = null;

        Brush previousGridStroke = null;
        double previousGridStrokeThickness = double.NaN;
        double penGridHalfThickness = double.NaN;

        Brush previousControllerStroke = null;
        double previousControllerStrokeThickness = double.NaN;
        double penControllerHalfThickness = double.NaN;

        Point p1 = new Point();
        Point p2 = new Point();
        Point p3 = new Point();
        Point p4 = new Point();

        double width = double.NaN;
        double height = double.NaN;

        void DrawGrid(DrawingContext dc)
        {
            width = this.ActualWidth;
            height = this.ActualHeight;

            // draw vertical grid lines
            for (double y = GridMargin.Top; y <= height - GridMargin.Bottom; y += GridSize)
            {
                p1.X = GridMargin.Left;
                p1.Y = y;
                p2.X = width - GridMargin.Right;
                p2.Y = y;

                GuidelineSet g = new GuidelineSet();
                g.GuidelinesX.Add(p1.X + penGridHalfThickness);
                g.GuidelinesX.Add(p2.X + penGridHalfThickness);
                g.GuidelinesY.Add(p1.Y + penGridHalfThickness);
                g.GuidelinesY.Add(p2.Y + penGridHalfThickness);
                dc.PushGuidelineSet(g);
                dc.DrawLine(penGrid, p1, p2);
                dc.Pop();
            }

            // draw horizontal grid lines
            for (double x = GridMargin.Left; x <= width - GridMargin.Right; x += GridSize)
            {
                p1.X = x;
                p1.Y = GridMargin.Top;
                p2.X = x;
                p2.Y = height - GridMargin.Bottom;

                GuidelineSet g = new GuidelineSet();
                g.GuidelinesX.Add(p1.X + penGridHalfThickness);
                g.GuidelinesX.Add(p2.X + penGridHalfThickness);
                g.GuidelinesY.Add(p1.Y + penGridHalfThickness);
                g.GuidelinesY.Add(p2.Y + penGridHalfThickness);
                dc.PushGuidelineSet(g);
                dc.DrawLine(penGrid, p1, p2);
                dc.Pop();
            }
        }

        void DrawController(DrawingContext dc)
        {
            width = this.ActualWidth;
            height = this.ActualHeight;

            // draw vertical controller line
            p1.X = 0.0;
            p1.Y = Point.Y;
            p2.X = width;
            p2.Y = Point.Y;
            
            GuidelineSet g1 = new GuidelineSet();
            g1.GuidelinesX.Add(p1.X + penControllerHalfThickness);
            g1.GuidelinesX.Add(p2.X + penControllerHalfThickness);
            g1.GuidelinesY.Add(p1.Y + penControllerHalfThickness);
            g1.GuidelinesY.Add(p2.Y + penControllerHalfThickness);
            dc.PushGuidelineSet(g1);
            dc.DrawLine(penController, p1, p2);
            dc.Pop();

            // draw horizontal controller line
            p3.X = Point.X;
            p3.Y = 0.0;
            p4.X = Point.X;
            p4.Y = height;

            GuidelineSet g2 = new GuidelineSet();
            g2.GuidelinesX.Add(p3.X + penControllerHalfThickness);
            g2.GuidelinesX.Add(p4.X + penControllerHalfThickness);
            g2.GuidelinesY.Add(p3.Y + penControllerHalfThickness);
            g2.GuidelinesY.Add(p4.Y + penControllerHalfThickness);
            dc.PushGuidelineSet(g2);
            dc.DrawLine(penController, p3, p4);
            dc.Pop();
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            // create ord update grid pen
            if (penGrid == null)
            {
                penGrid = new Pen(GridStroke, GridStrokeThickness);

                previousGridStroke = GridStroke;
                previousGridStrokeThickness = GridStrokeThickness;

                penGridHalfThickness = penGrid.Thickness / 2.0;
            }
            else
            {
                if (GridStroke != previousGridStroke || GridStrokeThickness != previousGridStrokeThickness)
                {
                    previousGridStroke = GridStroke;
                    previousGridStrokeThickness = GridStrokeThickness;
                    penGrid.Brush = GridStroke;
                    penGrid.Thickness = GridStrokeThickness;

                    penGridHalfThickness = penGrid.Thickness / 2.0;
                }
            }

            // create ord update controller pen
            if (penController == null)
            {
                penController = new Pen(ControllerStroke, ControllerStrokeThickness);

                previousControllerStroke = ControllerStroke;
                previousControllerStrokeThickness = ControllerStrokeThickness;

                penControllerHalfThickness = penController.Thickness / 2.0;
            }
            else
            {
                if (ControllerStroke != previousControllerStroke || ControllerStrokeThickness != previousControllerStrokeThickness)
                {
                    previousControllerStroke = ControllerStroke;
                    previousControllerStrokeThickness = ControllerStrokeThickness;
                    penController.Brush = ControllerStroke;
                    penController.Thickness = ControllerStrokeThickness;

                    penControllerHalfThickness = penController.Thickness / 2.0;
                }
            }

            // drag grid
            if (GridVisible)
            {
                DrawGrid(dc);
            }

            // draw controller
            DrawController(dc);
        }

        #endregion

        #region Mouse Events

        protected override void OnMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!this.IsMouseCaptured)
            {
                this.Point = e.GetPosition(this);

                this.Cursor = Cursors.Hand;
                this.CaptureMouse();
            }

            base.OnMouseLeftButtonDown(e);
        }

        protected override void OnMouseLeftButtonUp(System.Windows.Input.MouseButtonEventArgs e)
        {
            if (this.IsMouseCaptured)
            {
                this.Cursor = Cursors.Arrow;
                this.ReleaseMouseCapture();
            }

            base.OnMouseLeftButtonUp(e);
        }

        protected override void OnMouseMove(System.Windows.Input.MouseEventArgs e)
        {
            if (this.IsMouseCaptured)
            {
                this.Point = e.GetPosition(this);
            }

            base.OnMouseMove(e);
        }

        #endregion
    }
}
