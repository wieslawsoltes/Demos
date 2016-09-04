// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace BlockDesigner
{
    #region References

    using System;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;

    #endregion

    #region ZoomBorder

    public class ZoomBorder : Border
    {
        #region Properties

        public override UIElement Child
        {
            get { return base.Child; }
            set
            {
                if (value != null && value != this.Child)
                    this.Initialize(value);
                base.Child = value;
            }
        }

        #endregion

        private Action resetZoom = null;
        public void ResetZoom()
        {
            if (resetZoom != null)
                resetZoom();
        }

        #region Initialize

        public void Initialize(UIElement element)
        {
            UIElement child = element;
            Point? origin = null;
            Point? start = null;

            if (child == null)
                return;

            TransformGroup group = new TransformGroup();
            ScaleTransform st = new ScaleTransform();
            group.Children.Add(st);

            TranslateTransform tt = new TranslateTransform();
            group.Children.Add(tt);

            child.RenderTransform = group;
            child.RenderTransformOrigin = new Point(0.0, 0.0);

            this.MouseWheel += (sender, e) =>
            {
                if (child == null)
                    return;

                double zoom = e.Delta > 0 ? .2 : -.2;
                if (!(e.Delta > 0) && (st.ScaleX < .4 || st.ScaleY < .4))
                    return;

                Point relative = e.GetPosition(child);
                double abosuluteX;
                double abosuluteY;

                abosuluteX = relative.X * st.ScaleX + tt.X;
                abosuluteY = relative.Y * st.ScaleY + tt.Y;

                st.ScaleX += zoom;
                st.ScaleY += zoom;

                tt.X = abosuluteX - relative.X * st.ScaleX;
                tt.Y = abosuluteY - relative.Y * st.ScaleY;
            };

            this.MouseLeftButtonDown += (sender, e) =>
            {
                if (child == null)
                    return;

                start = e.GetPosition(this);
                origin = new Point(tt.X, tt.Y);
                this.Cursor = Cursors.Hand;
                child.CaptureMouse();
            };

            this.MouseLeftButtonUp += (sender, e) =>
            {
                if (child == null)
                    return;

                child.ReleaseMouseCapture();
                this.Cursor = Cursors.Arrow;
            };

            this.MouseMove += (sender, e) =>
            {
                if (child == null || child.IsMouseCaptured != true)
                    return;

                if (start == null || start.HasValue == false)
                    return;

                Vector v = start.Value - e.GetPosition(this);
                tt.X = origin.Value.X - v.X;
                tt.Y = origin.Value.Y - v.Y;
            };

            this.resetZoom = () =>
            {
                if (child == null)
                    return;

                st.ScaleX = 1.0;
                st.ScaleY = 1.0;

                tt.X = 0.0;
                tt.Y = 0.0;
            };

            this.PreviewMouseRightButtonDown += (sender, e) =>
            {
                this.resetZoom();
            };
        }

        #endregion
    }

    #endregion
}
