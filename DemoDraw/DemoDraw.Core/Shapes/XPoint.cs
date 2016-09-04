using DemoDraw.Core.Renderer;
using DemoDraw.Core.Style;

namespace DemoDraw.Core.Shapes
{
    public class XPoint : XShape
    {
        public double X { get; set; }
        public double Y { get; set; }
        public XShape Template { get; set; }

        public override void Draw(ShapeRenderer renderer, double dx, double dy)
        {
            if (Template != null)
            {
                Template.Draw(renderer, X, Y);
            }
        }

        public static XPoint Create(double x, double y, XStyle style)
        {
            return new XPoint()
            {
                Style = style,
                X = x,
                Y = y
            };
        }
    }
}
