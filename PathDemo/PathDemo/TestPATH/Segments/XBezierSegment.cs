// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace TestPATH
{
    public class XBezierSegment : XPathSegment
    {
        public XPathPoint Point1 { get; set; }
        public XPathPoint Point2 { get; set; }
        public XPathPoint Point3 { get; set; }

        public static XBezierSegment Create(
            XPathPoint point1,
            XPathPoint point2,
            XPathPoint point3,
            bool isStroked,
            bool isSmoothJoin)
        {
            return new XBezierSegment()
            {
                Point1 = point1,
                Point2 = point2,
                Point3 = point3,
                IsStroked = isStroked,
                IsSmoothJoin = isSmoothJoin
            };
        }
    }
}
