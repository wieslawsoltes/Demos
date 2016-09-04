#region References

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

#endregion

namespace CustomDrawing.Core
{
    #region EditMode

    public static class EditMode
    {
        public const int None = 0;
        public const int HitTest = 1;
        public const int Create = 2;
        public const int Move = 3;
    }

    #endregion
}
