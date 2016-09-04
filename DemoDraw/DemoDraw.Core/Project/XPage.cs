using System.Collections.Generic;
using DemoDraw.Core.Renderer;
using DemoDraw.Core.Shapes;
using DemoDraw.Core.Style;

namespace DemoDraw.Core.Project
{
    public class XPage : XDrawable
    {
        public XColor Background { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public IList<XLayer> Layers { get; set; }
        public IList<XStyle> Styles { get; set; }
        public IList<XShape> PointTemplates { get; set; }
        public XLayer CurrentLayer { get; set; }
        public XStyle CurrentStyle { get; set; }
        public XShape CurrentPointTemplate { get; set; }

        public override void Draw(ShapeRenderer renderer, double dx, double dy)
        {
            foreach (var layer in Layers)
            {
                layer.Draw(renderer, dx, dy);
            }
        }

        public static XPage Create()
        {
            return new XPage()
            {
                Layers = new List<XLayer>(),
                Styles = new List<XStyle>(),
                PointTemplates = new List<XShape>()
            };
        }
    }
}
