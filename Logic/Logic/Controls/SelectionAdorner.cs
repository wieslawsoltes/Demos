// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Logic.Controls
{
    #region References

    using Logic.Model;
    using Logic.ViewModels;
    using Logic.Utilities;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Data;

    #endregion

    #region Selection Adorner

    public class SelectionAdorner : Adorner
    {
        #region Properties

        private Pen pen = null;

        private Brush stroke = new SolidColorBrush(Color.FromArgb(162, 51, 153, 255));
        public Brush Stroke
        {
            get { return stroke; }
            set { stroke = value; }
        }

        private double strokeThickness = 1.0;
        public double StrokeThickness
        {
            get { return strokeThickness; }
            set { strokeThickness = value; }
        }

        private Brush fill = new SolidColorBrush(Color.FromArgb(80, 168, 202, 236));
        public Brush Fill
        {
            get { return fill; }
            set { fill = value; }
        }

        private Point start = new Point();
        public Point Start
        {
            get { return start; }
            set { start = value; }
        }

        private Point end = new Point();
        public Point End
        {
            get { return end; }
            set { end = value; }
        }

        #endregion

        #region Constructor

        public SelectionAdorner(UIElement adornedElement)
            : base(adornedElement)
        {

        }

        #endregion

        #region Render Events

        protected override void OnRender(DrawingContext drawingContext)
        {
            // update stroke thickness
            var options = (this.DataContext as MainViewModel).Options;
            if (options != null)
                this.strokeThickness = options.Thickness;

            // update pen
            if (pen == null)
            {
                pen = new Pen(stroke, strokeThickness);
            }
            else
            {
                pen.Brush = stroke;
                pen.Thickness = strokeThickness;
            }

            // draw selection rectangle
            Rect rect = new Rect(start, end);
            double half = pen.Thickness / 2;

            GuidelineSet guidelines = new GuidelineSet();
            guidelines.GuidelinesX.Add(rect.Left + half);
            guidelines.GuidelinesX.Add(rect.Right + half);
            guidelines.GuidelinesY.Add(rect.Top + half);
            guidelines.GuidelinesY.Add(rect.Bottom + half);

            drawingContext.PushGuidelineSet(guidelines);
            drawingContext.DrawRectangle(fill, pen, rect);
            drawingContext.Pop();
        }

        #endregion
    }

    #endregion
}
