// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
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

namespace TemplateCreator
{
    #region Tool

    public enum Tool
    {
        None,
        Line,
        Polyline,
        Path,
        Rect,
        Circle,
        Text,
        Move,
        Scale,
        Rotate,
        Duplicate
    }

    #endregion

    #region SnapUtil

    public static class SnapUtil
    {
        #region Snap

        public static double Snap(double original, double snap, double offset)
        {
            return Snap(original - offset, snap) + offset;
        }

        public static double Snap(double original, double snap)
        {
            return original + ((Math.Round(original / snap) - original / snap) * snap);
        }

        #endregion
    } 

    #endregion

    #region LineGuidesAdorner

    public class LineGuidesAdorner : Adorner
    {
        public double gridStrokeThickness = 1.0;

        #region Properties

        public double CanvasWidth
        {
            get { return (double)GetValue(CanvasWidthProperty); }
            set { SetValue(CanvasWidthProperty, value); }
        }

        public static readonly DependencyProperty CanvasWidthProperty =
            DependencyProperty.Register("CanvasWidth", typeof(double), typeof(LineGuidesAdorner),
                new FrameworkPropertyMetadata(0.0,
                    FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

        public double CanvasHeight
        {
            get { return (double)GetValue(CanvasHeightProperty); }
            set { SetValue(CanvasHeightProperty, value); }
        }

        public static readonly DependencyProperty CanvasHeightProperty =
            DependencyProperty.Register("CanvasHeight", typeof(double), typeof(LineGuidesAdorner),
                new FrameworkPropertyMetadata(0.0,
                    FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

        public double X
        {
            get { return (double)GetValue(XProperty); }
            set { SetValue(XProperty, value); }
        }

        public static readonly DependencyProperty XProperty =
            DependencyProperty.Register("X", typeof(double), typeof(LineGuidesAdorner),
                new FrameworkPropertyMetadata(0.0,
                    FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

        public double Y
        {
            get { return (double)GetValue(YProperty); }
            set { SetValue(YProperty, value); }
        }

        public static readonly DependencyProperty YProperty =
            DependencyProperty.Register("Y", typeof(double), typeof(LineGuidesAdorner),
                new FrameworkPropertyMetadata(0.0,
                    FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

        public double X1
        {
            get { return (double)GetValue(X1Property); }
            set { SetValue(X1Property, value); }
        }

        public static readonly DependencyProperty X1Property =
            DependencyProperty.Register("X1", typeof(double), typeof(LineGuidesAdorner),
                new FrameworkPropertyMetadata(0.0,
                    FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

        public double Y1
        {
            get { return (double)GetValue(Y1Property); }
            set { SetValue(Y1Property, value); }
        }

        public static readonly DependencyProperty Y1Property =
            DependencyProperty.Register("Y1", typeof(double), typeof(LineGuidesAdorner),
                new FrameworkPropertyMetadata(0.0,
                    FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

        public double X2
        {
            get { return (double)GetValue(X2Property); }
            set { SetValue(X2Property, value); }
        }

        public static readonly DependencyProperty X2Property =
            DependencyProperty.Register("X2", typeof(double), typeof(LineGuidesAdorner),
                new FrameworkPropertyMetadata(0.0,
                    FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

        public double Y2
        {
            get { return (double)GetValue(Y2Property); }
            set { SetValue(Y2Property, value); }
        }

        public static readonly DependencyProperty Y2Property =
            DependencyProperty.Register("Y2", typeof(double), typeof(LineGuidesAdorner),
                new FrameworkPropertyMetadata(0.0,
                    FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

        public Tool Tool
        {
            get { return (Tool)GetValue(ToolProperty); }
            set { SetValue(ToolProperty, value); }
        }

        public static readonly DependencyProperty ToolProperty =
            DependencyProperty.Register("Tool", typeof(Tool), typeof(LineGuidesAdorner),
                new FrameworkPropertyMetadata(Tool.None,
                    FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

        public bool EnableFill
        {
            get { return (bool)GetValue(EnableFillProperty); }
            set { SetValue(EnableFillProperty, value); }
        }

        public static readonly DependencyProperty EnableFillProperty =
            DependencyProperty.Register("EnableFill", typeof(bool), typeof(LineGuidesAdorner),
                new FrameworkPropertyMetadata(true,
                    FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

        public double StrokeThickness
        {
            get { return (double)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }

        public static readonly DependencyProperty StrokeThicknessProperty =
            DependencyProperty.Register("StrokeThickness", typeof(double), typeof(LineGuidesAdorner),
                new FrameworkPropertyMetadata(5.0,
                    FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

        #endregion

        #region Constructor

        public LineGuidesAdorner(UIElement adornedElement)
            : base(adornedElement)
        {
        }

        #endregion

        #region Pens & Brushes

        Brush BrushShape = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));

        Brush TransparentBrushShape = new SolidColorBrush(Color.FromArgb(0, 255, 255, 255));

        Pen PenShape = new Pen(new SolidColorBrush(Color.FromArgb(255, 0, 0, 0)), 5.0)
        {
            StartLineCap = PenLineCap.Flat,
            EndLineCap = PenLineCap.Flat,
            LineJoin = PenLineJoin.Miter
        };

        Pen PenGuides = new Pen(new SolidColorBrush(Color.FromArgb(255, 116, 116, 255)), 1.0)
        {
            StartLineCap = PenLineCap.Flat,
            EndLineCap = PenLineCap.Flat,
            LineJoin = PenLineJoin.Miter
        };

        Pen PenElement = new Pen(new SolidColorBrush(Color.FromArgb(255, 0, 0, 0)), 5.0)
        {
            StartLineCap = PenLineCap.Square,
            EndLineCap = PenLineCap.Square,
            LineJoin = PenLineJoin.Miter
        };

        #endregion

        #region OnRender

        protected override void OnRender(DrawingContext drawingContext)
        {
            if (IsEnabled == true)
            {
                bool enableFill = EnableFill;

                DrawGuides(drawingContext);

                if (Tool == Tool.Line || Tool == Tool.Polyline)
                {
                    DrawLine(drawingContext);
                }
                else if (Tool == Tool.Path)
                {
                    DrawPath(drawingContext, enableFill);
                }
                else if (Tool == Tool.Rect)
                {
                    DrawRect(drawingContext, enableFill);
                }
                else if (Tool == Tool.Circle)
                {
                    DrawCircle(drawingContext, enableFill);
                }
            }
        }

        private void DrawGuides(DrawingContext drawingContext)
        {
            double x = X;
            double y = Y;
            double width = CanvasWidth;
            double height = CanvasHeight;
            double offsetX = 0.0; //0.5;
            double offsetY = 0.0; //-0.5;

            if (x >= 0 && x <= width)
            {
                var verticalPoint0 = new Point(x + offsetX, 0);
                var verticalPoint1 = new Point(x + offsetX, height);

                double halfPenWidth = PenGuides.Thickness / 2.0;
                GuidelineSet guidelines = new GuidelineSet();
                guidelines.GuidelinesX.Add(verticalPoint0.X + halfPenWidth);
                guidelines.GuidelinesX.Add(verticalPoint1.X + halfPenWidth);
                guidelines.GuidelinesY.Add(verticalPoint0.Y + halfPenWidth);
                guidelines.GuidelinesY.Add(verticalPoint1.Y + halfPenWidth);
                drawingContext.PushGuidelineSet(guidelines);

                drawingContext.DrawLine(PenGuides, verticalPoint0, verticalPoint1);

                drawingContext.Pop();
            }

            if (y >= 0 && y <= height)
            {
                var horizontalPoint0 = new Point(0, y + offsetY);
                var horizontalPoint1 = new Point(width, y + offsetY);

                double halfPenWidth = PenGuides.Thickness / 2.0;
                GuidelineSet guidelines = new GuidelineSet();
                guidelines.GuidelinesX.Add(horizontalPoint0.X + halfPenWidth);
                guidelines.GuidelinesX.Add(horizontalPoint1.X + halfPenWidth);
                guidelines.GuidelinesY.Add(horizontalPoint0.Y + halfPenWidth);
                guidelines.GuidelinesY.Add(horizontalPoint1.Y + halfPenWidth);
                drawingContext.PushGuidelineSet(guidelines);

                drawingContext.DrawLine(PenGuides, horizontalPoint0, horizontalPoint1);

                drawingContext.Pop();
            }
        }

        private void DrawLine(DrawingContext drawingContext)
        {
            PenElement.Thickness = StrokeThickness;

            var p0 = new Point(X1, Y1);
            var p1 = new Point(X2, Y2);

            var geometry = new LineGeometry(p0, p1);

            double halfPenWidth = PenElement.Thickness / 2.0;
            GuidelineSet guidelines = new GuidelineSet();
            guidelines.GuidelinesX.Add(p0.X + halfPenWidth);
            guidelines.GuidelinesX.Add(p1.X + halfPenWidth);
            guidelines.GuidelinesY.Add(p0.Y + halfPenWidth);
            guidelines.GuidelinesY.Add(p1.Y + halfPenWidth);
            drawingContext.PushGuidelineSet(guidelines);

            drawingContext.DrawGeometry(null, PenElement, geometry);

            drawingContext.Pop();
        }

        private void DrawPath(DrawingContext drawingContext, bool enableFill)
        {
            PenShape.Thickness = StrokeThickness;

            var rect = GetShapeRect();

            var p0 = new Point(X1, Y1);
            var p1 = new Point(X2, Y2);

            var psc = new PathSegmentCollection();

            //var lineSegment = new LineSegment(p1, false);
            //psc.Add(lineSegment);

            var arcSegment = new ArcSegment(p1, rect.Size, 0, false, SweepDirection.Clockwise, false);
            psc.Add(arcSegment);

            //var bezierSegment = new BezierSegment(...)

            var pf = new PathFigure(p0, psc, true);
            var pfc = new PathFigureCollection();
            pfc.Add(pf);
            var pg = new PathGeometry(pfc);

            double halfPenWidth = PenShape.Thickness / 2.0;
            GuidelineSet guidelines = new GuidelineSet();
            guidelines.GuidelinesX.Add(rect.Left + halfPenWidth);
            guidelines.GuidelinesX.Add(rect.Right + halfPenWidth);
            guidelines.GuidelinesY.Add(rect.Top + halfPenWidth);
            guidelines.GuidelinesY.Add(rect.Bottom + halfPenWidth);
            drawingContext.PushGuidelineSet(guidelines);

            drawingContext.DrawGeometry(enableFill == true ? BrushShape : TransparentBrushShape,
                PenShape,
                pg);

            drawingContext.Pop();
        }

        private void DrawRect(DrawingContext drawingContext, bool enableFill)
        {
            PenShape.Thickness = StrokeThickness;

            var rect = GetShapeRect();

            var geometry = new RectangleGeometry(rect);

            double halfPenWidth = PenShape.Thickness / 2.0;
            GuidelineSet guidelines = new GuidelineSet();
            guidelines.GuidelinesX.Add(rect.Left + halfPenWidth);
            guidelines.GuidelinesX.Add(rect.Right + halfPenWidth);
            guidelines.GuidelinesY.Add(rect.Top + halfPenWidth);
            guidelines.GuidelinesY.Add(rect.Bottom + halfPenWidth);
            drawingContext.PushGuidelineSet(guidelines);

            drawingContext.DrawGeometry(enableFill == true ? BrushShape : TransparentBrushShape,
                PenShape, 
                geometry);

            drawingContext.Pop();
        }

        private void DrawCircle(DrawingContext drawingContext, bool enableFill)
        {
            PenShape.Thickness = StrokeThickness;

            var rect = GetShapeRect();

            var geometry = new EllipseGeometry(rect);

            double halfPenWidth = PenShape.Thickness / 2.0;
            GuidelineSet guidelines = new GuidelineSet();
            guidelines.GuidelinesX.Add(rect.Left + halfPenWidth);
            guidelines.GuidelinesX.Add(rect.Right + halfPenWidth);
            guidelines.GuidelinesY.Add(rect.Top + halfPenWidth);
            guidelines.GuidelinesY.Add(rect.Bottom + halfPenWidth);
            drawingContext.PushGuidelineSet(guidelines);

            drawingContext.DrawGeometry(enableFill == true ? BrushShape : TransparentBrushShape,
                PenShape,
                geometry);

            drawingContext.Pop();
        }

        private Rect GetShapeRect()
        {
            double halfPenWidth = (PenShape.Thickness - gridStrokeThickness) / 2.0;

            Point p0;
            Point p1;

            GetShapePoints(halfPenWidth, out p0, out p1);

            var rect = new Rect(p0, p1);

            return rect;
        }

        public void GetShapePoints(double halfPenWidth, out Point p0, out Point p1)
        {
            double x1 = X1;
            double y1 = Y1;
            double x2 = X2;
            double y2 = Y2;

            p0 = new Point
            (
                (x1 > x2) ? (x1 - halfPenWidth) : (x1 < x2) ? (x1 + halfPenWidth) : x1,
                (y1 > y2) ? (y1 - halfPenWidth) : (y1 < y2) ? (y1 + halfPenWidth) : y1
            );

            p1 = new Point
            (
                (x2 > x1) ? (x2 - halfPenWidth) : (x2 < x1) ? (x2 + halfPenWidth) : x2,
                (y2 > y1) ? (y2 - halfPenWidth) : (y2 < y1) ? (y2 + halfPenWidth) : y2
            );
        }

        #endregion
    }

    #endregion

    #region LineAdorner

    public class LineAdorner : Adorner
    {
        #region Fields

        private double size = 12;
        private double strokeThickness = 0;
        private Thumb thumb0 = null;
        private Thumb thumb1 = null;
        private VisualCollection visualCollection = null;

        private bool EnableSnap = true;
        private double SnapX = 15;
        private double SnapY = 15;
        private double SnapOffsetX = 0;
        private double SnapOffsetY = 5;

        #endregion

        #region Constructor

        public LineAdorner(UIElement adornedElement)
            : base(adornedElement)
        {
            visualCollection = new VisualCollection(this);

            thumb0 = new Thumb()
            {
                Template = Application.Current.Windows[0].Resources["ThumbEllipseKey"] as ControlTemplate,
                //Style = Application.Current.Windows[0].Resources["AdornerThumbStyleKey"] as Style,
                Cursor = Cursors.SizeAll
            };

            thumb1 = new Thumb()
            {
                Template = Application.Current.Windows[0].Resources["ThumbEllipseKey"] as ControlTemplate,
                //Style = Application.Current.Windows[0].Resources["AdornerThumbStyleKey"] as Style,
                Cursor = Cursors.SizeAll
            };

            thumb0.DragDelta += thumb0_DragDelta;
            thumb1.DragDelta += thumb1_DragDelta;

            visualCollection.Add(thumb0);
            visualCollection.Add(thumb1);
        }

        void thumb0_DragDelta(object sender, DragDeltaEventArgs e)
        {
            var line = this.AdornedElement as Line;

            double dX = e.HorizontalChange;
            double dY = e.VerticalChange;

            double x = EnableSnap == true ? SnapUtil.Snap(line.X1 + dX, SnapX, SnapOffsetX) : line.X1 + dX;
            double y = EnableSnap == true ? SnapUtil.Snap(line.Y1 + dY, SnapY, SnapOffsetY) : line.Y1 + dY;

            line.X1 = x;
            line.Y1 = y;
        }

        void thumb1_DragDelta(object sender, DragDeltaEventArgs e)
        {
            var line = this.AdornedElement as Line;

            double dX = e.HorizontalChange;
            double dY = e.VerticalChange;

            double x = EnableSnap == true ? SnapUtil.Snap(line.X2 + dX, SnapX, SnapOffsetX) : line.X2 + dX;
            double y = EnableSnap == true ? SnapUtil.Snap(line.Y2 + dY, SnapY, SnapOffsetY) : line.Y2 + dY;

            line.X2 = x;
            line.Y2 = y;
        }

        protected override Visual GetVisualChild(int index)
        {
            return visualCollection[index];
        }

        protected override int VisualChildrenCount
        {
            get
            {
                return visualCollection.Count;
            }
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            var line = this.AdornedElement as Line;

            double offset = (size + strokeThickness) / 2;

            thumb0.Arrange(new Rect(line.X1 - offset, line.Y1 - offset, size, size));
            thumb1.Arrange(new Rect(line.X2 - offset, line.Y2 - offset, size, size));

            return finalSize;
        }

        #endregion
    } 

    #endregion

    #region RectAdorner

    public class RectAdorner : Adorner
    {
        #region Fields

        private double size = 12;
        //private double strokeThickness = 2;
        private Thumb thumb0 = null;
        private Thumb thumb1 = null;
        private Thumb thumb2 = null;
        private Thumb thumb3 = null;
        private Thumb thumb4 = null;
        private VisualCollection visualCollection = null;

        private bool EnableSnap = true;
        private double SnapX = 15;
        private double SnapY = 15;
        private double SnapOffsetX = -0.5;
        private double SnapOffsetY = 4.5;

        private double SnapOffsetWidth = 1.0;
        private double SnapOffsetHeight = 1.0;

        #endregion

        #region Constructor

        public RectAdorner(UIElement adornedElement)
            : base(adornedElement)
        {
            visualCollection = new VisualCollection(this);

            thumb0 = new Thumb()
            {
                Template = Application.Current.Windows[0].Resources["ThumbEllipseKey"] as ControlTemplate,
                //Style = Application.Current.Windows[0].Resources["AdornerThumbStyleKey"] as Style,
                Cursor = Cursors.SizeNWSE
            };

            thumb1 = new Thumb()
            {
                Template = Application.Current.Windows[0].Resources["ThumbEllipseKey"] as ControlTemplate,
                //Style = Application.Current.Windows[0].Resources["AdornerThumbStyleKey"] as Style,
                Cursor = Cursors.SizeNESW
            };

            thumb2 = new Thumb()
            {
                Template = Application.Current.Windows[0].Resources["ThumbEllipseKey"] as ControlTemplate,
                //Style = Application.Current.Windows[0].Resources["AdornerThumbStyleKey"] as Style,
                Cursor = Cursors.SizeNESW
            };

            thumb3 = new Thumb()
            {
                Template = Application.Current.Windows[0].Resources["ThumbEllipseKey"] as ControlTemplate,
                //Style = Application.Current.Windows[0].Resources["AdornerThumbStyleKey"] as Style,
                Cursor = Cursors.SizeNWSE
            };

            thumb4 = new Thumb()
            {
                Template = Application.Current.Windows[0].Resources["ThumbTransparentKey"] as ControlTemplate,
                //Style = Application.Current.Windows[0].Resources["AdornerThumbStyleKey"] as Style,
                Cursor = Cursors.SizeAll
            };

            thumb0.DragDelta += thumb0_DragDelta;
            thumb1.DragDelta += thumb1_DragDelta;
            thumb2.DragDelta += thumb2_DragDelta;
            thumb3.DragDelta += thumb3_DragDelta;
            thumb4.DragDelta += thumb4_DragDelta;

            visualCollection.Add(thumb4);

            visualCollection.Add(thumb0);
            visualCollection.Add(thumb1);
            visualCollection.Add(thumb2);
            visualCollection.Add(thumb3);
        }

        void thumb0_DragDelta(object sender, DragDeltaEventArgs e)
        {
            var rect = this.AdornedElement as Rectangle;
            double dX = e.HorizontalChange;
            double dY = e.VerticalChange;
            double x = Canvas.GetLeft(rect);
            double y = Canvas.GetTop(rect);
            double width = rect.Width;
            double height = rect.Height;

            width = Math.Max(0, width - dX);

            if (width > 0)
                width = EnableSnap == true ? SnapUtil.Snap(width, SnapX, SnapOffsetWidth) : width;

            height = Math.Max(0, height - dY);

            if (height > 0)
                height = EnableSnap == true ? SnapUtil.Snap(height, SnapY, SnapOffsetHeight) : height;

            x = EnableSnap == true ? SnapUtil.Snap(x + dX, SnapX, SnapOffsetX) : x + dX;
            y = EnableSnap == true ? SnapUtil.Snap(y + dY, SnapY, SnapOffsetY) : y + dY;

            if (width > 0)
                Canvas.SetLeft(rect, x);

            if (height > 0)
                Canvas.SetTop(rect, y);

            rect.Width = width;
            rect.Height = height;
        }

        void thumb1_DragDelta(object sender, DragDeltaEventArgs e)
        {
            var rect = this.AdornedElement as Rectangle;
            double dX = e.HorizontalChange;
            double dY = e.VerticalChange;
            double y = Canvas.GetTop(rect);
            double width = rect.Width;
            double height = rect.Height;

            width = Math.Max(0, width + dX);

            if (width > 0)
                width = EnableSnap == true ? SnapUtil.Snap(width, SnapX, SnapOffsetWidth) : width;

            height = Math.Max(0, height - dY);

            if (height > 0)
                height = EnableSnap == true ? SnapUtil.Snap(height, SnapY, SnapOffsetHeight) : height;

            y = EnableSnap == true ? SnapUtil.Snap(y + dY, SnapY, SnapOffsetY) : y + dY;

            if (height > 0)
                Canvas.SetTop(rect, y);

            rect.Width = width;
            rect.Height = height;
        }

        void thumb2_DragDelta(object sender, DragDeltaEventArgs e)
        {
            var rect = this.AdornedElement as Rectangle;
            double dX = e.HorizontalChange;
            double dY = e.VerticalChange;
            double x = Canvas.GetLeft(rect);
            double width = rect.Width;
            double height = rect.Height;

            width = Math.Max(0, width - dX);

            if (width > 0)
                width = EnableSnap == true ? SnapUtil.Snap(width, SnapX, SnapOffsetWidth) : width;

            height = Math.Max(0, height + dY);

            if (height > 0)
                height = EnableSnap == true ? SnapUtil.Snap(height, SnapY, SnapOffsetHeight) : height;

            x = EnableSnap == true ? SnapUtil.Snap(x + dX, SnapX, SnapOffsetX) : x + dX;

            if (width > 0)
                Canvas.SetLeft(rect, x);

            rect.Width = width;
            rect.Height = height;
        }

        void thumb3_DragDelta(object sender, DragDeltaEventArgs e)
        {
            var rect = this.AdornedElement as Rectangle;
            double dX = e.HorizontalChange;
            double dY = e.VerticalChange;
            double width = rect.Width;
            double height = rect.Height;

            width = Math.Max(0, width + dX);

            if (width > 0)
                width = EnableSnap == true ? SnapUtil.Snap(width, SnapX, SnapOffsetWidth) : width;

            height = Math.Max(0, height + dY);

            if (height > 0)
                height = EnableSnap == true ? SnapUtil.Snap(height, SnapY, SnapOffsetHeight) : height;

            rect.Width = width;
            rect.Height = height;
        }

        void thumb4_DragDelta(object sender, DragDeltaEventArgs e)
        {
            var rect = this.AdornedElement as Rectangle;
            double dX = e.HorizontalChange;
            double dY = e.VerticalChange;
            double x = Canvas.GetLeft(rect);
            double y = Canvas.GetTop(rect);

            x = EnableSnap == true ? SnapUtil.Snap(x + dX, SnapX, SnapOffsetX) : x + dX;
            y = EnableSnap == true ? SnapUtil.Snap(y + dY, SnapY, SnapOffsetY) : y + dY;

            Canvas.SetLeft(rect, x);
            Canvas.SetTop(rect, y);
        }

        protected override Visual GetVisualChild(int index)
        {
            return visualCollection[index];
        }

        protected override int VisualChildrenCount
        {
            get
            {
                return visualCollection.Count;
            }
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            var rect = this.AdornedElement as Rectangle;

            double x = 0;
            double y = 0;
            double width = rect.Width;
            double height = rect.Height;
            double offset = size / 2;

            thumb0.Arrange(new Rect(x - offset, y - offset, size, size));
            thumb1.Arrange(new Rect(x + width - offset, y - offset, size, size));
            thumb2.Arrange(new Rect(x - offset, y + height - offset, size, size));
            thumb3.Arrange(new Rect(x + width - offset, y + height - offset, size, size));
            thumb4.Arrange(new Rect(x, y, width, height));

            return finalSize;
        }

        #endregion
    }

    #endregion

    #region EllipseAdorner

    public class EllipseAdorner : Adorner
    {
        #region Fields

        private double size = 12;
        //private double strokeThickness = 2;
        private Thumb thumb0 = null;
        private Thumb thumb1 = null;
        private Thumb thumb2 = null;
        private Thumb thumb3 = null;
        private Thumb thumb4 = null;
        private VisualCollection visualCollection = null;

        private bool EnableSnap = true;
        private double SnapX = 15;
        private double SnapY = 15;
        private double SnapOffsetX = -0.5;
        private double SnapOffsetY = 4.5;

        private double SnapOffsetWidth = 1.0;
        private double SnapOffsetHeight = 1.0;

        #endregion

        #region Constructor

        public EllipseAdorner(UIElement adornedElement)
            : base(adornedElement)
        {
            visualCollection = new VisualCollection(this);

            thumb0 = new Thumb()
            {
                Template = Application.Current.Windows[0].Resources["ThumbEllipseKey"] as ControlTemplate,
                //Style = Application.Current.Windows[0].Resources["AdornerThumbStyleKey"] as Style,
                Cursor = Cursors.SizeWE
            };

            thumb1 = new Thumb()
            {
                Template = Application.Current.Windows[0].Resources["ThumbEllipseKey"] as ControlTemplate,
                //Style = Application.Current.Windows[0].Resources["AdornerThumbStyleKey"] as Style,
                Cursor = Cursors.SizeWE
            };

            thumb2 = new Thumb()
            {
                Template = Application.Current.Windows[0].Resources["ThumbEllipseKey"] as ControlTemplate,
                //Style = Application.Current.Windows[0].Resources["AdornerThumbStyleKey"] as Style,
                Cursor = Cursors.SizeNS
            };

            thumb3 = new Thumb()
            {
                Template = Application.Current.Windows[0].Resources["ThumbEllipseKey"] as ControlTemplate,
                //Style = Application.Current.Windows[0].Resources["AdornerThumbStyleKey"] as Style,
                Cursor = Cursors.SizeNS
            };

            thumb4 = new Thumb()
            {
                Template = Application.Current.Windows[0].Resources["ThumbTransparentKey"] as ControlTemplate,
                //Style = Application.Current.Windows[0].Resources["AdornerThumbStyleKey"] as Style,
                Cursor = Cursors.SizeAll
            };

            thumb0.DragDelta += thumb0_DragDelta;
            thumb1.DragDelta += thumb1_DragDelta;
            thumb2.DragDelta += thumb2_DragDelta;
            thumb3.DragDelta += thumb3_DragDelta;
            thumb4.DragDelta += thumb4_DragDelta;

            visualCollection.Add(thumb4);

            visualCollection.Add(thumb0);
            visualCollection.Add(thumb1);
            visualCollection.Add(thumb2);
            visualCollection.Add(thumb3);
        }

        void thumb0_DragDelta(object sender, DragDeltaEventArgs e)
        {
            var ellipse = this.AdornedElement as Ellipse;
            double dX = e.HorizontalChange;
            double dY = e.VerticalChange;
            double x = Canvas.GetLeft(ellipse);
            double y = Canvas.GetTop(ellipse);
            double width = ellipse.Width;
            double height = ellipse.Height;

            width = Math.Max(0, width - dX);

            if (width > 0)
                width = EnableSnap == true ? SnapUtil.Snap(width, SnapX, SnapOffsetWidth) : width;

            x = EnableSnap == true ? SnapUtil.Snap(x + dX, SnapX, SnapOffsetX) : x + dX;

            if (width > 0)
                Canvas.SetLeft(ellipse, x);

            ellipse.Width = width;
        }

        void thumb1_DragDelta(object sender, DragDeltaEventArgs e)
        {
            var ellipse = this.AdornedElement as Ellipse;
            double dX = e.HorizontalChange;
            double dY = e.VerticalChange;
            double y = Canvas.GetTop(ellipse);
            double width = ellipse.Width;
            double height = ellipse.Height;

            width = Math.Max(0, width + dX);

            if (width > 0)
                width = EnableSnap == true ? SnapUtil.Snap(width, SnapX, SnapOffsetWidth) : width;

            ellipse.Width = width;
        }

        void thumb2_DragDelta(object sender, DragDeltaEventArgs e)
        {
            var ellipse = this.AdornedElement as Ellipse;
            double dX = e.HorizontalChange;
            double dY = e.VerticalChange;
            double y = Canvas.GetTop(ellipse);
            double width = ellipse.Width;
            double height = ellipse.Height;

            height = Math.Max(0, height - dY);

            if (height > 0)
                height = EnableSnap == true ? SnapUtil.Snap(height, SnapY, SnapOffsetHeight) : height;

            y = EnableSnap == true ? SnapUtil.Snap(y + dY, SnapY, SnapOffsetY) : y + dY;

            if (height > 0)
                Canvas.SetTop(ellipse, y);

            ellipse.Height = height;
        }

        void thumb3_DragDelta(object sender, DragDeltaEventArgs e)
        {
            var ellipse = this.AdornedElement as Ellipse;
            double dX = e.HorizontalChange;
            double dY = e.VerticalChange;
            double width = ellipse.Width;
            double height = ellipse.Height;

            height = Math.Max(0, height + dY);

            if (height > 0)
                height = EnableSnap == true ? SnapUtil.Snap(height, SnapY, SnapOffsetHeight) : height;

            ellipse.Height = height;
        }

        void thumb4_DragDelta(object sender, DragDeltaEventArgs e)
        {
            var ellipse = this.AdornedElement as Ellipse;
            double dX = e.HorizontalChange;
            double dY = e.VerticalChange;
            double x = Canvas.GetLeft(ellipse);
            double y = Canvas.GetTop(ellipse);

            x = EnableSnap == true ? SnapUtil.Snap(x + dX, SnapX, SnapOffsetX) : x + dX;
            y = EnableSnap == true ? SnapUtil.Snap(y + dY, SnapY, SnapOffsetY) : y + dY;

            Canvas.SetLeft(ellipse, x);
            Canvas.SetTop(ellipse, y);
        }

        protected override Visual GetVisualChild(int index)
        {
            return visualCollection[index];
        }

        protected override int VisualChildrenCount
        {
            get
            {
                return visualCollection.Count;
            }
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            var ellipse = this.AdornedElement as Ellipse;

            double x = 0;
            double y = 0;
            double width = ellipse.Width;
            double height = ellipse.Height;
            double offset = size / 2;

            thumb0.Arrange(new Rect(x - offset, (height / 2) - offset, size, size));
            thumb1.Arrange(new Rect(width - offset, (height / 2) - offset, size, size));
            thumb2.Arrange(new Rect((width / 2) - offset, y - offset, size, size));
            thumb3.Arrange(new Rect((width / 2) - offset, y + height - offset, size, size));
            thumb4.Arrange(new Rect(x, y, width, height));

            return finalSize;
        }

        #endregion
    }

    #endregion

    #region MainWindow

    public partial class MainWindow : Window
    {
        #region Fields

        private string appTitle = "Template Creator";

        private bool showElementAdorners = false;

        private bool snapWhenCreating = true;
        private bool snapWhenMoving = true;

        private bool enableFill = false;

        private double strokeThickness = 1.0;

        private Tool tool = Tool.Line;

        private Line OriginaLine = null;

        private double snapX = 15;
        private double snapY = 15;
        private double snapOffsetX = 0;
        private double snapOffsetY = 5;

        private double gridOffsetX = 0;
        private double gridOffsetY = 5;

        private Stack<string> undo = new Stack<string>();
        private Stack<string> redo = new Stack<string>();

        private LineGuidesAdorner adorner = null;

        #endregion

        #region Constructor

        public MainWindow()
        {
            InitializeComponent();

            canvas.MouseLeftButtonDown += Canvas_MouseLeftButtonDown;
            canvas.MouseRightButtonDown += Canvas_MouseRightButtonDown;
            canvas.MouseMove += Canvas_MouseMove;

            this.PreviewKeyDown += MainWindow_PreviewKeyDown;

            //GridGenerate(PathGrid, 330, 35, 600, 750, 30);
            GridGenerate(PathGrid, gridOffsetX, gridOffsetY, 1260, 891 - gridOffsetY - 16, 30);

            TextSnapX.Text = snapX.ToString();
            TextSnapY.Text = snapY.ToString();
            TextSnapOffsetX.Text = snapOffsetX.ToString();
            TextSnapOffsetY.Text = snapOffsetY.ToString();
        }

        #endregion

        #region Grid

        public static string GridGenerate(double originX, double originY, double width, double height, double size)
        {
            var sb = new StringBuilder();

            double sizeX = size;
            double sizeY = size;

            // horizontal lines
            for (double y = sizeY + originY; y < height + originY; y += size)
            {
                sb.AppendFormat("M{0},{1}", DoubleToString(originX), DoubleToString(y));
                sb.AppendFormat("L{0},{1}", DoubleToString(width + originX), DoubleToString(y));
            }

            // vertical lines
            for (double x = sizeX + originX; x < width + originX; x += size)
            {
                sb.AppendFormat("M{0},{1}", DoubleToString(x), DoubleToString(originY));
                sb.AppendFormat("L{0},{1}", DoubleToString(x), DoubleToString(height + originY));
            }

            return sb.ToString();
        }

        public static void GridGenerate(Path path, double originX, double originY, double width, double height, double size)
        {
            string grid = GridGenerate(originX, originY, width, height, size);
            path.Data = Geometry.Parse(grid);
        }

        #endregion

        #region Keyboard Events

        private void MainWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.OriginalSource is TextBox)
                return;

            HanleKey(e.Key);
        }

        private void HanleKey(Key key)
        {
            bool isControl = (Keyboard.Modifiers & ModifierKeys.Control) > 0;

            if (isControl)
            {
                switch (key)
                {
                    // open
                    case Key.O:
                        {
                            Open();
                        }
                        break;

                    // save
                    case Key.S:
                        {
                            Save();
                        }
                        break;

                    // undo
                    case Key.Z:
                        if (canvas.IsMouseCaptured == false)
                        {
                            PopModel();
                        }
                        break;

                    // redo
                    case Key.Y:
                        if (canvas.IsMouseCaptured == false)
                        {
                            RollbackModel();
                        }
                        break;

                    // reset
                    case Key.R:
                        if (canvas.IsMouseCaptured == false)
                        {
                            PushModel();
                            canvas.Children.Clear();
                        }
                        break;
                }
            }
            else
            {
                switch (key)
                {
                    // line
                    case Key.L:
                        {
                            tool = Tool.Line;
                            if (adorner != null)
                                adorner.Tool = tool;
                        }
                        break;

                    // polyline
                    case Key.P:
                        {
                            tool = Tool.Polyline;
                            if (adorner != null)
                                adorner.Tool = tool;
                        }
                        break;

                    // path
                    case Key.A:
                        {
                            tool = Tool.Path;
                            if (adorner != null)
                                adorner.Tool = tool;
                        }
                        break;

                    // rect
                    case Key.R:
                        {
                            tool = Tool.Rect;
                            if (adorner != null)
                                adorner.Tool = tool;
                        }
                        break;

                    // circle
                    case Key.C:
                        {
                            tool = Tool.Circle;
                            if (adorner != null)
                                adorner.Tool = tool;
                        }
                        break;

                    // text
                    case Key.T:
                        {
                            tool = Tool.Text;
                            if (adorner != null)
                                adorner.Tool = tool;
                        }
                        break;

                    // move
                    case Key.V:
                        {
                            tool = Tool.Move;
                        }
                        break;

                    // scale
                    case Key.S:
                        {
                            tool = Tool.Scale;
                        }
                        break;

                    // rotate
                    case Key.E:
                        {
                            tool = Tool.Rotate;
                        }
                        break;

                    // duplicate
                    case Key.D:
                        {
                            tool = Tool.Duplicate;
                        }
                        break;

                    // snap
                    case Key.M:
                        {
                            snapWhenCreating = !snapWhenCreating;
                        }
                        break;

                    // fill
                    case Key.F:
                        {
                            enableFill = !enableFill;
                            if (adorner != null)
                                adorner.EnableFill = enableFill;
                        }
                        break;

                    // increase stroke thickness
                    case Key.OemPlus:
                    case Key.Add:
                        {
                            if (strokeThickness < 15)
                            {
                                strokeThickness += 1;
                                if (adorner != null)
                                    adorner.StrokeThickness = strokeThickness;
                            }
                        }
                        break;

                    // decrease stroke thickness
                    case Key.OemMinus:
                    case Key.Subtract:
                        {
                            if (strokeThickness > 1)
                            {
                                strokeThickness -= 1;
                                if (adorner != null)
                                    adorner.StrokeThickness = strokeThickness;
                            }
                        }
                        break;

                        // toggle element adorners
                    case Key.H:
                        {
                            showElementAdorners = !showElementAdorners;

                            if (showElementAdorners == true)
                            {
                                foreach (var element in canvas.Children)
                                {
                                    if (element is Line)
                                    {
                                        var line = element as Line;
                                        AddLineAdorner(line);
                                    }
                                    else if (element is Rectangle)
                                    {
                                        var rect = element as Rectangle;
                                        AddRectAdorner(rect);
                                    }
                                    else if (element is Ellipse)
                                    {
                                        var ellipse = element as Ellipse;
                                        AddEllipseAdorner(ellipse);
                                    }
                                }
                            }
                            else
                            {
                                foreach (var element in canvas.Children)
                                {
                                    RemoveAdorner(element as FrameworkElement);
                                }
                            }
                        }
                        break;
                }
            }
        }

        #endregion

        #region Canvas Events

        private void UpdateTitle(double x1, double y1, double x2, double y2)
        {
            double dy = y2 - y1;
            double dx = x2 - x1;
            double theta = Math.Atan2(dy, dx);
            theta *= 180 / Math.PI;

            this.Title = string.Format("{0} [x: {1}, y: {2}, dx: {3}, dy: {4}, theta: {5}]",
                appTitle,
                DoubleToString(Math.Round(x2, 1)),
                DoubleToString(Math.Round(y2, 1)),
                DoubleToString(Math.Round(dx, 1)),
                DoubleToString(Math.Round(dy, 1)),
                DoubleToString(Math.Round(theta, 1)));
        }

        private void Canvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (canvas.IsMouseCaptured)
            {
                this.Title = appTitle;

                canvas.ReleaseMouseCapture();

                if (OriginaLine != null)
                {
                    canvas.Children.Add(OriginaLine);
                    OriginaLine = null;
                }

                RemoveAdorner();
            }
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (canvas.IsMouseCaptured)
            {
                var p = e.GetPosition(canvas);

                double x = (snapWhenCreating == true && snapWhenMoving == true) ? SnapUtil.Snap(p.X, snapX, snapOffsetX) : p.X;
                double y = (snapWhenCreating == true && snapWhenMoving == true) ? SnapUtil.Snap(p.Y, snapY, snapOffsetY) : p.Y;

                UpdateTitle(adorner.X1, adorner.Y1, x, y);

                if (adorner.X != x)
                {
                    adorner.X = x;
                    adorner.X2 = x;
                }

                if (adorner.Y != y)
                {
                    adorner.Y = y;
                    adorner.Y2 = y;
                }
            }
            else
            {
                var p = e.GetPosition(canvas);

                double x = snapWhenCreating == true ? SnapUtil.Snap(p.X, snapX, snapOffsetX) : p.X;
                double y = snapWhenCreating == true ? SnapUtil.Snap(p.Y, snapY, snapOffsetY) : p.Y;

                UpdateTitle(x, y, x, y);
            }
        }

        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (canvas.IsMouseCaptured)
            {
                if (OriginaLine != null || tool != Tool.Polyline)
                    canvas.ReleaseMouseCapture();

                var p = e.GetPosition(canvas);

                PushModel();

                double x = snapWhenCreating == true ? SnapUtil.Snap(p.X, snapX, snapOffsetX) : p.X;
                double y = snapWhenCreating == true ? SnapUtil.Snap(p.Y, snapY, snapOffsetY) : p.Y;

                if (tool == Tool.Line || 
                    tool == Tool.Polyline ||
                    (tool == Tool.Scale && OriginaLine != null))
                {
                    if (((adorner.X1 == adorner.X2) && (adorner.Y1 == adorner.Y2)) == false)
                    {
                        var line = CreateLine(adorner.X1, adorner.Y1, x, y, enableFill, strokeThickness);
                        canvas.Children.Add(line);

                        if (showElementAdorners == true)
                            AddLineAdorner(line);
                    }
                }
                else if (tool == Tool.Rect)
                {
                    if (((adorner.X1 == adorner.X2) || (adorner.Y1 == adorner.Y2)) == false)
                    {
                        var p0 = new Point(adorner.X1, adorner.Y1); 
                        var p1 = new Point(x, y);
                        var r = new Rect(p0, p1);
                        r.Offset(-0.5, -0.5);
                        r.Width = r.Width + 1.0;
                        r.Height = r.Height + 1.0;

                        //var rect = CreateRect(adorner.X1, adorner.Y1, x, y, enableFill, strokeThickness);
                        var rect = CreateRect(r.Left, r.Top, r.Right, r.Bottom, enableFill, strokeThickness);
                        canvas.Children.Add(rect);

                        if (showElementAdorners == true)
                            AddRectAdorner(rect);
                    }
                }
                else if (tool == Tool.Circle)
                {
                    if (((adorner.X1 == adorner.X2) || (adorner.Y1 == adorner.Y2)) == false)
                    {
                        var p0 = new Point(adorner.X1, adorner.Y1);
                        var p1 = new Point(x, y);
                        var r = new Rect(p0, p1);
                        r.Offset(-0.5, -0.5);
                        r.Width = r.Width + 1.0;
                        r.Height = r.Height + 1.0;

                        //var circle = CreateCircle(adorner.X1, adorner.Y1, x, y, enableFill, strokeThickness);
                        var circle = CreateCircle(r.Left, r.Top, r.Right, r.Bottom, enableFill, strokeThickness);
                        canvas.Children.Add(circle);

                        if (showElementAdorners == true)
                            AddEllipseAdorner(circle);
                    }
                }

                if (OriginaLine == null && tool == Tool.Polyline)
                {
                    // create next line
                    adorner.X = x;
                    adorner.Y = y;
                    adorner.X1 = x;
                    adorner.Y1 = y;
                    adorner.X2 = x;
                    adorner.Y2 = y;
                }
                else
                {
                    this.Title = appTitle;

                    RemoveAdorner();
                }

                OriginaLine = null;
            }
            else
            {
                System.Diagnostics.Debug.Print("{0}", e.OriginalSource.GetType());

                if (tool == Tool.Scale)
                {
                    if (e.OriginalSource is Line)
                    {
                        OriginaLine = e.OriginalSource as Line;

                        canvas.Children.Remove(OriginaLine);

                        var p = e.GetPosition(canvas);
                        double x0 = snapWhenCreating == true ? SnapUtil.Snap(p.X, snapX, snapOffsetX) : p.X;
                        double y0 = snapWhenCreating == true ? SnapUtil.Snap(p.Y, snapY, snapOffsetY) : p.Y;

                        double x1 = OriginaLine.X1;
                        double y1 = OriginaLine.Y1;
                        double x2 = OriginaLine.X2;
                        double y2 = OriginaLine.Y2;

                        bool result = CompareP0DistanceToP1P2(x0, y0, x1, y1, x2, y2) == 1;

                        if (result)
                        {
                            AddAdorner(x1, y1, x0, x0, Tool.Line);
                            UpdateTitle(x1, y1, x0, x0);
                        }
                        else
                        {
                            AddAdorner(x2, y2, x0, x0, Tool.Line);
                            UpdateTitle(x2, y2, x0, x0);
                        }

                        canvas.CaptureMouse();
                    }
                }
                else
                {
                    var p = e.GetPosition(canvas);
                    double x = snapWhenCreating == true ? SnapUtil.Snap(p.X, snapX, snapOffsetX) : p.X;
                    double y = snapWhenCreating == true ? SnapUtil.Snap(p.Y, snapY, snapOffsetY) : p.Y;

                    AddAdorner(x, y, x, y, tool);

                    UpdateTitle(x, y, x, y);

                    canvas.CaptureMouse();
                }
            }
        }

        public static int CompareP0DistanceToP1P2(double p0x, double p0y, double p1x, double p1y, double p2x, double p2y)
        {
            double dx1 = p1x - p0x;
            double dy1 = p1y - p0y;

            double dx2 = p2x - p0x;
            double dy2 = p2y - p0y;

            double d1 = Math.Sqrt(dx1 * dx1 + dy1 * dy1);
            double d2 = Math.Sqrt(dx2 * dx2 + dy2 * dy2);

            return (d1 > d2) ? 1 : (d1 < d2) ? -1 : 0;
        }

        #endregion

        #region Adorner

        private void AddAdorner(double x1, double y1, double x2, double y2, Tool tool)
        {
            var adornerLayer = AdornerLayer.GetAdornerLayer(canvas);
            adorner = new LineGuidesAdorner(canvas);

            //RenderOptions.SetEdgeMode(adorner, EdgeMode.Aliased);
            adorner.SnapsToDevicePixels = true;
    
            adorner.StrokeThickness = strokeThickness;
            adorner.EnableFill = enableFill;
            adorner.Tool = tool;
            adorner.CanvasWidth = canvas.Width;
            adorner.CanvasHeight = canvas.Height;
            adorner.X = x2;
            adorner.Y = y2;
            adorner.X1 = x1;
            adorner.Y1 = y1;
            adorner.X2 = x2;
            adorner.Y2 = y2;

            adornerLayer.Add(adorner);
        }

        private void RemoveAdorner()
        {
            var adornerLayer = AdornerLayer.GetAdornerLayer(canvas);
            adornerLayer.Remove(adorner);
            adorner = null;
        }

        private void AddLineAdorner(Line line)
        {
            var adornerLayer = AdornerLayer.GetAdornerLayer(line);
            var adorner = new LineAdorner(line);

            //RenderOptions.SetEdgeMode(adorner, EdgeMode.Aliased);

            adornerLayer.Add(adorner);
        }

        private void AddRectAdorner(Rectangle rect)
        {
            var adornerLayer = AdornerLayer.GetAdornerLayer(rect);
            var adorner = new RectAdorner(rect);

            //RenderOptions.SetEdgeMode(adorner, EdgeMode.Aliased);

            adornerLayer.Add(adorner);
        }

        private void AddEllipseAdorner(Ellipse ellipse)
        {
            var adornerLayer = AdornerLayer.GetAdornerLayer(ellipse);
            var adorner = new EllipseAdorner(ellipse);

            //RenderOptions.SetEdgeMode(adorner, EdgeMode.Aliased);

            adornerLayer.Add(adorner);
        }

        private void RemoveAdorner(FrameworkElement element)
        {
            var adornerLayer = AdornerLayer.GetAdornerLayer(element);
            
            var adorners = adornerLayer.GetAdorners(element);

            if (adorners != null)
            {
                foreach(var adorner in adorners)
                {
                    adornerLayer.Remove(adorner);
                }
            }
        }

        #endregion

        #region Line

        private Line CreateLine(double x1, double y1, double x2, double y2, bool enableFill, double strokeThickness)
        {
            var line = new Line()
            {
                Stroke = Brushes.Black,
                StrokeThickness = strokeThickness,
                StrokeLineJoin = PenLineJoin.Miter,
                StrokeStartLineCap = PenLineCap.Flat,
                StrokeEndLineCap = PenLineCap.Flat,
                Fill = Brushes.Transparent
            };

            Panel.SetZIndex(line, 3);

            //RenderOptions.SetEdgeMode(line, EdgeMode.Aliased);
            line.SnapsToDevicePixels = true;

            line.X1 = x1;
            line.Y1 = y1;
            line.X2 = x2;
            line.Y2 = y2;

            return line;
        }

        #endregion

        #region Rect

        private Rectangle CreateRect(double x1, double y1, double x2, double y2, bool enableFill, double strokeThickness)
        {
            var rect = new Rectangle()
            {
                Stroke = Brushes.Black,
                StrokeThickness = enableFill == true ? 0.0 : strokeThickness,
                StrokeLineJoin = PenLineJoin.Miter,
                StrokeStartLineCap = PenLineCap.Square,
                StrokeEndLineCap = PenLineCap.Square,
                Fill = enableFill == true ? Brushes.Black : Brushes.Transparent,
            };

            Panel.SetZIndex(rect, 3);

            //RenderOptions.SetEdgeMode(rect, EdgeMode.Aliased);

            rect.SnapsToDevicePixels = true;

            var p0 = new Point(x1, y1);
            var p1 = new Point(x2, y2);
            var r = new Rect(p0, p1);

            rect.Width = r.Width;
            rect.Height = r.Height;

            Canvas.SetLeft(rect, r.Left);
            Canvas.SetTop(rect, r.Top);

            return rect;
        }

        #endregion

        #region Circle

        private Ellipse CreateCircle(double x1, double y1, double x2, double y2, bool enableFill, double strokeThickness)
        {
            var ellipse = new Ellipse()
            {
                Stroke = Brushes.Black,
                StrokeThickness = enableFill == true ? 0.0 : strokeThickness,
                StrokeLineJoin = PenLineJoin.Miter,
                StrokeStartLineCap = PenLineCap.Square,
                StrokeEndLineCap = PenLineCap.Square,
                Fill = enableFill == true ? Brushes.Black : Brushes.Transparent
            };

            Panel.SetZIndex(ellipse, 3);

            //RenderOptions.SetEdgeMode(ellipse, EdgeMode.Aliased);

            ellipse.SnapsToDevicePixels = true;

            var p0 = new Point(x1, y1);
            var p1 = new Point(x2, y2);
            var r = new Rect(p0, p1);

            ellipse.Width = r.Width;
            ellipse.Height = r.Height;

            Canvas.SetLeft(ellipse, r.Left);
            Canvas.SetTop(ellipse, r.Top);

            return ellipse;
        }

        #endregion

        #region Model

        private static string DoubleToString(double value)
        {
            return value.ToString(System.Globalization.CultureInfo.GetCultureInfo("en-GB"));
        }

        private string GenerateModel()
        {
            var children = canvas.Children;
            var sb = new StringBuilder();

            foreach (var child in children)
            {
                if (child is Line)
                {
                    var line = child as Line;

                    double x1 = line.X1;
                    double y1 = line.Y1;
                    double x2 = line.X2;
                    double y2 = line.Y2;

                    sb.AppendFormat("{0};{1};{2};{3};{4};{5};{6}{7}",
                        "line",
                        x1,
                        y1,
                        x2,
                        y2,
                        line.Fill == Brushes.Transparent ? "0" : "1",
                        line.StrokeThickness,
                        Environment.NewLine);
                }
                else if (child is Rectangle)
                {
                    var rect = child as Rectangle;

                    double x1 = Canvas.GetLeft(rect);
                    double y1 = Canvas.GetTop(rect);
                    double x2 = x1 + rect.Width;
                    double y2 = y1 + rect.Height;

                    sb.AppendFormat("{0};{1};{2};{3};{4};{5};{6}{7}",
                        "rect",
                        x1,
                        y1,
                        x2,
                        y2,
                        rect.Fill == Brushes.Transparent ? "0" : "1",
                        rect.StrokeThickness,
                        Environment.NewLine);
                }
                else if (child is Ellipse)
                {
                    var circle = child as Ellipse;

                    double x1 = Canvas.GetLeft(circle);
                    double y1 = Canvas.GetTop(circle);
                    double x2 = x1 + circle.Width;
                    double y2 = y1 + circle.Height;
                    
                    sb.AppendFormat("{0};{1};{2};{3};{4};{5};{6}{7}",
                        "circle",
                        x1,
                        y1,
                        x2,
                        y2,
                        circle.Fill == Brushes.Transparent ? "0" : "1",
                        circle.StrokeThickness,
                        Environment.NewLine);
                }
            }


            return sb.ToString();
        }

        private void ParseModel(string model)
        {
            var lines = model.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines)
            {
                var args = line.Split(';');

                if (args.Length == 7 &&
                    string.Compare(args[0], "line", StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    double x1 = double.Parse(args[1]);
                    double y1 = double.Parse(args[2]);
                    double x2 = double.Parse(args[3]);
                    double y2 = double.Parse(args[4]);
                    int fillFlag = int.Parse(args[5]);
                    bool enableFill = (fillFlag == 0) ? false : (fillFlag == 1 ? true : false);
                    double strokeThickness = double.Parse(args[6]);

                    var l = CreateLine(x1, y1, x2, y2, enableFill, strokeThickness);
                    canvas.Children.Add(l);

                    if (showElementAdorners == true)
                        AddLineAdorner(l);
                }
                else if (args.Length == 7 &&
                    string.Compare(args[0], "rect", StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    double x1 = double.Parse(args[1]);
                    double y1 = double.Parse(args[2]);
                    double x2 = double.Parse(args[3]);
                    double y2 = double.Parse(args[4]);
                    int fillFlag = int.Parse(args[5]);
                    bool enableFill = (fillFlag == 0) ? false : (fillFlag == 1 ? true : false);
                    double strokeThickness = double.Parse(args[6]);

                    var r = CreateRect(x1, y1, x2, y2, enableFill, strokeThickness);
                    canvas.Children.Add(r);

                    if (showElementAdorners == true)
                        AddRectAdorner(r);
                }
                else if (args.Length == 7 &&
                    string.Compare(args[0], "circle", StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    double x1 = double.Parse(args[1]);
                    double y1 = double.Parse(args[2]);
                    double x2 = double.Parse(args[3]);
                    double y2 = double.Parse(args[4]);
                    int fillFlag = int.Parse(args[5]);
                    bool enableFill = (fillFlag == 0) ? false : (fillFlag == 1 ? true : false);
                    double strokeThickness = double.Parse(args[6]);

                    var c = CreateCircle(x1, y1, x2, y2, enableFill, strokeThickness);
                    canvas.Children.Add(c);

                    if (showElementAdorners == true)
                        AddEllipseAdorner(c);
                }
            }
        }

        private void PushModel()
        {
            if (redo.Count > 0)
                redo.Clear();

            undo.Push(GenerateModel());
        }

        private void PopModel()
        {
            if (undo.Count <= 0)
                return;

            redo.Push(GenerateModel());
            canvas.Children.Clear();
            ParseModel(undo.Pop());
        }

        private void RollbackModel()
        {
            if (redo.Count <= 0)
                return;

            undo.Push(GenerateModel());
            canvas.Children.Clear();
            ParseModel(redo.Pop());
        }

        public void Open()
        {
            var dlg = new Microsoft.Win32.OpenFileDialog()
            {
                Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*",
                Title = "Open Model",
            };

            var result = dlg.ShowDialog();
            if (result.HasValue && result.Value == true)
            {
                using (var reader = new System.IO.StreamReader(dlg.FileName))
                {
                    var model = reader.ReadToEnd();

                    PushModel();
                    ParseModel(model);
                }
            }
        }

        public void Save()
        {
            var dlg = new Microsoft.Win32.SaveFileDialog()
            {
                Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*",
                Title = "Save Model",
                FileName = "model"
            };

            var result = dlg.ShowDialog();
            if (result.HasValue && result.Value == true)
            {
                using (var writer = new System.IO.StreamWriter(dlg.FileName))
                {
                    var model = GenerateModel();
                    writer.Write(model);
                }
            }
        }

        #endregion

        #region Zoom Slider

        private void Zoom_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (this.IsLoaded)
            {
                var x = this.Zoom.Value;

                double middle = 10;
                double y = 0;

                // x <= middle => y = x/middle
                if (x <= middle)
                {
                    y = x / middle;
                }
                // x > middle => y = x - (middle - 1)
                else if (x > middle)
                {
                    y = x - (middle - 1);
                }

                var m = new Matrix();
                m.Scale(y, y);
                var mt = new MatrixTransform(m);

                grid.LayoutTransform = mt;
            }
        } 

        #endregion

        #region Text Events

        private void TextSnapX_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.IsLoaded == false)
                return;

            double value;

            if (double.TryParse(TextSnapX.Text, out value))
            {
                snapX = value;
            }
        }

        private void TextSnapY_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.IsLoaded == false)
                return;

            double value;

            if (double.TryParse(TextSnapY.Text, out value))
            {
                snapY = value;
            }
        }

        private void TextSnapOffsetX_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.IsLoaded == false)
                return;

            double value;

            if (double.TryParse(TextSnapOffsetX.Text, out value))
            {
                snapOffsetX = value;
            }
        }

        private void TextSnapOffsetY_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.IsLoaded == false)
                return;

            double value;

            if (double.TryParse(TextSnapOffsetY.Text, out value))
            {
                snapOffsetY = value;
            }
        } 

        #endregion
    }

    #endregion
}
