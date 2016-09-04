
namespace Logic.Model
{
    #region References

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    #endregion

    #region Element

    public abstract class Element
    {
        public Element() { }
        public UInt32 Id { get; set; }
        public Element Parent { get; set; }
        public List<Element> Children { get; set; }
    }

    #endregion
}
