
namespace Logic.Model
{
    #region References

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    #endregion

    #region Project

    public class Project : Element
    {
        public Project()
            : base()
        {
            this.Children = new List<Element>();
        }
    }

    #endregion
}
