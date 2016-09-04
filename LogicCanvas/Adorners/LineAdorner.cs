using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;
using System.Windows;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Controls;

using LogicCanvas.Model.Logic;
using LogicCanvas.Helpers;
using LogicCanvas.Extensions;
using System.Windows.Threading;
using System.Threading;
using LogicCanvas.CustomElements;

namespace LogicCanvas.Adorners
{
    public class LineAdorner : Adorner
    {
        public enum LineAdornerMode { Edit, Move }

        private LineAdornerMode mode = LineAdornerMode.Edit;
        public LineAdornerMode Mode
        {
            get { return mode; }
            set { mode = value; }
        }

        private double gridSize = UnitConverter.CmToDip(0.5);
        public double GridSize
        {
            get { return gridSize; }
            set { gridSize = value; }
        }

        private double gridOffsetLeft = UnitConverter.CmToDip(0.2);
        public double GridOffsetLeft
        {
            get { return gridOffsetLeft; }
            set { gridOffsetLeft = value; }
        }

        private double gridOffsetTop = UnitConverter.CmToDip(0.1);
        public double GridOffsetTop
        {
            get { return gridOffsetTop; }
            set { gridOffsetTop = value; }
        }

        private bool IsStartPoint = false;
        private bool IsControlModeOn = false;
        private Size size = new Size(12, 12);

        public LineAdorner(UIElement adornedElement) : base(adornedElement)
        {
            this.MouseLeftButtonDown += new MouseButtonEventHandler(LineAdorner_MouseLeftButtonDown);
            this.MouseLeftButtonUp += new MouseButtonEventHandler(LineAdorner_MouseLeftButtonUp);
            this.MouseMove += new MouseEventHandler(LineAdorner_MouseMove);
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Middle)
            {
                // don't process mouse middle button events in adorner (used for pan)
                IsHitTestVisible = false;
                e.Handled = true;
                Dispatcher.BeginInvoke(DispatcherPriority.Input,
                (ThreadStart)delegate()
                {
                    MouseButtonEventArgs args = new MouseButtonEventArgs(e.MouseDevice, e.Timestamp, e.ChangedButton);
                    args.RoutedEvent = Mouse.MouseDownEvent;
                    InputManager.Current.ProcessInput(args);
                    IsHitTestVisible = true;
                });
            }
            else
            {
                base.OnMouseDown(e);
            }
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Middle)
            {
                // don't process mouse middle button events in adorner (used for pan)
                IsHitTestVisible = false;
                e.Handled = true;
                Dispatcher.BeginInvoke(DispatcherPriority.Input,
                (ThreadStart)delegate()
                {
                    MouseButtonEventArgs args = new MouseButtonEventArgs(e.MouseDevice, e.Timestamp, e.ChangedButton);
                    args.RoutedEvent = Mouse.MouseUpEvent;
                    InputManager.Current.ProcessInput(args);
                    IsHitTestVisible = true;
                });
            }
            else
            {
                base.OnMouseUp(e);
            }
        }

        protected override void OnPreviewMouseWheel(MouseWheelEventArgs e)
        {
            // don't process mouse wheel events in adorner (used for zoom)
            IsHitTestVisible = false;
            e.Handled = true;
            Dispatcher.BeginInvoke(DispatcherPriority.Input,
            (ThreadStart)delegate()
            {
                MouseWheelEventArgs args = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);
                args.RoutedEvent = Mouse.PreviewMouseWheelEvent;
                InputManager.Current.ProcessInput(args);
                IsHitTestVisible = true;
            });
            //base.OnPreviewMouseWheel(e);
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            // don't process mouse wheel events in adorner (used for zoom)
            IsHitTestVisible = false;
            e.Handled = true;
            Dispatcher.BeginInvoke(DispatcherPriority.Input,
            (ThreadStart)delegate()
            {
                MouseWheelEventArgs args = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);
                args.RoutedEvent = Mouse.MouseWheelEvent;
                InputManager.Current.ProcessInput(args);
                IsHitTestVisible = true;
            });
            //base.OnMouseWheel(e);
        }

        public static T FindVisualParent<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);
            if (parentObject == null)
                return null;

            T parent = parentObject as T;
            if (parent != null)
            {
                return parent;
            }
            else
            {
                return FindVisualParent<T>(parentObject);
            }
        }

        LineLogicItem originalLineItem = null;

        void LineAdorner_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control)
                IsControlModeOn = true;

            CustomLineElement line = this.AdornedElement as CustomLineElement;
            LineLogicItem lineItem = line.DataContext as LineLogicItem;
            Point p = SnapGrid.Snap(e.GetPosition(line), GridSize, GridOffsetLeft, GridOffsetTop);

            double dStart = 0.0;
            double dEnd = 0.0;

            if (!this.IsMouseCaptured)
            {
                // copy original coordinates
                originalLineItem = new LineLogicItem()
                {
                    X1 = lineItem.X1,
                    Y1 = lineItem.Y1,
                    X2 = lineItem.X2,
                    Y2 = lineItem.Y2
                };

                // calculate start/end point coordinates
                dStart = Math.Sqrt(Math.Pow(lineItem.X1 - p.X, 2) + Math.Pow(lineItem.Y1 - p.Y, 2));
                dEnd = Math.Sqrt(Math.Pow(lineItem.X2 - p.X, 2) + Math.Pow(lineItem.Y2 - p.Y, 2));
            }

            if (IsControlModeOn)
            {
                if (this.IsMouseCaptured)
                {
                    if (IsStartPoint)
                    {
                        lineItem.X1 = p.X;
                        lineItem.Y1 = p.Y;
                    }
                    else
                    {
                        lineItem.X2 = p.X;
                        lineItem.Y2 = p.Y;
                    }

                    this.InvalidateVisual();
                    this.ReleaseMouseCapture();

                    IsControlModeOn = false;
                }
                else
                {
                    IsStartPoint = dStart < dEnd ? true : false;
                    this.Cursor = Cursors.Hand;
                    this.InvalidateVisual();
                    this.CaptureMouse();
                }
            }
            else
            {
                if (!this.IsMouseCaptured)
                {
                    IsStartPoint = dStart < dEnd ? true : false;
                    this.Cursor = Cursors.Hand;
                    this.InvalidateVisual();
                    this.CaptureMouse();
                }
            }
        }

        void LineAdorner_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!IsControlModeOn)
            {
                if (this.IsMouseCaptured)
                {
                    CustomLineElement line = this.AdornedElement as CustomLineElement;
                    LineLogicItem lineItem = line.DataContext as LineLogicItem;
                    Point p = SnapGrid.Snap(e.GetPosition(line), gridSize, GridOffsetLeft, GridOffsetTop);

                    if (IsStartPoint)
                    {
                        lineItem.X1 = p.X;
                        lineItem.Y1 = p.Y;
                    }
                    else
                    {
                        lineItem.X2 = p.X;
                        lineItem.Y2 = p.Y;
                    }

                    // update line item modification flag
                    //if (originalLineItem.CheckForModification(lineItem))
                    //    lineItem.IsModified = true;

                    this.InvalidateVisual();
                    this.ReleaseMouseCapture();
                    this.Cursor = Cursors.Arrow;
                }
            }
        }

        void LineAdorner_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.IsMouseCaptured)
            {
                if (this.AdornedElement.GetType() == typeof(CustomLineElement))
                {
                    CustomLineElement line = this.AdornedElement as CustomLineElement;
                    LineLogicItem lineItem = line.DataContext as LineLogicItem;

                    Point p = SnapGrid.Snap(e.GetPosition(line), gridSize, GridOffsetLeft, GridOffsetTop);

                    // edit mode: move line start point OR move line end point
                    if(mode == LineAdornerMode.Edit)
                    {
                        if (IsStartPoint)
                        {
                            lineItem.X1 = p.X;
                            lineItem.Y1 = p.Y;
                        }
                        else
                        {
                            lineItem.X2 = p.X;
                            lineItem.Y2 = p.Y;
                        }
                    }

                    // move mode: move line start point AND move line end point

                    if (mode == LineAdornerMode.Move)
                    {
                        if (IsStartPoint)
                        {
                            double dX = lineItem.X1 - p.X;
                            double dY = lineItem.Y1 - p.Y;
                            Point pEnd = new Point(lineItem.X2 - dX, lineItem.Y2 - dY);

                            lineItem.X1 = p.X;
                            lineItem.Y1 = p.Y;
                            lineItem.X2 = pEnd.X;
                            lineItem.Y2 = pEnd.Y;
                        }
                        else
                        {
                            double dX = lineItem.X2 - p.X;
                            double dY = lineItem.Y2 - p.Y;
                            Point pStart = new Point(lineItem.X1 - dX, lineItem.Y1 - dY);

                            lineItem.X2 = p.X;
                            lineItem.Y2 = p.Y;
                            lineItem.X1 = pStart.X;
                            lineItem.Y1 = pStart.Y;
                        }
                    }

                    this.InvalidateVisual();
                }
            }
        }

        static SolidColorBrush brush = new SolidColorBrush(Color.FromArgb(200, 255, 255, 255));
        static Pen pen = new Pen(new SolidColorBrush(Color.FromArgb(200, 0, 0, 0)), 1.0);
        //static Pen penGuide = new Pen(new SolidColorBrush(Color.FromArgb(128, 255, 127, 0)), 1.0);

        protected override void OnRender(DrawingContext drawingContext)
        {
            if (this.AdornedElement.GetType() == typeof(CustomLineElement))
            {
                CustomLineElement line = this.AdornedElement as CustomLineElement;
                LineLogicItem lineItem = line.DataContext as LineLogicItem;

                Canvas c = FindVisualParent<Canvas>(line);
                double w = c.Width;
                double h = c.Height;

                // draw guide lines
                if (this.IsMouseCaptured)
                {
                    /*
                    double halfPenWidthGuide = penGuide.Thickness / 2;

                    // horizontal guide
                    Point h0 = new Point(0, IsStartPoint ? lineItem.Y1 : lineItem.Y2);
                    Point h1 = new Point(w, IsStartPoint ? lineItem.Y1 : lineItem.Y2);

                    // vertical guide
                    Point v0 = new Point(IsStartPoint ? lineItem.X1 : lineItem.X2, 0);
                    Point v1 = new Point(IsStartPoint ? lineItem.X1 : lineItem.X2, h);

                    // draw lines
                    GuidelineSet gg1 = new GuidelineSet();
                    gg1.GuidelinesX.Add(h0.X + halfPenWidthGuide);
                    gg1.GuidelinesX.Add(h1.X + halfPenWidthGuide);
                    gg1.GuidelinesY.Add(h0.Y + halfPenWidthGuide);
                    gg1.GuidelinesY.Add(h1.Y + halfPenWidthGuide);
                    drawingContext.PushGuidelineSet(gg1);

                    drawingContext.DrawLine(penGuide, h0, h1);
                    drawingContext.Pop();

                    GuidelineSet gg2 = new GuidelineSet();
                    gg2.GuidelinesX.Add(v0.X + halfPenWidthGuide);
                    gg2.GuidelinesX.Add(v1.X + halfPenWidthGuide);
                    gg2.GuidelinesY.Add(v0.Y + halfPenWidthGuide);
                    gg2.GuidelinesY.Add(v1.Y + halfPenWidthGuide);
                    drawingContext.PushGuidelineSet(gg2);

                    drawingContext.DrawLine(penGuide, v0, v1);
                    drawingContext.Pop();
                    */
                }

                double halfPenWidth = pen.Thickness / 2;

                //Point p1 = new Point(lineItem.X1, lineItem.Y1);
                //Point p2 = new Point(lineItem.X2, lineItem.Y2);

                Point p1 = new Point(line.X1, line.Y1);
                Point p2 = new Point(line.X2, line.Y2);

                p1.Offset(-size.Width / 2, -size.Height / 2);
                p2.Offset(-size.Width / 2, -size.Height / 2);

                Rect r1 = new Rect(p1, size);
                Rect r2 = new Rect(p2, size);

                GuidelineSet g1 = new GuidelineSet();
                g1.GuidelinesX.Add(r1.Left + halfPenWidth);
                g1.GuidelinesX.Add(r1.Right + halfPenWidth);
                g1.GuidelinesY.Add(r1.Top + halfPenWidth);
                g1.GuidelinesY.Add(r1.Bottom + halfPenWidth);
                drawingContext.PushGuidelineSet(g1);

                drawingContext.DrawRectangle(brush, pen, r1);
                drawingContext.Pop();

                GuidelineSet g2 = new GuidelineSet();
                g2.GuidelinesX.Add(r2.Left + halfPenWidth);
                g2.GuidelinesX.Add(r2.Right + halfPenWidth);
                g2.GuidelinesY.Add(r2.Top + halfPenWidth);
                g2.GuidelinesY.Add(r2.Bottom + halfPenWidth);
                drawingContext.PushGuidelineSet(g2);

                drawingContext.DrawRectangle(brush, pen, r2);
                drawingContext.Pop();
            }

            base.OnRender(drawingContext);
        }

        public override GeneralTransform GetDesiredTransform(GeneralTransform transform)
        {
            return base.GetDesiredTransform(transform);
        }
    }
}
