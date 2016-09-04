#region References

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

#endregion

namespace CustomDrawing.Core
{
    #region Element

    public abstract partial class Element
    {
        public int Id { get; set; }
        public Element Parent { get; set; }
        public bool IsSelected { get; set; }
        public bool UseTransforms { get; set; }
        public float Angle { get; set; }
    }

    #endregion
}
