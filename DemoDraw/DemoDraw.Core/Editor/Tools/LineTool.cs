using DemoDraw.Core.Shapes;

namespace DemoDraw.Core.Editor.Tools
{
    public class LineTool : Tool
    {
        public override string Name 
        { 
            get { return "Line"; } 
        }
        
        public enum ToolState { None, Start };

        public ToolState CurrentState { get; set; }
        public XLine Line { get; set; }

        public override void Point(double x, double y)
        {
            switch (CurrentState) 
            {
                case ToolState.None:
                    Line = XLine.Create(x,  y, Page.CurrentStyle);
                    Line.Start.Template = Page.CurrentPointTemplate;
                    Line.End.Template = Page.CurrentPointTemplate;
                    Page.CurrentLayer.Shapes.Add(Line);
                    CurrentState = ToolState.Start;
                    break;
                case ToolState.Start:
                    Line.End.X = x;
                    Line.End.Y = y;
                    Line = null;
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
                case ToolState.Start:
                    Page.CurrentLayer.Shapes.Remove(Line);
                    Line = null;
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
                case ToolState.Start:
                    Line.End.X = x;
                    Line.End.Y = y;
                    break;
            }
        }
        
        public static LineTool Create()
        {
            return new LineTool()
            {
                CurrentState = ToolState.None,
                Line = null
            };
        }
    }
}
