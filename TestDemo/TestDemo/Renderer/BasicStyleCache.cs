using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace TestDemo
{
    public class BasicStyleCache
    {
        public Brush FillBrush { get; private set; }
        public Brush StrokeBrush { get; private set; }
        public Pen StrokePen { get; private set; }
        public double Thickness { get; private set; }
        public double HalfThickness { get; private set; }
        
        public static SolidColorBrush ToBrush(Argb color)
        {
            return new SolidColorBrush(
                Color.FromArgb(
                    color.A,
                    color.R, 
                    color.G, 
                    color.B));
        }
        
        public BasicStyleCache(BasicStyle style)
        {
            Thickness = style.Thickness;
            HalfThickness = Thickness / 2.0;
            
            FillBrush = ToBrush(style.Fill);
            FillBrush.Freeze();
            
            StrokeBrush = ToBrush(style.Stroke);
            StrokeBrush.Freeze();
            
            StrokePen = new Pen(StrokeBrush, Thickness);
            StrokePen.Freeze();
        }
    }
}
