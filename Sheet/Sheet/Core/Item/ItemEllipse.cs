// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Sheet.Core
{
    public class ItemEllipse : Item
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public bool IsFilled { get; set; }
        public ArgbColor Stroke { get; set; }
        public ArgbColor Fill { get; set; }
        public double StrokeThickness { get; set; }
    }
}
