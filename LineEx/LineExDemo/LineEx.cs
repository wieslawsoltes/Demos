﻿// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
#region References

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes; 

#endregion

namespace LineExDemo
{
    #region LineEx

    public class LineEx : Shape
    {
        #region Properties

        public double X1
        {
            get { return (double)GetValue(X1Property); }
            set { SetValue(X1Property, value); }
        }

        public static readonly DependencyProperty X1Property =
            DependencyProperty.Register("X1", typeof(double), typeof(LineEx),
            new FrameworkPropertyMetadata(0.0,
                FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

        public double Y1
        {
            get { return (double)GetValue(Y1Property); }
            set { SetValue(Y1Property, value); }
        }

        public static readonly DependencyProperty Y1Property =
            DependencyProperty.Register("Y1", typeof(double), typeof(LineEx),
            new FrameworkPropertyMetadata(0.0,
                FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

        public double X2
        {
            get { return (double)GetValue(X2Property); }
            set { SetValue(X2Property, value); }
        }

        public static readonly DependencyProperty X2Property =
            DependencyProperty.Register("X2", typeof(double), typeof(LineEx),
            new FrameworkPropertyMetadata(0.0,
                FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

        public double Y2
        {
            get { return (double)GetValue(Y2Property); }
            set { SetValue(Y2Property, value); }
        }

        public static readonly DependencyProperty Y2Property =
            DependencyProperty.Register("Y2", typeof(double), typeof(LineEx),
            new FrameworkPropertyMetadata(0.0,
                FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

        public double Radius
        {
            get { return (double)GetValue(RadiusProperty); }
            set { SetValue(RadiusProperty, value); }
        }

        public static readonly DependencyProperty RadiusProperty =
            DependencyProperty.Register("Radius", typeof(double), typeof(LineEx),
            new FrameworkPropertyMetadata(3.0,
                FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

        public bool IsStartVisible
        {
            get { return (bool)GetValue(IsStartVisibleProperty); }
            set { SetValue(IsStartVisibleProperty, value); }
        }

        public static readonly DependencyProperty IsStartVisibleProperty =
            DependencyProperty.Register("IsStartVisible", typeof(bool), typeof(LineEx),
            new FrameworkPropertyMetadata(false,
                FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

        public bool IsEndVisible
        {
            get { return (bool)GetValue(IsEndVisibleProperty); }
            set { SetValue(IsEndVisibleProperty, value); }
        }

        public static readonly DependencyProperty IsEndVisibleProperty =
            DependencyProperty.Register("IsEndVisible", typeof(bool), typeof(LineEx),
            new FrameworkPropertyMetadata(false,
                FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

        #endregion

        #region Calculate Size

        public static double CalculateZet(double startX, double startY, double endX, double endY)
        {
            double alpha = Math.Atan2(startY - endY, endX - startX);
            double theta = Math.PI - alpha;
            double zet = theta - Math.PI / 2;
            return zet;
        }

        public static double CalculateSizeX(double radius, double thickness, double zet)
        {
            double sizeX = Math.Sin(zet) * (radius + thickness);
            return sizeX;
        }

        public static double CalculateSizeY(double radius, double thickness, double zet)
        {
            double sizeY = Math.Cos(zet) * (radius + thickness);
            return sizeY;
        }

        #endregion

        #region Get Points

        public static Point GetLineStart(double startX, double startY, double sizeX, double sizeY, bool isStartVisible)
        {
            Point lineStart;

            if (isStartVisible)
            {
                double lx = startX + (2 * sizeX);
                double ly = startY - (2 * sizeY);

                lineStart = new Point(lx, ly);
            }
            else
            {
                lineStart = new Point(startX, startY);
            }

            return lineStart;
        }

        public static Point GetLineEnd(double endX, double endY, double sizeX, double sizeY, bool isEndVisible)
        {
            Point lineEnd;

            if (isEndVisible)
            {
                double lx = endX - (2 * sizeX);
                double ly = endY + (2 * sizeY);

                lineEnd = new Point(lx, ly);
            }
            else
            {
                lineEnd = new Point(endX, endY);
            }

            return lineEnd;
        }

        public static Point GetEllipseStartCenter(double startX, double startY, double sizeX, double sizeY, bool isStartVisible)
        {
            Point ellipseStartCenter;

            if (isStartVisible)
            {
                double ex = startX + sizeX;
                double ey = startY - sizeY;

                ellipseStartCenter = new Point(ex, ey);
            }
            else
            {
                ellipseStartCenter = new Point(startX, startY);
            }

            return ellipseStartCenter;
        }

        public static Point GetEllipseEndCenter(double endX, double endY, double sizeX, double sizeY, bool isEndVisible)
        {
            Point ellipseEndCenter;

            if (isEndVisible)
            {
                double ex = endX - sizeX;
                double ey = endY + sizeY;

                ellipseEndCenter = new Point(ex, ey);
            }
            else
            {
                ellipseEndCenter = new Point(endX, endY);
            }

            return ellipseEndCenter;
        }

        #endregion

        #region Get DefiningGeometry

        public double GetThickness()
        {
            return StrokeThickness / 2.0;
        }

        protected virtual Geometry GetDefiningGeometry()
        {
            bool isStartVisible = IsStartVisible;
            bool isEndVisible = IsEndVisible;

            double radius = Radius;
            double thickness = GetThickness();

            double startX = X1;
            double startY = Y1;
            double endX = X2;
            double endY = Y2;

            double zet = CalculateZet(startX, startY, endX, endY);
            double sizeX = CalculateSizeX(radius, thickness, zet);
            double sizeY = CalculateSizeY(radius, thickness, zet);

            Point ellipseStartCenter = GetEllipseStartCenter(startX, startY, sizeX, sizeY, isStartVisible);
            Point ellipseEndCenter = GetEllipseEndCenter(endX, endY, sizeX, sizeY, isEndVisible);
            Point lineStart = GetLineStart(startX, startY, sizeX, sizeY, isStartVisible);
            Point lineEnd = GetLineEnd(endX, endY, sizeX, sizeY, isEndVisible);

            var g = new GeometryGroup() { FillRule = FillRule.Nonzero };

            if (isStartVisible == true)
            {
                var startEllipse = new EllipseGeometry(ellipseStartCenter, radius, radius);
                g.Children.Add(startEllipse);
            }

            if (isEndVisible == true)
            {
                var endEllipse = new EllipseGeometry(ellipseEndCenter, radius, radius);
                g.Children.Add(endEllipse);
            }

            var line = new LineGeometry(lineStart, lineEnd);
            g.Children.Add(line);

            return g;
        }

        #endregion

        #region DefiningGeometry

        protected override Geometry DefiningGeometry
        {
            get
            {
                var g = GetDefiningGeometry();
                return g;
            }
        }

        #endregion
    }

    #endregion
}
