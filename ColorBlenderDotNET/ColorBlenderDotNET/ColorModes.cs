using System;
using System.Windows.Media;

namespace ColorBlenderDotNET
{
    public static class ColorConversion
    {
        public static RGB ColorToRGB(Color c)
        {
            RGB rgb = new RGB(c.R, c.G, c.B);
            return rgb;
        }

        public static HSV ColorToHSV(Color c)
        {
            return ColorToRGB(c).ToHSV();
        }

        public static Color HSVtoColor(HSV hs)
        {
            RGB rgb = hs.ToRGB();
            return RGBtoColor(rgb);
        }

        public static Color RGBtoColor(RGB rgb)
        {
            return Color.FromRgb((byte)Math.Round(rgb.r), (byte)Math.Round(rgb.g), (byte)Math.Round(rgb.b));
        }
    }

    public class HSV
    {
        public double h;
        public double s;
        public double v;

        public HSV() { }

        public HSV(double h, double s, double v)
        {
            this.h = h;
            this.s = s;
            this.v = v;
        }

        public HSV(HSV hs)
        {
            this.h = hs.h;
            this.s = hs.s;
            this.v = hs.v;
        }

        public HSV(RGB rg)
        {
            HSV hs = rg.ToHSV();
            this.h = hs.h;
            this.s = hs.s;
            this.v = hs.v;
        }

        public RGB ToRGB()
        {
            // Converts an HSV color object to a RGB color object
            RGB rg = new RGB();
            HSV hsx = new HSV(this.h, this.s, this.v);

            if (hsx.s == 0)
            {
                rg.r = rg.g = rg.b = Math.Round(hsx.v * 2.55); return (rg);
            }

            hsx.s = hsx.s / 100;
            hsx.v = hsx.v / 100;
            hsx.h /= 60;

            var i = Math.Floor(hsx.h);
            var f = hsx.h - i;
            var p = hsx.v * (1 - hsx.s);
            var q = hsx.v * (1 - hsx.s * f);
            var t = hsx.v * (1 - hsx.s * (1 - f));

            switch ((int)i)
            {
                case 0: rg.r = hsx.v; rg.g = t; rg.b = p; break;
                case 1: rg.r = q; rg.g = hsx.v; rg.b = p; break;
                case 2: rg.r = p; rg.g = hsx.v; rg.b = t; break;
                case 3: rg.r = p; rg.g = q; rg.b = hsx.v; break;
                case 4: rg.r = t; rg.g = p; rg.b = hsx.v; break;
                default: rg.r = hsx.v; rg.g = p; rg.b = q; break;
            }

            rg.r = Math.Round(rg.r * 255);
            rg.g = Math.Round(rg.g * 255);
            rg.b = Math.Round(rg.b * 255);

            return rg;
        }
    }

    public class RGB
    {
        public double r;
        public double g;
        public double b;

        public RGB() { }

        public RGB(double r, double g, double b)
        {
            this.r = r;
            this.g = g;
            this.b = b;
        }

        public RGB(RGB rg)
        {
            this.r = rg.r;
            this.g = rg.g;
            this.b = rg.b;
        }

        public RGB(HSV hs)
        {
            RGB rg = hs.ToRGB();
            this.r = rg.r;
            this.g = rg.g;
            this.b = rg.b;
        }

        public HSV ToHSV()
        {
            // Converts an RGB color object to a HSV color object
            HSV hs = new HSV();
            RGB rg = new RGB(this.r, this.g, this.b);

            var m = rg.r;
            if (rg.g < m) { m = rg.g; }
            if (rg.b < m) { m = rg.b; }
            var v = rg.r;
            if (rg.g > v) { v = rg.g; }
            if (rg.b > v) { v = rg.b; }
            var value = 100 * v / 255;
            var delta = v - m;
            if (v == 0.0) { hs.s = 0; } else { hs.s = 100 * delta / v; }

            if (hs.s == 0) { hs.h = 0; }
            else
            {
                if (rg.r == v) { hs.h = 60.0 * (rg.g - rg.b) / delta; }
                else if (rg.g == v) { hs.h = 120.0 + 60.0 * (rg.b - rg.r) / delta; }
                else if (rg.b == v) { hs.h = 240.0 + 60.0 * (rg.r - rg.g) / delta; }
                if (hs.h < 0.0) { hs.h = hs.h + 360.0; }
            }

            hs.h = Math.Round(hs.h);
            hs.s = Math.Round(hs.s);
            hs.v = Math.Round(value);

            return hs;
        }
    }
}
