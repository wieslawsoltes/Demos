#region References

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

#endregion

namespace CustomDrawing.Core
{
    #region Selected

    public struct Selected
    {
        public readonly Pin Pin;
        public readonly float X;
        public readonly float Y;
        public readonly float Angle;
        public readonly float CenterX;
        public readonly float CenterY;
        public Selected(Pin pin, float x, float y, float angle, float cx, float cy)
        {
            Pin = pin;
            X = x;
            Y = y;
            Angle = angle;
            CenterX = cx;
            CenterY = cy;
        }
    }

    #endregion
}
