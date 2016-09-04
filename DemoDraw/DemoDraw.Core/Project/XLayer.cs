using System.Collections.Generic;
using DemoDraw.Core.Renderer;

namespace DemoDraw.Core.Project
{
    public class XLayer : XDrawable
    {
        public IList<XShape> Shapes { get; set; }
        public bool IsVisible { get; set; }

        public override void Draw(ShapeRenderer renderer, double dx, double dy)
        {
            if (IsVisible)
            {
                foreach (var shape in Shapes)
                {
                    shape.Draw(renderer, dx, dy);
                }
            }
        }

        public static XLayer Create()
        {
            return new XLayer()
            {
                Shapes = new List<XShape>(),
                IsVisible = true
            };
        }
    }
}
