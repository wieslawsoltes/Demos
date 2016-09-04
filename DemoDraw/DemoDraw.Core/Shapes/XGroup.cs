using System.Collections.Generic;
using DemoDraw.Core.Renderer;

namespace DemoDraw.Core.Shapes
{
    public class XGroup : XShape
    {
        public IList<XShape> Shapes { get; set; }

        public override void Draw(ShapeRenderer renderer, double dx, double dy)
        {
            foreach (var shape in Shapes)
            {
                shape.Draw(renderer, dx, dy);
            }
        }

        public static XGroup Create()
        {
            return new XGroup()
            {
                Shapes = new List<XShape>()
            };
        }
    }
}
