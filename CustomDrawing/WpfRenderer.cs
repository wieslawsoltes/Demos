#region References

using CustomDrawing.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

#endregion

namespace CustomDrawing
{
    #region WpfRenderer

    public class WpfRenderer : IRenderer
    {
        #region Fields

        private DrawingContext dc;
        private Pen pen;
        private Pen spen;
        private Brush brush;
        private Brush sbrush;
        private float pinRadius = 3f;
        private RotateTransform rt;

        #endregion

        #region Constructor

        public WpfRenderer()
        {
            Initialize(1.0, Color.FromArgb(0xFF, 0x00, 0x00, 0x00), Color.FromArgb(0xC0, 0xFF, 0xFF, 0x00));
        }

        private void Initialize(double thickness, Color normal, Color selected)
        {
            brush = new SolidColorBrush(normal);
            brush.Freeze();
            sbrush = new SolidColorBrush(selected);
            sbrush.Freeze();
            pen = new Pen(brush, thickness) { StartLineCap = PenLineCap.Round, EndLineCap = PenLineCap.Round };
            pen.Freeze();
            spen = new Pen(sbrush, thickness) { StartLineCap = PenLineCap.Round, EndLineCap = PenLineCap.Round };
            spen.Freeze();
            rt = new RotateTransform(0f, 0f, 0f);
        }

        #endregion

        #region Set

        public void Set(DrawingContext drawingContext)
        {
            dc = drawingContext;
        }

        #endregion

        #region Origin

        public void GetReferenceOrigin(Pin pin, out float x, out float y)
        {
            if (pin.Parent != null && pin.Parent is Reference)
            {
                var origin = (pin.Parent as Reference).Origin;
                x = origin.X;
                y = origin.Y;
            }
            else
            {
                x = 0f;
                y = 0f;
            }
        }

        #endregion

        #region IRenderer

        public void Transform(float x, float y, out float tx, out float ty)
        {
            var p = rt.Transform(new Point(x, y));
            tx = (float)p.X;
            ty = (float)p.Y;
        }

        public void PushRotate()
        {
            var frt = new RotateTransform(rt.Angle, rt.CenterX, rt.CenterY);
            frt.Freeze();
            dc.PushTransform(frt);
        }

        public void Pop()
        {
            dc.Pop();
        }

        public void SetAngle(float angle, float cx, float cy)
        {
            rt.Angle = angle;
            rt.CenterX = cx;
            rt.CenterY = cy;
        }

        public float GetAngle()
        {
            return (float)rt.Angle;
        }

        public float GetCenterX()
        {
            return (float)rt.CenterX;
        }

        public float GetCenterY()
        {
            return (float)rt.CenterY;
        }

        public void Draw(Pin pin, float x, float y, IDictionary<int, string> variables)
        {
            var c = new Point(pin.X + x, pin.Y + y);

            double half = pin.IsSelected ? spen.Thickness / 2.0 : pen.Thickness / 2.0;
            GuidelineSet guidelines = new GuidelineSet();
            guidelines.GuidelinesX.Add(c.X - pinRadius + half);
            guidelines.GuidelinesX.Add(c.X + pinRadius + half);
            guidelines.GuidelinesY.Add(c.Y - pinRadius + half);
            guidelines.GuidelinesY.Add(c.Y + pinRadius + half);
            dc.PushGuidelineSet(guidelines);
            dc.DrawEllipse(pin.IsSelected ? sbrush : brush, pin.IsSelected ? spen : pen, c, pinRadius, pinRadius);
            dc.Pop();
        }

        public void Draw(Line line, float x, float y, IDictionary<int, string> variables)
        {
            float ox0, oy0, ox1, oy1;
            GetReferenceOrigin(line.Start, out ox0, out oy0);
            GetReferenceOrigin(line.End, out ox1, out oy1);
            var p0 = new Point(line.UseTransforms ? line.Start.TransX : ox0 + line.Start.X + x, line.UseTransforms ? line.Start.TransY : oy0 + line.Start.Y + y);
            var p1 = new Point(line.UseTransforms ? line.End.TransX : ox1 + line.End.X + x, line.UseTransforms ? line.End.TransY : oy1 + line.End.Y + y);

            double half = line.IsSelected ? spen.Thickness / 2.0 : pen.Thickness / 2.0;
            GuidelineSet guidelines = new GuidelineSet();
            guidelines.GuidelinesX.Add(p0.X + half);
            guidelines.GuidelinesX.Add(p1.X + half);
            guidelines.GuidelinesY.Add(p0.Y + half);
            guidelines.GuidelinesY.Add(p1.Y + half);
            dc.PushGuidelineSet(guidelines);
            dc.DrawLine(line.IsSelected ? spen : pen, p0, p1);
            dc.Pop();
        }

        public void Draw(Rectangle rectangle, float x, float y, IDictionary<int, string> variables)
        {
            float ox0, oy0, ox1, oy1;
            GetReferenceOrigin(rectangle.TopLeft, out ox0, out oy0);
            GetReferenceOrigin(rectangle.BottomRight, out ox1, out oy1);
            float x0 = rectangle.UseTransforms ? rectangle.TopLeft.TransX : ox0 + rectangle.TopLeft.X + x;
            float y0 = rectangle.UseTransforms ? rectangle.TopLeft.TransY : oy0 + rectangle.TopLeft.Y + y;
            float x1 = rectangle.UseTransforms ? rectangle.BottomRight.TransX : ox1 + rectangle.BottomRight.X + x;
            float y1 = rectangle.UseTransforms ? rectangle.BottomRight.TransY : oy1 + rectangle.BottomRight.Y + y;
            var r = new Rect(Math.Min(x0, x1), Math.Min(y0, y1), Math.Abs(x1 - x0), Math.Abs(y1 - y0));

            double half = rectangle.IsSelected ? spen.Thickness / 2.0 : pen.Thickness / 2.0;
            GuidelineSet guidelines = new GuidelineSet();
            guidelines.GuidelinesX.Add(r.Left + half);
            guidelines.GuidelinesX.Add(r.Right + half);
            guidelines.GuidelinesY.Add(r.Top + half);
            guidelines.GuidelinesY.Add(r.Bottom + half);
            dc.PushGuidelineSet(guidelines);
            dc.DrawRectangle(rectangle.IsSelected ? sbrush : brush, rectangle.IsSelected ? spen : pen, r);
            dc.Pop();
        }

        public void Draw(Text text, float x, float y, IDictionary<int, string> variables)
        {
            string textToFormat;
            if (variables == null || !variables.TryGetValue(text.Id, out textToFormat))
                textToFormat = text.Value;

            FormattedText ft = GetFormattedTextFromCache(text, textToFormat);

            float ox, oy;
            GetReferenceOrigin(text.Origin, out ox, out oy);

            double px = text.UseTransforms ? text.Origin.TransX : ox + text.Origin.X + x; // HAlign.Left
            double py = text.UseTransforms ? text.Origin.TransY : oy + text.Origin.Y + y; // VAlign.Top

            switch (text.HAlign)
            {
                case HAlign.Right: px = px - ft.WidthIncludingTrailingWhitespace; break;
                case HAlign.Center: px = px - (ft.WidthIncludingTrailingWhitespace / 2.0); break;
            }

            switch (text.VAlign)
            {
                case VAlign.Bottom: py = py - ft.Height; break;
                case VAlign.Center: py = py - (ft.Height / 2.0); break;
            }

            var p = new Point(px, py);
            dc.DrawText(ft, p);
        }

        #endregion

        #region FormattedText Cache

        private IDictionary<string, FormattedText> FormattedTextCache = new Dictionary<string, FormattedText>();

        private FormattedText GetFormattedTextFromCache(Text text, string textToFormat)
        {
            FormattedText ft;
            if (!FormattedTextCache.TryGetValue(textToFormat, out ft))
            {
                ft = new FormattedText(textToFormat, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface("Arial"), 15.0, text.IsSelected ? sbrush : brush);
                FormattedTextCache.Add(textToFormat, ft);
            }
            else
            {
                ft.SetForegroundBrush(text.IsSelected ? sbrush : brush);
            }
            return ft;
        }

        #endregion
    }

    #endregion
}
