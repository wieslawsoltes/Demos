using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.ComponentModel;

namespace LogicCanvas.CustomElements
{
    public class CustomLineElement : FrameworkElement
    {
        #region Dependency Properties

        public static readonly DependencyProperty StrokeProperty =
            DependencyProperty.Register("Stroke",
            typeof(Brush),
            typeof(CustomLineElement),
            new FrameworkPropertyMetadata(Brushes.Black, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

        public static readonly DependencyProperty ThicknessProperty =
            DependencyProperty.Register("Thickness",
            typeof(double),
            typeof(CustomLineElement),
            new FrameworkPropertyMetadata(0.05 * 96.0 / 2.54, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

        public static readonly DependencyProperty InvertedThicknessProperty =
            DependencyProperty.Register("InvertedThickness",
            typeof(double),
            typeof(CustomLineElement),
            new FrameworkPropertyMetadata(0.125 * 96.0 / 2.54, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

        public static readonly DependencyProperty X1Property =
            DependencyProperty.Register("X1",
            typeof(double),
            typeof(CustomLineElement),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

        public static readonly DependencyProperty Y1Property =
            DependencyProperty.Register("Y1",
            typeof(double),
            typeof(CustomLineElement),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

        public static readonly DependencyProperty X2Property =
            DependencyProperty.Register("X2",
            typeof(double),
            typeof(CustomLineElement),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

        public static readonly DependencyProperty Y2Property =
            DependencyProperty.Register("Y2",
            typeof(double),
            typeof(CustomLineElement),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

        public static readonly DependencyProperty IsStartInvertedProperty =
            DependencyProperty.Register("IsStartInverted",
            typeof(bool),
            typeof(CustomLineElement),
            new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

        public static readonly DependencyProperty IsEndInvertedProperty =
            DependencyProperty.Register("IsEndInverted",
            typeof(bool),
            typeof(CustomLineElement),
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

        #region Overrides

        protected override void OnRender(System.Windows.Media.DrawingContext drawingContext)
        {
            Pen pen = new Pen(Stroke, Thickness)
            {
                StartLineCap = PenLineCap.Round,
                EndLineCap = PenLineCap.Round
            };

            Point start = new Point(X1, Y1);
            Point end = new Point(X2, Y2);

            double alpha = Math.Atan2(start.Y - end.Y, end.X - start.X);
            double theta = Math.PI - alpha;
            double zet = theta - Math.PI / 2;
            //double headWidthY = Math.Sin(zet) * (2 + InvertedThickness);
            //double headWidthX = Math.Cos(zet) * (2 + InvertedThickness);
            double headHeightY = Math.Cos(zet) * (InvertedThickness + Thickness / 2);
            double headHeightX = Math.Sin(zet) * (InvertedThickness + Thickness / 2);

            if (IsStartInverted)
            {
                Point startCenter = new Point(start.X, start.Y);

                startCenter.X += headHeightX;
                startCenter.Y -= headHeightY;

                start.X += 2 * headHeightX;
                start.Y -= 2 * headHeightY;

                drawingContext.DrawEllipse(null, pen, new Point(startCenter.X, startCenter.Y), InvertedThickness, InvertedThickness);
            }

            if (IsEndInverted)
            {
                Point endCenter = new Point(end.X, end.Y);

                endCenter.X -= headHeightX;
                endCenter.Y += headHeightY;

                end.X -= 2 * headHeightX;
                end.Y += 2 * headHeightY;

                drawingContext.DrawEllipse(null, pen, new Point(endCenter.X, endCenter.Y), InvertedThickness, InvertedThickness);
            }

            drawingContext.DrawLine(pen, start, end);

            //System.Diagnostics.Debug.Print("[{0}] CustomLineElement OnRender", DateTime.Now);
        }

        #endregion
    }
}
