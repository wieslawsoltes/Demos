// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Sheet.Core;
using Splat;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Sheet.WPF
{
    public class WpfBlockFactory : IBlockFactory
    {
        #region Brushes

        public static SolidColorBrush NormalBrush = Brushes.Black;
        public static SolidColorBrush SelectedBrush = Brushes.Red;
        public static SolidColorBrush HoverBrush = Brushes.Yellow;
        public static SolidColorBrush TransparentBrush = Brushes.Transparent;

        #endregion

        #region Styles

        private void SetStyle(Ellipse ellipse, bool isVisible)
        {
            var style = new Style(typeof(Ellipse));
            style.Setters.Add(new Setter(Ellipse.FillProperty, isVisible ? NormalBrush : TransparentBrush));
            style.Setters.Add(new Setter(Ellipse.StrokeProperty, isVisible ? NormalBrush : TransparentBrush));

            var isSelectedTrigger = new Trigger() { Property = WpfFrameworkElementProperties.IsSelectedProperty, Value = true };
            isSelectedTrigger.Setters.Add(new Setter(Ellipse.FillProperty, SelectedBrush));
            isSelectedTrigger.Setters.Add(new Setter(Ellipse.StrokeProperty, SelectedBrush));
            style.Triggers.Add(isSelectedTrigger);

            var isMouseOverTrigger = new Trigger() { Property = Ellipse.IsMouseOverProperty, Value = true };
            isMouseOverTrigger.Setters.Add(new Setter(Ellipse.FillProperty, HoverBrush));
            isMouseOverTrigger.Setters.Add(new Setter(Ellipse.StrokeProperty, HoverBrush));
            style.Triggers.Add(isMouseOverTrigger);

            ellipse.Style = style;

            WpfFrameworkElementProperties.SetIsSelected(ellipse, false);
        }

        #endregion

        #region Thumb

        private string ThumbTemplate = "<Thumb Cursor=\"SizeAll\" xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"><Thumb.Template><ControlTemplate><Rectangle Fill=\"Transparent\" Stroke=\"Red\" StrokeThickness=\"2\" Width=\"8\" Height=\"8\" Margin=\"-4,-4,0,0\"/></ControlTemplate></Thumb.Template></Thumb>";

        private void SetLineDragDeltaHandler(XLine line, XThumb thumb, Action<XLine, XThumb, double, double> drag)
        {
            (thumb.Native as Thumb).DragDelta += (sender, e) => drag(line, thumb, e.HorizontalChange, e.VerticalChange);
        }

        private void SetElementDragDeltaHandler(XElement element, XThumb thumb, Action<XElement, XThumb, double, double> drag)
        {
            (thumb.Native as Thumb).DragDelta += (sender, e) => drag(element, thumb, e.HorizontalChange, e.VerticalChange);
        }

        #endregion

        #region Color

        private ArgbColor ToArgbColor(ArgbColor color)
        {
            if (color == null)
            {
                throw new ArgumentNullException("color");
            }
            return new ArgbColor(color.A, color.R, color.G, color.B);
        }

        private SolidColorBrush ToSolidColorBrush(ArgbColor color)
        {
            if (color == null)
            {
                throw new ArgumentNullException("color");
            }
            return new SolidColorBrush(Color.FromArgb(color.A, color.R, color.G, color.B));
        }

        #endregion

        #region Create

        public XThumb CreateThumb(double x, double y)
        {
            using (var stringReader = new System.IO.StringReader(ThumbTemplate))
            {
                using (var xmlReader = System.Xml.XmlReader.Create(stringReader))
                {
                    var thumb = XamlReader.Load(xmlReader) as Thumb;
                    Canvas.SetLeft(thumb, x);
                    Canvas.SetTop(thumb, y);
                    return new XThumb(thumb);
                }
            }
        }

        public XThumb CreateThumb(double x, double y, XLine line, Action<XLine, XThumb, double, double> drag)
        {
            var thumb = CreateThumb(x, y);
            SetLineDragDeltaHandler(line, thumb, drag);
            return thumb;
        }

        public XThumb CreateThumb(double x, double y, XElement element, Action<XElement, XThumb, double, double> drag)
        {
            var thumb = CreateThumb(x, y);
            SetElementDragDeltaHandler(element, thumb, drag);
            return thumb;
        }

        public XPoint CreatePoint(double thickness, double x, double y, bool isVisible)
        {
            var ellipse = new Ellipse()
            {
                StrokeThickness = thickness,
                StrokeStartLineCap = PenLineCap.Round,
                StrokeEndLineCap = PenLineCap.Round,
                Width = 8.0,
                Height = 8.0,
                Margin = new Thickness(-4.0, -4.0, 0.0, 0.0),
            };

            SetStyle(ellipse, isVisible);
            Panel.SetZIndex(ellipse, 1);

            Canvas.SetLeft(ellipse, x);
            Canvas.SetTop(ellipse, y);

            var xpoint = new XPoint(ellipse, x, y, isVisible);

            return xpoint;
        }

        public XLine CreateLine(double thickness, double x1, double y1, double x2, double y2, ArgbColor stroke)
        {
            var strokeBrush = ToSolidColorBrush(stroke);

            strokeBrush.Freeze();

            var line = new Line()
            {
                Stroke = strokeBrush,
                StrokeThickness = thickness,
                StrokeStartLineCap = PenLineCap.Round,
                StrokeEndLineCap = PenLineCap.Round,
                X1 = x1,
                Y1 = y1,
                X2 = x2,
                Y2 = y2
            };

            var xline = new XLine(line);

            return xline;
        }

        public XLine CreateLine(double thickness, XPoint start, XPoint end, ArgbColor stroke)
        {
            var xline = CreateLine(thickness, start.X, start.Y, end.X, end.Y, stroke);
            xline.Start = start;
            xline.End = end;
            return xline;
        }

        public XRectangle CreateRectangle(double thickness, double x, double y, double width, double height, bool isFilled, ArgbColor stroke, ArgbColor fill)
        {
            var strokeBrush = ToSolidColorBrush(stroke);
            var fillBrush = ToSolidColorBrush(fill);

            strokeBrush.Freeze();
            fillBrush.Freeze();

            var rectangle = new Rectangle()
            {
                Fill = isFilled ? fillBrush : TransparentBrush,
                Stroke = strokeBrush,
                StrokeThickness = thickness,
                StrokeStartLineCap = PenLineCap.Round,
                StrokeEndLineCap = PenLineCap.Round,
                Width = width,
                Height = height
            };

            Canvas.SetLeft(rectangle, x);
            Canvas.SetTop(rectangle, y);

            var xrectangle = new XRectangle(rectangle);

            return xrectangle;
        }

        public XEllipse CreateEllipse(double thickness, double x, double y, double width, double height, bool isFilled, ArgbColor stroke, ArgbColor fill)
        {
            var strokeBrush = ToSolidColorBrush(stroke);
            var fillBrush = ToSolidColorBrush(fill);

            strokeBrush.Freeze();
            fillBrush.Freeze();

            var ellipse = new Ellipse()
            {
                Fill = isFilled ? fillBrush : TransparentBrush,
                Stroke = strokeBrush,
                StrokeThickness = thickness,
                StrokeStartLineCap = PenLineCap.Round,
                StrokeEndLineCap = PenLineCap.Round,
                Width = width,
                Height = height
            };

            Canvas.SetLeft(ellipse, x);
            Canvas.SetTop(ellipse, y);

            var xellipse = new XEllipse(ellipse);

            return xellipse;
        }

        public XText CreateText(string text, double x, double y, double width, double height, int halign, int valign, double fontSize, ArgbColor backgroud, ArgbColor foreground)
        {
            var backgroundBrush = ToSolidColorBrush(backgroud);
            var foregroundBrush = ToSolidColorBrush(foreground);

            backgroundBrush.Freeze();
            foregroundBrush.Freeze();

            var grid = new Grid();
            grid.Background = backgroundBrush;
            grid.Width = width;
            grid.Height = height;
            Canvas.SetLeft(grid, x);
            Canvas.SetTop(grid, y);

            var tb = new TextBlock();
            tb.HorizontalAlignment = (HorizontalAlignment)halign;
            tb.VerticalAlignment = (VerticalAlignment)valign;
            tb.Background = backgroundBrush;
            tb.Foreground = foregroundBrush;
            tb.FontSize = fontSize;
            tb.FontFamily = new FontFamily("Calibri");
            tb.Text = text;

            grid.Children.Add(tb);

            var xtext = new XText(grid);

            return xtext;
        }

        public XImage CreateImage(double x, double y, double width, double height, byte[] data)
        {
            Image image = new Image();

            // enable high quality image scaling
            RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.HighQuality);

            // store original image data is Tag property
            image.Tag = data;

            // opacity mask is used for determining selection state
            image.OpacityMask = NormalBrush;

            //using(var ms = new System.IO.MemoryStream(data))
            //{
            //    image = Image.FromStream(ms);
            //}
            using (var ms = new System.IO.MemoryStream(data))
            {
                IBitmap profileImage = BitmapLoader.Current.Load(ms, null, null).Result;
                image.Source = profileImage.ToNative();
            }

            image.Width = width;
            image.Height = height;

            Canvas.SetLeft(image, x);
            Canvas.SetTop(image, y);

            var ximage = new XImage(image);

            return ximage;
        }

        public XBlock CreateBlock(int id, double x, double y, double width, double height, int dataId, string name, ArgbColor backgroud)
        {
            var xblock = new XBlock(id, x, y, width, height, dataId, name);
            if (backgroud != null)
            {
                xblock.Backgroud = ToArgbColor(backgroud);
            }
            return xblock;
        }

        #endregion
    }
}
