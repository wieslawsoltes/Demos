using DemoDraw.Core.Renderer;

namespace DemoDraw.Core
{
    public abstract class XDrawable
    {
        public abstract void Draw(ShapeRenderer renderer, double dx, double dy);
    }
}
