#region References

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

#endregion

namespace CustomDrawing.Core
{
    #region PinTypes

    public enum PinTypes : int
    {
        None = 0,
        Origin = 1,
        Snap = 2,
        Connector = 4,
        Guide = 8
    }

    #endregion
}
