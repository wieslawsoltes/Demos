using DemoDraw.Core.Project;

namespace DemoDraw.Core.Editor.Factory
{
    public interface IProjectFactory
    {
        XLayer CreateLayer();
        XPage CreatePage();
        XDocument CreateDocument();
        XProject CreateProject();
    }
}
