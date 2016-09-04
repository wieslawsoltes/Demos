using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System;
using System.Diagnostics;

namespace LogicCanvas.Controls
{
    public class ZoomBorder : Border
    {
        private UIElement child = null;
        private Point origin;
        private Point start;

        public TranslateTransform GetTranslateTransform(UIElement element)
        {
            return (TranslateTransform)((TransformGroup)element.RenderTransform).Children.First(tr => tr is TranslateTransform);
        }

        public ScaleTransform GetScaleTransform(UIElement element)
        {
            return (ScaleTransform)((TransformGroup)element.RenderTransform).Children.First(tr => tr is ScaleTransform);
        }

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

                //child.MouseWheel += child_MouseWheel;
                child.PreviewMouseWheel += new MouseWheelEventHandler(child_PreviewMouseWheel);
                child.MouseLeftButtonDown += child_MouseLeftButtonDown;
                child.MouseLeftButtonUp += child_MouseLeftButtonUp;
                child.MouseDown += new MouseButtonEventHandler(child_MouseDown);
                child.MouseUp += new MouseButtonEventHandler(child_MouseUp);
                child.MouseMove += child_MouseMove;
                child.PreviewMouseRightButtonDown += new MouseButtonEventHandler(child_PreviewMouseRightButtonDown);
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

        #region Child Events

        double Minimum = 0.1;
        double Maximum = 100;

        private void Zoom(int delta, Point point)
        {
            if (child != null)
            {
                var st = GetScaleTransform(child);
                var tt = GetTranslateTransform(child);

                //double zoom = e.Delta > 0 ? .2 : -.2;
                //if (!(e.Delta > 0) && (st.ScaleX < .4 || st.ScaleY < .4))
                //    return;

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

        private void child_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            this.Zoom(e.Delta, e.GetPosition(child));
        }

        void child_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            this.Zoom(e.Delta, e.GetPosition(child));
        }

        void child_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Middle)
            {
                var tt = GetTranslateTransform(child);
                start = e.GetPosition(this);
                origin = new Point(tt.X, tt.Y);
                this.Cursor = Cursors.Hand;

                child.CaptureMouse();
            }
        }

        void child_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Middle)
            {
                child.ReleaseMouseCapture();
                this.Cursor = Cursors.Arrow;
            }
        }

        private void child_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (child != null)
            {
                //var tt = GetTranslateTransform(child);
                //start = e.GetPosition(this);
                //origin = new Point(tt.X, tt.Y);
                //this.Cursor = Cursors.Hand;

                //child.CaptureMouse();
            }
        }

        private void child_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (child != null)
            {
                //child.ReleaseMouseCapture();
                //this.Cursor = Cursors.Arrow;
            }
        }

        void child_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            //this.Reset();
        }

        private void child_MouseMove(object sender, MouseEventArgs e)
        {
            if (child != null)
            {
                if (child.IsMouseCaptured)
                {
                    var tt = GetTranslateTransform(child);
                    Vector v = start - e.GetPosition(this);
                    tt.X = origin.X - v.X;
                    tt.Y = origin.Y - v.Y;

                    // update adorner positions
                    child.InvalidateArrange();
                }
            }
        }

        #endregion
    }
}
