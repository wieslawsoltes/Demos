
namespace DemoDraw.Core.Editor.Tools
{
    public class NoneTool : Tool
    {
        public override string Name
        {
            get { return "None"; }
        }

        public override void Point(double x, double y)
        {
        }

        public override void Cancel()
        {
        }

        public override void Move(double x, double y)
        {
        }

        public static NoneTool Create()
        {
            return new NoneTool();
        }
    }
}
