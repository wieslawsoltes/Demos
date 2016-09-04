// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Sheet.Core;
using System;
using System.Collections.Generic;

namespace Sheet.Editor
{
    public interface IBlockController
    {
        IList<XPoint> Add(ISheet sheet, IEnumerable<ItemPoint> pointItems, XBlock parent, XBlock selected, bool select, double thickness);
        IList<XLine> Add(ISheet sheet, IEnumerable<ItemLine> lineItems, XBlock parent, XBlock selected, bool select, double thickness);
        IList<XRectangle> Add(ISheet sheet, IEnumerable<ItemRectangle> rectangleItems, XBlock parent, XBlock selected, bool select, double thickness);
        IList<XEllipse> Add(ISheet sheet, IEnumerable<ItemEllipse> ellipseItems, XBlock parent, XBlock selected, bool select, double thickness);
        IList<XText> Add(ISheet sheet, IEnumerable<ItemText> textItems, XBlock parent, XBlock selected, bool select, double thickness);
        IList<XImage> Add(ISheet sheet, IEnumerable<ItemImage> imageItems, XBlock parent, XBlock selected, bool select, double thickness);
        IList<XBlock> Add(ISheet sheet, IEnumerable<ItemBlock> blockItems, XBlock parent, XBlock selected, bool select, double thickness);
        void AddContents(ISheet sheet, ItemBlock blockItem, XBlock content, XBlock selected, bool select, double thickness);
        void AddBroken(ISheet sheet, ItemBlock blockItem, XBlock content, XBlock selected, bool select, double thickness);

        void Remove(ISheet sheet, IEnumerable<XPoint> points);
        void Remove(ISheet sheet, IEnumerable<XLine> lines);
        void Remove(ISheet sheet, IEnumerable<XRectangle> rectangles);
        void Remove(ISheet sheet, IEnumerable<XEllipse> ellipses);
        void Remove(ISheet sheet, IEnumerable<XText> texts);
        void Remove(ISheet sheet, IEnumerable<XImage> images);
        void Remove(ISheet sheet, IEnumerable<XBlock> blocks);
        void Remove(ISheet sheet, XBlock block);
        void RemoveSelected(ISheet sheet, XBlock parent, XBlock selected);

        void MoveDelta(double dx, double dy, XPoint point);
        void MoveDelta(double dx, double dy, IEnumerable<XPoint> points);
        void MoveDelta(double dx, double dy, IEnumerable<XLine> lines);
        void MoveDeltaStart(double dx, double dy, XLine line);
        void MoveDeltaEnd(double dx, double dy, XLine line);
        void MoveDelta(double dx, double dy, XRectangle rectangle);
        void MoveDelta(double dx, double dy, IEnumerable<XRectangle> rectangles);
        void MoveDelta(double dx, double dy, XEllipse ellipse);
        void MoveDelta(double dx, double dy, IEnumerable<XEllipse> ellipses);
        void MoveDelta(double dx, double dy, XText text);
        void MoveDelta(double dx, double dy, IEnumerable<XText> texts);
        void MoveDelta(double dx, double dy, XImage image);
        void MoveDelta(double dx, double dy, IEnumerable<XImage> images);
        void MoveDelta(double dx, double dy, XBlock block);
        void MoveDelta(double dx, double dy, IEnumerable<XBlock> blocks);

        void Deselect(XPoint point);
        void Deselect(XLine line);
        void Deselect(XRectangle rectangle);
        void Deselect(XEllipse ellipse);
        void Deselect(XText text);
        void Deselect(XImage image);
        void Deselect(XBlock parent);

        void Select(XPoint point);
        void Select(XLine line);
        void Select(XRectangle rectangle);
        void Select(XEllipse ellipse);
        void Select(XText text);
        void Select(XImage image);
        void Select(XBlock parent);

        bool HaveSelected(XBlock selected);
        bool HaveOnlyOnePointSelected(XBlock selected);
        bool HaveOnlyOneLineSelected(XBlock selected);
        bool HaveOnlyOneRectangleSelected(XBlock selected);
        bool HaveOnlyOneEllipseSelected(XBlock selected);
        bool HaveOnlyOneTextSelected(XBlock selected);
        bool HaveOnlyOneImageSelected(XBlock selected);
        bool HaveOnlyOneBlockSelected(XBlock selected);

        bool HitTest(IEnumerable<XPoint> points, ImmutableRect rect, object relativeTo, XBlock selected, bool findOnlyOne);
        bool HitTest(IEnumerable<XLine> lines, ImmutableRect rect, XBlock selected, bool findOnlyOne);
        bool HitTest(IEnumerable<XRectangle> rectangles, ImmutableRect rect, object relativeTo, XBlock selected, bool findOnlyOne);
        bool HitTest(IEnumerable<XEllipse> ellipses, ImmutableRect rect, object relativeTo, XBlock selected, bool findOnlyOne);
        bool HitTest(IEnumerable<XText> texts, ImmutableRect rect, object relativeTo, XBlock selected, bool findOnlyOne);
        bool HitTest(IEnumerable<XImage> images, ImmutableRect rect, object relativeTo, XBlock selected, bool findOnlyOne);
        bool HitTest(IEnumerable<XBlock> blocks, ImmutableRect rect, object relativeTo, XBlock selected, bool findOnlyOne, bool findInsideBlock);
        bool HitTest(XBlock block, ImmutableRect rect, object relativeTo, XBlock selected, bool findOnlyOne, bool findInsideBlock);
        bool HitTest(ISheet sheet, XBlock block, ImmutableRect rect, XBlock selected, bool findOnlyOne, bool findInsideBlock);
        bool HitTest(ISheet sheet, XBlock block, ImmutablePoint p, double size, XBlock selected, bool findOnlyOne, bool findInsideBlock);

        void ToggleFill(XRectangle rectangle);
        void ToggleFill(XEllipse ellipse);
        void ToggleFill(XPoint point);

        void ShallowCopy(XBlock original, XBlock copy);
    }
}
