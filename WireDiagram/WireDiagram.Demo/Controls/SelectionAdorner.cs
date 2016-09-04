// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace WireDiagram.Controls
{
    public class SelectionAdorner : Adorner
    {
        private Brush stroke = new SolidColorBrush(Color.FromArgb(162, 0, 0, 255));
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

        private Brush fill = new SolidColorBrush(Color.FromArgb(50, 0, 0, 255));
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

        private Pen pen = null;

        public SelectionAdorner(UIElement adornedElement)
            : base(adornedElement)
        {
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            if (pen == null)
            {
                pen = new Pen(stroke, strokeThickness);
            }
            else
            {
                pen.Brush = stroke;
                pen.Thickness = strokeThickness;
            }

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
    }
}
