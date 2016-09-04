// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Sheet.Core
{
    public class ItemLine : Item
    {
        public double X1 { get; set; }
        public double Y1 { get; set; }
        public double X2 { get; set; }
        public double Y2 { get; set; }
        public ArgbColor Stroke { get; set; }
        public double StrokeThickness { get; set; }
        public int StartId { get; set; }
        public int EndId { get; set; }
    }
}
