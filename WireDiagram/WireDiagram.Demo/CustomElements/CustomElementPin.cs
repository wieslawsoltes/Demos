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
    public class CustomElementPin : FrameworkElement
    {
        #region Dependency Properties

        public static readonly DependencyProperty ForegroundProperty =
            DependencyProperty.Register("Foreground",
            typeof(Brush),
            typeof(CustomElementPin),
            new FrameworkPropertyMetadata(Brushes.Black,
                FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

        public static readonly DependencyProperty BackgroundProperty =
            DependencyProperty.Register("Background",
            typeof(Brush),
            typeof(CustomElementPin),
            new FrameworkPropertyMetadata(Brushes.Black, 
                FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

        public static readonly DependencyProperty ThicknessProperty =
            DependencyProperty.Register("Thickness",
            typeof(double),
            typeof(CustomElementPin),
            new FrameworkPropertyMetadata(0.05 * 96.0 / 2.54, 
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

        [TypeConverter(typeof(LengthConverter))]
        public double Thickness
        {
            get { return (double)GetValue(ThicknessProperty); }
            set { SetValue(ThicknessProperty, value); }
        }

        #endregion

        #region Overrides

        private Brush previousForeground = null;
        private double previousThickness = double.NaN;
        private double penHalfThickness = double.NaN;
		private Pen pen = new Pen();
        private Rect rect = new Rect();

        public CustomElementPin() { }

        protected override void OnRender(System.Windows.Media.DrawingContext drawingContext)
        {
        	if(this.pen == null)
			{
	            pen = new Pen(Foreground, Thickness);

                previousForeground = Foreground;
                previousThickness = Thickness;

                penHalfThickness = pen.Thickness / 2.0;
			}
			else 
			{
				if(Foreground != previousForeground || Thickness != previousThickness)
				{
                    previousForeground = Foreground;
					previousThickness = Thickness;
					pen.Brush = Foreground;
					pen.Thickness = Thickness;

                    penHalfThickness = pen.Thickness / 2.0;
				}
			}

            rect.X = -this.ActualWidth / 2.0;
            rect.Y = -this.ActualHeight / 2.0;
            rect.Width = this.ActualWidth;
            rect.Height = this.ActualHeight;

            GuidelineSet g = new GuidelineSet();
            g.GuidelinesX.Add(rect.Left + penHalfThickness);
            g.GuidelinesX.Add(rect.Right + penHalfThickness);
            g.GuidelinesY.Add(rect.Top + penHalfThickness);
            g.GuidelinesY.Add(rect.Bottom + penHalfThickness);
            drawingContext.PushGuidelineSet(g);
            drawingContext.DrawRectangle(Background, pen, rect);
            drawingContext.Pop();

            //drawingContext.DrawRectangle(Background, pen, rect);
        }

        #endregion
    }
}
