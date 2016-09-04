// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace TestPATH
{
    public class XPathPoint
    {
        public double X { get; set; }
        public double Y { get; set; }

        public static XPathPoint Create(double x, double y)
        {
            return new XPathPoint()
            {
                X = x,
                Y = y
            };
        }
    }
}
