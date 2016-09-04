#region References

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

#endregion

namespace CustomDrawing.Core
{
    #region Rectangle

    public class Rectangle : Element
    {
        public Pin TopLeft { get; set; }
        public Pin BottomRight { get; set; }
    }

    #endregion
}
