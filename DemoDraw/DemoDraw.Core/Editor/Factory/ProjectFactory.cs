using DemoDraw.Core.Project;
using DemoDraw.Core.Shapes;
using DemoDraw.Core.Style;

namespace DemoDraw.Core.Editor.Factory
{
    public class ProjectFactory : IProjectFactory
    {
        public XLayer CreateLayer()
        {
            var layer = XLayer.Create();

            return layer;
        }

        public XPage CreatePage()
        {
            var page = XPage.Create();

            page.Background = XColor.Create(0xFF, 0xA9, 0xA9, 0xA9);
            page.Width = 800.0;
            page.Height = 600.0;

            var currentLayer = XLayer.Create();
            page.CurrentLayer = currentLayer;
            page.Layers.Add(currentLayer);

            var currentStyle = XStyle.Create(XColor.Create(255, 255, 0, 0), XColor.Create(192, 255, 255, 0), 2.0);
            page.CurrentStyle = currentStyle;
            page.Styles.Add(page.CurrentStyle);

            var pointTemplateStyle = XStyle.Create(XColor.Create(255, 0, 0, 255), XColor.Create(255, 0, 0, 255), 2.0);
            var pointTemplate = XGroup.Create();
            pointTemplate.Shapes.Add(XLine.Create(-4, -4, 4, 4, pointTemplateStyle));
            pointTemplate.Shapes.Add(XLine.Create(4, -4, -4, 4, pointTemplateStyle));
            page.CurrentPointTemplate = pointTemplate;
            page.PointTemplates.Add(page.CurrentPointTemplate);

            return page;
        }

        public XDocument CreateDocument()
        {
            var document = XDocument.Create();

            return document;
        }

        public XProject CreateProject()
        {
            var project = XProject.Create();

            return project;
        }

        public static ProjectFactory Create()
        {
            return new ProjectFactory();
        }
    }
}
