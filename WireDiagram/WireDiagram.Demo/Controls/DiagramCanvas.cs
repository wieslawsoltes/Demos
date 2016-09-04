// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using WireDiagram.Core.Items;
using WireDiagram.Core.Lists;
using WireDiagram.Core.Model;

namespace WireDiagram.Controls
{
    public class DiagramCanvas : Canvas, INotifyPropertyChanged
    {
        #region Dependency Properties

        public Brush Stroke
        {
            get { return (Brush)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }

        public static readonly DependencyProperty StrokeProperty =
            DependencyProperty.Register("Stroke", typeof(Brush), typeof(DiagramCanvas), 
            new FrameworkPropertyMetadata(new SolidColorBrush(Color.FromArgb(162, 51, 153, 255)), FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

        public double StrokeThickness
        {
            get { return (double)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }

        public static readonly DependencyProperty StrokeThicknessProperty =
            DependencyProperty.Register("StrokeThickness", typeof(double), typeof(DiagramCanvas), 
            new FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

        public Brush Fill
        {
            get { return (Brush)GetValue(FillProperty); }
            set { SetValue(FillProperty, value); }
        }

        public static readonly DependencyProperty FillProperty =
            DependencyProperty.Register("Fill", typeof(Brush), typeof(DiagramCanvas), 
            new FrameworkPropertyMetadata(new SolidColorBrush(Color.FromArgb(80, 168, 202, 236)), FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

        public bool Preview
        {
            get { return (bool)GetValue(PreviewProperty); }
            set { SetValue(PreviewProperty, value); }
        }

        public static readonly DependencyProperty PreviewProperty =
            DependencyProperty.Register("Preview", typeof(bool), typeof(DiagramCanvas), 
            new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

        public bool IsBackgroundGridEnabled
        {
            get { return (bool)GetValue(IsBackgroundGridEnabledProperty); }
            set { SetValue(IsBackgroundGridEnabledProperty, value); }
        }

        public static readonly DependencyProperty IsBackgroundGridEnabledProperty =
            DependencyProperty.Register("IsBackgroundGridEnabled", typeof(bool), typeof(DiagramCanvas), 
            new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

        public Thickness BackgroundGridMargin
        {
            get { return (Thickness)GetValue(BackgroundGridMarginProperty); }
            set { SetValue(BackgroundGridMarginProperty, value); }
        }

        public static readonly DependencyProperty BackgroundGridMarginProperty =
            DependencyProperty.Register("BackgroundGridMargin", typeof(Thickness), typeof(DiagramCanvas), 
            new FrameworkPropertyMetadata(new Thickness(0.0, 0.0, 0.0, 0.0), FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

        [TypeConverter(typeof(LengthConverter))]
        public double BackgroundGridWidth
        {
            get { return (double)GetValue(BackgroundGridWidthProperty); }
            set { SetValue(BackgroundGridWidthProperty, value); }
        }

        public static readonly DependencyProperty BackgroundGridWidthProperty =
            DependencyProperty.Register("BackgroundGridWidth", typeof(double), typeof(DiagramCanvas),
            new FrameworkPropertyMetadata(1.0 * 96.0 / 2.54, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

        [TypeConverter(typeof(LengthConverter))]
        public double BackgroundGridHeight
        {
            get { return (double)GetValue(BackgroundGridHeightProperty); }
            set { SetValue(BackgroundGridHeightProperty, value); }
        }

        public static readonly DependencyProperty BackgroundGridHeightProperty =
            DependencyProperty.Register("BackgroundGridHeight", typeof(double), typeof(DiagramCanvas),
            new FrameworkPropertyMetadata(1.0 * 96.0 / 2.54, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

        #endregion

        #region Properties

        SelectionAdorner adorner = null;
        List<DependencyObject> hitResultsList = new List<DependencyObject>();
        List<DependencyObject> hitResultsTempList = new List<DependencyObject>();

        public IList<DependencyObject> Results
        {
            get
            {
                return hitResultsList;
            }
        }

        public int Count
        {
            get
            {
                if (this.IsMouseCaptured)
                {
                    return hitResultsTempList.Count;
                }
                else
                {
                    return hitResultsList.Count;
                }
            }
        }

        private bool haveStart = false;
        private Wire currentWire = null;
        private Pin startPin = null;

        public bool HaveStart
        {
            get
            {
                return haveStart;
            }
            set
            {
                if (value != haveStart)
                {
                    haveStart = value;
                    Notify("HaveStart");
                }
            }
        }

        public Wire CurrentWire
        {
            get
            {
                return currentWire;
            }
            set
            {
                if (value != currentWire)
                {
                    currentWire = value;
                    Notify("CurrentWire");
                }
            }
        }

        public Pin StartPin
        {
            get
            {
                return startPin;
            }
            set
            {
                if (value != startPin)
                {
                    startPin = value;
                    Notify("StartPin");
                }
            }
        }

        #endregion

        #region Methods

        void AddAdorner()
        {
            var adornerLayer = AdornerLayer.GetAdornerLayer(this);
            if (adornerLayer != null)
            {
                this.adorner = new SelectionAdorner(this);
                adornerLayer.Add(this.adorner);
            }
        }

        void RemoveAdorner()
        {
            var adornerLayer = AdornerLayer.GetAdornerLayer(this);
            if (adornerLayer != null)
            {
                adornerLayer.Remove(this.adorner);
                this.adorner = null;
            }
        }

        void UpdateAdorner(Point p)
        {
            this.adorner.End = p;
            this.adorner.InvalidateVisual();
        }

        void UpdateSelectedItems()
        {
            var diagram = this.DataContext as Diagram;
            if (diagram.SelectedItems == null)
            {
                diagram.SelectedItems = new ItemList();
            }

            foreach (var result in hitResultsTempList)
            {
                var elememt = result as FrameworkElement;
                var item = elememt.DataContext as Item;
                if (item is Item && !diagram.SelectedItems.Contains(item))
                {
                    diagram.SelectedItems.Add(item);
                }
            }
        }

        void ClearSelectedItems()
        {
            var diagram = this.DataContext as Diagram;
            if (diagram.SelectedItems != null)
            {
                diagram.SelectedItems.Clear();
                diagram.SelectedItems = null;
            }
        }

        void ProcessSelection()
        {
            if (adorner != null)
            {
                var expandedHitTestArea = new RectangleGeometry(
                    new Rect(
                        adorner.Start, 
                        adorner.End));

                ClearTempHitTestResults(true);

                VisualTreeHelper.HitTest(
                    this, 
                    null, 
                    new HitTestResultCallback(MyHitTestResultCallback), 
                    new GeometryHitTestParameters(expandedHitTestArea));
                if (hitResultsTempList.Count > 0)
                {
                    ProcessHitTestResultsList();
                }

                Notify("Count");
            }
        }

        #endregion

        #region Mouse Events

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (this.adorner == null)
            {
                if (!this.IsMouseCaptured)
                {
                    this.AddAdorner();

                    if (this.adorner != null)
                    {
                        if (Keyboard.Modifiers != ModifierKeys.Control)
                        {
                            ClearTempHitTestResults(true);
                            ClearHitTestResults();
                            ClearSelectedItems();
                        }
                        else
                        {
                            ClearTempHitTestResults(false);
                        }

                        Point p = e.GetPosition(this);

                        this.adorner.Start = new Point(p.X, p.Y);
                        this.adorner.End = new Point(p.X, p.Y);
                        this.adorner.InvalidateVisual();

                        this.CaptureMouse();

                        Notify("Count");
                    }
                }
            }

            base.OnMouseLeftButtonDown(e);
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (this.IsMouseCaptured && this.adorner != null)
            {
                this.ReleaseMouseCapture();

                if (this.Preview == false)
                {
                    ProcessSelection();
                }

                this.RemoveAdorner();

                hitResultsList.AddRange(hitResultsTempList);

                this.UpdateSelectedItems();

                Notify("Count");
            }

            base.OnMouseLeftButtonUp(e);
        }

        protected override void OnPreviewMouseRightButtonDown(MouseButtonEventArgs e)
        {
            if (!(this.DataContext is Diagram))
            {
                base.OnPreviewMouseRightButtonDown(e);
                return;
            }

            var diagram = this.DataContext as Diagram;
            var element = e.Source as FrameworkElement;

            if (this.IsMouseCaptured && this.CurrentWire != null && this.HaveStart == true)
            {
                diagram.Items.Remove(this.CurrentWire);

                this.CurrentWire = null;
                this.StartPin.IsConnected = false;
                this.StartPin = null;
                this.HaveStart = false;

                this.ReleaseMouseCapture();

                e.Handled = true;
            }
            else
            {
                base.OnPreviewMouseRightButtonDown(e);
            }
        }

        protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
        {
            if (this.IsMouseCaptured && this.adorner != null)
            {
                this.ReleaseMouseCapture();
                this.RemoveAdorner();
            }

            ClearTempHitTestResults(true);
            ClearHitTestResults();
            ClearSelectedItems();

            Notify("Count");

            base.OnMouseRightButtonDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (this.IsMouseCaptured && this.adorner != null)
            {
                UpdateAdorner(e.GetPosition(this));

                if (this.Preview == true)
                {
                    ProcessSelection();
                }
            }

        	if (!(this.DataContext is Diagram) || (this.IsMouseCaptured != true) || currentWire == null || haveStart != true)
            {
                base.OnMouseMove(e);
                return;
            }

            var diagram = this.DataContext as Diagram;
            Point p = e.GetPosition(this);

            currentWire.EndPin.X = p.X;
            currentWire.EndPin.Y = p.Y;

            e.Handled = true;
        }

        #endregion

        #region VisualTreeHelper.HitTest Methods

        public void ClearResults()
        {
            if (!this.IsMouseCaptured)
            {
                ClearTempHitTestResults(true);
                ClearHitTestResults();
                ClearSelectedItems();
            }
        }

        void ClearTempHitTestResults(bool removeSelection)
        {
            if (removeSelection)
            {
                foreach (var result in hitResultsTempList)
                {
                    var elememt = result as FrameworkElement;
                    var selected = elememt.DataContext as ISelected;
                    if (selected is ISelected)
                    {
                        selected.IsSelected = false;
                    }
                }
            }

            hitResultsTempList.Clear();
        }

        void ClearHitTestResults()
        {
            foreach (var result in hitResultsList)
            {
                var elememt = result as FrameworkElement;
                var selected = elememt.DataContext as ISelected;
                if (selected is ISelected)
                {
                    selected.IsSelected = false;
                }
            }

            hitResultsList.Clear();
        }

        void ProcessHitTestResultsList()
        {
            foreach (var result in hitResultsTempList)
            {
                var elememt = result as FrameworkElement;
                var selected = elememt.DataContext as ISelected;
                if (selected is ISelected && elememt.DataContext.GetType() != typeof(Diagram))
                {
                    selected.IsSelected = true;
                }
            }
        }

        HitTestResultBehavior MyHitTestResultCallback(HitTestResult result)
        {
            IntersectionDetail intersectionDetail = ((GeometryHitTestResult)result).IntersectionDetail;
            switch (intersectionDetail)
            {
                case IntersectionDetail.FullyContains:
                    {
                        if (!hitResultsList.Contains(result.VisualHit) && (result.VisualHit as FrameworkElement).DataContext is ISelected)
                        {
                            hitResultsTempList.Add(result.VisualHit);
                        }
                        return HitTestResultBehavior.Continue;
                    }
                case IntersectionDetail.Intersects:
                    {
                        if (!hitResultsList.Contains(result.VisualHit) && (result.VisualHit as FrameworkElement).DataContext is ISelected)
                        {
                            hitResultsTempList.Add(result.VisualHit);
                        }
                        return HitTestResultBehavior.Continue;
                    }
                case IntersectionDetail.FullyInside:
                    {
                        if (!hitResultsList.Contains(result.VisualHit) && (result.VisualHit as FrameworkElement).DataContext is ISelected)
                        {
                            hitResultsTempList.Add(result.VisualHit);
                        }
                        return HitTestResultBehavior.Continue;
                    }
                default:
                    {
                        return HitTestResultBehavior.Stop;
                    }
            }

        }

        #endregion

        #region Background Grid

        private Pen pen = new Pen(new SolidColorBrush(Color.FromArgb(0x5F, 0x7F, 0x7F, 0x7F)), 1.0);

        void DrawBackgroundGrid(DrawingContext dc)
        {
            Point p1 = new Point();
            Point p2 = new Point();

            double width = this.ActualWidth;
            double height = this.ActualHeight;
            double penHalfThickness = pen.Thickness / 2.0;

            for (double y = BackgroundGridHeight + BackgroundGridMargin.Top; y < height - BackgroundGridMargin.Bottom; y += BackgroundGridHeight)
            {
                p1.X = BackgroundGridMargin.Left;
                p1.Y = y;
                p2.X = width - BackgroundGridMargin.Right;
                p2.Y = y;

                GuidelineSet g = new GuidelineSet(new double[] { 0.0, 0.0 }, new double[] { 0.0, 0.0 });
                g.GuidelinesX[0] = p1.X + penHalfThickness;
                g.GuidelinesX[1] = p2.X + penHalfThickness;
                g.GuidelinesY[0] = p1.Y + penHalfThickness;
                g.GuidelinesY[1] = p2.Y + penHalfThickness;

                dc.PushGuidelineSet(g);
                dc.DrawLine(pen, p1, p2);
                dc.Pop();
            }

            for (double x = BackgroundGridWidth + BackgroundGridMargin.Left; x < width - BackgroundGridMargin.Right; x += BackgroundGridWidth)
            {
                p1.X = x;
                p1.Y = BackgroundGridMargin.Top;
                p2.X = x;
                p2.Y = height - BackgroundGridMargin.Bottom;

                GuidelineSet g = new GuidelineSet(new double[] { 0.0, 0.0 }, new double[] { 0.0, 0.0 });
                g.GuidelinesX[0] = p1.X + penHalfThickness;
                g.GuidelinesX[1] = p2.X + penHalfThickness;
                g.GuidelinesY[0] = p1.Y + penHalfThickness;
                g.GuidelinesY[1] = p2.Y + penHalfThickness;

                dc.PushGuidelineSet(g);
                dc.DrawLine(pen, p1, p2);
                dc.Pop();
            }  
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            if (IsBackgroundGridEnabled)
            {
                DrawBackgroundGrid(dc);
            }
        }

        #endregion

        #region INotifyPropertyChanged

        public virtual void Notify(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
