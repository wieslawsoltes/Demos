using System.Windows;
using DemoDraw.Core.Editor;
using DemoDraw.Core.Project;
using DemoDraw.Core.Renderer;

namespace DemoDraw.Editor
{
    public class EditorContext : DependencyObject
    {
        // Page

        public static XPage GetPage(DependencyObject obj)
        {
            return (XPage)obj.GetValue(PageProperty);
        }

        public static void SetPage(DependencyObject obj, XPage value)
        {
            obj.SetValue(PageProperty, value);
        }

        public static readonly DependencyProperty PageProperty =
            DependencyProperty.RegisterAttached(
                "Page",
                typeof(XPage),
                typeof(EditorContext),
                new FrameworkPropertyMetadata(
                    null,
                    FrameworkPropertyMetadataOptions.Inherits));

        // Editor

        public static ProjectEditor GetEditor(DependencyObject obj)
        {
            return (ProjectEditor)obj.GetValue(EditorProperty);
        }

        public static void SetEditor(DependencyObject obj, ProjectEditor value)
        {
            obj.SetValue(EditorProperty, value);
        }

        public static readonly DependencyProperty EditorProperty =
            DependencyProperty.RegisterAttached(
                "Editor",
                typeof(ProjectEditor),
                typeof(EditorContext),
                new FrameworkPropertyMetadata(
                    ProjectEditor.Create(),
                    FrameworkPropertyMetadataOptions.Inherits));

        // Renderer

        public static ShapeRenderer GetRenderer(DependencyObject obj)
        {
            return (ShapeRenderer)obj.GetValue(RendererProperty);
        }

        public static void SetRenderer(DependencyObject obj, ShapeRenderer value)
        {
            obj.SetValue(RendererProperty, value);
        }

        public static readonly DependencyProperty RendererProperty =
            DependencyProperty.RegisterAttached(
                "Renderer",
                typeof(ShapeRenderer),
                typeof(EditorContext),
                new FrameworkPropertyMetadata(
                    WpfRenderer.Create(),
                    FrameworkPropertyMetadataOptions.Inherits));
    }
}
