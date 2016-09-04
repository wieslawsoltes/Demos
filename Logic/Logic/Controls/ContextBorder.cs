// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Logic.Controls
{
    #region References

    using Logic.Model;
    using Logic.ViewModels.Core;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;

    #endregion

    #region Context Border

    public class ContextBorder : Border, IZoomService
    {
        #region Dependency Properties

        public Options Options
        {
            get { return (Options)GetValue(OptionsProperty); }
            set { SetValue(OptionsProperty, value); }
        }

        public static readonly DependencyProperty OptionsProperty =
            DependencyProperty.Register("Options", typeof(Options), typeof(ContextBorder), new PropertyMetadata(null));

        public double Minimum
        {
            get { return (double)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register("Minimum", typeof(double), typeof(ContextBorder), new PropertyMetadata(0.01));

        public double Maximum
        {
            get { return (double)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register("Maximum", typeof(double), typeof(ContextBorder), new PropertyMetadata(1000.0));

        #endregion

        #region Override Properties

        private UIElement child = null;

        public override UIElement Child
        {
            get { return base.Child; }
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

        #region Properties

        private Point panOrigin;
        private Point panStart;
        //private double zoomToFitTreshold = 0.1;

        #endregion

        #region Methods

        private TranslateTransform GetTranslateTransform(UIElement element)
        {
            return (TranslateTransform)((TransformGroup)element.RenderTransform).Children.First(tr => tr is TranslateTransform);
        }

        private ScaleTransform GetScaleTransform(UIElement element)
        {
            return (ScaleTransform)((TransformGroup)element.RenderTransform).Children.First(tr => tr is ScaleTransform);
        }

        private void Initialize(UIElement element)
        {
            child = element;

            if (child == null)
                return;

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

            this.SizeChanged += ContextBorder_SizeChanged;
        }

        private void ZoomToFit(Size viewport, Size source)
        {
            var st = GetScaleTransform(child);
            var tt = GetTranslateTransform(child);

            if (st == null || tt == null)
                return;

            // calculate zoom to fit canvas in border
            var scaleWidth = viewport.Width / source.Width;
            var scaleHeight = viewport.Height / source.Height;

            var zoom = Math.Min(scaleWidth, scaleHeight);

            double deltaWidthZoomed = viewport.Width - (source.Width * zoom);
            double deltaHeightZoomed = viewport.Height - (source.Height * zoom);

            double deltaWidth = viewport.Width - source.Width;
            double deltaHeight = viewport.Height - source.Height;

            double x = 0.0;
            double y = 0.0;

            // correct child position in border
            if (source.Width >= viewport.Width && source.Height >= viewport.Height)
            {
                x = scaleWidth > scaleHeight ? deltaWidthZoomed / 2.0 : 0.0;
                y = scaleWidth > scaleHeight ? 0.0 : deltaHeightZoomed / 2.0;
            }
            else
            {
                x = source.Width >= viewport.Width ? 0.0 : (deltaWidthZoomed - deltaWidth) / 2.0;
                y = source.Height >= viewport.Height ? 0.0 : (deltaHeightZoomed - deltaHeight) / 2.0;
            }

            st.ScaleX = zoom;
            st.ScaleY = zoom;
            tt.X = x;
            tt.Y = y;

            // update properties
            Options.Zoom = zoom;
            Options.Thickness = 1.0;
            Options.X = tt.X;
            Options.Y = tt.Y;

            // update adorner positions
            child.InvalidateArrange();
        }

        private void ZoomToPoint(int delta, Point point)
        {
            if (child == null)
                return;

            double zoom = Options.Zoom;
            double zoomSpeed = Options.ZoomSpeed;

            // calculate new zoom factor
            zoom = delta > 0 ? zoom + zoom / zoomSpeed : zoom - zoom / zoomSpeed;

            // zoom to fit if zoom delta <= zoom treshold

            //double zoomDelta = Math.Abs(zoom - 1.0);
            //if (zoomDelta <= zoomToFitTreshold)
            //{
            //    ZoomToFit();
            //    return;
            //}

            // get transforms
            var st = GetScaleTransform(child);
            var tt = GetTranslateTransform(child);

            // zoom to point
            if (zoom >= Minimum && zoom <= Maximum)
            {
                Point relative = point;

                double absoluteX = relative.X * st.ScaleX + tt.X;
                double absoluteY = relative.Y * st.ScaleY + tt.Y;

                st.ScaleX = zoom;
                st.ScaleY = zoom;

                tt.X = absoluteX - relative.X * st.ScaleX;
                tt.Y = absoluteY - relative.Y * st.ScaleY;

                // update properties
                Options.Zoom = zoom;
                Options.X = tt.X;
                Options.Y = tt.Y;

                // update adorner positions
                child.InvalidateArrange();
            }
        }

        private void BeginPan(Point p)
        {
            if (child == null)
                return;

            var tt = GetTranslateTransform(child);
            panStart = p;
            panOrigin = new Point(tt.X, tt.Y);
            this.Cursor = Cursors.Hand;

            child.CaptureMouse();
        }

        private void EndPan()
        {
            if (child == null)
                return;

            child.ReleaseMouseCapture();
            this.Cursor = Cursors.Arrow;
        }

        private void Pan(Point point)
        {
            if (child == null)
                return;

            if (child.IsMouseCaptured)
            {
                var tt = GetTranslateTransform(child);
                Vector v = panStart - point;
                tt.X = panOrigin.X - v.X;
                tt.Y = panOrigin.Y - v.Y;

                // update properties
                Options.X = tt.X;
                Options.Y = tt.Y;

                // update adorner positions
                child.InvalidateArrange();
            }
        }

        #endregion

        #region IZoomService Methods

        public void ZoomIn()
        {
            var element = child as FrameworkElement;
            if (element == null)
                return;

            var p = new Point(element.ActualWidth / 2.0, element.ActualHeight / 2.0);

            this.ZoomToPoint(1, p);
        }

        public void ZoomOut()
        {
            var element = child as FrameworkElement;
            if (element == null)
                return;

            var p = new Point(element.ActualWidth / 2.0, element.ActualHeight / 2.0);

            this.ZoomToPoint(-1, p);
        }

        public void ZoomToPoint(int delta, double x, double y)
        {
            ZoomToPoint(delta, new Point(x, y));
        }

        public void ZoomToFit()
        {
            var element = child as FrameworkElement;
            if (element == null)
                return;

            var source = new Size(1260, 891);
            var viewport = new Size(this.ActualWidth, this.ActualHeight);

            this.ZoomToFit(viewport, source);
        }

        public void Pan(double x, double y)
        {
            Pan(new Point(x, y));
        }

        public void Reset()
        {
            if (child == null)
                return;

            // reset zoom
            var st = GetScaleTransform(child);
            st.ScaleX = 1.0;
            st.ScaleY = 1.0;

            // reset pan
            var tt = GetTranslateTransform(child);
            tt.X = 0.0;
            tt.Y = 0.0;

            // update properties
            Options.Zoom = 1.0;
            Options.Thickness = 1.0;
            Options.X = tt.X;
            Options.Y = tt.Y;

            // update adorner positions
            child.InvalidateArrange();
        }

        #endregion

        #region Events

        void ContextBorder_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (Options == null)
                return;

            if (Options.IsAutoFitEnabled && this.child != null)
                this.ZoomToFit();
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            if (this.child != null)
                this.ZoomToPoint(e.Delta, e.GetPosition(child));

            base.OnMouseWheel(e);
        }

        void child_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (this.child != null)
                this.ZoomToPoint(e.Delta, e.GetPosition(child));

            e.Handled = true;
        }

        void child_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Right/Middle button click on canvas to pan 
            if (e.ChangedButton == MouseButton.Right || e.ChangedButton == MouseButton.Middle)
            {
                var element = e.OriginalSource as FrameworkElement;

                if (element.DataContext is Context || e.ChangedButton == MouseButton.Middle)
                {
                    BeginPan(e.GetPosition(this));

                    e.Handled = true;
                }
            }
        }

        void child_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Right || e.ChangedButton == MouseButton.Middle)
                EndPan();
        }

        void child_MouseMove(object sender, MouseEventArgs e)
        {
            Pan(e.GetPosition(this));
        }

        #endregion
    }

    #endregion
}
