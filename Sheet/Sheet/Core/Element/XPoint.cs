// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;

namespace Sheet.Core
{
    public class XPoint : XElement
    {
        public double X { get; set; }
        public double Y { get; set; }
        public bool IsVisible { get; set; }
        public IList<Dependency> Connected { get; set; }
        public XPoint(object element, double x, double y, bool isVisible)
        {
            Native = element;
            X = x;
            Y = y;
            IsVisible = isVisible;
            Connected = new List<Dependency>();
        }
    }
}
