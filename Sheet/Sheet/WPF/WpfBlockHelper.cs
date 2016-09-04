// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Sheet.Core;
using Sheet.Editor;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Sheet.WPF
{
    public class WpfBlockHelper : IBlockHelper
    {
        #region Get

        public static ArgbColor GetXArgbColor(Brush brush)
        {
            var color = (brush as SolidColorBrush).Color;
            return new ArgbColor(
                color.A,
                color.R,
                color.G,
                color.B);
        }

        public static TextBlock GetTextBlock(XText text)
        {
            return (text.Native as Grid).Children[0] as TextBlock;
        }

        #endregion

        #region HitTest

        public bool HitTest(XElement element, ImmutableRect rect)
        {
            var r = new Rect(rect.X, rect.Y, rect.Width, rect.Height);
            var bounds = WpfVisualHelper.GetContentBounds(element);
            if (r.IntersectsWith(bounds))
            {
                return true;
            }
            return false;
        }

        public bool HitTest(XElement element, ImmutableRect rect, object relativeTo)
        {
            var r = new Rect(rect.X, rect.Y, rect.Width, rect.Height);
            var bounds = WpfVisualHelper.GetContentBounds(element, relativeTo);
            if (r.IntersectsWith(bounds))
            {
                return true;
            }
            return false;
        }

        #endregion

        #region IsSelected

        public void SetIsSelected(XElement element, bool value)
        {
            WpfFrameworkElementProperties.SetIsSelected(element.Native as FrameworkElement, value);
        }

        public bool GetIsSelected(XElement element)
        {
            return WpfFrameworkElementProperties.GetIsSelected(element.Native as FrameworkElement);
        }

        public bool IsSelected(XPoint point)
        {
            throw new NotImplementedException();
        }

        public bool IsSelected(XLine line)
        {
            return (line.Native as Line).Stroke != WpfBlockFactory.SelectedBrush;
        }

        public bool IsSelected(XRectangle rectangle)
        {
            return (rectangle.Native as Rectangle).Stroke != WpfBlockFactory.SelectedBrush;
        }

        public bool IsSelected(XEllipse ellipse)
        {
            return (ellipse.Native as Ellipse).Stroke != WpfBlockFactory.SelectedBrush;
        }

        public bool IsSelected(XText text)
        {
            var tb = WpfBlockHelper.GetTextBlock(text);
            return tb.Foreground != WpfBlockFactory.SelectedBrush;
        }

        public bool IsSelected(XImage image)
        {
            return (image.Native as Image).OpacityMask != WpfBlockFactory.SelectedBrush;
        }

        #endregion

        #region Deselect

        public void Deselect(XPoint point)
        {
            throw new NotImplementedException();
        }

        public void Deselect(XLine line)
        {
            (line.Native as Line).Stroke = WpfBlockFactory.NormalBrush;
        }

        public void Deselect(XRectangle rectangle)
        {
            (rectangle.Native as Rectangle).Stroke = WpfBlockFactory.NormalBrush;
            (rectangle.Native as Rectangle).Fill = (rectangle.Native as Rectangle).Fill == WpfBlockFactory.TransparentBrush ? WpfBlockFactory.TransparentBrush : WpfBlockFactory.NormalBrush;
        }

        public void Deselect(XEllipse ellipse)
        {
            (ellipse.Native as Ellipse).Stroke = WpfBlockFactory.NormalBrush;
            (ellipse.Native as Ellipse).Fill = (ellipse.Native as Ellipse).Fill == WpfBlockFactory.TransparentBrush ? WpfBlockFactory.TransparentBrush : WpfBlockFactory.NormalBrush;
        }

        public void Deselect(XText text)
        {
            WpfBlockHelper.GetTextBlock(text).Foreground = WpfBlockFactory.NormalBrush;
        }

        public void Deselect(XImage image)
        {
            (image.Native as Image).OpacityMask = WpfBlockFactory.NormalBrush;
        }

        #endregion

        #region Select

        public void Select(XPoint point)
        {
            throw new NotImplementedException();
        }

        public void Select(XLine line)
        {
            (line.Native as Line).Stroke = WpfBlockFactory.SelectedBrush;
        }

        public void Select(XRectangle rectangle)
        {
            (rectangle.Native as Rectangle).Stroke = WpfBlockFactory.SelectedBrush;
            (rectangle.Native as Rectangle).Fill = (rectangle.Native as Rectangle).Fill == WpfBlockFactory.TransparentBrush ? WpfBlockFactory.TransparentBrush : WpfBlockFactory.SelectedBrush;
        }

        public void Select(XEllipse ellipse)
        {
            (ellipse.Native as Ellipse).Stroke = WpfBlockFactory.SelectedBrush;
            (ellipse.Native as Ellipse).Fill = (ellipse.Native as Ellipse).Fill == WpfBlockFactory.TransparentBrush ? WpfBlockFactory.TransparentBrush : WpfBlockFactory.SelectedBrush;
        }

        public void Select(XText text)
        {
            WpfBlockHelper.GetTextBlock(text).Foreground = WpfBlockFactory.SelectedBrush;
        }

        public void Select(XImage image)
        {
            (image.Native as Image).OpacityMask = WpfBlockFactory.SelectedBrush;
        }

        #endregion

        #region ZIndex

        public void SetZIndex(XElement element, int index)
        {
            Panel.SetZIndex(element.Native as FrameworkElement, index);
        }

        #endregion

        #region Fill

        public void ToggleFill(XRectangle rectangle)
        {
            (rectangle.Native as Rectangle).Fill = (rectangle.Native as Rectangle).Fill == WpfBlockFactory.TransparentBrush ? WpfBlockFactory.NormalBrush : WpfBlockFactory.TransparentBrush;
        }

        public void ToggleFill(XEllipse ellipse)
        {
            (ellipse.Native as Ellipse).Fill = (ellipse.Native as Ellipse).Fill == WpfBlockFactory.TransparentBrush ? WpfBlockFactory.NormalBrush : WpfBlockFactory.TransparentBrush;
        }

        public void ToggleFill(XPoint point)
        {
            (point.Native as Ellipse).Fill = (point.Native as Ellipse).Fill == WpfBlockFactory.TransparentBrush ? WpfBlockFactory.NormalBrush : WpfBlockFactory.TransparentBrush;
        }

        #endregion

        #region XElement

        public double GetLeft(XElement element)
        {
            return Canvas.GetLeft(element.Native as FrameworkElement);
        }

        public double GetTop(XElement element)
        {
            return Canvas.GetTop(element.Native as FrameworkElement);
        }

        public double GetWidth(XElement element)
        {
            return (element.Native as FrameworkElement).Width;
        }

        public double GetHeight(XElement element)
        {
            return (element.Native as FrameworkElement).Height;
        }

        public void SetLeft(XElement element, double left)
        {
            Canvas.SetLeft(element.Native as FrameworkElement, left);
        }

        public void SetTop(XElement element, double top)
        {
            Canvas.SetTop(element.Native as FrameworkElement, top);
        }

        public void SetWidth(XElement element, double width)
        {
            (element.Native as FrameworkElement).Width = width;
        }

        public void SetHeight(XElement element, double height)
        {
            (element.Native as FrameworkElement).Height = height;
        }

        #endregion

        #region XLine

        public double GetX1(XLine line)
        {
            return (line.Native as Line).X1;
        }

        public double GetY1(XLine line)
        {
            return (line.Native as Line).Y1;
        }

        public double GetX2(XLine line)
        {
            return (line.Native as Line).X2;
        }

        public double GetY2(XLine line)
        {
            return (line.Native as Line).Y2;
        }

        public ArgbColor GetStroke(XLine line)
        {
            return GetXArgbColor((line.Native as Line).Stroke);
        }

        public void SetX1(XLine line, double x1)
        {
            (line.Native as Line).X1 = x1;
        }

        public void SetY1(XLine line, double y1)
        {
            (line.Native as Line).Y1 = y1;
        }

        public void SetX2(XLine line, double x2)
        {
            (line.Native as Line).X2 = x2;
        }

        public void SetY2(XLine line, double y2)
        {
            (line.Native as Line).Y2 = y2;
        }

        public void SetStrokeThickness(XLine line, double thickness)
        {
            (line.Native as Line).StrokeThickness = thickness;
        }

        public double GetStrokeThickness(XLine line)
        {
            return (line.Native as Line).StrokeThickness;
        }

        #endregion

        #region XRectangle

        public ArgbColor GetStroke(XRectangle rectangle)
        {
            return GetXArgbColor((rectangle.Native as Rectangle).Stroke);
        }

        public ArgbColor GetFill(XRectangle rectangle)
        {
            return GetXArgbColor((rectangle.Native as Rectangle).Fill);
        }

        public bool IsTransparent(XRectangle rectangle)
        {
            return (rectangle.Native as Rectangle).Fill == WpfBlockFactory.TransparentBrush ? false : true;
        }

        public void SetStrokeThickness(XRectangle rectangle, double thickness)
        {
            (rectangle.Native as Rectangle).StrokeThickness = thickness;
        }

        public double GetStrokeThickness(XRectangle rectangle)
        {
            return (rectangle.Native as Rectangle).StrokeThickness;
        }

        #endregion

        #region XEllipse

        public ArgbColor GetStroke(XEllipse ellipse)
        {
            return GetXArgbColor((ellipse.Native as Ellipse).Stroke);
        }

        public ArgbColor GetFill(XEllipse ellipse)
        {
            return GetXArgbColor((ellipse.Native as Ellipse).Fill);
        }

        public bool IsTransparent(XEllipse ellipse)
        {
            return (ellipse.Native as Ellipse).Fill == WpfBlockFactory.TransparentBrush ? false : true;
        }

        public void SetStrokeThickness(XEllipse ellipse, double thickness)
        {
            (ellipse.Native as Ellipse).StrokeThickness = thickness;
        }

        public double GetStrokeThickness(XEllipse ellipse)
        {
            return (ellipse.Native as Ellipse).StrokeThickness;
        }

        #endregion

        #region XText

        public ArgbColor GetBackground(XText text)
        {
            var tb = WpfBlockHelper.GetTextBlock(text);
            return GetXArgbColor(tb.Background);

        }

        public ArgbColor GetForeground(XText text)
        {
            var tb = WpfBlockHelper.GetTextBlock(text);
            return GetXArgbColor(tb.Foreground);
        }

        public string GetText(XText text)
        {
            var tb = WpfBlockHelper.GetTextBlock(text);
            return tb.Text;
        }

        public int GetHAlign(XText text)
        {
            var tb = WpfBlockHelper.GetTextBlock(text);
            return (int)tb.HorizontalAlignment;
        }

        public int GetVAlign(XText text)
        {
            var tb = WpfBlockHelper.GetTextBlock(text);
            return (int)tb.VerticalAlignment;
        }

        public double GetSize(XText text)
        {
            var tb = WpfBlockHelper.GetTextBlock(text);
            return tb.FontSize;
        }

        #endregion

        #region XImage

        public byte[] GetData(XImage image)
        {
            return (image.Native as Image).Tag as byte[];
        }

        #endregion
    }
}
