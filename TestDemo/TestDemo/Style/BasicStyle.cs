using System;

namespace TestDemo
{
    public class BasicStyle
    {
        public Argb Stroke { get; private set; }
        public Argb Fill { get; private set; }
        public double Thickness { get; private set; }
        
        public BasicStyle(Argb stroke, Argb fill, double thickness)
        {
            Stroke = stroke;
            Fill = fill;
            Thickness = thickness;
        }
    }
}
