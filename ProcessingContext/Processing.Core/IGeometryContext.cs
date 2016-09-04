using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Processing.Core
{
    public interface IGeometryContext
    {
        void PushMatrix();
        void PopMatrix();

        void Move(double x, double y);
        void Rotate(double degree);
        void Scale(double x, double y);

        void Line(double x, double y, bool isStroked);
        void Arc(double x, double y, double width, double height, double angle, bool isStroked);
        void Bezier(double x, double y, double controlX, double controlY, bool isStroked);
        void Ellipse(double radiusX, double radiusY);
        void Rectangle(double radiusX, double radiusY, double radiusCornX, double radiusCornY);

        void Basis(double size);

        void Closed();
        void NotClosed();
        void Filled();
        void NotFilled();

        void EndFigure();
    }
}
