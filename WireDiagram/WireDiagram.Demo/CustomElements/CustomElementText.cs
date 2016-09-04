// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.ComponentModel;

namespace WireDiagram.CustomElements
{
    public class CustomElementText : FrameworkElement
    {
        #region Dependency Properties

        public static readonly DependencyProperty ForegroundProperty =
            DependencyProperty.Register("Foreground",
            typeof(Brush),
            typeof(CustomElementText),
            new FrameworkPropertyMetadata(Brushes.Black,
                FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

        public static readonly DependencyProperty BackgroundProperty =
            DependencyProperty.Register("Background",
            typeof(Brush),
            typeof(CustomElementText),
            new FrameworkPropertyMetadata(Brushes.Transparent,
                FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text",
            typeof(string),
            typeof(CustomElementText),
            new FrameworkPropertyMetadata("",
                FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

        public static readonly DependencyProperty TypefaceNameProperty =
            DependencyProperty.Register("TypefaceName",
            typeof(string),
            typeof(CustomElementText),
            new FrameworkPropertyMetadata("Arial",
                FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

        public static readonly DependencyProperty EmSizeProperty =
            DependencyProperty.Register("EmSize",
            typeof(double),
            typeof(CustomElementText),
            new FrameworkPropertyMetadata(1.00 * 96.0 / 2.54,
                FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender),
                 new ValidateValueCallback(IsValidEmSize));

        public static bool IsValidEmSize(object value)
        {
            Double v = (Double)value;
            return (!v.Equals(Double.NegativeInfinity) && !v.Equals(Double.PositiveInfinity) && (v > 0.0));
        }

        public static readonly DependencyProperty HorizontalTextAlignmentProperty =
            DependencyProperty.Register("HorizontalTextAlignment",
            typeof(HorizontalAlignment),
            typeof(CustomElementText),
            new FrameworkPropertyMetadata(HorizontalAlignment.Left,
                FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

        public static readonly DependencyProperty VerticalTextAlignmentProperty =
            DependencyProperty.Register("VerticalTextAlignment",
            typeof(VerticalAlignment),
            typeof(CustomElementText),
            new FrameworkPropertyMetadata(VerticalAlignment.Top,
                FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

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

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public string TypefaceName
        {
            get { return (string)GetValue(TypefaceNameProperty); }
            set { SetValue(TypefaceNameProperty, value); }
        }

        [TypeConverter(typeof(LengthConverter))]
        public double EmSize
        {
            get { return (double)GetValue(EmSizeProperty); }
            set { SetValue(EmSizeProperty, value); }
        }

        public HorizontalAlignment HorizontalTextAlignment
        {
            get { return (HorizontalAlignment)GetValue(HorizontalTextAlignmentProperty); }
            set { SetValue(HorizontalAlignmentProperty, value); }
        }

        public VerticalAlignment VerticalTextAlignment
        {
            get { return (VerticalAlignment)GetValue(VerticalTextAlignmentProperty); }
            set { SetValue(VerticalTextAlignmentProperty, value); }
        }

        #endregion

        #region Overrides

        private FormattedText text = null;
        private string previousTypefaceName = string.Empty;
        private double previousEmSize = double.NaN;
        private string previousText = string.Empty;
        private Brush previousForeground = null;

        public CustomElementText() { }

        protected override void OnRender(DrawingContext drawingContext)
        {
            if (text == null || this.Text != this.previousText)
            {
                text = new FormattedText(this.Text,
                    System.Globalization.CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    new Typeface(this.TypefaceName),
                    EmSize,
                    this.Foreground);

                this.previousText = this.Text;
                this.previousTypefaceName = this.TypefaceName;
                this.previousEmSize = this.EmSize;
                this.previousForeground = this.Foreground;
            }
            else
            {
                if (this.previousTypefaceName != this.TypefaceName)
                {
                    text.SetFontFamily(this.TypefaceName);
                    this.previousTypefaceName = this.TypefaceName;
                }

                if (this.previousEmSize != this.EmSize)
                {
                    text.SetFontSize(this.EmSize);
                    this.previousEmSize = this.EmSize;
                }

                if (this.previousForeground != this.Foreground)
                {
                    text.SetForegroundBrush(this.Foreground);
                    this.previousForeground = this.Foreground;
                }
            }

            drawingContext.DrawRectangle(this.Background,
                null,
                new Rect(0.0, 0.0, this.ActualWidth, this.ActualHeight));

            drawingContext.DrawText(text, this.GetTextPosition());
        }

        #endregion

        #region Methods

        public Point GetTextPosition()
        {
            Point alignment = new Point();
            if (text == null)
                return alignment;

            switch (this.HorizontalTextAlignment)
            {
                case HorizontalAlignment.Left:
                    alignment.X = 0.0;
                    break;
                case HorizontalAlignment.Right:
                    alignment.X = this.ActualWidth - text.WidthIncludingTrailingWhitespace;
                    break;
                case HorizontalAlignment.Center:
                case HorizontalAlignment.Stretch:
                    alignment.X = (this.ActualWidth - text.WidthIncludingTrailingWhitespace) / 2.0;
                    break;
            }
            
            switch (this.VerticalTextAlignment)
            {
                case VerticalAlignment.Top:
                    alignment.Y = 0.0;
                    break;
                case VerticalAlignment.Bottom:
                    alignment.Y = this.ActualHeight - text.Height;
                    break;
                case VerticalAlignment.Center:
                case VerticalAlignment.Stretch:
                    alignment.Y = (this.ActualHeight - text.Height) / 2.0;
                    break;
            }

            return alignment;
        }

        public double GetTextWidth()
        {
            if (text != null)
                return text.Width;
            else
                return double.NaN;
        }

        public double GetTextHeight()
        {
            if (text != null)
                return text.Height;
            else
                return double.NaN;
        }

        #endregion
    }
}
