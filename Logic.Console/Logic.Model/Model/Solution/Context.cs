
namespace Logic.Model
{
    #region References

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    #endregion

    #region Context

    public class Context : Element
    {
        public Context()
            : base()
        {
            this.Children = new List<Element>();
        }
    }

    #endregion
}
