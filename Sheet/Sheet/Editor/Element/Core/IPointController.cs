// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Sheet.Core;
using System;
using System.Collections.Generic;

namespace Sheet.Editor
{
    public interface IPointController
    {
        void ConnectStart(XPoint point, XLine line);
        void ConnectEnd(XPoint point, XLine line);
        void UpdateDependencies(IList<XBlock> blocks, IList<XPoint> points, IList<XLine> lines);
    }
}
