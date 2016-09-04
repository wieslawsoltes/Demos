
using System;
namespace ColorBlenderDotNET
{
    public class MatchColors
    {
        public string Name;
        public HSV[] hsv = new HSV[6];
    }

    public static class ColorMatch
    {
        static double rc(double x, double m)
        {
            if(x>m) { return m; }
            if (x < 0) { return 0; } else { return x; }
        }

        static double hueToWheel(double h)
        {
            if(h<=120)
            {
                return(Math.Round(h*1.5));
            }
            else
            {
                return(Math.Round(180+(h-120)*0.75));
            }
        }

        static double wheelToHue(double w)
        {
            if(w<=180)
            {
                return(Math.Round(w/1.5));
            }
            else
            {
                return(Math.Round(120+(w-180)/0.75));
            }
        }

        /* Color Matching Algorithm
            "classic"               ColorMatch 5K Classic
            "colorexplorer"         ColorExplorer - "Sweet Spot Offset"
            "singlehue"             Single Hue
            "complementary"         Complementary
            "splitcomplementary"    Split-Complementary
            "analogue"              Analogue
            "triadic"               Triadic
            "square"                Square
        */

        public static MatchColors DoColorMatchAlgorithm(RGB rg, string method)
        {
            HSV hs = rg.ToHSV();
            return DoColorMatchAlgorithm(hs, method);
        }

        public static MatchColors DoColorMatchAlgorithm(HSV hs, string method)
        {
            // Color matching algorithm. All work is done in HSV color space, because all
            // calculations are based on hue, saturation and value of the working color.
            // The hue spectrum is divided into sections, are the matching colors are
            // calculated differently depending on the hue of the color.
            // input: hs = a HSV style color object
            MatchColors outp = new MatchColors();
            HSV y=new HSV();
            HSV yx=new HSV();

            outp.hsv[0] = new HSV(hs);

            switch(method)
            {
            case "classic": // colormatch classic
                {
                    y.s=hs.s;
                    y.h=hs.h;
                    if(hs.v>70){y.v=hs.v-30;}else{y.v=hs.v+30;};
                    outp.hsv[1] = new HSV(y);

                    if((hs.h>=0)&&(hs.h<30))
                    {
                        yx.h=y.h=hs.h+30;yx.s=y.s=hs.s;y.v=hs.v;
                        if(hs.v>70){yx.v=hs.v-30;}else{yx.v=hs.v+30;}
                    }

                    if((hs.h>=30)&&(hs.h<60))
                    {
                        yx.h=y.h=hs.h+150;
                        y.s=rc(hs.s-30,100);
                        y.v=rc(hs.v-20,100);
                        yx.s=rc(hs.s-50,100);
                        yx.v=rc(hs.v+20,100);
                    }

                    if((hs.h>=60)&&(hs.h<180))
                    {
                        yx.h=y.h=hs.h-40;
                        y.s=yx.s=hs.s;
                        y.v=hs.v;if(hs.v>70){yx.v=hs.v-30;}else{yx.v=hs.v+30;}
                    }

                    if((hs.h>=180)&&(hs.h<220))
                    {
                        yx.h=hs.h-170;
                        y.h=hs.h-160;
                        yx.s=y.s=hs.s;
                        y.v=hs.v;
                        if(hs.v>70){yx.v=hs.v-30;}else{yx.v=hs.v+30;}

                    }
                    if((hs.h>=220)&&(hs.h<300))
                    {
                        yx.h=y.h=hs.h;
                        yx.s=y.s=rc(hs.s-40,100);
                        y.v=hs.v;
                        if(hs.v>70){yx.v=hs.v-30;}else{yx.v=hs.v+30;}
                    }
                    if(hs.h>=300)
                    {
                        if(hs.s>50){y.s=yx.s=hs.s-40;}else{y.s=yx.s=hs.s+40;}yx.h=y.h=(hs.h+20)%360;
                        y.v=hs.v;
                        if (hs.v > 70) { yx.v = hs.v - 30; } else { yx.v = hs.v + 30; }
                    }

                    outp.hsv[2] = new HSV(y);
                    outp.hsv[3] = new HSV(yx);

                    y.h=0;
                    y.s=0;
                    y.v=100-hs.v;
                    outp.hsv[4] = new HSV(y);

                    y.h=0;
                    y.s=0;
                    y.v=hs.v;
                    outp.hsv[5] = new HSV(y);
                }
                break;

            case "colorexplorer": // colorexplorer
                {
                    HSV z = new HSV();

                    z.h = hs.h;
                    z.s = Math.Round(hs.s * 0.3);
                    z.v = Math.Min(Math.Round(hs.v * 1.3),100);
                    outp.hsv[1] = new HSV(z);

                    z = new HSV();
                    z.h = (hs.h+300)%360;
                    z.s = hs.s;
                    z.v = hs.v;
                    outp.hsv[3] = new HSV(z);

                    z.s = Math.Min(Math.Round(z.s * 1.2),100);
                    z.v = Math.Min(Math.Round(z.v * 0.5),100);
                    outp.hsv[2] = new HSV(z);

                    z.s = 0;
                    z.v = (hs.v + 50) % 100;
                    outp.hsv[4] = new HSV(z);

                    z.v = (z.v + 50) % 100;
                    outp.hsv[5] = new HSV(z);
                }
                break;

            case "singlehue": // single hue
                {
                    HSV z = new HSV();

                    z.h = hs.h;
                    z.s = hs.s;
                    z.v = hs.v+((hs.v<50)?20:-20);
                    outp.hsv[1] = new HSV(z);

                    z.s = hs.s;
                    z.v = hs.v+((hs.v<50)?40:-40);
                    outp.hsv[2] = new HSV(z);

                    z.s = hs.s+((hs.s<50)?20:-20);
                    z.v = hs.v;
                    outp.hsv[3] = new HSV(z);

                    z.s = hs.s+((hs.s<50)?40:-40);
                    z.v = hs.v;
                    outp.hsv[4] = new HSV(z);

                    z.s = hs.s+((hs.s<50)?40:-40);
                    z.v = hs.v+((hs.v<50)?40:-40);
                    outp.hsv[5] = new HSV(z);
                }
                break;

            case "complementary": // complementary      
                {
                    HSV z = new HSV();

                    z.h = hs.h;
                    z.s = (hs.s>50)?(hs.s * 0.5):(hs.s * 2);
                    z.v = (hs.v<50)?(Math.Min(hs.v*1.5,100)):(hs.v/1.5);
                    outp.hsv[1] = new HSV(z);

                    var w = hueToWheel(hs.h);
                    z.h = wheelToHue((w+180)%360);
                    z.s = hs.s;
                    z.v = hs.v;
                    outp.hsv[2] = new HSV(z); 

                    z.s = (z.s>50)?(z.s * 0.5):(z.s * 2);
                    z.v = (z.v<50)?(Math.Min(z.v*1.5,100)):(z.v/1.5);
                    outp.hsv[3] = new HSV(z);

                    z = new HSV();
                    z.s = 0;
                    z.h = 0;
                    z.v = hs.v;
                    outp.hsv[4] = new HSV(z);        

                    z.v = 100 - hs.v;
                    outp.hsv[5] = new HSV(z);
                }
                break;

            case "splitcomplementary": // splitcomplementary
                {
                    var w = hueToWheel(hs.h);
                    HSV z = new HSV();

                    z.h = hs.h;
                    z.s = hs.s;
                    z.v = hs.v;

                    z.h = wheelToHue((w+150)%360);
                    z.s = hs.s;
                    z.v = hs.v;
                    outp.hsv[1] = new HSV(z);

                    z.h = wheelToHue((w+210)%360);
                    z.s = hs.s;
                    z.v = hs.v;
                    outp.hsv[2] = new HSV(z);

                    z.s = 0;
                    z.v = hs.s;
                    outp.hsv[3] = new HSV(z);

                    z.s = 0;
                    z.v = hs.v;
                    outp.hsv[4] = new HSV(z);

                    z.s = 0;
                    z.v = (100 - hs.v);
                    outp.hsv[5] = new HSV(z);
                }
                break;

            case "analogue": // analogue
                {
                    var w = hueToWheel(hs.h);
                    HSV z = new HSV();

                    z.h = wheelToHue((w+30)%360);
                    z.s = hs.s;
                    z.v = hs.v;
                    outp.hsv[1] = new HSV(z);

                    z = new HSV();
                    z.h = wheelToHue((w+60)%360);
                    z.s = hs.s;
                    z.v = hs.v;
                    outp.hsv[2] = new HSV(z);

                    z = new HSV();
                    z.s = 0;
                    z.h = 0;
                    z.v = 100 - hs.v;
                    outp.hsv[3] = new HSV(z);

                    z.v = Math.Round(hs.v * 1.3) % 100;
                    outp.hsv[4] = new HSV(z);

                    z.v = Math.Round(hs.v / 1.3) % 100;
                    outp.hsv[5] = new HSV(z);
                }
                break;

            case "triadic": // triadic
                {
                    var w = hueToWheel(hs.h);
                    HSV z = new HSV();

                    z.s = hs.s;
                    z.h = hs.h;
                    z.v = 100 - hs.v;
                    outp.hsv[1] = new HSV(z);

                    z = new HSV();
                    z.h = wheelToHue((w+120)%360);
                    z.s = hs.s;
                    z.v = hs.v;
                    outp.hsv[2] = new HSV(z);

                    z.v = 100 - z.v;
                    outp.hsv[3] = new HSV(z);

                    z = new HSV();
                    z.h = wheelToHue((w+240)%360);
                    z.s = hs.s;
                    z.v = hs.v;
                    outp.hsv[4] = new HSV(z);

                    z.v = 100 - z.v;
                    outp.hsv[5] = new HSV(z);
                }
                break;

            case "square": // square
                {
                    var w = hueToWheel(hs.h);
                    HSV z = new HSV();

                    z.h = wheelToHue((w+90)%360);
                    z.s = hs.s;
                    z.v = hs.v;
                    outp.hsv[1] = new HSV(z);

                    z.h = wheelToHue((w+180)%360);
                    z.s = hs.s;
                    z.v = hs.v;
                    outp.hsv[2] = new HSV(z);

                    z.h = wheelToHue((w+270)%360);
                    z.s = hs.s;
                    z.v = hs.v;
                    outp.hsv[3] = new HSV(z);

                    z.s = 0;
                    outp.hsv[4] = new HSV(z);

                    z.v = 100 - z.v;
                    outp.hsv[5] = new HSV(z);
                }
                break;
            }

            return outp;
        }

        public static MatchColors DoMatch(RGB rg)
        {
            HSV hs = rg.ToHSV();
            return DoMatch(hs);
        }

        public static MatchColors DoMatch(HSV hs)
        {
            // Color matching algorithm. All work is done in HSV color space, because all
            // calculations are based on hue, saturation and value of the working color.
            // The hue spectrum is divided into sections, are the matching colors are
            // calculated differently depending on the hue of the color.

            MatchColors z = new MatchColors();
            HSV y=new HSV();
            HSV yx=new HSV();

            z.hsv[0] = new HSV(hs);

            y.s=hs.s;
            y.h=hs.h;

            if(hs.v>70){y.v=hs.v-30;}else{y.v=hs.v+30;}

            z.hsv[1] = new HSV(y);

            if((hs.h>=0)&&(hs.h<30)){
                yx.h=y.h=hs.h+30;yx.s=y.s=hs.s;y.v=hs.v;
                if(hs.v>70){yx.v=hs.v-30;}else{yx.v=hs.v+30;}
            }

            if((hs.h>=30)&&(hs.h<60)){yx.h=y.h=hs.h+150;
                y.s=rc(hs.s-30,100);
                y.v=rc(hs.v-20,100);
                yx.s=rc(hs.s-50,100);
                yx.v=rc(hs.v+20,100);
            }

            if((hs.h>=60)&&(hs.h<180)){
                yx.h=y.h=hs.h-40;
                y.s=yx.s=hs.s;
                y.v=hs.v;if(hs.v>70){yx.v=hs.v-30;}else{yx.v=hs.v+30;}
            }

            if((hs.h>=180)&&(hs.h<220)){
                yx.h=hs.h-170;
                y.h=hs.h-160;
                yx.s=y.s=hs.s;
                y.v=hs.v;
                if(hs.v>70){yx.v=hs.v-30;}else{yx.v=hs.v+30;}
            }if((hs.h>=220)&&(hs.h<300)){
                yx.h=y.h=hs.h;
                yx.s=y.s=rc(hs.s-40,100);
                y.v=hs.v;
                if(hs.v>70){yx.v=hs.v-30;}else{yx.v=hs.v+30;}
            }

            if(hs.h>=300){
                if(hs.s>50){y.s=yx.s=hs.s-40;}else{y.s=yx.s=hs.s+40;}yx.h=y.h=(hs.h+20)%360;
                y.v=hs.v;
                if (hs.v > 70) { yx.v = hs.v - 30; } else { yx.v = hs.v + 30; }
            }

            z.hsv[2] = new HSV(y);
            z.hsv[3] = new HSV(yx);

            y.h=0;
            y.s=0;
            y.v=100-hs.v;

            z.hsv[4] = new HSV(y);

            y.h=0;
            y.s=0;
            y.v=hs.v;

            z.hsv[5] = new HSV(y);

            return z;
        }
    }
}
