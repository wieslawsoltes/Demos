// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace WireDiagram.Controls
{
    public class ZoomBorder : Border
    {
        #region Properties

        public double Minimum
        {
            get { return (double)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register("Minimum", typeof(double), typeof(ZoomBorder), new UIPropertyMetadata(0.1));

        public double Maximum
        {
            get { return (double)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register("Maximum", typeof(double), typeof(ZoomBorder), new UIPropertyMetadata(100.0));

        #endregion

        #region Override Properties

        private UIElement child = null;

        public override UIElement Child
        {
            get
            {
                return base.Child;
            }
            set
            {
                if (value != null && value != this.Child)
                {
                    this.Initialize(value);
                }

                base.Child = value;
            }
        }

        #endregion

        #region Methods

        public TranslateTransform GetTranslateTransform(UIElement element)
        {
            return (TranslateTransform)((TransformGroup)element.RenderTransform).Children.First(tr => tr is TranslateTransform);
        }

        public ScaleTransform GetScaleTransform(UIElement element)
        {
            return (ScaleTransform)((TransformGroup)element.RenderTransform).Children.First(tr => tr is ScaleTransform);
        }

        public void Initialize(UIElement element)
        {
            this.child = element;
            if (child != null)
            {
                TransformGroup group = new TransformGroup();

                ScaleTransform st = new ScaleTransform();
                group.Children.Add(st);

                TranslateTransform tt = new TranslateTransform();
                group.Children.Add(tt);

                child.RenderTransform = group;
                child.RenderTransformOrigin = new Point(0.0, 0.0);

                child.PreviewMouseWheel += new MouseWheelEventHandler(child_PreviewMouseWheel);
                child.MouseDown += new MouseButtonEventHandler(child_MouseDown);
                child.MouseUp += new MouseButtonEventHandler(child_MouseUp);
                child.MouseMove += new MouseEventHandler(child_MouseMove);
            }
        }

        public void Zoom(int delta, Point point)
        {
            if (child != null)
            {
                var st = GetScaleTransform(child);
                var tt = GetTranslateTransform(child);

                double val = Math.Round(Math.Min(st.ScaleX, st.ScaleY), 1);

                if (delta > 0)
                {
                    if (val < 1)
                        val += 0.1;
                    else if (val >= 1 && val < 2.2)
                        val += 0.4;
                    else if (val >= 2.2 && val < 4)
                        val += 0.6;
                    else if (val >= 4)
                        val += 0.8;
                }
                if (delta < 0)
                {
                    if (val <= 1)
                        val -= 0.1;
                    else if (val > 1 && val <= 2.2)
                        val -= 0.4;
                    else if (val > 2.2 && val <= 4)
                        val -= 0.6;
                    else if (val > 4)
                        val -= 0.8;
                }

                if (val >= Minimum && val <= Maximum)
                {
                    Point relative = point;
                    double abosuluteX;
                    double abosuluteY;

                    abosuluteX = relative.X * st.ScaleX + tt.X;
                    abosuluteY = relative.Y * st.ScaleY + tt.Y;

                    st.ScaleX = val;
                    st.ScaleY = val;

                    tt.X = abosuluteX - relative.X * st.ScaleX;
                    tt.Y = abosuluteY - relative.Y * st.ScaleY;

                    // update adorner positions
                    child.InvalidateArrange();
                }
            }
        }

        private Point panOrigin;
        private Point panStart;

        public void Pan(Point p)
        {
            if (child != null)
            {
                if (child.IsMouseCaptured)
                {
                    var tt = GetTranslateTransform(child);
                    Vector v = panStart - p;
                    tt.X = panOrigin.X - v.X;
                    tt.Y = panOrigin.Y - v.Y;

                    // update adorner positions
                    child.InvalidateArrange();
                }
            }
        }

        public void BeginPan(Point p)
        {
            if (child != null)
            {
                var tt = GetTranslateTransform(child);
                panStart = p;
                panOrigin = new Point(tt.X, tt.Y);
                this.Cursor = Cursors.Hand;

                child.CaptureMouse();
            }
        }

        public void EndPan()
        {
            if (child != null)
            {
                child.ReleaseMouseCapture();
                this.Cursor = Cursors.Arrow;
            }
        }

        public void Reset()
        {
            if (child != null)
            {
                // reset zoom
                var st = GetScaleTransform(child);
                st.ScaleX = 1.0;
                st.ScaleY = 1.0;

                // reset pan
                var tt = GetTranslateTransform(child);
                tt.X = 0.0;
                tt.Y = 0.0;

                // update adorner positions
                child.InvalidateArrange();
            }
        }

        #endregion

        #region Child Events

        void child_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            this.Zoom(e.Delta, e.GetPosition(child));
        }

        void child_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Middle)
            {
                BeginPan(e.GetPosition(this));
            }
        }

        void child_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Middle)
            {
                EndPan();
            }
        }

        void child_MouseMove(object sender, MouseEventArgs e)
        {
            Pan(e.GetPosition(this));
        }

        #endregion
    }
}
