// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

// High-Resolution Mandelbrot
// http://preshing.com/20110926/high-resolution-mandelbrot-in-obfuscated-python/
// https://github.com/panzi/mandelbrot/blob/master/mandelbrot.c

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace Mandelbrot
{
    class Program
    {
        static void Help()
        {
            Console.WriteLine("High-Resolution Mandelbrot");
            Console.WriteLine("Mandelbrot <nthreads> <width> <height> <filename>");
            Console.WriteLine("Mandelbrot <nthreads> <width> <height> <filename> <x> <y> <width> <height>");
        }

        static void Main(string[] args)
        {
            if (args.Length < 4)
            {
                Help();
                return;
            }

            uint nthreads = uint.Parse(args[0]);
            
            uint w = uint.Parse(args[1]),
                 h = uint.Parse(args[2]),
                 x1 = 0, x2 = w,
                 y1 = 0, y2 = h,
                 iw = w, ih = h;

            if (nthreads < 1 || (nthreads > 1 && nthreads % 2 != 0))
            {
                Console.WriteLine("illegal number of threads: {0}", args[0]);
                return;
            }

            if (w == 0 || w >= 0xffff || w % 4 != 0)
            {
                Console.WriteLine("illegal width: {0}", args[1]);
                return;
            }

            if (h == 0 || h >= 0xffff)
            {
                Console.WriteLine("illegal height: {0}\n", args[2]);
                return;
            }

            if (args.Length >= 8)
            {
                x1 = uint.Parse(args[4]);
                y1 = uint.Parse(args[5]);
                iw = uint.Parse(args[6]);
                ih = uint.Parse(args[7]);

                if (x1 >= w)
                {
                    Console.WriteLine("illegal x: {0}", args[4]);
                    return;
                }

                if (y1 >= h)
                {
                    Console.WriteLine("illegal y: {0}", args[5]);
                    return;
                }

                if (iw == 0 || iw >= 0xffff)
                {
                    Console.WriteLine("illegal area width: {0}", args[6]);
                    return;
                }

                if (ih == 0 || ih >= 0xffff)
                {
                    Console.WriteLine("illegal area height: {0}", args[7]);
                    return;
                }

                x2 = x1 + iw;
                y2 = y1 + ih;

                if (x2 > w)
                {
                    x2 = w;
                    iw = x2 - x1;
                }

                if (y2 > h)
                {
                    y2 = h;
                    ih = y2 - y1;
                }
            }

            bool mt = nthreads > 1;
            uint s = iw * ih * 3;

            if (mt)
            {
                byte[][] buf = new byte[nthreads][];
                Task[] t = new Task[nthreads];

                for (uint i = 0; i < nthreads; i++)
                {
                    var _i = i;
                    t[i] = Task.Factory.StartNew(() =>
                    {
                        buf[_i] = new byte[s / nthreads];
                        M(buf[_i], 
                          h,
                          x1,
                          y1 + (ih / nthreads) * _i,
                          x2, 
                          y1 + ih - (ih / nthreads) * (nthreads - 1 - _i));
                    });
                }

                Task.WaitAll(t);

                using (var f = System.IO.File.Open(args[3], System.IO.FileMode.Create))
                {
                    H(iw, ih, f);
                    for (uint i = 0; i < nthreads; i++)
                        f.Write(buf[nthreads - i - 1], 0, (int)(s / nthreads));
                }
            }
            else
            {
                byte[] buf = new byte[s];
                M(buf, h, x1, y1, x2, y2);

                using (var f = System.IO.File.Open(args[3], System.IO.FileMode.Create))
                {
                    H(iw, ih, f);
                    f.Write(buf, 0, (int)s);
                }
            }
        }

        static Complex Y(Complex V, Complex B, Complex c)
        {
            return (Complex.Abs(V) < 6) ?
                   (c != 0 ? Y(V * V + B, B, c - 1) : c) :
                   (2 + c - 4 * Complex.Pow(Complex.Abs(V), -0.4)) / 255;
        }

        static void M(byte[] buf, uint h, uint x1, uint y1, uint x2, uint y2)
        {
            uint p = 0;
            uint A, x, y, hhalf = h / 2;
            for (y = y2; y > y1; )
            {
                --y;
                for (x = x1; x < x2; ++x)
                {
                    Complex T = 0, t;
                    for (A = 0; A < 9; ++A)
                    {
                        t = Y(0, (A % 3 / 3.0 + x + (y + A / 3 / 3.0 - hhalf) / 1 * Complex.ImaginaryOne) * 2.5 / h - 2.7, 255);
                        T += t * t;
                    }

                    T /= 9;
                    buf[p + 0] = (byte)(T * 80 + Complex.Pow(T, 9) * 255 - 950 * Complex.Pow(T, 99)).Real;
                    buf[p + 1] = (byte)(T * 70 - 880 * Complex.Pow(T, 18) + 701 * Complex.Pow(T, 9)).Real;
                    buf[p + 2] = (byte)(T * Complex.Pow(255, (1 - Complex.Pow(T, 45) * 2))).Real;
                    p += 3;
                }
            }
        }

        private static void H(uint iw, uint ih, System.IO.FileStream f)
        {
            uint S = (iw * ih) * 3 + 26;

            byte[] header = 
            { 
                66, 77,
                (byte)(S & 255), (byte)((S >> 8) & 255), (byte)((S >> 16) & 255), (byte)(S >> 24), 
                0, 0, 0, 0, 
                26, 0, 0, 0, 
                12, 0, 0, 0, 
                (byte)(iw & 255), (byte)(iw >> 8), 
                (byte)(ih & 255), (byte)(ih >> 8), 
                1, 0, 
                24, 0 
            };

            f.Write(header, 0, 26);
        }
    }
}
