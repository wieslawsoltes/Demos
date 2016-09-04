// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Sheet.Core;
using System;
using System.Collections.Generic;

namespace Sheet.Editor
{
    public interface IPageFactory
    {
        void CreateLine(ISheet sheet, IList<XLine> lines, double thickness, double x1, double y1, double x2, double y2, ArgbColor stroke);
        void CreateText(ISheet sheet, IList<XText> texts, string content, double x, double y, double width, double height, int halign, int valign, double size, ArgbColor foreground);
        void CreateFrame(ISheet sheet, XBlock block, double size, double thickness, ArgbColor stroke);
        void CreateGrid(ISheet sheet, XBlock block, double startX, double startY, double width, double height, double size, double thickness, ArgbColor stroke);

        XRectangle CreateSelectionRectangle(double thickness, double x, double y, double width, double height);
    }
}
