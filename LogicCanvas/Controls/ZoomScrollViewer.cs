using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LogicCanvas.Controls
{
    public class ZoomScrollViewer : ScrollViewer
    {
        private Point? LastCenterPositionOnTarget { get; set; }
        private Point? LastMousePositionOnTarget { get; set; }
        private Point? LastDragPoint { get; set; }

        public Boolean IsRightButtonAvailable
        {
            get { return (Boolean)this.GetValue(IsRightButtonAvailableProperty); }
            set { this.SetValue(IsRightButtonAvailableProperty, value); }
        }

        public Double Zoom
        {
            get { return (Double)this.GetValue(ZoomProperty); }
            set
            {
                var centerOfViewport = new Point(this.ViewportWidth / 2, this.ViewportHeight / 2);
                LastCenterPositionOnTarget = this.TranslatePoint(centerOfViewport, this.Content as UIElement);

                this.SetValue(ZoomProperty, value);
            }
        }

        public Double Minimum
        {
            get { return (Double)this.GetValue(MinimumProperty); }
            set { this.SetValue(MinimumProperty, value); }
        }

        public Double Maximum
        {
            get { return (Double)this.GetValue(MaximumProperty); }
            set { this.SetValue(MaximumProperty, value); }
        }

        public static readonly DependencyProperty IsRightButtonAvailableProperty = DependencyProperty.Register(
          "IsRightButtonAvailable", typeof(Boolean), typeof(ZoomScrollViewer), new PropertyMetadata(false));

        public static readonly DependencyProperty ZoomProperty = DependencyProperty.Register(
          "Zoom", typeof(Double), typeof(ZoomScrollViewer), new PropertyMetadata(1.0));

        public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register(
          "Minimum", typeof(Double), typeof(ZoomScrollViewer), new PropertyMetadata(0.1));

        public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register(
          "Maximum", typeof(Double), typeof(ZoomScrollViewer), new PropertyMetadata(16.0));

        public void ResetZoom()
        {
            Zoom = 1.0;
        }

        private void PanMouseUp(System.Windows.Input.MouseButtonEventArgs e)
        {
            this.Cursor = Cursors.Arrow;
            this.ReleaseMouseCapture();
            LastDragPoint = null;
        }

        private void PanMouseDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            var mousePos = e.GetPosition(this);

            if (mousePos.X <= this.ViewportWidth && mousePos.Y < this.ViewportHeight)
            {
                this.Cursor = Cursors.SizeAll;
                LastDragPoint = mousePos;
                Mouse.Capture(this);
            }
        }

        protected override void OnPreviewMouseWheel(System.Windows.Input.MouseWheelEventArgs e)
        {
            // TIP: to use scroll instead of default Zoom feature just hold 'Control' key and scroll Mouse Wheel
            if (Keyboard.Modifiers != ModifierKeys.Control)
            {
                LastMousePositionOnTarget = Mouse.GetPosition(this.Content as IInputElement);
                double val = Math.Round(Zoom, 1);

                if (e.Delta > 0)
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
                if (e.Delta < 0)
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
                    Zoom = val;

                e.Handled = true;
            }
            else
            {
                base.OnPreviewMouseWheel(e);
            }
        }

        protected override void OnScrollChanged(ScrollChangedEventArgs e)
        {
            if (e.ExtentHeightChange != 0 || e.ExtentWidthChange != 0)
            {
                Point? targetBefore = null;
                Point? targetNow = null;

                if (!LastMousePositionOnTarget.HasValue)
                {
                    if (LastCenterPositionOnTarget.HasValue)
                    {
                        var centerOfViewport = new Point(this.ViewportWidth / 2, this.ViewportHeight / 2);
                        Point centerOfTargetNow = this.TranslatePoint(centerOfViewport, this.Content as UIElement);
                        targetBefore = LastCenterPositionOnTarget;
                        targetNow = centerOfTargetNow;
                    }
                }
                else
                {
                    targetBefore = LastMousePositionOnTarget;
                    targetNow = Mouse.GetPosition(this.Content as IInputElement);
                    LastMousePositionOnTarget = null;
                }

                if (targetBefore.HasValue)
                {
                    var element = this.Content as FrameworkElement;

                    double dXInTargetPixels = targetNow.Value.X - targetBefore.Value.X;
                    double dYInTargetPixels = targetNow.Value.Y - targetBefore.Value.Y;
                    double multiplicatorX = e.ExtentWidth / element.ActualWidth;
                    double multiplicatorY = e.ExtentHeight / element.ActualHeight;
                    double newOffsetX = this.HorizontalOffset - dXInTargetPixels * multiplicatorX;
                    double newOffsetY = this.VerticalOffset - dYInTargetPixels * multiplicatorY;

                    if (double.IsNaN(newOffsetX) || double.IsNaN(newOffsetY))
                        return;

                    this.ScrollToHorizontalOffset(newOffsetX);
                    this.ScrollToVerticalOffset(newOffsetY);
                }

                e.Handled = true;
            }
            else
            {
                base.OnScrollChanged(e);
            }
        }
        protected override void OnPreviewMouseUp(MouseButtonEventArgs e)
        {
            // TIP: when right button is not available use middle button
            if (IsRightButtonAvailable == false && e.ChangedButton == MouseButton.Middle)
            {
                PanMouseUp(e);
                e.Handled = true;
            }
            else
            {
                base.OnPreviewMouseUp(e);
            } 
        }

        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            // TIP: when right button is not available use middle button
            if (IsRightButtonAvailable == false && e.ChangedButton == MouseButton.Middle)
            {
                PanMouseDown(e);
                e.Handled = true;
            }
            else
            {
                base.OnPreviewMouseDown(e);
            } 
        }

        protected override void OnPreviewMouseRightButtonUp(System.Windows.Input.MouseButtonEventArgs e)
        {
            if (IsRightButtonAvailable == true)
            {
                PanMouseUp(e);
                e.Handled = true;
            }
            else
            {
                base.OnPreviewMouseRightButtonUp(e);
            }
        }

        protected override void OnPreviewMouseRightButtonDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            if (IsRightButtonAvailable == true)
            {
                PanMouseDown(e);
                e.Handled = true;
            }
            else
            {
                base.OnPreviewMouseRightButtonDown(e);
            }
        }

        protected override void OnMouseMove(System.Windows.Input.MouseEventArgs e)
        {
            if (LastDragPoint.HasValue)
            {
                Point posNow = e.GetPosition(this);

                double dX = posNow.X - LastDragPoint.Value.X;
                double dY = posNow.Y - LastDragPoint.Value.Y;

                LastDragPoint = posNow;

                this.ScrollToHorizontalOffset(this.HorizontalOffset - dX);
                this.ScrollToVerticalOffset(this.VerticalOffset - dY);

                e.Handled = true;
            }
            else
            {
                base.OnMouseMove(e);
            }
        }
    }
}
