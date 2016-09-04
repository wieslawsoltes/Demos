// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;

namespace Sheet.Core
{
    public class XBlock : XElement
    {
        public double X { get; set; }
        public double Y { get; set; }
        public string Name { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public int DataId { get; set; }
        public ArgbColor Backgroud { get; set; }
        public IList<XPoint> Points { get; set; }
        public IList<XLine> Lines { get; set; }
        public IList<XRectangle> Rectangles { get; set; }
        public IList<XEllipse> Ellipses { get; set; }
        public IList<XText> Texts { get; set; }
        public IList<XImage> Images { get; set; }
        public IList<XBlock> Blocks { get; set; }

        public XBlock()
        {
            Backgroud = new ArgbColor(0, 0, 0, 0);
            Points = new List<XPoint>();
            Lines = new List<XLine>();
            Rectangles = new List<XRectangle>();
            Ellipses = new List<XEllipse>();
            Texts = new List<XText>();
            Images = new List<XImage>();
            Blocks = new List<XBlock>();
        }

        public XBlock(
            int id, 
            double x, 
            double y, 
            double width, 
            double height, 
            int dataId, 
            string name)
            : this()
        {
            Id = id;
            X = x;
            Y = y;
            Width = width;
            Height = height;
            DataId = dataId;
            Name = name;
        }
    }
}
