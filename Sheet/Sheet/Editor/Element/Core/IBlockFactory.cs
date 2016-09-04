// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Sheet.Core;
using System;

namespace Sheet
{
    public interface IBlockFactory
    {
        XThumb CreateThumb(double x, double y);
        XThumb CreateThumb(double x, double y, XLine line, Action<XLine, XThumb, double, double> drag);
        XThumb CreateThumb(double x, double y, XElement element, Action<XElement, XThumb, double, double> drag);
        XPoint CreatePoint(double thickness, double x, double y, bool isVisible);
        XLine CreateLine(double thickness, double x1, double y1, double x2, double y2, ArgbColor stroke);
        XLine CreateLine(double thickness, XPoint start, XPoint end, ArgbColor stroke);
        XRectangle CreateRectangle(double thickness, double x, double y, double width, double height, bool isFilled, ArgbColor stroke, ArgbColor fill);
        XEllipse CreateEllipse(double thickness, double x, double y, double width, double height, bool isFilled, ArgbColor stroke, ArgbColor fill);
        XText CreateText(string text, double x, double y, double width, double height, int halign, int valign, double fontSize, ArgbColor backgroud, ArgbColor foreground);
        XImage CreateImage(double x, double y, double width, double height, byte[] data);
        XBlock CreateBlock(int id, double x, double y, double width, double height, int dataId, string name, ArgbColor backgroud);
    }
}
