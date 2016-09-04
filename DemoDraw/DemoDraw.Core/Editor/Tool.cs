using DemoDraw.Core.Project;

namespace DemoDraw.Core.Editor
{
    public abstract class Tool
    {
        public abstract string Name { get; }
        public XPage Page { get; set; }
        public abstract void Point(double x, double y);
        public abstract void Cancel();
        public abstract void Move(double x, double y);
    }
}
