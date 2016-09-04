// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace TestPATH
{
    public class XArcSegment : XPathSegment
    {
        public XPathPoint Point { get; set; }
        public XPathSize Size { get; set; }
        public double RotationAngle { get; set; }
        public bool IsLargeArc { get; set; }
        public XSweepDirection SweepDirection { get; set; }

        public static XArcSegment Create(
            XPathPoint point,
            XPathSize size,
            double rotationAngle,
            bool isLargeArc,
            XSweepDirection sweepDirection,
            bool isStroked,
            bool isSmoothJoin)
        {
            return new XArcSegment()
            {
                Point = point,
                Size = size,
                RotationAngle = rotationAngle,
                IsLargeArc = isLargeArc,
                SweepDirection = sweepDirection,
                IsStroked = isStroked,
                IsSmoothJoin = isSmoothJoin
            };
        }
    }
}
