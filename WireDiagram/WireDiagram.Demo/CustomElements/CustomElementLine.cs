// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using WireDiagram.Core.Model;

namespace WireDiagram.CustomElements
{
    public class CustomElementLine : FrameworkElement
    {
        #region Dependency Properties

        public static readonly DependencyProperty StrokeProperty =
            DependencyProperty.Register("Stroke",
            typeof(Brush),
            typeof(CustomElementLine),
            new FrameworkPropertyMetadata(Brushes.Black, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

        public static readonly DependencyProperty ThicknessProperty =
            DependencyProperty.Register("Thickness",
            typeof(double),
            typeof(CustomElementLine),
            new FrameworkPropertyMetadata(0.05 * 96.0 / 2.54, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

        public static readonly DependencyProperty InvertedThicknessProperty =
            DependencyProperty.Register("InvertedThickness", 
            typeof(double), 
            typeof(CustomElementLine),
            new FrameworkPropertyMetadata(0.125 * 96.0 / 2.54, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

        public static readonly DependencyProperty X1Property =
            DependencyProperty.Register("X1",
            typeof(double),
            typeof(CustomElementLine),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

        public static readonly DependencyProperty Y1Property =
            DependencyProperty.Register("Y1",
            typeof(double),
            typeof(CustomElementLine),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

        public static readonly DependencyProperty X2Property =
            DependencyProperty.Register("X2",
            typeof(double),
            typeof(CustomElementLine),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

        public static readonly DependencyProperty Y2Property =
            DependencyProperty.Register("Y2",
            typeof(double),
            typeof(CustomElementLine),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

        public static readonly DependencyProperty StartAlignmentProperty =
            DependencyProperty.Register("StartAlignment",
            typeof(PinAlignment),
            typeof(CustomElementLine),
            new FrameworkPropertyMetadata(PinAlignment.None, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

        public static readonly DependencyProperty EndAlignmentProperty =
            DependencyProperty.Register("EndAlignment",
            typeof(PinAlignment),
            typeof(CustomElementLine),
            new FrameworkPropertyMetadata(PinAlignment.None, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

        public static readonly DependencyProperty EnableLineBreakProperty =
            DependencyProperty.Register("EnableLineBreak", 
            typeof(bool),
            typeof(CustomElementLine),
            new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

        public static readonly DependencyProperty IsStartInvertedProperty =
            DependencyProperty.Register("IsStartInverted", 
            typeof(bool),
            typeof(CustomElementLine),
            new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

        public static readonly DependencyProperty IsEndInvertedProperty =
            DependencyProperty.Register("IsEndInverted",
            typeof(bool),
            typeof(CustomElementLine),
            new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

        #endregion

        #region Properties

        public Brush Stroke
        {
            get { return (Brush)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }

        [TypeConverter(typeof(LengthConverter))]
        public double Thickness
        {
            get { return (double)GetValue(ThicknessProperty); }
            set { SetValue(ThicknessProperty, value); }
        }

        [TypeConverter(typeof(LengthConverter))]
        public double InvertedThickness
        {
            get { return (double)GetValue(InvertedThicknessProperty); }
            set { SetValue(InvertedThicknessProperty, value); }
        }

        [TypeConverter(typeof(LengthConverter))]
        public double X1
        {
            get { return (double)GetValue(X1Property); }
            set { SetValue(X1Property, value); }
        }

        [TypeConverter(typeof(LengthConverter))]
        public double Y1
        {
            get { return (double)GetValue(Y1Property); }
            set { SetValue(Y1Property, value); }
        }

        [TypeConverter(typeof(LengthConverter))]
        public double X2
        {
            get { return (double)GetValue(X2Property); }
            set { SetValue(X2Property, value); }
        }

        [TypeConverter(typeof(LengthConverter))]
        public double Y2
        {
            get { return (double)GetValue(Y2Property); }
            set { SetValue(Y2Property, value); }
        }

        public PinAlignment StartAlignment
        {
            get { return (PinAlignment)GetValue(StartAlignmentProperty); }
            set { SetValue(StartAlignmentProperty, value); }
        }

        public PinAlignment EndAlignment
        {
            get { return (PinAlignment)GetValue(EndAlignmentProperty); }
            set { SetValue(EndAlignmentProperty, value); }
        }

        public bool EnableLineBreak
        {
            get { return (bool)GetValue(EnableLineBreakProperty); }
            set { SetValue(EnableLineBreakProperty, value); }
        }

        public bool IsStartInverted
        {
            get { return (bool)GetValue(IsStartInvertedProperty); }
            set { SetValue(IsStartInvertedProperty, value); }
        }

        public bool IsEndInverted
        {
            get { return (bool)GetValue(IsEndInvertedProperty); }
            set { SetValue(IsEndInvertedProperty, value); }
        }

        #endregion

        #region Fields

        private Pen pen;
        private Brush previousStroke;
        private double previousThickness;
        private double penHalfThickness;

        #endregion

        #region Render

        private void UpdatePen()
        {
            if (this.pen == null)
            {
                pen = new Pen(Stroke, Thickness)
                {
                    StartLineCap = PenLineCap.Round,
                    EndLineCap = PenLineCap.Round
                };
                previousStroke = Stroke;
                previousThickness = Thickness;
                penHalfThickness = pen.Thickness / 2.0;
            }
            else
            {
                if (Stroke != previousStroke || Thickness != previousThickness)
                {
                    previousStroke = Stroke;
                    previousThickness = Thickness;
                    pen.Brush = Stroke;
                    pen.Thickness = Thickness;
                    penHalfThickness = pen.Thickness / 2.0;
                }
            }
        }

        protected override void OnRender(DrawingContext dc)
        {
            UpdatePen();
            DrawLine(dc);
        }

        private void DrawLine(DrawingContext dc)
        {
            Point A = new Point(X1, Y1);
            Point B = new Point(X2, Y2);
            double deltaX;
            double deltaY;
            bool isX = (A.X == B.X);
            bool isY = (A.Y == B.Y);

            if (EnableLineBreak == true && isX == false && isY == false)
            {
                deltaX = B.X - A.X;
                deltaY = B.Y - A.Y;

                if ((StartAlignment == PinAlignment.Left || StartAlignment == PinAlignment.Right)
                    && (EndAlignment == PinAlignment.Left || EndAlignment == PinAlignment.Right))
                {
                    Point C = new Point(A.X + deltaX / 2.0, A.Y);
                    Point D = new Point(A.X + deltaX / 2.0, B.Y);

                    DrawLine(dc, A, C, IsStartInverted, false);
                    DrawLine(dc, C, D, false, false);
                    DrawLine(dc, D, B, false, IsEndInverted);
                }
                else if ((StartAlignment == PinAlignment.Left || StartAlignment == PinAlignment.Right)
                    && (EndAlignment == PinAlignment.Top || EndAlignment == PinAlignment.Bottom))
                {
                    Point C = new Point(A.X + deltaX, A.Y);

                    DrawLine(dc, A, C, IsStartInverted, false);
                    DrawLine(dc, C, B, false, IsEndInverted);
                }
                else if ((StartAlignment == PinAlignment.Top || StartAlignment == PinAlignment.Bottom)
                    && (EndAlignment == PinAlignment.Top || EndAlignment == PinAlignment.Bottom))
                {
                    Point C = new Point(A.X, A.Y + deltaY / 2.0);
                    Point D = new Point(B.X, A.Y + deltaY / 2.0);

                    DrawLine(dc, A, C, IsStartInverted, false);
                    DrawLine(dc, C, D, false, false);
                    DrawLine(dc, D, B, false, IsEndInverted);
                }
                else if ((StartAlignment == PinAlignment.Top || StartAlignment == PinAlignment.Bottom)
                    && (EndAlignment == PinAlignment.Left || EndAlignment == PinAlignment.Right))
                {
                    Point C = new Point(A.X, A.Y + deltaY);

                    DrawLine(dc, A, C, IsStartInverted, false);
                    DrawLine(dc, C, B, false, IsEndInverted);
                }
                else
                {
                    DrawLine(dc, A, B, IsStartInverted, IsEndInverted);
                }
            }
            else
            {
                DrawLine(dc, A, B, IsStartInverted, IsEndInverted);
            }
        }

        private void DrawLine(
            DrawingContext dc, 
            Point start, 
            Point end, 
            bool isStartInverted, 
            bool isEndInverted)
        {
            double alpha = Math.Atan2(start.Y - end.Y, end.X - start.X);
            double theta = Math.PI - alpha;
            double zet = theta - Math.PI / 2;
            double headHeightY = Math.Cos(zet) * (InvertedThickness + Thickness / 2);
            double headHeightX = Math.Sin(zet) * (InvertedThickness + Thickness / 2);

            GuidelineSet g = new GuidelineSet(
                new double[] 
                { 
                    start.X + penHalfThickness, 
                    end.X + penHalfThickness 
                }, 
                new double[] 
                { 
                    start.Y + penHalfThickness, 
                    end.Y + penHalfThickness
                });
            dc.PushGuidelineSet(g);

            if (isStartInverted == true)
            {
                Point startCenter = new Point(start.X + headHeightX, start.Y - headHeightY);

                start.X += 2 * headHeightX;
                start.Y -= 2 * headHeightY;

                dc.DrawEllipse(null, pen, startCenter, InvertedThickness, InvertedThickness);
            }

            if (isEndInverted == true)
            {
                Point endCenter = new Point(end.X - headHeightX, end.Y + headHeightY);

                end.X -= 2 * headHeightX;
                end.Y += 2 * headHeightY; 

                dc.DrawEllipse(null, pen, endCenter, InvertedThickness, InvertedThickness);
            }

            dc.DrawLine(pen, start, end);
            dc.Pop();
        }

        #endregion
    }
}
