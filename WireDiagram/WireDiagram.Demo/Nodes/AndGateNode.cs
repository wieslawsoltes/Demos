// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using WireDiagram.Core.Model;
using WireDiagram.Core.Items;
using WireDiagram.Core.Lists;

namespace WireDiagram.Nodes.Model
{
    public class AndGateNode : Node
    {
        public override void CreateDefaultPins()
        {
            var points = new PinPointList() 
            {
                new PinPoint(0.0, 0.5, PinAlignment.Left), 
                new PinPoint(1.0, 0.5, PinAlignment.Right),
                new PinPoint(0.5, 0.0, PinAlignment.Top), 
                new PinPoint(0.5, 1.0, PinAlignment.Bottom) 
            };

            base.CreatePins(points);
        }
    }
}
