// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Sheet.Core;
using Sheet.IoC;
using System;

namespace Sheet.Editor
{
    public class BlockSerializer : IBlockSerializer
    {
        #region IoC

        private readonly IServiceLocator _serviceLocator;
        private readonly IBlockHelper _blockHelper;
        private readonly IBlockFactory _blockFactory;

        public BlockSerializer(IServiceLocator serviceLocator)
        {
            this._serviceLocator = serviceLocator;
            this._blockHelper = serviceLocator.GetInstance<IBlockHelper>();
            this._blockFactory = serviceLocator.GetInstance<IBlockFactory>();
        }

        #endregion

        #region Ids

        private int SetId(XBlock parent, int nextId)
        {
            foreach (var point in parent.Points)
            {
                point.Id = nextId++;
            }

            foreach (var line in parent.Lines)
            {
                line.Id = nextId++;
            }

            foreach (var rectangle in parent.Rectangles)
            {
                rectangle.Id = nextId++;
            }

            foreach (var ellipse in parent.Ellipses)
            {
                ellipse.Id = nextId++;
            }

            foreach (var text in parent.Texts)
            {
                text.Id = nextId++;
            }

            foreach (var image in parent.Images)
            {
                image.Id = nextId++;
            }

            foreach (var block in parent.Blocks)
            {
                block.Id = nextId++;
                nextId = SetId(block, nextId);
            }

            return nextId;
        }

        #endregion

        #region Serialize

        private ArgbColor ToXArgbColor(ArgbColor color)
        {
            return new ArgbColor(
                color.A,
                color.R,
                color.G,
                color.B);
        }

        public ItemPoint Serialize(XPoint point)
        {
            return new ItemPoint()
            {
                Id = point.Id,
                X = _blockHelper.GetLeft(point),
                Y = _blockHelper.GetTop(point)
            };
        }

        public ItemLine Serialize(XLine line)
        {
            return new ItemLine()
            {
                Id = line.Id,
                X1 = _blockHelper.GetX1(line),
                Y1 = _blockHelper.GetY1(line),
                X2 = _blockHelper.GetX2(line),
                Y2 = _blockHelper.GetY2(line),
                Stroke = _blockHelper.GetStroke(line),
                StartId = line.Start == null ? -1 : line.Start.Id,
                EndId = line.End == null ? -1 : line.End.Id
            };
        }

        public ItemRectangle Serialize(XRectangle rectangle)
        {
            return new ItemRectangle()
            {
                Id = rectangle.Id,
                X = _blockHelper.GetLeft(rectangle),
                Y = _blockHelper.GetTop(rectangle),
                Width = _blockHelper.GetWidth(rectangle),
                Height = _blockHelper.GetHeight(rectangle),
                IsFilled = _blockHelper.IsTransparent(rectangle),
                Stroke = _blockHelper.GetStroke(rectangle),
                Fill = _blockHelper.GetFill(rectangle)
            };
        }

        public ItemEllipse Serialize(XEllipse ellipse)
        {
            return new ItemEllipse()
            {
                Id = ellipse.Id,
                X = _blockHelper.GetLeft(ellipse),
                Y = _blockHelper.GetTop(ellipse),
                Width = _blockHelper.GetWidth(ellipse),
                Height = _blockHelper.GetHeight(ellipse),
                IsFilled = _blockHelper.IsTransparent(ellipse),
                Stroke = _blockHelper.GetStroke(ellipse),
                Fill = _blockHelper.GetFill(ellipse)
            };
        }

        public ItemText Serialize(XText text)
        {
            return new ItemText()
            {
                Id = text.Id,
                X = _blockHelper.GetLeft(text),
                Y = _blockHelper.GetTop(text),
                Width = _blockHelper.GetWidth(text),
                Height = _blockHelper.GetHeight(text),
                Text = _blockHelper.GetText(text),
                HAlign = _blockHelper.GetHAlign(text),
                VAlign = _blockHelper.GetVAlign(text),
                Size = _blockHelper.GetSize(text),
                Backgroud = _blockHelper.GetBackground(text),
                Foreground = _blockHelper.GetForeground(text)
            };
        }

        public ItemImage Serialize(XImage image)
        {
            return new ItemImage()
            {
                Id = image.Id,
                X = _blockHelper.GetLeft(image),
                Y = _blockHelper.GetTop(image),
                Width = _blockHelper.GetWidth(image),
                Height = _blockHelper.GetHeight(image),
                Data = _blockHelper.GetData(image)
            };
        }

        private ItemBlock Serialize(XBlock parent, int id, double x, double y, double width, double height, int dataId, string name, ArgbColor backgroud)
        {
            var blockItem = new ItemBlock(id, x, y, width, height, dataId, name, ToXArgbColor(backgroud));

            foreach (var point in parent.Points)
            {
                blockItem.Points.Add(Serialize(point));
            }

            foreach (var line in parent.Lines)
            {
                blockItem.Lines.Add(Serialize(line));
            }

            foreach (var rectangle in parent.Rectangles)
            {
                blockItem.Rectangles.Add(Serialize(rectangle));
            }

            foreach (var ellipse in parent.Ellipses)
            {
                blockItem.Ellipses.Add(Serialize(ellipse));
            }

            foreach (var text in parent.Texts)
            {
                blockItem.Texts.Add(Serialize(text));
            }

            foreach (var image in parent.Images)
            {
                blockItem.Images.Add(Serialize(image));
            }

            foreach (var block in parent.Blocks)
            {
                blockItem.Blocks.Add(Serialize(block));
            }

            return blockItem;
        }

        public ItemBlock Serialize(XBlock parent)
        {
            return Serialize(parent, parent.Id, parent.X, parent.Y, parent.Width, parent.Height, parent.DataId, parent.Name, parent.Backgroud);
        }

        public ItemBlock SerializerAndSetId(XBlock parent, int id, double x, double y, double width, double height, int dataId, string name)
        {
            SetId(parent, id + 1);
            return Serialize(parent, id, x, y, width, height, dataId, name, parent.Backgroud);
        }

        #endregion

        #region Deserialize

        public XPoint Deserialize(ISheet sheet, XBlock parent, ItemPoint pointItem, double thickness)
        {
            var point = _blockFactory.CreatePoint(thickness, pointItem.X, pointItem.Y, false);

            point.Id = pointItem.Id;

            parent.Points.Add(point);
            sheet.Add(point);

            return point;
        }

        public XLine Deserialize(ISheet sheet, XBlock parent, ItemLine lineItem, double thickness)
        {
            var line = _blockFactory.CreateLine(thickness, lineItem.X1, lineItem.Y1, lineItem.X2, lineItem.Y2, lineItem.Stroke);

            line.Id = lineItem.Id;
            line.StartId = lineItem.StartId;
            line.EndId = lineItem.EndId;

            parent.Lines.Add(line);
            sheet.Add(line);

            return line;
        }

        public XRectangle Deserialize(ISheet sheet, XBlock parent, ItemRectangle rectangleItem, double thickness)
        {
            var rectangle = _blockFactory.CreateRectangle(thickness,
                rectangleItem.X,
                rectangleItem.Y,
                rectangleItem.Width,
                rectangleItem.Height,
                rectangleItem.IsFilled,
                rectangleItem.Stroke,
                rectangleItem.Fill);

            rectangle.Id = rectangleItem.Id;

            parent.Rectangles.Add(rectangle);
            sheet.Add(rectangle);

            return rectangle;
        }

        public XEllipse Deserialize(ISheet sheet, XBlock parent, ItemEllipse ellipseItem, double thickness)
        {
            var ellipse = _blockFactory.CreateEllipse(thickness,
                ellipseItem.X,
                ellipseItem.Y,
                ellipseItem.Width,
                ellipseItem.Height,
                ellipseItem.IsFilled,
                ellipseItem.Stroke,
                ellipseItem.Fill);

            ellipse.Id = ellipseItem.Id;

            parent.Ellipses.Add(ellipse);
            sheet.Add(ellipse);

            return ellipse;
        }

        public XText Deserialize(ISheet sheet, XBlock parent, ItemText textItem)
        {
            var text = _blockFactory.CreateText(textItem.Text,
                textItem.X, textItem.Y,
                textItem.Width, textItem.Height,
                textItem.HAlign,
                textItem.VAlign,
                textItem.Size,
                textItem.Backgroud,
                textItem.Foreground);

            text.Id = textItem.Id;

            parent.Texts.Add(text);
            sheet.Add(text);

            return text;
        }

        public XImage Deserialize(ISheet sheet, XBlock parent, ItemImage imageItem)
        {
            var image = _blockFactory.CreateImage(imageItem.X,
                imageItem.Y,
                imageItem.Width,
                imageItem.Height,
                imageItem.Data);

            image.Id = imageItem.Id;

            parent.Images.Add(image);
            sheet.Add(image);

            return image;
        }

        public XBlock Deserialize(ISheet sheet, XBlock parent, ItemBlock blockItem, double thickness)
        {
            var block = _blockFactory.CreateBlock(blockItem.Id,
                blockItem.X,
                blockItem.Y,
                blockItem.Width,
                blockItem.Height,
                blockItem.DataId,
                blockItem.Name,
                blockItem.Backgroud);

            foreach (var textItem in blockItem.Texts)
            {
                Deserialize(sheet, block, textItem);
            }

            foreach (var imageItem in blockItem.Images)
            {
                Deserialize(sheet, block, imageItem);
            }

            foreach (var lineItem in blockItem.Lines)
            {
                Deserialize(sheet, block, lineItem, thickness);
            }

            foreach (var rectangleItem in blockItem.Rectangles)
            {
                Deserialize(sheet, block, rectangleItem, thickness);
            }

            foreach (var ellipseItem in blockItem.Ellipses)
            {
                Deserialize(sheet, block, ellipseItem, thickness);
            }

            foreach (var childBlockItem in blockItem.Blocks)
            {
                Deserialize(sheet, block, childBlockItem, thickness);
            }

            foreach (var pointItem in blockItem.Points)
            {
                Deserialize(sheet, block, pointItem, thickness);
            }

            parent.Blocks.Add(block);

            return block;
        }

        #endregion
    }
}
