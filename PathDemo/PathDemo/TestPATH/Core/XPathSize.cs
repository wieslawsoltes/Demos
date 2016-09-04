// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace TestPATH
{
    public class XPathSize
    {
        public double Width { get; set; }
        public double Height { get; set; }

        public static XPathSize Create(double width, double height)
        {
            return new XPathSize()
            {
                Width = width,
                Height = height
            };
        }
    }
}
