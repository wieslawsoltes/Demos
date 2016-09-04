using DemoDraw.Core.Shapes;

namespace DemoDraw.Core.Editor.Tools
{
    public class EllipseTool : Tool
    {
        public override string Name
        {
            get { return "Ellipse"; }
        }

        public enum ToolState { None, TopLeft };

        public ToolState CurrentState { get; set; }
        public XEllipse Ellipse { get; set; }

        public override void Point(double x, double y)
        {
            switch (CurrentState)
            {
                case ToolState.None:
                    Ellipse = XEllipse.Create(x, y, Page.CurrentStyle);
                    Ellipse.TopLeft.Template = Page.CurrentPointTemplate;
                    Ellipse.BottomRight.Template = Page.CurrentPointTemplate;
                    Page.CurrentLayer.Shapes.Add(Ellipse);
                    CurrentState = ToolState.TopLeft;
                    break;
                case ToolState.TopLeft:
                    Ellipse.BottomRight.X = x;
                    Ellipse.BottomRight.Y = y;
                    Ellipse = null;
                    CurrentState = ToolState.None;
                    break;
            }
        }

        public override void Cancel()
        {
            switch (CurrentState)
            {
                case ToolState.None:
                    break;
                case ToolState.TopLeft:
                    Page.CurrentLayer.Shapes.Remove(Ellipse);
                    Ellipse = null;
                    CurrentState = ToolState.None;
                    break;
            }
        }

        public override void Move(double x, double y)
        {
            switch (CurrentState)
            {
                case ToolState.None:
                    break;
                case ToolState.TopLeft:
                    Ellipse.BottomRight.X = x;
                    Ellipse.BottomRight.Y = y;
                    break;
            }
        }

        public static EllipseTool Create()
        {
            return new EllipseTool()
            {
                CurrentState = ToolState.None,
                Ellipse = null
            };
        }
    }
}
