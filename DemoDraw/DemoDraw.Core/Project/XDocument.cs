using System.Collections.Generic;

namespace DemoDraw.Core.Project
{
    public class XDocument
    {
        public IList<XPage> Pages { get; set; }

        public static XDocument Create()
        {
            return new XDocument()
            {
                Pages = new List<XPage>()
            };
        }
    }
}
