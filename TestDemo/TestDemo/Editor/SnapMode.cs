using System;

namespace TestDemo
{
    [Flags]
    public enum SnapMode
    {
        None = 0,
        Point = 1,
        Middle = 2,
        Nearest = 4,
        Intersection = 8,
        Horizontal = 16,
        Vertical = 32
    }
}
