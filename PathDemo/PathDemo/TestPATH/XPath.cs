// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace TestPATH
{
    public class XPath
    {
        public string Name { get; set; }
        public string Source { get; set; }
        public XPathGeometry Geometry { get; set; }
        public XTransform Transform { get; set; }
        public bool IsStroked { get; set; }
        public bool IsFilled { get; set; }

        public static XPath Create(
            string name,
            string source,
            XPathGeometry geometry,
            XTransform transform,
            bool isStroked = true,
            bool isFilled = true)
        {
            return new XPath()
            {
                Name = name,
                Source = source,
                Geometry = geometry,
                Transform = transform,
                IsStroked = isStroked,
                IsFilled = isFilled
            };
        }
    }
}
