// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Sheet.Core;
using System;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Collections.Generic;

namespace Sheet.Editor
{
    public class ItemController : IItemController
    {
        #region Text

        public async Task<string> OpenText(string fileName)
        {
            try
            {
                using (var stream = System.IO.File.OpenText(fileName))
                {
                    return await stream.ReadToEndAsync();
                }
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
                Debug.Print(ex.StackTrace);
            }
            return null;
        }

        public async void SaveText(string fileName, string text)
        {
            try
            {
                if (text != null)
                {
                    using (var stream = System.IO.File.CreateText(fileName))
                    {
                        await stream.WriteAsync(text);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
                Debug.Print(ex.StackTrace);
            }
        }

        #endregion

        #region Position

        public void ResetPosition(ItemBlock block, double originX, double originY, double width, double height)
        {
            double minX = width;
            double minY = height;
            double maxX = originX;
            double maxY = originY;
            MinMax(block, ref minX, ref minY, ref maxX, ref maxY);
            double x = -(maxX - (maxX - minX));
            double y = -(maxY - (maxY - minY));
            Move(block, x, y);
        }

        #endregion

        #region MinMax

        public void MinMax(ItemBlock block, ref double minX, ref double minY, ref double maxX, ref double maxY)
        {
            MinMax(block.Points, ref minX, ref minY, ref maxX, ref maxY);
            MinMax(block.Lines, ref minX, ref minY, ref maxX, ref maxY);
            MinMax(block.Rectangles, ref minX, ref minY, ref maxX, ref maxY);
            MinMax(block.Ellipses, ref minX, ref minY, ref maxX, ref maxY);
            MinMax(block.Texts, ref minX, ref minY, ref maxX, ref maxY);
            MinMax(block.Images, ref minX, ref minY, ref maxX, ref maxY);
            MinMax(block.Blocks, ref minX, ref minY, ref maxX, ref maxY);
        }

        public void MinMax(IEnumerable<ItemBlock> blocks, ref double minX, ref double minY, ref double maxX, ref double maxY)
        {
            foreach (var block in blocks)
            {
                MinMax(block, ref minX, ref minY, ref maxX, ref maxY);
            }
        }

        public void MinMax(IEnumerable<ItemPoint> points, ref double minX, ref double minY, ref double maxX, ref double maxY)
        {
            foreach (var point in points)
            {
                minX = Math.Min(minX, point.X);
                minY = Math.Min(minY, point.Y);
                maxX = Math.Max(maxX, point.X);
                maxY = Math.Max(maxY, point.Y);
            }
        }

        public void MinMax(IEnumerable<ItemLine> lines, ref double minX, ref double minY, ref double maxX, ref double maxY)
        {
            foreach (var line in lines)
            {
                minX = Math.Min(minX, line.X1);
                minX = Math.Min(minX, line.X2);
                minY = Math.Min(minY, line.Y1);
                minY = Math.Min(minY, line.Y2);
                maxX = Math.Max(maxX, line.X1);
                maxX = Math.Max(maxX, line.X2);
                maxY = Math.Max(maxY, line.Y1);
                maxY = Math.Max(maxY, line.Y2);
            }
        }

        public void MinMax(IEnumerable<ItemRectangle> rectangles, ref double minX, ref double minY, ref double maxX, ref double maxY)
        {
            foreach (var rectangle in rectangles)
            {
                minX = Math.Min(minX, rectangle.X);
                minY = Math.Min(minY, rectangle.Y);
                maxX = Math.Max(maxX, rectangle.X);
                maxY = Math.Max(maxY, rectangle.Y);
            }
        }

        public void MinMax(IEnumerable<ItemEllipse> ellipses, ref double minX, ref double minY, ref double maxX, ref double maxY)
        {
            foreach (var ellipse in ellipses)
            {
                minX = Math.Min(minX, ellipse.X);
                minY = Math.Min(minY, ellipse.Y);
                maxX = Math.Max(maxX, ellipse.X);
                maxY = Math.Max(maxY, ellipse.Y);
            }
        }

        public void MinMax(IEnumerable<ItemText> texts, ref double minX, ref double minY, ref double maxX, ref double maxY)
        {
            foreach (var text in texts)
            {
                minX = Math.Min(minX, text.X);
                minY = Math.Min(minY, text.Y);
                maxX = Math.Max(maxX, text.X);
                maxY = Math.Max(maxY, text.Y);
            }
        }

        public void MinMax(IEnumerable<ItemImage> images, ref double minX, ref double minY, ref double maxX, ref double maxY)
        {
            foreach (var image in images)
            {
                minX = Math.Min(minX, image.X);
                minY = Math.Min(minY, image.Y);
                maxX = Math.Max(maxX, image.X);
                maxY = Math.Max(maxY, image.Y);
            }
        }

        #endregion

        #region Move

        public void Move(IEnumerable<ItemBlock> blocks, double x, double y)
        {
            foreach (var block in blocks)
            {
                Move(block, x, y);
            }
        }

        public void Move(ItemBlock block, double x, double y)
        {
            Move(block.Points, x, y);
            Move(block.Lines, x, y);
            Move(block.Rectangles, x, y);
            Move(block.Ellipses, x, y);
            Move(block.Texts, x, y);
            Move(block.Images, x, y);
            Move(block.Blocks, x, y);
        }

        public void Move(IEnumerable<ItemPoint> points, double x, double y)
        {
            foreach (var point in points)
            {
                Move(point, x, y);
            }
        }

        public void Move(IEnumerable<ItemLine> lines, double x, double y)
        {
            foreach (var line in lines)
            {
                Move(line, x, y);
            }
        }

        public void Move(IEnumerable<ItemRectangle> rectangles, double x, double y)
        {
            foreach (var rectangle in rectangles)
            {
                Move(rectangle, x, y);
            }
        }

        public void Move(IEnumerable<ItemEllipse> ellipses, double x, double y)
        {
            foreach (var ellipse in ellipses)
            {
                Move(ellipse, x, y);
            }
        }

        public void Move(IEnumerable<ItemText> texts, double x, double y)
        {
            foreach (var text in texts)
            {
                Move(text, x, y);
            }
        }

        public void Move(IEnumerable<ItemImage> images, double x, double y)
        {
            foreach (var image in images)
            {
                Move(image, x, y);
            }
        }

        public void Move(ItemPoint point, double x, double y)
        {
            point.X += x;
            point.Y += y;
        }

        public void Move(ItemLine line, double x, double y)
        {
            line.X1 += x;
            line.Y1 += y;
            line.X2 += x;
            line.Y2 += y;
        }

        public void Move(ItemRectangle rectangle, double x, double y)
        {
            rectangle.X += x;
            rectangle.Y += y;
        }

        public void Move(ItemEllipse ellipse, double x, double y)
        {
            ellipse.X += x;
            ellipse.Y += y;
        }

        public void Move(ItemText text, double x, double y)
        {
            text.X += x;
            text.Y += y;
        }

        public void Move(ItemImage image, double x, double y)
        {
            image.X += x;
            image.Y += y;
        }

        #endregion

        #region Snap

        public double Snap(double val, double snap)
        {
            double r = val % snap;
            return r >= snap / 2.0 ? val + snap - r : val - r;
        }

        #endregion
    }
}
