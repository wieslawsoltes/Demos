// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 

namespace Mandelbrot_WPF
{
    public static class Mandelbrot
    {
        private static void H(Stream stream, uint width, uint height)
        {
            uint size = (width * height) * 3 + 26;

            byte[] header = 
            { 
                66, 77,
                (byte)(size & 255), (byte)((size >> 8) & 255), (byte)((size >> 16) & 255), (byte)(size >> 24), 
                0, 0, 0, 0, 
                26, 0, 0, 0, 
                12, 0, 0, 0, 
                (byte)(width & 255), (byte)(width >> 8), 
                (byte)(height & 255), (byte)(height >> 8), 
                1, 0, 
                24, 0 
            };

            stream.Write(header, 0, 26);
        }

        public static void M(Stream stream, uint width, uint height, int max_iteration)
        {
            // Mandelbrot X scale (-2.5, 1), Y scale (-1, 1))
            // x0 = Px * (3.5 / width) - 2.5; 
            // y0 = Py * (2.0 / height) - 1.0;
            double minX = -2.5; // -2.5
            double maxX = 1.0; // 1.0
            double minY = -1.0; // -1.0
            double maxY = 1.0; // 1.0
            double sfx = Math.Abs(maxX - minX) / width;
            double sfy = Math.Abs(maxY - minY) / height;
            uint size = width * height * 3;
            byte[] buf = new byte[size];
            uint p = 0;

            var sw1 = Stopwatch.StartNew();

            for (uint Py = height; Py > 0; )
            {
                --Py;
                for (uint Px = 0; Px < width; ++Px)
                {
                    double x0 = Px * sfx + minX;
                    double y0 = Py * sfy + minY;
                    double x = 0.0;
                    double y = 0.0;
                    double xtemp;
                    int iteration = 0;

                    while ((x * x + y * y < 2 * 2) && (iteration < max_iteration))
                    {
                        xtemp = x * x - y * y + x0;
                        y = 2 * x * y + y0;
                        x = xtemp;
                        iteration = iteration + 1;
                    }

                    double c = (double)iteration / (double)max_iteration;
                    double r1 = 255.0, r2 = 0.0;
                    double g1 = 255.0, g2 = 0.0;
                    double b1 = 255.0, b2 = 0.0;
                    buf[p + 0] = (byte)((b2 - b1) * c + b1);
                    buf[p + 1] = (byte)((g2 - g1) * c + g1);
                    buf[p + 2] = (byte)((r2 - r1) * c + r1);
                    p += 3;
                }
            }

            sw1.Stop();
            Debug.Print("M: {0}ms", sw1.Elapsed.TotalMilliseconds);

            var sw2 = Stopwatch.StartNew();

            H(stream, width, height);
            stream.Write(buf, 0, (int)size);

            sw2.Stop();
            Debug.Print("I/O: {0}ms", sw2.Elapsed.TotalMilliseconds);
        }
    } 
}
