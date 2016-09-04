// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Sheet.Core;
using System;

namespace Sheet.Editor
{
    public interface IBlockSerializer
    {
        ItemPoint Serialize(XPoint point);
        ItemLine Serialize(XLine line);
        ItemRectangle Serialize(XRectangle rectangle);
        ItemEllipse Serialize(XEllipse ellipse);
        ItemText Serialize(XText text);
        ItemImage Serialize(XImage image);
        ItemBlock Serialize(XBlock parent);
        ItemBlock SerializerAndSetId(XBlock parent, int id, double x, double y, double width, double height, int dataId, string name);
        XPoint Deserialize(ISheet sheet, XBlock parent, ItemPoint pointItem, double thickness);
        XLine Deserialize(ISheet sheet, XBlock parent, ItemLine lineItem, double thickness);
        XRectangle Deserialize(ISheet sheet, XBlock parent, ItemRectangle rectangleItem, double thickness);
        XEllipse Deserialize(ISheet sheet, XBlock parent, ItemEllipse ellipseItem, double thickness);
        XText Deserialize(ISheet sheet, XBlock parent, ItemText textItem);
        XImage Deserialize(ISheet sheet, XBlock parent, ItemImage imageItem);
        XBlock Deserialize(ISheet sheet, XBlock parent, ItemBlock blockItem, double thickness);
    }
}
