/*
======================================================================
rt3.js

Ernie Wright  23 Feb 2014

Initialize the raytracer's drawing surface (an HTML5 canvas).  This is
a Javascript version of Eric Graham's rt3.c raytracer source file.
====================================================================== */

using System;

namespace rt
{
    public static class rt3
    {
        /*
        ======================================================================
        initsc()
        
        Create a canvas, a 2D drawing context, and an ImageData to hold the
        pixel values.
        ====================================================================== */

        public static void initsc(ref Observer o)
        {
            o.imgdata = new byte[3 * o.nx * o.ny];
        }


        /*
        ======================================================================
        ham()
        
        Scale the floating-point RGB value of a pixel into bytes and store the
        result in the ImageData.
        ====================================================================== */

        public static void ham(int i, int j, ref Vector brite, ref Observer o)
        {
            int d = 3 * ((o.ny - j - 1) * o.nx + i);
            o.imgdata[d + 0] = (byte)Math.Round(Math.Max(Math.Min(brite.Z * 255, 255), 0));
            o.imgdata[d + 1] = (byte)Math.Round(Math.Max(Math.Min(brite.Y * 255, 255), 0));
            o.imgdata[d + 2] = (byte)Math.Round(Math.Max(Math.Min(brite.X * 255, 255), 0));
        }
    }
}
