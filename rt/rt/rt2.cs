/*
======================================================================
rt2.js

Ernie Wright  23 Feb 2014

Initialize the raytracer's observer and world.  This is a Javascript
version of Eric Graham's rt2.c raytracer source file.  I've added the
object and scene data from his robots.dat, and I've generalized the
setup() function so that it can be used with other scenes.

The original rt2.c contained the following notice.

RT2.C
Copyright 1987 Eric Graham
All rights reserved.
This file may not be copied, modified or uploaded to a bulletin
board system, except as provided below.
Permission is granted to make a reasonable number of backup copies,
in order that it may be used to generate executable code for use
on a single computer system.
Permission is granted to modify this code and use the modified code
for non commercial use by the original purchaser of this software,
and provided that this notice is included in the modified version.
====================================================================== */

// objects are made of spheres
// types are dull (0), shiny (1), mirror (2)

using System;

namespace rt
{
    public static class rt2
    {
        public static Sphere[] rtspheres = 
        {
            new Sphere() {pos = new Vector(-0.9,-2.1,5.3), color = new Vector(0.9,0.9,0.9), radius = 0.6, type = 2},
            new Sphere() {pos = new Vector(-1.1,1.9,5.9), color = new Vector(0.9,0.9,0.9), radius = 0.6, type = 2},
            new Sphere() {pos = new Vector(-0.4,-1.2,6.8), color = new Vector(0.9,0.9,0.9), radius = 0.6, type = 2},
            new Sphere() {pos = new Vector(0,0,6.1), color = new Vector(1,0.7,0.7), radius = 0.5, type = 1},
            new Sphere() {pos = new Vector(0.02,0,6.12), color = new Vector(0.2,0.1,0.1), radius = 0.5, type = 1},
            new Sphere() {pos = new Vector(-0.4,0.2,6.1), color = new Vector(0.1,0.1,1), radius = 0.15, type = 1},
            new Sphere() {pos = new Vector(-0.4,-0.2,6.1), color = new Vector(0.1,0.1,1), radius = 0.15, type = 1},
            new Sphere() {pos = new Vector(0,0,5.5), color = new Vector(1,0.7,0.7), radius = 0.2, type = 1},
            new Sphere() {pos = new Vector(0,0,4.6), color = new Vector(1,0.1,0.1), radius = 0.8, type = 1},
            new Sphere() {pos = new Vector(0,0,4.34), color = new Vector(1,0.1,0.1), radius = 0.76, type = 1},
            new Sphere() {pos = new Vector(0,0,4.08), color = new Vector(1,0.1,0.1), radius = 0.72, type = 1},
            new Sphere() {pos = new Vector(0,0,3.82), color = new Vector(1,0.1,0.1), radius = 0.68, type = 1},
            new Sphere() {pos = new Vector(0,0,3.56), color = new Vector(1,0.1,0.1), radius = 0.64, type = 1},
            new Sphere() {pos = new Vector(0,0,3.3), color = new Vector(1,0.1,0.1), radius = 0.6, type = 1},
            new Sphere() {pos = new Vector(0,0.6,2.9), color = new Vector(1,0.7,0.7), radius = 0.2, type = 1},
            new Sphere() {pos = new Vector(-0.1,0.6,2.68333), color = new Vector(1,0.7,0.7), radius = 0.2, type = 1},
            new Sphere() {pos = new Vector(-0.2,0.6,2.46667), color = new Vector(1,0.7,0.7), radius = 0.2, type = 1},
            new Sphere() {pos = new Vector(-0.3,0.6,2.25), color = new Vector(1,0.7,0.7), radius = 0.2, type = 1},
            new Sphere() {pos = new Vector(-0.4,0.6,2.03333), color = new Vector(1,0.7,0.7), radius = 0.2, type = 1},
            new Sphere() {pos = new Vector(-0.5,0.6,1.81667), color = new Vector(1,0.7,0.7), radius = 0.2, type = 1},
            new Sphere() {pos = new Vector(-0.6,0.6,1.6), color = new Vector(1,0.7,0.7), radius = 0.2, type = 1},
            new Sphere() {pos = new Vector(-0.571429,0.6,1.37143), color = new Vector(1,0.7,0.7), radius = 0.185714, type = 1},
            new Sphere() {pos = new Vector(-0.542857,0.6,1.14286), color = new Vector(1,0.7,0.7), radius = 0.171429, type = 1},
            new Sphere() {pos = new Vector(-0.514286,0.6,0.914286), color = new Vector(1,0.7,0.7), radius = 0.157143, type = 1},
            new Sphere() {pos = new Vector(-0.485714,0.6,0.685714), color = new Vector(1,0.7,0.7), radius = 0.142857, type = 1},
            new Sphere() {pos = new Vector(-0.457143,0.6,0.457143), color = new Vector(1,0.7,0.7), radius = 0.128571, type = 1},
            new Sphere() {pos = new Vector(-0.428571,0.6,0.228571), color = new Vector(1,0.7,0.7), radius = 0.114286, type = 1},
            new Sphere() {pos = new Vector(-0.4,0.6,0), color = new Vector(1,0.7,0.7), radius = 0.1, type = 1},
            new Sphere() {pos = new Vector(0,-0.6,2.9), color = new Vector(1,0.7,0.7), radius = 0.2, type = 1},
            new Sphere() {pos = new Vector(0.0333333,-0.6,2.68333), color = new Vector(1,0.7,0.7), radius = 0.2, type = 1},
            new Sphere() {pos = new Vector(0.0666667,-0.6,2.46667), color = new Vector(1,0.7,0.7), radius = 0.2, type = 1},
            new Sphere() {pos = new Vector(0.1,-0.6,2.25), color = new Vector(1,0.7,0.7), radius = 0.2, type = 1},
            new Sphere() {pos = new Vector(0.133333,-0.6,2.03333), color = new Vector(1,0.7,0.7), radius = 0.2, type = 1},
            new Sphere() {pos = new Vector(0.166667,-0.6,1.81667), color = new Vector(1,0.7,0.7), radius = 0.2, type = 1},
            new Sphere() {pos = new Vector(0.2,-0.6,1.6), color = new Vector(1,0.7,0.7), radius = 0.2, type = 1},
            new Sphere() {pos = new Vector(0.228571,-0.6,1.37143), color = new Vector(1,0.7,0.7), radius = 0.185714, type = 1},
            new Sphere() {pos = new Vector(0.257143,-0.6,1.14286), color = new Vector(1,0.7,0.7), radius = 0.171429, type = 1},
            new Sphere() {pos = new Vector(0.285714,-0.6,0.914286), color = new Vector(1,0.7,0.7), radius = 0.157143, type = 1},
            new Sphere() {pos = new Vector(0.314286,-0.6,0.685714), color = new Vector(1,0.7,0.7), radius = 0.142857, type = 1},
            new Sphere() {pos = new Vector(0.342857,-0.6,0.457143), color = new Vector(1,0.7,0.7), radius = 0.128571, type = 1},
            new Sphere() {pos = new Vector(0.371429,-0.6,0.228571), color = new Vector(1,0.7,0.7), radius = 0.114286, type = 1},
            new Sphere() {pos = new Vector(0.4,-0.6,0), color = new Vector(1,0.7,0.7), radius = 0.1, type = 1},
            new Sphere() {pos = new Vector(0,-0.7,5.1), color = new Vector(1,0.7,0.7), radius = 0.2, type = 1},
            new Sphere() {pos = new Vector(-0.0333333,-0.783333,4.95), color = new Vector(1,0.7,0.7), radius = 0.2, type = 1},
            new Sphere() {pos = new Vector(-0.0666667,-0.866667,4.8), color = new Vector(1,0.7,0.7), radius = 0.2, type = 1},
            new Sphere() {pos = new Vector(-0.1,-0.95,4.65), color = new Vector(1,0.7,0.7), radius = 0.2, type = 1},
            new Sphere() {pos = new Vector(-0.133333,-1.03333,4.5), color = new Vector(1,0.7,0.7), radius = 0.2, type = 1},
            new Sphere() {pos = new Vector(-0.166667,-1.11667,4.35), color = new Vector(1,0.7,0.7), radius = 0.2, type = 1},
            new Sphere() {pos = new Vector(-0.2,-1.2,4.2), color = new Vector(1,0.7,0.7), radius = 0.2, type = 1},
            new Sphere() {pos = new Vector(-0.328571,-1.31429,4.18571), color = new Vector(1,0.7,0.7), radius = 0.185714, type = 1},
            new Sphere() {pos = new Vector(-0.457143,-1.42857,4.17143), color = new Vector(1,0.7,0.7), radius = 0.171429, type = 1},
            new Sphere() {pos = new Vector(-0.585714,-1.54286,4.15714), color = new Vector(1,0.7,0.7), radius = 0.157143, type = 1},
            new Sphere() {pos = new Vector(-0.714286,-1.65714,4.14286), color = new Vector(1,0.7,0.7), radius = 0.142857, type = 1},
            new Sphere() {pos = new Vector(-0.842857,-1.77143,4.12857), color = new Vector(1,0.7,0.7), radius = 0.128571, type = 1},
            new Sphere() {pos = new Vector(-0.971429,-1.88571,4.11429), color = new Vector(1,0.7,0.7), radius = 0.114286, type = 1},
            new Sphere() {pos = new Vector(-1.1,-2,4.1), color = new Vector(1,0.7,0.7), radius = 0.1, type = 1},
            new Sphere() {pos = new Vector(0,0.7,5.1), color = new Vector(1,0.7,0.7), radius = 0.2, type = 1},
            new Sphere() {pos = new Vector(-0.0333333,0.783333,4.95), color = new Vector(1,0.7,0.7), radius = 0.2, type = 1},
            new Sphere() {pos = new Vector(-0.0666667,0.866667,4.8), color = new Vector(1,0.7,0.7), radius = 0.2, type = 1},
            new Sphere() {pos = new Vector(-0.1,0.95,4.65), color = new Vector(1,0.7,0.7), radius = 0.2, type = 1},
            new Sphere() {pos = new Vector(-0.133333,1.03333,4.5), color = new Vector(1,0.7,0.7), radius = 0.2, type = 1},
            new Sphere() {pos = new Vector(-0.166667,1.11667,4.35), color = new Vector(1,0.7,0.7), radius = 0.2, type = 1},
            new Sphere() {pos = new Vector(-0.2,1.2,4.2), color = new Vector(1,0.7,0.7), radius = 0.2, type = 1},
            new Sphere() {pos = new Vector(-0.314286,1.3,4.28571), color = new Vector(1,0.7,0.7), radius = 0.185714, type = 1},
            new Sphere() {pos = new Vector(-0.428571,1.4,4.37143), color = new Vector(1,0.7,0.7), radius = 0.171429, type = 1},
            new Sphere() {pos = new Vector(-0.542857,1.5,4.45714), color = new Vector(1,0.7,0.7), radius = 0.157143, type = 1},
            new Sphere() {pos = new Vector(-0.657143,1.6,4.54286), color = new Vector(1,0.7,0.7), radius = 0.142857, type = 1},
            new Sphere() {pos = new Vector(-0.771429,1.7,4.62857), color = new Vector(1,0.7,0.7), radius = 0.128571, type = 1},
            new Sphere() {pos = new Vector(-0.885714,1.8,4.71429), color = new Vector(1,0.7,0.7), radius = 0.114286, type = 1},
            new Sphere() {pos = new Vector(-1,1.9,4.8), color = new Vector(1,0.7,0.7), radius = 0.1, type = 1}
        };

        public static Lamp[] rtlamps =                // lights in the scene
        {
           new Lamp() { shape = new Sphere() { pos = new Vector( -100.0, 50.0, 150.0 ), radius = 15.0, color = new Vector( 1, 1, 1 )} }
        };

        public static Observer rtobserver = new Observer()
        {
            obspos = new Vector(-10, -4, 5.5),  // camera position
            nx = 640,                   // output image width, pixels
            ny = 480,                   // output image height, pixels
            hratio = 0.75,              // image height to width ratio (frame aspect)
            alt = -10.0,                // altitude of the viewing direction, degrees
            az = 20.0,                  // azimuth of the viewing direction, degrees
            fl = 35.0                // focal length, mm
        };

        public static Patch[] rttiles =                // floor tile colors
        {
           new Patch() { pos = new Vector( 0, 0, 0 ), normal = new Vector( 0, 0, 1 ), color = new Vector( 1.5, 1.5, 0 )},
           new Patch() { pos = new Vector( 0, 0, 0 ), normal = new Vector( 0, 0, 1 ), color = new Vector( 0, 1.5, 0 )}
        };

        public static Vector rtambient = new Vector(0.25, 0.25, 0.25);  // ambient color
        public static Vector rtskyzen = new Vector(0.1, 0.1, 1.0);      // sky zenith color
        public static Vector rtskyhor = new Vector(0.7, 0.7, 1.0);      // sky horizon color


        /*
        ======================================================================
        setup()
        
        Initialize the observer and the world.
        ====================================================================== */

        public static void setup(ref Observer o, ref World w)
        {
            int i, j;
            double r, t, lampfac;
            double degtorad = Math.PI / 180.0;

            o.obspos = rtobserver.obspos;
            o.nx = rtobserver.nx;
            o.ny = rtobserver.ny;
            o.hratio = rtobserver.hratio;
            o.alt = rtobserver.alt * degtorad;
            o.az = rtobserver.az * degtorad;
            o.fl = rtobserver.fl * 0.028;

            o.px = 1.0 / o.nx;
            o.py = o.hratio / o.ny;
            o.viewdir = new Vector(
                Math.Cos(o.az) * Math.Cos(o.alt),
                Math.Sin(o.az) * Math.Cos(o.alt),
                Math.Sin(o.alt));
            o.uhat = new Vector(Math.Sin(o.az), -Math.Cos(o.az), 0.0);
            o.vhat = new Vector(
                -Math.Cos(o.az) * Math.Sin(o.alt),
                -Math.Sin(o.az) * Math.Sin(o.alt),
                Math.Cos(o.alt));

            w.numsp = rtspheres.Length;
            w.sp = rtspheres;

            w.numlmp = rtlamps.Length;
            w.lmp = rtlamps;

            w.horizon = rttiles;
            w.illum = rtambient;
            w.skyzen = rtskyzen;
            w.skyhor = rtskyhor;

            /* modify the lamp brightness so as to */
            /* get the right exposure              */

            lampfac = rt1.BIG;
            var tp = Vector.Zero;
            for (i = 0; i < w.numsp; i++)
            {
                for (j = 0; j < w.numlmp; j++)
                {
                    rt1.vecsub(ref tp, ref w.sp[i].pos, ref w.lmp[j].shape.pos);
                    r = Math.Sqrt(rt1.dot(ref tp, ref tp)) - w.sp[i].radius;

                    t = w.sp[i].color.X * w.lmp[j].shape.color.X / (r * r);
                    if (t != 0.0)
                    {
                        t = (1.0 - w.sp[i].color.X * w.illum.X) / t;
                        if (t < lampfac)
                            lampfac = t;
                    }

                    t = w.sp[i].color.Y * w.lmp[j].shape.color.Y / (r * r);
                    if (t != 0.0)
                    {
                        t = (1.0 - w.sp[i].color.Y * w.illum.Y) / t;
                        if (t < lampfac)
                            lampfac = t;
                    }

                    t = w.sp[i].color.Z * w.lmp[j].shape.color.Z / (r * r);
                    if (t != 0.0)
                    {
                        t = (1.0 - w.sp[i].color.Z * w.illum.Z) / t;
                        if (t < lampfac)
                            lampfac = t;
                    }
                }
            }

            for (j = 0; j < w.numlmp; j++)
            {
                w.lmp[j].shape.color.X *= lampfac;
                w.lmp[j].shape.color.Y *= lampfac;
                w.lmp[j].shape.color.Z *= lampfac;
            }
        }
    }
}
