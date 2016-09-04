// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Logic.Services
{
    #region References

    using Logic.Controls;
    using Logic.Model;
    using Logic.Model.Core;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Documents;
    using System.Windows.Media;

    #endregion

    #region SelectionService

    public class SelectionService
    {
        #region Fields

        private bool enableSelectionPreview = false;

        private FrameworkElement element = null;

        #endregion

        #region Constructor

        public SelectionService(FrameworkElement element)
        {
            if (element == null)
                throw new ArgumentNullException();

            this.element = element;
        }

        #endregion

        #region Fields

        private SelectionAdorner adorner = null;
        private List<DependencyObject> hitResultsList = null;
        private List<Element> previousElements = null;
        private SolidColorBrush adornerStroke = new SolidColorBrush(Color.FromArgb(0xFF, 0x7F, 0x7F, 0x7F));
        private SolidColorBrush adornerFill = new SolidColorBrush(Color.FromArgb(0x28, 0xFF, 0xFF, 0xFF));

        #endregion

        #region Methods

        private void AddAdorner()
        {
            var adornerLayer = AdornerLayer.GetAdornerLayer(this.element);
            if (adornerLayer == null)
                return;

            this.adorner = new SelectionAdorner(this.element)
            {
                Stroke = adornerStroke,
                StrokeThickness = 1.0,
                Fill = adornerFill
            };

            adornerLayer.Add(this.adorner);
        }

        private void RemoveAdorner()
        {
            var adornerLayer = AdornerLayer.GetAdornerLayer(this.element);
            if (adornerLayer == null)
                return;

            adornerLayer.Remove(this.adorner);
            this.adorner = null;
        }

        private void UpdateAdorner(Point point)
        {
            if (this.adorner == null)
                return;

            this.adorner.End = point;
            this.adorner.InvalidateVisual();
        }

        private void ProcessSelection()
        {
            if (adorner == null)
                return;

            var expandedHitTestArea = new RectangleGeometry(new Rect(adorner.Start, adorner.End));

            hitResultsList = new List<DependencyObject>();

            VisualTreeHelper.HitTest(this.element, null, new HitTestResultCallback(MyHitTestResultCallback), new GeometryHitTestParameters(expandedHitTestArea));
            if (hitResultsList.Count > 0)
            {
                var elements = hitResultsList.Select(result => (result as FrameworkElement).DataContext as Element)
                                                .Where(element => element is ISelected && !(element is Context) && (element.Parent == null || element.Parent is Context))
                                                .ToList();

                //bool isControlKeyPressed = Keyboard.Modifiers != ModifierKeys.Control;

                if (previousElements != null)
                {
                    var diffElements = previousElements.Except(elements);

                    Manager.SelectElements(diffElements, false);
                }

                previousElements = elements;

                Manager.SelectElements(elements, true);
            }
        }

        public void Capture()
        {
            if (this.adorner == null)
            {
                if (previousElements != null)
                    Manager.SelectElements(previousElements, false);

                var context = this.element.DataContext as Context;
                Manager.ClearSelectedElements(context);

                this.element.CaptureMouse();
            }
        }

        public void Relase()
        {
            if (this.adorner != null)
            {
                if (enableSelectionPreview == false)
                    ProcessSelection();

                this.RemoveAdorner();

                var context = this.element.DataContext as Context;

                Manager.UpdateSelectedElements(context, previousElements);
            }
        }

        public void ReleaseAndClear()
        {
            if (this.element.IsMouseCaptured && this.adorner != null)
            {
                this.element.ReleaseMouseCapture();
                this.RemoveAdorner();
            }

            if (previousElements != null)
                Manager.SelectElements(previousElements, false);

            var context = this.element.DataContext as Context;
            Manager.ClearSelectedElements(context);
        }

        public void Move(Point point)
        {
            if (this.adorner != null)
            {
                UpdateAdorner(point);

                if (enableSelectionPreview == true)
                    ProcessSelection();
            }
            else
            {
                this.AddAdorner();

                if (this.adorner != null)
                {
                    // update adorner position
                    this.adorner.Start = new Point(point.X, point.Y);
                    this.adorner.End = new Point(point.X, point.Y);
                    this.adorner.InvalidateVisual();

                    // capture mouse
                    this.element.CaptureMouse();
                }
            }
        }

        #endregion

        #region VisualTreeHelper.HitTest Callback

        private HitTestResultBehavior MyHitTestResultCallback(HitTestResult result)
        {
            bool isISelected = (result.VisualHit as FrameworkElement).DataContext is ISelected;

            IntersectionDetail intersectionDetail = ((GeometryHitTestResult)result).IntersectionDetail;
            switch (intersectionDetail)
            {
                case IntersectionDetail.FullyContains:
                    {
                        if (isISelected)
                            hitResultsList.Add(result.VisualHit);

                        return HitTestResultBehavior.Continue;
                    }
                case IntersectionDetail.Intersects:
                    {
                        if (isISelected)
                            hitResultsList.Add(result.VisualHit);

                        return HitTestResultBehavior.Continue;
                    }
                case IntersectionDetail.FullyInside:
                    {
                        if (isISelected)
                            hitResultsList.Add(result.VisualHit);

                        return HitTestResultBehavior.Continue;
                    }
                default:
                    return HitTestResultBehavior.Stop;
            }
        }

        #endregion
    }

    #endregion
}
