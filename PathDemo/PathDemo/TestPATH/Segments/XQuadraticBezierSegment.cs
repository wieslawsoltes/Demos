// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace TestPATH
{
    public class XQuadraticBezierSegment : XPathSegment
    {
        public XPathPoint Point1 { get; set; }
        public XPathPoint Point2 { get; set; }

        public static XQuadraticBezierSegment Create(
            XPathPoint point1,
            XPathPoint point2,
            bool isStroked,
            bool isSmoothJoin)
        {
            return new XQuadraticBezierSegment()
            {
                Point1 = point1,
                Point2 = point2,
                IsStroked = isStroked,
                IsSmoothJoin = isSmoothJoin
            };
        }
    }
}
