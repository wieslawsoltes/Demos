// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;

namespace TestPATH
{
    public class XPolyLineSegment : XPathSegment
    {
        public IList<XPathPoint> Points { get; set; }

        public static XPolyLineSegment Create(
            IList<XPathPoint> points,
            bool isStroked,
            bool isSmoothJoin)
        {
            return new XPolyLineSegment()
            {
                Points = points,
                IsStroked = isStroked,
                IsSmoothJoin = isSmoothJoin
            };
        }
    }
}
