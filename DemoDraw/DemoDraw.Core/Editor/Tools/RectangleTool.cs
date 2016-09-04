using DemoDraw.Core.Shapes;

namespace DemoDraw.Core.Editor.Tools
{
    public class RectangleTool : Tool
    {
        public override string Name 
        { 
            get { return "Rectangle"; } 
        }
        
        public enum ToolState { None, TopLeft };

        public ToolState CurrentState { get; set; }
        public XRectangle Rectangle { get; set; }

        public override void Point(double x, double y)
        {
            switch (CurrentState) 
            {
                case ToolState.None:
                    Rectangle = XRectangle.Create(x,  y, Page.CurrentStyle);
                    Rectangle.TopLeft.Template = Page.CurrentPointTemplate;
                    Rectangle.BottomRight.Template = Page.CurrentPointTemplate;
                    Page.CurrentLayer.Shapes.Add(Rectangle);
                    CurrentState = ToolState.TopLeft;
                    break;
                case ToolState.TopLeft:
                    Rectangle.BottomRight.X = x;
                    Rectangle.BottomRight.Y = y;
                    Rectangle = null;
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
                    Page.CurrentLayer.Shapes.Remove(Rectangle);
                    Rectangle = null;
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
                    Rectangle.BottomRight.X = x;
                    Rectangle.BottomRight.Y = y;
                    break;
            }
        }
        
        public static RectangleTool Create()
        {
            return new RectangleTool()
            {
                CurrentState = ToolState.None,
                Rectangle = null
            };
        }
    }
}
