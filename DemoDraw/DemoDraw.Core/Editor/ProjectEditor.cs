using System;
using System.Collections.Generic;
using DemoDraw.Core.Editor.Factory;
using DemoDraw.Core.Project;

namespace DemoDraw.Core.Editor
{
    public class ProjectEditor
    {
        public IDictionary<Type, Tool> Tools { get; set; }
        public IProjectFactory Factory { get; set; }
        public Type CurrentTool { get; set; }

        public void Point(XPage page, double x, double y)
        {
            var tool = Tools[CurrentTool];
            tool.Page = page;
            tool.Point(x, y);
            tool.Page = null;
        }

        public void Cancel(XPage page)
        {
            var tool = Tools[CurrentTool];
            tool.Page = page;
            tool.Cancel();
            tool.Page = null;
        }

        public void Move(XPage page, double x, double y)
        {
            var tool = Tools[CurrentTool];
            tool.Page = page;
            tool.Move(x, y);
            tool.Page = null;
        }

        public static ProjectEditor Create()
        {
            return new ProjectEditor()
            {
                Tools = new Dictionary<Type, Tool>()
            };
        }
    }
}
