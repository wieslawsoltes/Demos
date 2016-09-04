#region References

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;

#endregion

namespace CustomDrawing.Core
{
    #region IPointer

    public interface IPointer
    {
        Point GetPosition();
        bool IsCaptured();
        void Capture();
        void Release();
    }

    #endregion
}
