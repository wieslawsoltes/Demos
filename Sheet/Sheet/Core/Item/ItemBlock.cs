// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;

namespace Sheet.Core
{
    public class ItemBlock : Item
    {
        public double X { get; set; }
        public double Y { get; set; }
        public string Name { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public ArgbColor Backgroud { get; set; }
        public int DataId { get; set; }
        public IList<ItemPoint> Points { get; set; }
        public IList<ItemLine> Lines { get; set; }
        public IList<ItemRectangle> Rectangles { get; set; }
        public IList<ItemEllipse> Ellipses { get; set; }
        public IList<ItemText> Texts { get; set; }
        public IList<ItemImage> Images { get; set; }
        public IList<ItemBlock> Blocks { get; set; }

        public ItemBlock(
            int id, 
            double x, 
            double y, 
            double width, 
            double height, 
            int dataId, 
            string name, 
            ArgbColor background)
        {
            X = x;
            Y = y;
            Id = id;
            DataId = dataId;
            Name = name;
            Width = width;
            Height = height;
            Backgroud = background;
            Points = new List<ItemPoint>();
            Lines = new List<ItemLine>();
            Rectangles = new List<ItemRectangle>();
            Ellipses = new List<ItemEllipse>();
            Texts = new List<ItemText>();
            Images = new List<ItemImage>();
            Blocks = new List<ItemBlock>();
        }
    }
}
