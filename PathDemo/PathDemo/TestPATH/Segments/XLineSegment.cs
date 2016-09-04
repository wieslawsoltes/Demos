// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace TestPATH
{
    public class XLineSegment : XPathSegment
    {
        public XPathPoint Point { get; set; }

        public static XLineSegment Create(
            XPathPoint point,
            bool isStroked,
            bool isSmoothJoin)
        {
            return new XLineSegment()
            {
                Point = point,
                IsStroked = isStroked,
                IsSmoothJoin = isSmoothJoin
            };
        }
    }
}
