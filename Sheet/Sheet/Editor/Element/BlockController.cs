// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Sheet.Core;
using Sheet.IoC;
using System;
using System.Collections.Generic;

namespace Sheet.Editor
{
    public class BlockController : IBlockController
    {
        #region IoC

        private readonly IServiceLocator _serviceLocator;
        private readonly IBlockHelper _blockHelper;
        private readonly IBlockSerializer _blockSerializer;
        private readonly IPointController _pointController;

        public BlockController(IServiceLocator serviceLocator)
        {
            this._serviceLocator = serviceLocator;
            this._blockHelper = serviceLocator.GetInstance<IBlockHelper>();
            this._blockSerializer = serviceLocator.GetInstance<IBlockSerializer>();
            this._pointController = serviceLocator.GetInstance<IPointController>();
        }

        #endregion

        #region Add

        public IList<XPoint> Add(ISheet sheet, IEnumerable<ItemPoint> pointItems, XBlock parent, XBlock selected, bool select, double thickness)
        {
            var points = new List<XPoint>();

            foreach (var pointItem in pointItems)
            {
                var point = _blockSerializer.Deserialize(sheet, parent, pointItem, thickness);

                points.Add(point);

                if (select)
                {
                    Select(point);
                    selected.Points.Add(point);
                }
            }

            return points;
        }

        public IList<XLine> Add(ISheet sheet, IEnumerable<ItemLine> lineItems, XBlock parent, XBlock selected, bool select, double thickness)
        {
            var lines = new List<XLine>();

            foreach (var lineItem in lineItems)
            {
                var line = _blockSerializer.Deserialize(sheet, parent, lineItem, thickness);

                lines.Add(line);

                if (select)
                {
                    Select(line);
                    selected.Lines.Add(line);
                }
            }

            return lines;
        }

        public IList<XRectangle> Add(ISheet sheet, IEnumerable<ItemRectangle> rectangleItems, XBlock parent, XBlock selected, bool select, double thickness)
        {
            var rectangles = new List<XRectangle>();

            foreach (var rectangleItem in rectangleItems)
            {
                var rectangle = _blockSerializer.Deserialize(sheet, parent, rectangleItem, thickness);

                rectangles.Add(rectangle);

                if (select)
                {
                    Select(rectangle);
                    selected.Rectangles.Add(rectangle);
                }
            }

            return rectangles;
        }

        public IList<XEllipse> Add(ISheet sheet, IEnumerable<ItemEllipse> ellipseItems, XBlock parent, XBlock selected, bool select, double thickness)
        {
            var ellipses = new List<XEllipse>();

            foreach (var ellipseItem in ellipseItems)
            {
                var ellipse = _blockSerializer.Deserialize(sheet, parent, ellipseItem, thickness);

                ellipses.Add(ellipse);

                if (select)
                {
                    Select(ellipse);
                    selected.Ellipses.Add(ellipse);
                }
            }

            return ellipses;
        }

        public IList<XText> Add(ISheet sheet, IEnumerable<ItemText> textItems, XBlock parent, XBlock selected, bool select, double thickness)
        {
            var texts = new List<XText>();

            foreach (var textItem in textItems)
            {
                var text = _blockSerializer.Deserialize(sheet, parent, textItem);

                texts.Add(text);

                if (select)
                {
                    Select(text);
                    selected.Texts.Add(text);
                }
            }

            return texts;
        }

        public IList<XImage> Add(ISheet sheet, IEnumerable<ItemImage> imageItems, XBlock parent, XBlock selected, bool select, double thickness)
        {
            var images = new List<XImage>();

            foreach (var imageItem in imageItems)
            {
                var image = _blockSerializer.Deserialize(sheet, parent, imageItem);

                images.Add(image);

                if (select)
                {
                    Select(image);
                    selected.Images.Add(image);
                }
            }

            return images;
        }

        public IList<XBlock> Add(ISheet sheet, IEnumerable<ItemBlock> blockItems, XBlock parent, XBlock selected, bool select, double thickness)
        {
            var blocks = new List<XBlock>();

            foreach (var blockItem in blockItems)
            {
                var block = _blockSerializer.Deserialize(sheet, parent, blockItem, thickness);

                blocks.Add(block);

                if (select)
                {
                    Select(block);

                    selected.Blocks.Add(block);
                }
            }

            return blocks;
        }

        public void AddContents(ISheet sheet, ItemBlock blockItem, XBlock content, XBlock selected, bool select, double thickness)
        {
            if (blockItem != null)
            {
                var texts = Add(sheet, blockItem.Texts, content, selected, select, thickness);
                var images = Add(sheet, blockItem.Images, content, selected, select, thickness);
                var lines = Add(sheet, blockItem.Lines, content, selected, select, thickness);
                var rectangles = Add(sheet, blockItem.Rectangles, content, selected, select, thickness);
                var ellipses = Add(sheet, blockItem.Ellipses, content, selected, select, thickness);
                var blocks = Add(sheet, blockItem.Blocks, content, selected, select, thickness);
                var points = Add(sheet, blockItem.Points, content, selected, select, thickness);

                _pointController.UpdateDependencies(blocks, points, lines);
            }
        }

        public void AddBroken(ISheet sheet, ItemBlock blockItem, XBlock content, XBlock selected, bool select, double thickness)
        {
            Add(sheet, blockItem.Texts, content, selected, select, thickness);
            Add(sheet, blockItem.Images, content, selected, select, thickness);
            Add(sheet, blockItem.Lines, content, selected, select, thickness);
            Add(sheet, blockItem.Rectangles, content, selected, select, thickness);
            Add(sheet, blockItem.Ellipses, content, selected, select, thickness);

            foreach (var block in blockItem.Blocks)
            {
                Add(sheet, block.Texts, content, selected, select, thickness);
                Add(sheet, block.Images, content, selected, select, thickness);
                Add(sheet, block.Lines, content, selected, select, thickness);
                Add(sheet, block.Rectangles, content, selected, select, thickness);
                Add(sheet, block.Ellipses, content, selected, select, thickness);
                Add(sheet, block.Blocks, content, selected, select, thickness);
                Add(sheet, block.Points, content, selected, select, thickness);
            }

            Add(sheet, blockItem.Points, content, selected, select, thickness);
        }

        #endregion

        #region Remove

        public void Remove(ISheet sheet, IEnumerable<XPoint> points)
        {
            foreach (var point in points)
            {
                sheet.Remove(point);
            }
        }

        public void Remove(ISheet sheet, IEnumerable<XLine> lines)
        {
            foreach (var line in lines)
            {
                sheet.Remove(line);
            }
        }

        public void Remove(ISheet sheet, IEnumerable<XRectangle> rectangles)
        {
            foreach (var rectangle in rectangles)
            {
                sheet.Remove(rectangle);
            }
        }

        public void Remove(ISheet sheet, IEnumerable<XEllipse> ellipses)
        {
            foreach (var ellipse in ellipses)
            {
                sheet.Remove(ellipse);
            }
        }

        public void Remove(ISheet sheet, IEnumerable<XText> texts)
        {
            foreach (var text in texts)
            {
                sheet.Remove(text);
            }
        }

        public void Remove(ISheet sheet, IEnumerable<XImage> images)
        {
            foreach (var image in images)
            {
                sheet.Remove(image);
            }
        }

        public void Remove(ISheet sheet, IEnumerable<XBlock> blocks)
        {
            foreach (var block in blocks)
            {
                Remove(sheet, block);
            }
        }

        public void Remove(ISheet sheet, XBlock block)
        {
            Remove(sheet, block.Points);
            Remove(sheet, block.Lines);
            Remove(sheet, block.Rectangles);
            Remove(sheet, block.Ellipses);
            Remove(sheet, block.Texts);
            Remove(sheet, block.Images);
            Remove(sheet, block.Blocks);
        }

        public void RemoveSelected(ISheet sheet, XBlock parent, XBlock selected)
        {
            Remove(sheet, selected.Points);
            foreach (var point in selected.Points)
            {
                parent.Points.Remove(point);
            }

            Remove(sheet, selected.Lines);
            foreach (var line in selected.Lines)
            {
                parent.Lines.Remove(line);
            }

            Remove(sheet, selected.Rectangles);
            foreach (var rectangle in selected.Rectangles)
            {
                parent.Rectangles.Remove(rectangle);
            }

            Remove(sheet, selected.Ellipses);
            foreach (var ellipse in selected.Ellipses)
            {
                parent.Ellipses.Remove(ellipse);
            }

            Remove(sheet, selected.Texts);
            foreach (var text in selected.Texts)
            {
                parent.Texts.Remove(text);
            }

            Remove(sheet, selected.Images);
            foreach (var image in selected.Images)
            {
                parent.Images.Remove(image);
            }

            foreach (var block in selected.Blocks)
            {
                Remove(sheet, block.Points);
                Remove(sheet, block.Lines);
                Remove(sheet, block.Rectangles);
                Remove(sheet, block.Ellipses);
                Remove(sheet, block.Texts);
                Remove(sheet, block.Images);
                Remove(sheet, block.Blocks);

                parent.Blocks.Remove(block);
            }
        }

        #endregion

        #region Move

        public void MoveDelta(double dx, double dy, XPoint point)
        {
            if (point.Native != null)
            {
                point.X = _blockHelper.GetLeft(point) + dx;
                point.Y = _blockHelper.GetTop(point) + dy;

                _blockHelper.SetLeft(point, point.X);
                _blockHelper.SetTop(point, point.Y);
            }
            else
            {
                point.X += dx;
                point.Y += dy;
            }

            foreach (var dependency in point.Connected)
            {
                dependency.Update(dependency.Element, point);
            }
        }

        public void MoveDelta(double dx, double dy, IEnumerable<XPoint> points)
        {
            foreach (var point in points)
            {
                MoveDelta(dx, dy, point);
            }
        }

        public void MoveDelta(double dx, double dy, IEnumerable<XLine> lines)
        {
            foreach (var line in lines)
            {
                if (line.Start == null)
                {
                    MoveDeltaStart(dx, dy, line);
                }

                if (line.End == null)
                {
                    MoveDeltaEnd(dx, dy, line);
                }
            }
        }

        public void MoveDeltaStart(double dx, double dy, XLine line)
        {
            double oldx = _blockHelper.GetX1(line);
            double oldy = _blockHelper.GetY1(line);
            _blockHelper.SetX1(line, oldx + dx);
            _blockHelper.SetY1(line, oldy + dy);
        }

        public void MoveDeltaEnd(double dx, double dy, XLine line)
        {
            double oldx = _blockHelper.GetX2(line);
            double oldy = _blockHelper.GetY2(line);
            _blockHelper.SetX2(line, oldx + dx);
            _blockHelper.SetY2(line, oldy + dy);
        }

        public void MoveDelta(double dx, double dy, XRectangle rectangle)
        {
            double left = _blockHelper.GetLeft(rectangle) + dx;
            double top = _blockHelper.GetTop(rectangle) + dy;
            _blockHelper.SetLeft(rectangle, left);
            _blockHelper.SetTop(rectangle, top);
        }

        public void MoveDelta(double dx, double dy, IEnumerable<XRectangle> rectangles)
        {
            foreach (var rectangle in rectangles)
            {
                MoveDelta(dx, dy, rectangle);
            }
        }

        public void MoveDelta(double dx, double dy, XEllipse ellipse)
        {
            double left = _blockHelper.GetLeft(ellipse) + dx;
            double top = _blockHelper.GetTop(ellipse) + dy;
            _blockHelper.SetLeft(ellipse, left);
            _blockHelper.SetTop(ellipse, top);
        }

        public void MoveDelta(double dx, double dy, IEnumerable<XEllipse> ellipses)
        {
            foreach (var ellipse in ellipses)
            {
                MoveDelta(dx, dy, ellipse);
            }
        }

        public void MoveDelta(double dx, double dy, XText text)
        {
            double left = _blockHelper.GetLeft(text) + dx;
            double top = _blockHelper.GetTop(text) + dy;
            _blockHelper.SetLeft(text, left);
            _blockHelper.SetTop(text, top);
        }

        public void MoveDelta(double dx, double dy, IEnumerable<XText> texts)
        {
            foreach (var text in texts)
            {
                MoveDelta(dx, dy, text);
            }
        }

        public void MoveDelta(double dx, double dy, XImage image)
        {
            double left = _blockHelper.GetLeft(image) + dx;
            double top = _blockHelper.GetTop(image) + dy;
            _blockHelper.SetLeft(image, left);
            _blockHelper.SetTop(image, top);
        }

        public void MoveDelta(double dx, double dy, IEnumerable<XImage> images)
        {
            foreach (var image in images)
            {
                MoveDelta(dx, dy, image);
            }
        }

        public void MoveDelta(double dx, double dy, XBlock block)
        {
            MoveDelta(dx, dy, block.Points);
            MoveDelta(dx, dy, block.Lines);
            MoveDelta(dx, dy, block.Rectangles);
            MoveDelta(dx, dy, block.Ellipses);
            MoveDelta(dx, dy, block.Texts);
            MoveDelta(dx, dy, block.Images);
            MoveDelta(dx, dy, block.Blocks);
        }

        public void MoveDelta(double dx, double dy, IEnumerable<XBlock> blocks)
        {
            foreach (var block in blocks)
            {
                MoveDelta(dx, dy, block.Points);
                MoveDelta(dx, dy, block.Lines);
                MoveDelta(dx, dy, block.Rectangles);
                MoveDelta(dx, dy, block.Ellipses);
                MoveDelta(dx, dy, block.Texts);
                MoveDelta(dx, dy, block.Images);
                MoveDelta(dx, dy, block.Blocks);
            }
        }

        #endregion

        #region Select

        private int DeselectedZIndex = 0;
        private int SelectedZIndex = 1;

        public void Deselect(XPoint point)
        {
            _blockHelper.SetIsSelected(point, false);
        }

        public void Deselect(XLine line)
        {
            _blockHelper.Deselect(line);
            _blockHelper.SetZIndex(line, DeselectedZIndex);
        }

        public void Deselect(XRectangle rectangle)
        {
            _blockHelper.Deselect(rectangle);
            _blockHelper.SetZIndex(rectangle, DeselectedZIndex);
        }

        public void Deselect(XEllipse ellipse)
        {
            _blockHelper.Deselect(ellipse);
            _blockHelper.SetZIndex(ellipse, DeselectedZIndex);
        }

        public void Deselect(XText text)
        {
            _blockHelper.Deselect(text);
            _blockHelper.SetZIndex(text, DeselectedZIndex);
        }

        public void Deselect(XImage image)
        {
            _blockHelper.Deselect(image);
            _blockHelper.SetZIndex(image, DeselectedZIndex);
        }

        public void Deselect(XBlock parent)
        {
            foreach (var point in parent.Points)
            {
                Deselect(point);
            }

            foreach (var line in parent.Lines)
            {
                Deselect(line);
            }

            foreach (var rectangle in parent.Rectangles)
            {
                Deselect(rectangle);
            }

            foreach (var ellipse in parent.Ellipses)
            {
                Deselect(ellipse);
            }

            foreach (var text in parent.Texts)
            {
                Deselect(text);
            }

            foreach (var image in parent.Images)
            {
                Deselect(image);
            }

            foreach (var block in parent.Blocks)
            {
                Deselect(block);
            }
        }

        public void Select(XPoint point)
        {
            _blockHelper.SetIsSelected(point, true);
        }

        public void Select(XLine line)
        {
            _blockHelper.Select(line);
            _blockHelper.SetZIndex(line, SelectedZIndex);
        }

        public void Select(XRectangle rectangle)
        {
            _blockHelper.Select(rectangle);
            _blockHelper.SetZIndex(rectangle, SelectedZIndex);
        }

        public void Select(XEllipse ellipse)
        {
            _blockHelper.Select(ellipse);
            _blockHelper.SetZIndex(ellipse, SelectedZIndex);
        }

        public void Select(XText text)
        {
            _blockHelper.Select(text);
            _blockHelper.SetZIndex(text, SelectedZIndex);
        }

        public void Select(XImage image)
        {
            _blockHelper.Select(image);
            _blockHelper.SetZIndex(image, SelectedZIndex);
        }

        public void Select(XBlock parent)
        {
            foreach (var point in parent.Points)
            {
                Select(point);
            }

            foreach (var line in parent.Lines)
            {
                Select(line);
            }

            foreach (var rectangle in parent.Rectangles)
            {
                Select(rectangle);
            }

            foreach (var ellipse in parent.Ellipses)
            {
                Select(ellipse);
            }

            foreach (var text in parent.Texts)
            {
                Select(text);
            }

            foreach (var image in parent.Images)
            {
                Select(image);
            }

            foreach (var block in parent.Blocks)
            {
                Select(block);
            }
        }

        #endregion

        #region HaveSelected

        public bool HaveSelected(XBlock selected)
        {
            return (selected.Points.Count > 0
                || selected.Lines.Count > 0
                || selected.Rectangles.Count > 0
                || selected.Ellipses.Count > 0
                || selected.Texts.Count > 0
                || selected.Images.Count > 0
                || selected.Blocks.Count > 0);
        }

        public bool HaveOnlyOnePointSelected(XBlock selected)
        {
            return (selected.Points.Count == 1
                && selected.Lines.Count == 0
                && selected.Rectangles.Count == 0
                && selected.Ellipses.Count == 0
                && selected.Texts.Count == 0
                && selected.Images.Count == 0
                && selected.Blocks.Count == 0);
        }

        public bool HaveOnlyOneLineSelected(XBlock selected)
        {
            return (selected.Points.Count == 0
                && selected.Lines.Count == 1
                && selected.Rectangles.Count == 0
                && selected.Ellipses.Count == 0
                && selected.Texts.Count == 0
                && selected.Images.Count == 0
                && selected.Blocks.Count == 0);
        }

        public bool HaveOnlyOneRectangleSelected(XBlock selected)
        {
            return (selected.Points.Count == 0
                && selected.Lines.Count == 0
                && selected.Rectangles.Count == 1
                && selected.Ellipses.Count == 0
                && selected.Texts.Count == 0
                && selected.Images.Count == 0
                && selected.Blocks.Count == 0);
        }

        public bool HaveOnlyOneEllipseSelected(XBlock selected)
        {
            return (selected.Points.Count == 0
                && selected.Lines.Count == 0
                && selected.Rectangles.Count == 0
                && selected.Ellipses.Count == 1
                && selected.Texts.Count == 0
                && selected.Images.Count == 0
                && selected.Blocks.Count == 0);
        }

        public bool HaveOnlyOneTextSelected(XBlock selected)
        {
            return (selected.Points.Count == 0
                && selected.Lines.Count == 0
                && selected.Rectangles.Count == 0
                && selected.Ellipses.Count == 0
                && selected.Texts.Count == 1
                && selected.Images.Count == 0
                && selected.Blocks.Count == 0);
        }

        public bool HaveOnlyOneImageSelected(XBlock selected)
        {
            return (selected.Points.Count == 0
                && selected.Lines.Count == 0
                && selected.Rectangles.Count == 0
                && selected.Ellipses.Count == 0
                && selected.Texts.Count == 0
                && selected.Images.Count == 1
                && selected.Blocks.Count == 0);
        }

        public bool HaveOnlyOneBlockSelected(XBlock selected)
        {
            return (selected.Points.Count == 0
                && selected.Lines.Count == 0
                && selected.Rectangles.Count == 0
                && selected.Ellipses.Count == 0
                && selected.Texts.Count == 0
                && selected.Images.Count == 0
                && selected.Blocks.Count == 1);
        }

        #endregion

        #region HitTest

        public bool HitTest(IEnumerable<XPoint> points, ImmutableRect rect, object relativeTo, XBlock selected, bool findOnlyOne)
        {
            foreach (var point in points)
            {
                if (_blockHelper.HitTest(point, rect, relativeTo))
                {
                    if (selected != null)
                    {
                        selected.Points.Add(point);
                    }

                    if (findOnlyOne)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool HitTest(IEnumerable<XLine> lines, ImmutableRect rect, XBlock selected, bool findOnlyOne)
        {
            foreach (var line in lines)
            {
                if (_blockHelper.HitTest(line, rect))
                {
                    if (selected != null)
                    {
                        selected.Lines.Add(line);
                    }

                    if (findOnlyOne)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool HitTest(IEnumerable<XRectangle> rectangles, ImmutableRect rect, object relativeTo, XBlock selected, bool findOnlyOne)
        {
            foreach (var rectangle in rectangles)
            {
                if (_blockHelper.HitTest(rectangle, rect, relativeTo))
                {
                    if (selected != null)
                    {
                        selected.Rectangles.Add(rectangle);
                    }

                    if (findOnlyOne)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool HitTest(IEnumerable<XEllipse> ellipses, ImmutableRect rect, object relativeTo, XBlock selected, bool findOnlyOne)
        {
            foreach (var ellipse in ellipses)
            {
                if (_blockHelper.HitTest(ellipse, rect, relativeTo))
                {
                    if (selected != null)
                    {
                        selected.Ellipses.Add(ellipse);
                    }

                    if (findOnlyOne)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool HitTest(IEnumerable<XText> texts, ImmutableRect rect, object relativeTo, XBlock selected, bool findOnlyOne)
        {
            foreach (var text in texts)
            {
                if (_blockHelper.HitTest(text, rect, relativeTo))
                {
                    if (selected != null)
                    {
                        selected.Texts.Add(text);
                    }

                    if (findOnlyOne)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool HitTest(IEnumerable<XImage> images, ImmutableRect rect, object relativeTo, XBlock selected, bool findOnlyOne)
        {
            foreach (var image in images)
            {
                if (_blockHelper.HitTest(image, rect, relativeTo))
                {
                    if (selected != null)
                    {
                        selected.Images.Add(image);
                    }

                    if (findOnlyOne)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool HitTest(IEnumerable<XBlock> blocks, ImmutableRect rect, object relativeTo, XBlock selected, bool findOnlyOne, bool findInsideBlock)
        {
            foreach (var block in blocks)
            {
                if (HitTest(block, rect, relativeTo, findInsideBlock ? selected : null, true, false))
                {
                    if (selected != null)
                    {
                        selected.Blocks.Add(block);
                    }

                    if (findOnlyOne)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool HitTest(XBlock block, ImmutableRect rect, object relativeTo, XBlock selected, bool findOnlyOne, bool findInsideBlock)
        {
            if (HitTest(block.Points, rect, relativeTo, selected, findOnlyOne))
            {
                if (findOnlyOne)
                {
                    return true;
                }
            }

            if (HitTest(block.Texts, rect, relativeTo, selected, findOnlyOne))
            {
                if (findOnlyOne)
                {
                    return true;
                }
            }

            if (HitTest(block.Images, rect, relativeTo, selected, findOnlyOne))
            {
                if (findOnlyOne)
                {
                    return true;
                }
            }

            if (HitTest(block.Lines, rect, selected, findOnlyOne))
            {
                if (findOnlyOne)
                {
                    return true;
                }
            }

            if (HitTest(block.Rectangles, rect, relativeTo, selected, findOnlyOne))
            {
                if (findOnlyOne)
                {
                    return true;
                }
            }

            if (HitTest(block.Ellipses, rect, relativeTo, selected, findOnlyOne))
            {
                if (findOnlyOne)
                {
                    return true;
                }
            }

            if (HitTest(block.Blocks, rect, relativeTo, selected, findOnlyOne, findInsideBlock))
            {
                if (findOnlyOne)
                {
                    return true;
                }
            }

            return false;
        }

        public bool HitTest(ISheet sheet, XBlock block, ImmutableRect rect, XBlock selected, bool findOnlyOne, bool findInsideBlock)
        {
            if (HitTest(block.Points, rect, sheet.GetParent(), selected, findOnlyOne))
            {
                if (findOnlyOne)
                {
                    return true;
                }
            }

            if (HitTest(block.Texts, rect, sheet.GetParent(), selected, findOnlyOne))
            {
                if (findOnlyOne)
                {
                    return true;
                }
            }

            if (HitTest(block.Images, rect, sheet.GetParent(), selected, findOnlyOne))
            {
                if (findOnlyOne)
                {
                    return true;
                }
            }

            if (HitTest(block.Lines, rect, selected, findOnlyOne))
            {
                if (findOnlyOne)
                {
                    return true;
                }
            }

            if (HitTest(block.Rectangles, rect, sheet.GetParent(), selected, findOnlyOne))
            {
                if (findOnlyOne)
                {
                    return true;
                }
            }

            if (HitTest(block.Ellipses, rect, sheet.GetParent(), selected, findOnlyOne))
            {
                if (findOnlyOne)
                {
                    return true;
                }
            }

            if (HitTest(block.Blocks, rect, sheet.GetParent(), selected, findOnlyOne, findInsideBlock))
            {
                if (findOnlyOne)
                {
                    return true;
                }
            }

            return false;
        }

        public bool HitTest(ISheet sheet, XBlock block, ImmutablePoint p, double size, XBlock selected, bool findOnlyOne, bool findInsideBlock)
        {
            return HitTest(sheet, block, new ImmutableRect(p.X - size, p.Y - size, 2 * size, 2 * size), selected, findOnlyOne, findInsideBlock);
        }

        #endregion

        #region Toggle Fill

        public void ToggleFill(XRectangle rectangle)
        {
            _blockHelper.ToggleFill(rectangle);
        }

        public void ToggleFill(XEllipse ellipse)
        {
            _blockHelper.ToggleFill(ellipse);
        }

        public void ToggleFill(XPoint point)
        {
            _blockHelper.ToggleFill(point);
        }

        #endregion

        #region Shallow Copy

        public void ShallowCopy(XBlock original, XBlock copy)
        {
            copy.Backgroud = original.Backgroud;
            copy.Points = new List<XPoint>(original.Points);
            copy.Lines = new List<XLine>(original.Lines);
            copy.Rectangles = new List<XRectangle>(original.Rectangles);
            copy.Ellipses = new List<XEllipse>(original.Ellipses);
            copy.Texts = new List<XText>(original.Texts);
            copy.Images = new List<XImage>(original.Images);
            copy.Blocks = new List<XBlock>(original.Blocks);
            copy.Points = new List<XPoint>(original.Points);
        }

        #endregion
    }
}
