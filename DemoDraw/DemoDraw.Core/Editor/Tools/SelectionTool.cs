
namespace DemoDraw.Core.Editor.Tools
{
    public class SelectionTool : Tool
    {
        public override string Name
        {
            get { return "Selection"; }
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

        public static SelectionTool Create()
        {
            return new SelectionTool();
        }
    }
}
