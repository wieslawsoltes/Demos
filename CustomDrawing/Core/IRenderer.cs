#region References

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

#endregion

namespace CustomDrawing.Core
{
    #region IRenderer

    public interface IRenderer
    {
        #region Transform

        void Transform(float x, float y, out float tx, out float ty);
        void PushRotate();
        void Pop();
        void SetAngle(float angle, float cx, float cy);
        float GetAngle();
        float GetCenterX();
        float GetCenterY();

        #endregion

        #region Draw

        void Draw(Pin pin, float x, float y, IDictionary<int, string> variables);
        void Draw(Line line, float x, float y, IDictionary<int, string> variables);
        void Draw(Rectangle rectangle, float x, float y, IDictionary<int, string> variables);
        void Draw(Text text, float x, float y, IDictionary<int, string> variables);

        #endregion
    }

    #endregion
}
