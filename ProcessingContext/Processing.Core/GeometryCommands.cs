using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Processing.Core
{
    public class GeometryCommands
    {
        private IGeometryContext _context;

        public GeometryCommands(IGeometryContext context)
        {
            _context = context;
        }

        [GeometryCommand("PUSH")]
        public void PushMatrix()
        {
            _context.PushMatrix();
        }

        [GeometryCommand("POP")]
        public void PopMatrix()
        {
            _context.PopMatrix();
        }

        [GeometryCommand("M")]
        public void Move(double x)
        {
            _context.Move(x, 0);
        }

        [GeometryCommand("M")]
        public void Move(double x, double y)
        {
            _context.Move(x, y);
        }

        [GeometryCommand("ROT")]
        public void Rotate(double degree)
        {
            _context.Rotate(degree);
        }

        [GeometryCommand("S")]
        public void Scale(double factor)
        {
            _context.Scale(factor, factor);
        }

        [GeometryCommand("S")]
        public void Scale(double x, double y)
        {
            _context.Scale(x, y);
        }

        [GeometryCommand("L")]
        public void Line(double x)
        {
            _context.Line(x, 0, true);
        }

        [GeometryCommand("L")]
        public void Line(double x, double y)
        {
            _context.Line(x, y, true);
        }

        [GeometryCommand("L")]
        public void Line(double x, double y, bool isStroked)
        {
            _context.Line(x, y, isStroked);
        }

        [GeometryCommand("A")]
        public void Arc(double x, double y, double width, double height)
        {
            _context.Arc(x, y, width, height, 0, true);
        }

        [GeometryCommand("A")]
        public void Arc(double x, double y, double width, double height, double angle)
        {
            _context.Arc(x, y, width, height, angle, true);
        }

        [GeometryCommand("A")]
        public void Arc(double x, double y, double width, double height, double angle, bool isStroked = true)
        {
            _context.Arc(x, y, width, height, angle, isStroked);
        }

        [GeometryCommand("BEZ")]
        public void Bezier(double x, double y, double controlX, double controlY, bool isStroked = true)
        {
            _context.Bezier(x, y, controlX, controlY, isStroked);
        }

        [GeometryCommand("E")]
        public void Ellipse(double radius)
        {
            _context.Ellipse(radius, radius);
        }

        [GeometryCommand("E")]
        public void Ellipse(double radiusX, double radiusY)
        {
            _context.Ellipse(radiusX, radiusY);
        }

        [GeometryCommand("R")]
        public void Rectangle(double radius)
        {
            _context.Rectangle(radius, radius, 0.0, 0.0);
        }

        [GeometryCommand("R")]
        public void Rectangle(double radiusX, double radiusY)
        {
            _context.Rectangle(radiusX, radiusY, 0.0, 0.0);
        }

        [GeometryCommand("R")]
        public void Rectangle(double radiusX, double radiusY, double radiusCornX, double radiusCornY)
        {
            _context.Rectangle(radiusX, radiusY, radiusCornX, radiusCornY);
        }

        [GeometryCommand("BASIS")]
        public void Basis(double size)
        {
            _context.Basis(size);
        }

        [GeometryCommand("C")]
        public void Closed()
        {
            _context.Closed();
        }

        [GeometryCommand("NC")]
        public void NotClosed()
        {
            _context.NotClosed();
        }

        [GeometryCommand("F")]
        public void Filled()
        {
            _context.Filled();
        }

        [GeometryCommand("NF")]
        public void NotFilled()
        {
            _context.NotFilled();
        }

        [GeometryCommand("Z")]
        public void EndFigure()
        {
            _context.EndFigure();
        }
    }
}
