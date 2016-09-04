// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;

namespace TestPATH
{
    public class XPathFigure
    {
        public XPathPoint StartPoint { get; set; }
        public IList<XPathSegment> Segments { get; set; }
        public bool IsFilled { get; set; }
        public bool IsClosed { get; set; }

        public static XPathFigure Create(
            XPathPoint startPoint,
            IList<XPathSegment> segments,
            bool isFilled,
            bool isClosed)
        {
            return new XPathFigure()
            {
                StartPoint = startPoint,
                Segments = segments,
                IsFilled = isFilled,
                IsClosed = isClosed
            };
        }
    }
}
