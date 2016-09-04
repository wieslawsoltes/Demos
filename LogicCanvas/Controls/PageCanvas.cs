using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Documents;

using LogicCanvas.Helpers;
using LogicCanvas.Adorners;
using LogicCanvas.Model.Logic;
using LogicCanvas.Model.Page;
using LogicCanvas.CustomElements;

namespace LogicCanvas.Controls
{
    class PageCanvas : Canvas
    {
        private double GridSize = UnitConverter.CmToDip(0.5);
        private double GridSizeCreate = UnitConverter.CmToDip(1.0);
        private double GridOffsetLeft = UnitConverter.CmToDip(0.2);
        private double GridOffsetTop = UnitConverter.CmToDip(0.1);

        private LineLogicItem currentLineItem = null;
        private CustomLineElement selectedLine = null;
        //private bool IsControlModeOn = false;

        Point GetClickLocation(Point p, double offsetLeft, double offsetTop)
        {
            return new Point()
            {
                X = SnapGrid.Snap(p.X, GridSizeCreate, offsetLeft),
                Y = SnapGrid.Snap(p.Y, GridSizeCreate, offsetTop)
            };
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (this.DataContext is PageItem)
            {
                //if (Keyboard.Modifiers == ModifierKeys.Control)
                //    IsControlModeOn = true;

                /*
                PageItem Page = this.DataContext as PageItem;
                Point p = GetClickLocation(e.GetPosition(this), GridOffsetLeft, GridOffsetTop);
                Page.Items.Add(new AndGateLogicItem()
                {
                    IsNew = true,
                    IsModified = false,
                    IsDeleted = false,
                    X = p.X,
                    Y = p.Y,
                    Z = 1
                });
                */

                PageItem Page = this.DataContext as PageItem;
                Page.SelectedItem = null;

                Point p = SnapGrid.Snap(e.GetPosition(this), GridSize, GridOffsetLeft, GridOffsetTop);
                if (this.IsMouseCaptured)
                {
                    //currentLineItem.X2 = p.X;
                    //currentLineItem.Y2 = p.Y;
                    //c.ReleaseMouseCapture();
                }
                else
                {
                    Point pHit = e.GetPosition(this);

                    CustomLineElement lineHit = null;
                    HitTestResult result = VisualTreeHelper.HitTest(this, pHit);
                    if (result != null)
                    {
                        if (result.VisualHit.GetType() == typeof(CustomLineElement))
                        {
                            lineHit = result.VisualHit as CustomLineElement;
                        }
                    }

                    if (lineHit == null && selectedLine == null)
                    {
                        currentLineItem = new LineLogicItem()
                        {
                            IsNew = false,
                            IsModified = false,
                            IsDeleted = false,
                            X1 = p.X, 
                            Y1 = p.Y, 
                            X2 = p.X, 
                            Y2 = p.Y, 
                            Z = 0
                        };

                        Page.Items.Add(currentLineItem);
                        this.CaptureMouse();
                    }

                    if (lineHit != selectedLine && selectedLine != null)
                    {
                        if (Keyboard.Modifiers != ModifierKeys.Control)
                        {
                            var LineAdornerLayer = AdornerLayer.GetAdornerLayer(selectedLine);
                            if (LineAdornerLayer != null)
                                LineAdornerLayer.Remove(LineAdornerLayer.GetAdorners(selectedLine)[0]);

                            selectedLine = null;
                        }
                    }

                    if (lineHit != null && lineHit != selectedLine)
                    {
                        selectedLine = lineHit;
                        var LineAdornerLayer = AdornerLayer.GetAdornerLayer(lineHit);

                        if (LineAdornerLayer != null)
                            LineAdornerLayer.Add(new LineAdorner(lineHit));
                    }

                    //currentLineItem = new LineLogicItem() { X1 = p.X, Y1 = p.Y, X2 = p.X, Y2 = p.Y, Z = 0 };
                    //Page.Items.Add(currentLineItem);
                    //c.CaptureMouse();
                }
            }
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (this.DataContext is PageItem)
            {
                PageItem Page = this.DataContext as PageItem;
                Point p = SnapGrid.Snap(e.GetPosition(this), GridSize, GridOffsetLeft, GridOffsetTop);

                if (this.IsMouseCaptured)
                {
                    if ((currentLineItem.X1 == p.X && currentLineItem.Y1 == p.Y) ||
                        (currentLineItem.X1 == currentLineItem.X2 && currentLineItem.Y1 == currentLineItem.Y2))
                    {
                        Page.Items.Remove(currentLineItem);
                        currentLineItem = null;
                        this.ReleaseMouseCapture();
                    }
                    else
                    {
                        currentLineItem.X2 = p.X;
                        currentLineItem.Y2 = p.Y;
                        this.ReleaseMouseCapture();
                    }
                }
            }
        }

        protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
        {
            if (this.DataContext is PageItem)
            {
                PageItem Page = this.DataContext as PageItem;
                Point p = SnapGrid.Snap(e.GetPosition(this), GridSize, GridOffsetLeft, GridOffsetTop);

                if (this.IsMouseCaptured)
                {
                    Page.Items.Remove(currentLineItem);
                    currentLineItem = null;
                    this.ReleaseMouseCapture();
                }
                else
                {
                    // delete line element
                    /*
                    Point pHit = e.GetPosition(this);

                    CustomLineElement lineHit = null;
                    HitTestResult result = VisualTreeHelper.HitTest(this, pHit);
                    if (result != null)
                    {
                        if (result.VisualHit.GetType() == typeof(CustomLineElement))
                        {
                            lineHit = result.VisualHit as CustomLineElement;
                            Page.Items.Remove(lineHit.DataContext as LineLogicItem);
                        }
                    }
                    */
                }

                /*
                Canvas c = sender as Canvas;
                Point p = SnapGrid.Snap(e.GetPosition(this), GridSize, GridOffsetLeft, GridOffsetTop);
                CustomLineElement lineHit = null;

                HitTestResult result = VisualTreeHelper.HitTest(c, p);
                if (result != null)
                {
                    if (result.VisualHit.GetType() == typeof(CustomLineElement))
                    {
                        lineHit = result.VisualHit as CustomLineElement;
                    }
                }

                if (selectedLine != null && lineHit != selectedLine)
                {
                    var LineAdornerLayer = AdornerLayer.GetAdornerLayer(selectedLine);
                    LineAdornerLayer.Remove(LineAdornerLayer.GetAdorners(selectedLine)[0]);
                    selectedLine = null;
                }

                if (lineHit != null && lineHit != selectedLine)
                {
                    selectedLine = lineHit;
                    var LineAdornerLayer = AdornerLayer.GetAdornerLayer(lineHit);
                    LineAdornerLayer.Add(new LineAdorner(lineHit));
                }
                */
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (this.DataContext is PageItem)
            {
                PageItem Page = this.DataContext as PageItem;
                Point p = SnapGrid.Snap(e.GetPosition(this), GridSize, GridOffsetLeft, GridOffsetTop);

                if (this.IsMouseCaptured)
                {
                    currentLineItem.X2 = p.X;
                    currentLineItem.Y2 = p.Y;
                }
            }
        }
    }
}
