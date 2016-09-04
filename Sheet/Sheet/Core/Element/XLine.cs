// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Sheet.Core
{
    public class XLine : XElement
    {
        public int StartId { get; set; }
        public int EndId { get; set; }
        public XPoint Start { get; set; }
        public XPoint End { get; set; }
        public XLine(object element)
        {
            Native = element;
        }
        public XLine(object element, int startId, int endId)
        {
            StartId = startId;
            EndId = endId;
            Native = element;
        }
        public XLine(object element, XPoint start, XPoint end)
        {
            Start = start;
            End = end;
            Native = element;
        }
    }
}
