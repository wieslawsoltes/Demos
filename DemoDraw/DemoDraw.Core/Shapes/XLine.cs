using DemoDraw.Core.Renderer;
using DemoDraw.Core.Style;

namespace DemoDraw.Core.Shapes
{
    public class XLine : XShape
    {
        public XPoint Start { get; set; }
        public XPoint End { get; set; }

        public override void Draw(ShapeRenderer renderer, double dx, double dy)
        {
            renderer.DrawLine(Start, End, Style, dx, dy);
            Start.Draw(renderer, dx, dy);
            End.Draw(renderer, dx, dy);
        }

        public static XLine Create(double x, double y, XStyle style)
        {
            return new XLine()
            {
                Style = style,
                Start = XPoint.Create(x, y, null),
                End = XPoint.Create(x, y, null)
            };
        }

        public static XLine Create(double x1, double y1, double x2, double y2, XStyle style)
        {
            return new XLine()
            {
                Style = style,
                Start = XPoint.Create(x1, y1, null),
                End = XPoint.Create(x2, y2, null)
            };
        }
    }
}
