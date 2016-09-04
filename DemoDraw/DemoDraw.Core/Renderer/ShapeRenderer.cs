using DemoDraw.Core.Shapes;
using DemoDraw.Core.Style;

namespace DemoDraw.Core.Renderer
{
    public abstract class ShapeRenderer : XDisposable
    {
        public object Context { get; set; }

        public abstract void DrawLine(XPoint start, XPoint end, XStyle style, double dx, double dy);

        public abstract void DrawRectangle(XPoint tl, XPoint br, XStyle style, double dx, double dy);

        public abstract void DrawEllipse(XPoint tl, XPoint br, XStyle style, double dx, double dy);
    }
}
