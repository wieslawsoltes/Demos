// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace TestPATH
{
    public class XPathRect
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }

        public static XPathRect Create(double x, double y, double width, double height)
        {
            return new XPathRect()
            {
                X = x,
                Y = y,
                Width = width,
                Height = height,
            };
        }
    }
}
