using System.Windows;
using DemoDraw.Core.Editor;
using DemoDraw.Core.Editor.Factory;
using DemoDraw.Core.Editor.Tools;
using DemoDraw.Core.Renderer;
using DemoDraw.Editor;

namespace DemoDraw
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var window = new MainWindow();
            var editor = (ProjectEditor)window.GetValue(EditorContext.EditorProperty);
            var renderer = (ShapeRenderer)window.GetValue(EditorContext.RendererProperty);

            // Factory

            editor.Factory = ProjectFactory.Create();

            // Tools

            editor.Tools.Add(typeof(NoneTool), NoneTool.Create());
            editor.Tools.Add(typeof(SelectionTool), SelectionTool.Create());
            editor.Tools.Add(typeof(LineTool), LineTool.Create());
            editor.Tools.Add(typeof(RectangleTool), RectangleTool.Create());
            editor.Tools.Add(typeof(EllipseTool), EllipseTool.Create());

            // Current

            editor.CurrentTool = typeof(RectangleTool);

            // Pages

            window.Page1.SetValue(EditorContext.PageProperty, editor.Factory.CreatePage());
            window.Page2.SetValue(EditorContext.PageProperty, editor.Factory.CreatePage());

            window.ShowDialog();
        }
    }
}
