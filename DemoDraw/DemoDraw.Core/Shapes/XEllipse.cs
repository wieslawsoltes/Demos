using DemoDraw.Core.Renderer;
using DemoDraw.Core.Style;

namespace DemoDraw.Core.Shapes
{
    public class XEllipse : XShape
    {
        public XPoint TopLeft { get; set; }
        public XPoint BottomRight { get; set; }

        public override void Draw(ShapeRenderer renderer, double dx, double dy)
        {
            renderer.DrawEllipse(TopLeft, BottomRight, Style, dx, dy);
            TopLeft.Draw(renderer, dx, dy);
            BottomRight.Draw(renderer, dx, dy);
        }

        public static XEllipse Create(double x, double y, XStyle style)
        {
            return new XEllipse()
            {
                Style = style,
                TopLeft = XPoint.Create(x, y, null),
                BottomRight = XPoint.Create(x, y, null)
            };
        }

        public static XEllipse Create(double tlx, double tly, double brx, double bry, XStyle style)
        {
            return new XEllipse()
            {
                Style = style,
                TopLeft = XPoint.Create(tlx, tly, null),
                BottomRight = XPoint.Create(brx, bry, null)
            };
        }
    }
}
