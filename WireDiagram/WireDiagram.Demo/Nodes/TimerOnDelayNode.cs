﻿// Copyright (c) Wiesław Šoltés. All rights reserved.
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
    public class TimerOnDelayNode : Node
    {
        public override void CreateDefaultPins()
        {
            var points = new PinPointList() 
            {
                new PinPoint(0.0, 0.5, PinAlignment.Left), 
                new PinPoint(2.0, 0.5, PinAlignment.Right),
                new PinPoint(1.0, 0.0, PinAlignment.Top), 
                new PinPoint(1.0, 1.0, PinAlignment.Bottom) 
            };

            base.CreatePins(points);
        }
    }
}