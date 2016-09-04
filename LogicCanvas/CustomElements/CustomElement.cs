using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.ComponentModel;

namespace LogicCanvas.CustomElements
{
    public class CustomElement : FrameworkElement
    {
        #region Dependency Properties

        public static readonly DependencyProperty ForegroundProperty =
            DependencyProperty.Register("Foreground",
            typeof(Brush),
            typeof(CustomElement),
            new FrameworkPropertyMetadata(Brushes.Black, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

        public static readonly DependencyProperty BackgroundProperty =
            DependencyProperty.Register("Background",
            typeof(Brush),
            typeof(CustomElement),
            new FrameworkPropertyMetadata(Brushes.White, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

        public static readonly DependencyProperty ThicknessProperty =
            DependencyProperty.Register("Thickness",
            typeof(double),
            typeof(CustomElement),
            new FrameworkPropertyMetadata(0.05 * 96.0 / 2.54, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

        public static readonly DependencyProperty FontSizeProperty =
            DependencyProperty.Register("FontSize",
            typeof(double),
            typeof(CustomElement),
            new FrameworkPropertyMetadata(0.5 * 96.0 / 2.54, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text",
            typeof(string),
            typeof(CustomElement),
            new FrameworkPropertyMetadata("≥1", FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

        #endregion

        #region Properties

        public Brush Foreground
        {
            get { return (Brush)GetValue(ForegroundProperty); }
            set { SetValue(ForegroundProperty, value); }
        }

        public Brush Background
        {
            get { return (Brush)GetValue(BackgroundProperty); }
            set { SetValue(BackgroundProperty, value); }
        }

        [TypeConverter(typeof(LengthConverter))]
        public double Thickness
        {
            get { return (double)GetValue(ThicknessProperty); }
            set { SetValue(ThicknessProperty, value); }
        }

        [TypeConverter(typeof(LengthConverter))]
        public double FontSize
        {
            get { return (double)GetValue(FontSizeProperty); }
            set { SetValue(FontSizeProperty, value); }
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        #endregion

        #region Overrides

        protected override void OnRender(System.Windows.Media.DrawingContext drawingContext)
        {
            Vector v = new Vector(ActualWidth, ActualHeight);
            Rect r = new Rect(new Point(0, 0), v);

            drawingContext.DrawRectangle(Background, new Pen(Foreground, Thickness), r);

            FormattedText ft = new FormattedText(Text, System.Globalization.CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface("Arial"), FontSize, Foreground, new NumberSubstitution())
            {
                TextAlignment = TextAlignment.Center,
            };

            Point p = new Point(v.X / 2, (v.Y / 2) - (FontSize / 2) - Thickness);

            drawingContext.DrawText(ft, p);

            //System.Diagnostics.Debug.Print("[{0}] OnRender Text: {1}", DateTime.Now, Text);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            //System.Diagnostics.Debug.Print("[{0}] MeasureOverride", DateTime.Now);

            return new Size(1.0 * 96.0 / 2.54, 1.0 * 96.0 / 2.54);
        }

        #endregion
    }
}
