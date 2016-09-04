// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Sheet.Core;
using Sheet.IoC;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Sheet.Editor
{
    public class ItemSerializer : IItemSerializer
    {
        #region IoC

        private readonly IServiceLocator _serviceLocator;
        private readonly IBase64 _base64;

        public ItemSerializer(IServiceLocator serviceLocator)
        {
            this._serviceLocator = serviceLocator;
            this._base64 = serviceLocator.GetInstance<IBase64>();
        }

        #endregion

        #region Serialize

        public void Serialize(StringBuilder sb, ArgbColor color, ItemSerializeOptions options)
        {
            sb.Append(color.A);
            sb.Append(options.ModelSeparator);
            sb.Append(color.R);
            sb.Append(options.ModelSeparator);
            sb.Append(color.G);
            sb.Append(options.ModelSeparator);
            sb.Append(color.B);
        }

        public void Serialize(StringBuilder sb, ItemPoint point, string indent, ItemSerializeOptions options)
        {
            sb.Append(indent);
            sb.Append("POINT");
            sb.Append(options.ModelSeparator);
            sb.Append(point.Id);
            sb.Append(options.ModelSeparator);
            sb.Append(point.X);
            sb.Append(options.ModelSeparator);
            sb.Append(point.Y);
            sb.Append(options.LineSeparator);
        }

        public void Serialize(StringBuilder sb, ItemLine line, string indent, ItemSerializeOptions options)
        {
            sb.Append(indent);
            sb.Append("LINE");
            sb.Append(options.ModelSeparator);
            sb.Append(line.Id);
            sb.Append(options.ModelSeparator);
            sb.Append(line.X1);
            sb.Append(options.ModelSeparator);
            sb.Append(line.Y1);
            sb.Append(options.ModelSeparator);
            sb.Append(line.X2);
            sb.Append(options.ModelSeparator);
            sb.Append(line.Y2);
            sb.Append(options.ModelSeparator);
            Serialize(sb, line.Stroke, options);
            sb.Append(options.ModelSeparator);
            sb.Append(line.StartId);
            sb.Append(options.ModelSeparator);
            sb.Append(line.EndId);
            sb.Append(options.LineSeparator);
        }

        public void Serialize(StringBuilder sb, ItemRectangle rectangle, string indent, ItemSerializeOptions options)
        {
            sb.Append(indent);
            sb.Append("RECTANGLE");
            sb.Append(options.ModelSeparator);
            sb.Append(rectangle.Id);
            sb.Append(options.ModelSeparator);
            sb.Append(rectangle.X);
            sb.Append(options.ModelSeparator);
            sb.Append(rectangle.Y);
            sb.Append(options.ModelSeparator);
            sb.Append(rectangle.Width);
            sb.Append(options.ModelSeparator);
            sb.Append(rectangle.Height);
            sb.Append(options.ModelSeparator);
            sb.Append(rectangle.IsFilled);
            sb.Append(options.ModelSeparator);
            Serialize(sb, rectangle.Stroke, options);
            sb.Append(options.ModelSeparator);
            Serialize(sb, rectangle.Fill, options);
            sb.Append(options.LineSeparator);
        }

        public void Serialize(StringBuilder sb, ItemEllipse ellipse, string indent, ItemSerializeOptions options)
        {
            sb.Append(indent);
            sb.Append("ELLIPSE");
            sb.Append(options.ModelSeparator);
            sb.Append(ellipse.Id);
            sb.Append(options.ModelSeparator);
            sb.Append(ellipse.X);
            sb.Append(options.ModelSeparator);
            sb.Append(ellipse.Y);
            sb.Append(options.ModelSeparator);
            sb.Append(ellipse.Width);
            sb.Append(options.ModelSeparator);
            sb.Append(ellipse.Height);
            sb.Append(options.ModelSeparator);
            sb.Append(ellipse.IsFilled);
            sb.Append(options.ModelSeparator);
            Serialize(sb, ellipse.Stroke, options);
            sb.Append(options.ModelSeparator);
            Serialize(sb, ellipse.Fill, options);
            sb.Append(options.LineSeparator);
        }

        public void Serialize(StringBuilder sb, ItemText text, string indent, ItemSerializeOptions options)
        {
            sb.Append(indent);
            sb.Append("TEXT");
            sb.Append(options.ModelSeparator);
            sb.Append(text.Id);
            sb.Append(options.ModelSeparator);
            sb.Append(text.X);
            sb.Append(options.ModelSeparator);
            sb.Append(text.Y);
            sb.Append(options.ModelSeparator);
            sb.Append(text.Width);
            sb.Append(options.ModelSeparator);
            sb.Append(text.Height);
            sb.Append(options.ModelSeparator);
            sb.Append(text.HAlign);
            sb.Append(options.ModelSeparator);
            sb.Append(text.VAlign);
            sb.Append(options.ModelSeparator);
            sb.Append(text.Size);
            sb.Append(options.ModelSeparator);
            Serialize(sb, text.Foreground, options);
            sb.Append(options.ModelSeparator);
            Serialize(sb, text.Backgroud, options);
            sb.Append(options.ModelSeparator);
            sb.Append(text.Text);
            sb.Append(options.LineSeparator);
        }

        public void Serialize(StringBuilder sb, ItemImage image, string indent, ItemSerializeOptions options)
        {
            sb.Append(indent);
            sb.Append("IMAGE");
            sb.Append(options.ModelSeparator);
            sb.Append(image.Id);
            sb.Append(options.ModelSeparator);
            sb.Append(image.X);
            sb.Append(options.ModelSeparator);
            sb.Append(image.Y);
            sb.Append(options.ModelSeparator);
            sb.Append(image.Width);
            sb.Append(options.ModelSeparator);
            sb.Append(image.Height);
            sb.Append(options.ModelSeparator);
            sb.Append(_base64.ToBase64(image.Data));
            sb.Append(options.LineSeparator);
        }

        public void Serialize(StringBuilder sb, ItemBlock block, string indent, ItemSerializeOptions options)
        {
            sb.Append(indent);
            sb.Append("BLOCK");
            sb.Append(options.ModelSeparator);
            sb.Append(block.Id);
            sb.Append(options.ModelSeparator);
            sb.Append(block.X);
            sb.Append(options.ModelSeparator);
            sb.Append(block.Y);
            sb.Append(options.ModelSeparator);
            sb.Append(block.Name);
            sb.Append(options.ModelSeparator);
            sb.Append(block.Width);
            sb.Append(options.ModelSeparator);
            sb.Append(block.Height);
            sb.Append(options.ModelSeparator);
            Serialize(sb, block.Backgroud, options);
            sb.Append(options.ModelSeparator);
            sb.Append(block.DataId);
            sb.Append(options.LineSeparator);

            Serialize(sb, block.Points, indent + options.IndentWhiteSpace, options);
            Serialize(sb, block.Lines, indent + options.IndentWhiteSpace, options);
            Serialize(sb, block.Rectangles, indent + options.IndentWhiteSpace, options);
            Serialize(sb, block.Ellipses, indent + options.IndentWhiteSpace, options);
            Serialize(sb, block.Texts, indent + options.IndentWhiteSpace, options);
            Serialize(sb, block.Images, indent + options.IndentWhiteSpace, options);
            Serialize(sb, block.Blocks, indent + options.IndentWhiteSpace, options);

            sb.Append(indent);
            sb.Append("END");
            sb.Append(options.LineSeparator);
        }

        public void Serialize(StringBuilder sb, IEnumerable<ItemPoint> points, string indent, ItemSerializeOptions options)
        {
            foreach (var point in points)
            {
                Serialize(sb, point, indent, options);
            }
        }

        public void Serialize(StringBuilder sb, IEnumerable<ItemLine> lines, string indent, ItemSerializeOptions options)
        {
            foreach (var line in lines)
            {
                Serialize(sb, line, indent, options);
            }
        }

        public void Serialize(StringBuilder sb, IEnumerable<ItemRectangle> rectangles, string indent, ItemSerializeOptions options)
        {
            foreach (var rectangle in rectangles)
            {
                Serialize(sb, rectangle, indent, options);
            }
        }

        public void Serialize(StringBuilder sb, IEnumerable<ItemEllipse> ellipses, string indent, ItemSerializeOptions options)
        {
            foreach (var ellipse in ellipses)
            {
                Serialize(sb, ellipse, indent, options);
            }
        }

        public void Serialize(StringBuilder sb, IEnumerable<ItemText> texts, string indent, ItemSerializeOptions options)
        {
            foreach (var text in texts)
            {
                Serialize(sb, text, indent, options);
            }
        }

        public void Serialize(StringBuilder sb, IEnumerable<ItemImage> images, string indent, ItemSerializeOptions options)
        {
            foreach (var image in images)
            {
                Serialize(sb, image, indent, options);
            }
        }

        public void Serialize(StringBuilder sb, IEnumerable<ItemBlock> blocks, string indent, ItemSerializeOptions options)
        {
            foreach (var block in blocks)
            {
                Serialize(sb, block, indent, options);
            }
        }

        public string SerializeContents(ItemBlock block, ItemSerializeOptions options)
        {
            var sb = new StringBuilder();

            Serialize(sb, block.Points, "", options);
            Serialize(sb, block.Lines, "", options);
            Serialize(sb, block.Rectangles, "", options);
            Serialize(sb, block.Ellipses, "", options);
            Serialize(sb, block.Texts, "", options);
            Serialize(sb, block.Images, "", options);
            Serialize(sb, block.Blocks, "", options);

            return sb.ToString();
        }

        public string SerializeContents(ItemBlock block)
        {
            return SerializeContents(block, ItemSerializeOptions.Default);
        }

        #endregion

        #region Deserialize

        private ItemPoint DeserializePoint(string[] m)
        {
            var pointItem = new ItemPoint();
            pointItem.Id = int.Parse(m[1]);
            pointItem.X = double.Parse(m[2]);
            pointItem.Y = double.Parse(m[3]);
            return pointItem;
        }

        private ItemLine DeserializeLine(string[] m)
        {
            var lineItem = new ItemLine();
            lineItem.Id = int.Parse(m[1]);
            lineItem.X1 = double.Parse(m[2]);
            lineItem.Y1 = double.Parse(m[3]);
            lineItem.X2 = double.Parse(m[4]);
            lineItem.Y2 = double.Parse(m[5]);
            lineItem.Stroke = new ArgbColor(
                byte.Parse(m[6]),
                byte.Parse(m[7]),
                byte.Parse(m[8]),
                byte.Parse(m[9]));
            if (m.Length == 12)
            {
                lineItem.StartId = int.Parse(m[10]);
                lineItem.EndId = int.Parse(m[11]);
            }
            else
            {
                lineItem.StartId = -1;
                lineItem.EndId = -1;
            }
            return lineItem;
        }

        private ItemRectangle DeserializeRectangle(string[] m)
        {
            var rectangleItem = new ItemRectangle();
            rectangleItem.Id = int.Parse(m[1]);
            rectangleItem.X = double.Parse(m[2]);
            rectangleItem.Y = double.Parse(m[3]);
            rectangleItem.Width = double.Parse(m[4]);
            rectangleItem.Height = double.Parse(m[5]);
            rectangleItem.IsFilled = bool.Parse(m[6]);
            rectangleItem.Stroke = new ArgbColor(
                byte.Parse(m[7]),
                byte.Parse(m[8]),
                byte.Parse(m[9]),
                byte.Parse(m[10]));
            rectangleItem.Fill = new ArgbColor(
                byte.Parse(m[11]),
                byte.Parse(m[12]),
                byte.Parse(m[13]),
                byte.Parse(m[14]));
            return rectangleItem;
        }

        private ItemEllipse DeserializeEllipse(string[] m)
        {
            var ellipseItem = new ItemEllipse();
            ellipseItem.Id = int.Parse(m[1]);
            ellipseItem.X = double.Parse(m[2]);
            ellipseItem.Y = double.Parse(m[3]);
            ellipseItem.Width = double.Parse(m[4]);
            ellipseItem.Height = double.Parse(m[5]);
            ellipseItem.IsFilled = bool.Parse(m[6]);
            ellipseItem.Stroke = new ArgbColor(
                byte.Parse(m[7]),
                byte.Parse(m[8]),
                byte.Parse(m[9]),
                byte.Parse(m[10]));
            ellipseItem.Fill = new ArgbColor(
                byte.Parse(m[11]),
                byte.Parse(m[12]),
                byte.Parse(m[13]),
                byte.Parse(m[14]));
            return ellipseItem;
        }

        private ItemText DeserializeText(string[] m)
        {
            var textItem = new ItemText();
            textItem.Id = int.Parse(m[1]);
            textItem.X = double.Parse(m[2]);
            textItem.Y = double.Parse(m[3]);
            textItem.Width = double.Parse(m[4]);
            textItem.Height = double.Parse(m[5]);
            textItem.HAlign = int.Parse(m[6]);
            textItem.VAlign = int.Parse(m[7]);
            textItem.Size = double.Parse(m[8]);
            textItem.Foreground = new ArgbColor(
                byte.Parse(m[9]),
                byte.Parse(m[10]),
                byte.Parse(m[11]),
                byte.Parse(m[12]));
            textItem.Backgroud = new ArgbColor(
                byte.Parse(m[13]),
                byte.Parse(m[14]),
                byte.Parse(m[15]),
                byte.Parse(m[16]));
            textItem.Text = m[17];
            return textItem;
        }

        private ItemImage DeserializeImage(string[] m)
        {
            var imageItem = new ItemImage();
            imageItem.Id = int.Parse(m[1]);
            imageItem.X = double.Parse(m[2]);
            imageItem.Y = double.Parse(m[3]);
            imageItem.Width = double.Parse(m[4]);
            imageItem.Height = double.Parse(m[5]);
            imageItem.Data = _base64.ToBytes(m[6]);
            return imageItem;
        }

        private ItemBlock DeserializeBlockRecursive(string[] lines, int length, ref int end, string[] m, ItemSerializeOptions options)
        {
            var blockItem = DeserializeRootBlock(lines,
                length,
                ref end,
                m[4],
                int.Parse(m[1]),
                double.Parse(m[2]),
                double.Parse(m[3]),
                double.Parse(m[5]),
                double.Parse(m[6]),
                int.Parse(m[11]),
                options);

            blockItem.Backgroud = new ArgbColor(
                byte.Parse(m[7]),
                byte.Parse(m[8]),
                byte.Parse(m[9]),
                byte.Parse(m[10]));

            blockItem.DataId = int.Parse(m[11]);

            return blockItem;
        }

        private ItemBlock DeserializeRootBlock(string[] lines, int length, ref int end, string name, int id, double x, double y, double width, double height, int dataId, ItemSerializeOptions options)
        {
            var root = new ItemBlock(id, x, y, width, height, dataId, name, new ArgbColor(0, 0, 0, 0));

            for (; end < length; end++)
            {
                string line = lines[end].TrimStart(options.WhiteSpace);
                var m = line.Split(options.ModelSeparators);

                if (m.Length == 4 && string.Compare(m[0], "POINT", true) == 0)
                {
                    if (m.Length == 4)
                    {
                        var pointItem = DeserializePoint(m);
                        root.Points.Add(pointItem);
                    }
                    else
                    {
                        throw new Exception(string.Format("Invalid POINT item at line {0}", end + 1));
                    }
                }
                else if ((m.Length == 10 || m.Length == 12) && string.Compare(m[0], "LINE", true) == 0)
                {
                    if (m.Length == 10 || m.Length == 12)
                    {
                        var lineItem = DeserializeLine(m);
                        root.Lines.Add(lineItem);
                    }
                    else
                    {
                        throw new Exception(string.Format("Invalid LINE item at line {0}", end + 1));
                    }
                }
                else if (string.Compare(m[0], "RECTANGLE", true) == 0)
                {
                    if (m.Length == 15)
                    {
                        var rectangleItem = DeserializeRectangle(m);
                        root.Rectangles.Add(rectangleItem);
                    }
                    else
                    {
                        throw new Exception(string.Format("Invalid RECTANGLE item at line {0}", end + 1));
                    }
                }
                else if (string.Compare(m[0], "ELLIPSE", true) == 0)
                {
                    if (m.Length == 15)
                    {
                        var ellipseItem = DeserializeEllipse(m);
                        root.Ellipses.Add(ellipseItem);
                    }
                    else
                    {
                        throw new Exception(string.Format("Invalid ELLIPSE item at line {0}", end + 1));
                    }
                }
                else if (string.Compare(m[0], "TEXT", true) == 0)
                {
                    if (m.Length == 18)
                    {
                        var textItem = DeserializeText(m);
                        root.Texts.Add(textItem);
                    }
                    else
                    {
                        throw new Exception(string.Format("Invalid TEXT item at line {0}", end + 1));
                    }
                }
                else if (string.Compare(m[0], "IMAGE", true) == 0)
                {
                    if (m.Length == 7)
                    {
                        var imageItem = DeserializeImage(m);
                        root.Images.Add(imageItem);
                    }
                    else
                    {
                        throw new Exception(string.Format("Invalid IMAGE item at line {0}", end + 1));
                    }
                }
                else if (string.Compare(m[0], "BLOCK", true) == 0)
                {
                    if (m.Length == 12)
                    {
                        end++;
                        var blockItem = DeserializeBlockRecursive(lines, length, ref end, m, options);
                        root.Blocks.Add(blockItem);
                        continue;
                    }
                    else
                    {
                        throw new Exception(string.Format("Invalid BLOCK item at line {0}", end + 1));
                    }
                }
                else if (string.Compare(m[0], "END", true) == 0)
                {
                    if (m.Length == 1)
                    {
                        return root;
                    }
                    else
                    {
                        throw new Exception(string.Format("Invalid END item at line {0}", end + 1));
                    }
                }
                else if (m[0].StartsWith("//"))
                {
                    continue;
                }
                else
                {
                    throw new Exception(string.Format("Invalid item at line {0}", end + 1));
                }
            }

            return root;
        }

        public ItemBlock DeserializeContents(string model, ItemSerializeOptions options)
        {
            try
            {
                string[] lines = model.Split(options.LineSeparators, StringSplitOptions.RemoveEmptyEntries);
                int length = lines.Length;
                int end = 0;
                return DeserializeRootBlock(lines, length, ref end, "", 0, 0.0, 0.0, 0.0, 0.0, -1, options);
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
                Debug.Print(ex.StackTrace);
            }
            return null;
        }

        public ItemBlock DeserializeContents(string model)
        {
            return DeserializeContents(model, ItemSerializeOptions.Default);
        }

        #endregion
    }
}
