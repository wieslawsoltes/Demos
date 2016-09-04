// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Demo
{
    public class Program
    {
        static void Main(string[] args)
        {
            TestCircle();
        }

        static void TestCircle()
        {
            double precision = 0.001;

            Circle(
                r0: 1, 
                r1: 1, 
                x0: 0, 
                y0: 0,
                t0: -Math.PI,
                t1: Math.PI,
                step: precision,
                path: "c0.csv");

            Circle(
                r0: 1, 
                r1: 1, 
                x0: 0, 
                y0: 0,
                t0: -Math.PI / 2,
                t1: Math.PI / 2,
                step: precision,
                path: "c1.csv");

            Circle(
                r0: 1, 
                r1: 1, 
                x0: 2, 
                y0: 2,
                t0: -Math.PI,
                t1: Math.PI / 2,
                step: precision,
                path: "c2.csv");

            Circle(
                r0: 1, 
                r1: 1, 
                x0: 2, 
                y0: 2,
                t0: 0,
                t1: 2 * Math.PI,
                step: precision,
                path: "c3.csv");

            Circle(
                r0: 1, 
                r1: 2,
                x0: 0, 
                y0: 0,
                t0: 0,
                t1: 2 * Math.PI,
                step: precision,
                path: "e0.csv");

            Circle(
                r0: 1, 
                r1: 2,
                x0: 0, 
                y0: 0,
                t0: Math.PI / 3,
                t1: 1.5 * Math.PI,
                step: precision,
                path: "e1.csv");

            Circle(
                r0: 1, 
                r1: 1,
                x0: 0, 
                y0: 0,
                t0: 0,
                t1: Math.PI / 2,
                step: precision,
                path: "a0.csv");
        }

        // parameters
        // r0: radius x axis
        // r1: radius y axis
        // x0: center point x coordinate
        // y0: center point y coordinate
        // t0: start angle in radians
        // t1: end angle in radians
        // step: calculations precision
        // path: output .csv file path
        static void Circle(
            double r0,
            double r1,
            double x0,
            double y0, 
            double t0, 
            double t1,
            double step, 
            string path)
        {
            using(var fs = System.IO.File.CreateText(path))
            {
                fs.Write("x;y\r\n");
                for (double t = t0; t <= t1; t += step)
                {
                    double x = x0 + r0 * Math.Cos(t);
                    double y = y0 + r1 * Math.Sin(t);
                    fs.Write(string.Concat(x, ';', y, "\r\n"));
                }
            }
        }
    }
}
