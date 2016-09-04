#region References

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

#endregion

namespace CustomDrawing.Core
{
    #region Line

    public class Line : Element
    {
        public Pin Start { get; set; }
        public Pin End { get; set; }
    }

    #endregion
}
