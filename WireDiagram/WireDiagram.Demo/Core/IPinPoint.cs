﻿// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WireDiagram.Core.Model
{
    interface IPinPoint
    {
        double X { get; set; }
        double Y { get; set; }
        PinAlignment Alignment { get; set; }
    }
}
