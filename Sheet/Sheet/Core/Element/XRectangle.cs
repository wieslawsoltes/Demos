// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Sheet.Core
{
    public class XRectangle : XElement
    {
        public XRectangle(object element)
        {
            Native = element;
        }
    }
}
