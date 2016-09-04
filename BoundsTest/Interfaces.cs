using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoundsTest
{
    public interface IBounds
    {
        void Update();
        bool IsVisible();
        void Show();
        void Hide();
        bool Contains(double x, double y);
    }

    public interface IPoint
    {
        double X { get; set; }
        double Y { get; set; }
    }

    public interface ILine
    {
        object Native { get; set; }
        IBounds Bounds { get; set; }
        void Move(IPoint p1, IPoint p2);
    }

    public interface IEllipse
    {
        object Native { get; set; }
        IBounds Bounds { get; set; }
        double X { get; set; }
        double Y { get; set; }
        double Width { get; set; }
        double Height { get; set; }
    }

    public interface IPolygon
    {
        IPoint[] Points { get; set; }
        ILine[] Lines { get; set; }
        bool Contains(IPoint point);
        bool Contains(double x, double y);
    }

    public interface ICanvas
    {
        object Native { get; set; }
    }
}
