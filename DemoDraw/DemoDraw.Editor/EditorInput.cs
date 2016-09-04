using System.Windows;
using System.Windows.Input;
using DemoDraw.Core.Editor;
using DemoDraw.Core.Editor.Tools;
using DemoDraw.Core.Project;

namespace DemoDraw.Editor
{
    public class EditorInput
    {
        public void Point(MouseButtonEventArgs e, FrameworkElement surface)
        {
            var editor = (ProjectEditor)surface.GetValue(EditorContext.EditorProperty);
            var page = (XPage)surface.GetValue(EditorContext.PageProperty);
            if (editor != null && page != null)
            {
                var point = e.GetPosition(surface);
                editor.Point(page, point.X, point.Y);
                surface.InvalidateVisual();
            }
        }

        public void Cancel(MouseButtonEventArgs e, FrameworkElement surface)
        {
            var editor = (ProjectEditor)surface.GetValue(EditorContext.EditorProperty);
            var page = (XPage)surface.GetValue(EditorContext.PageProperty);
            if (editor != null)
            {
                editor.Cancel(page);
                surface.InvalidateVisual();
            }
        }

        public void Move(MouseEventArgs e, FrameworkElement surface)
        {
            var editor = (ProjectEditor)surface.GetValue(EditorContext.EditorProperty);
            var page = (XPage)surface.GetValue(EditorContext.PageProperty);
            if (editor != null)
            {
                var point = e.GetPosition(surface);
                editor.Move(page, point.X, point.Y);
                surface.InvalidateVisual();
            }
        }

        public void Type(KeyEventArgs e, FrameworkElement surface)
        {
            var editor = (ProjectEditor)surface.GetValue(EditorContext.EditorProperty);
            if (editor != null && Keyboard.Modifiers == ModifierKeys.None)
            {
                switch (e.Key)
                {
                    case Key.N:
                        editor.CurrentTool = typeof(NoneTool);
                        break;
                    case Key.S:
                        editor.CurrentTool = typeof(SelectionTool);
                        break;
                    case Key.L:
                        editor.CurrentTool = typeof(LineTool);
                        break;
                    case Key.R:
                        editor.CurrentTool = typeof(RectangleTool);
                        break;
                    case Key.E:
                        editor.CurrentTool = typeof(EllipseTool);
                        break;
                }
            }
        }
    }
}
