
namespace Logic.Model
{
    #region References

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    #endregion

    #region Wire

    public class Wire : Element
    {
        public Wire()
            : base()
        {
        }

        public Pin Start { get; set; }
        public Pin End { get; set; }

        public bool InvertStart { get; set; }
        public bool InvertEnd { get; set; }
    }

    #endregion
}
