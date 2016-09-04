using System.Collections.Generic;

namespace DemoDraw.Core.Project
{
    public class XProject
    {
        public IList<XDocument> Documents { get; set; }

        public static XProject Create()
        {
            return new XProject()
            {
                Documents = new List<XDocument>()
            };
        }
    }
}
