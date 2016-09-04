using System.Windows;
using System.Windows.Media;
using DemoDraw.Core.Editor;
using DemoDraw.Core.Project;
using DemoDraw.Core.Renderer;

namespace DemoDraw.Editor
{
    public class EditorContextRenderer
    {
        private Brush BackgroundBrush;

        public void Resize(FrameworkElement surface, XPage page)
        {
            if (page != null)
            {
                surface.Width = page.Width;
                surface.Height = page.Height;
            }
            else
            {
                surface.Width = 0.0;
                surface.Height = 0.0;
            }
        }

        public void DrawBackground(DrawingContext dc, XPage page)
        {
            if (page != null)
            {
                if (BackgroundBrush == null)
                {
                    BackgroundBrush = WpfRenderer.ToBrush(page.Background);
                }

                var rect = new Rect(0, 0, page.Width, page.Height);
                dc.DrawRectangle(BackgroundBrush, null, rect);
            }
        }

        public void DrawShapes(DrawingContext dc, ShapeRenderer renderer, XPage page)
        {
            if (renderer != null && page != null)
            {
                renderer.Context = dc;
                page.Draw(renderer, 0.0, 0.0);
                renderer.Context = null;
            }
        }

        public void Draw(DrawingContext dc, FrameworkElement surface)
        {
            var editor = (ProjectEditor)surface.GetValue(EditorContext.EditorProperty);
            var renderer = (ShapeRenderer)surface.GetValue(EditorContext.RendererProperty);
            var page = (XPage)surface.GetValue(EditorContext.PageProperty);

            Resize(surface, page);
            DrawBackground(dc, page);
            DrawShapes(dc, renderer, page);
        }
    }
}
