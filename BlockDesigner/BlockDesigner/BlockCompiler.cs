// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace BlockDesigner
{
    #region References

    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Markup;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Navigation;
    using System.Windows.Shapes;
    using System.Xml;

    #endregion

    #region BlockCompiler

    public class BlockCompiler
    {
        public static IEnumerable<Tuple<string, object>> Compile(IEnumerable<dynamic> commands)
        {
            double offset = 0.0;
            StringBuilder linesStringBuilder = null;
            Path currentPath = null;
            Grid currentGrid = null;
            Canvas currentCanvas = null;
            string blockName = string.Empty;

            foreach (dynamic command in commands)
            {
                switch (command.Command as string)
                {
                    #region Execute

                    // execute <path>
                    case "execute":
                        {
                            string fileName = command.Path;
                            if (System.IO.File.Exists(fileName))
                            {
                                var lines = BlockParser.LoadLines(fileName);

                                var cmds = BlockParser.ParseLines(lines);

                                foreach (var tuple in Compile(cmds))
                                {
                                    yield return tuple;
                                }
                            }
                        }
                        break;

                    #endregion

                    #region Block

                    // block begin <name> <width> <height>
                    // block end
                    case "block":
                        {
                            switch (command.State as string)
                            {
                                case "begin":
                                    {
                                        if (currentCanvas != null)
                                            break;

                                        double width = 0.0, height = 0.0;
                                        if (double.TryParse(command.Width, out width) &&
                                            double.TryParse(command.Height, out height))
                                        {
                                            var canvas = new Canvas()
                                            {
                                                Background = Brushes.Transparent,
                                                ClipToBounds = false
                                            };

                                            canvas.Width = width;
                                            canvas.Height = height;

                                            blockName = command.Name;

                                            currentCanvas = canvas;
                                        }
                                    }
                                    break;
                                case "end":
                                    {
                                        if (currentCanvas == null)
                                            break;

                                        var tuple = new Tuple<string, object>(blockName + "ControlTemplateKey", currentCanvas);

                                        yield return tuple;

                                        currentCanvas = null;
                                    }
                                    break;
                            }
                        }
                        break;

                    #endregion

                    #region Simulation

                    // simulation <path>
                    case "simulation":
                        {

                        }
                        break;

                    #endregion

                    #region Path

                    // path begin
                    // path end
                    case "path":
                        {
                            switch (command.State as string)
                            {
                                case "begin":
                                    {
                                        if (currentCanvas == null || currentPath != null)
                                            break;

                                        var path = new Path()
                                        {
                                            StrokeThickness = 1.0,
                                            StrokeStartLineCap = PenLineCap.Square,
                                            StrokeEndLineCap = PenLineCap.Square,
                                            StrokeLineJoin = PenLineJoin.Miter,
                                            Stroke = Brushes.Red,
                                            Fill = Brushes.Red
                                        };

                                        RenderOptions.SetEdgeMode(path, EdgeMode.Aliased);
                                        path.SetValue(UIElement.SnapsToDevicePixelsProperty, false);
                                        Canvas.SetLeft(path, offset);
                                        Canvas.SetTop(path, offset);

                                        currentCanvas.Children.Add(path);

                                        currentPath = path;
                                        linesStringBuilder = new StringBuilder();
                                    }
                                    break;
                                case "end":
                                    {
                                        if (currentPath == null)
                                            break;

                                        string pathData = linesStringBuilder.ToString();
                                        Geometry geometry = null;

                                        try
                                        {
                                            geometry = Geometry.Parse(pathData);

                                        }
                                        catch (Exception ex)
                                        {
                                            System.Diagnostics.Debug.Print(ex.Message);
                                        }

                                        if (geometry != null)
                                            currentPath.Data = geometry;

                                        currentPath = null;
                                        linesStringBuilder = null;
                                    }
                                    break;
                            }
                        }
                        break;

                    #endregion

                    #region Line

                    // Add new line to current path
                    // line <x1,y1> <x2,y2> [<x3,y3> ... <xn,yn>] [close]
                    case "line":
                        {
                            if (currentPath == null || linesStringBuilder == null)
                                break;

                            linesStringBuilder.AppendFormat("M{0}", command.Start);
                            linesStringBuilder.AppendFormat("L{0}", command.End);

                            if (command.Points.Length > 0)
                            {
                                foreach (var point in command.Points)
                                    linesStringBuilder.AppendFormat(" {0}", point);
                            }

                            if (command.IsClosed)
                                linesStringBuilder.AppendFormat(" Z\n");
                            else
                                linesStringBuilder.AppendFormat("\n");
                        }
                        break;

                    #endregion

                    #region Pin

                    // pin <name> <x> <y>
                    case "pin":
                        {
                            if (currentCanvas == null)
                                break;

                            double x = 0.0, y = 0.0;
                            if (double.TryParse(command.X, out x) &&
                                double.TryParse(command.Y, out y))
                            {
                                string name = command.Name;

                                var ellipse = new Ellipse();

                                ellipse.SetResourceReference(FrameworkElement.StyleProperty, "BlockEllipseKey");
                                //ellipse.Name = name;
                                ellipse.ToolTip = name;

                                //ellipse.SetValue(UseLayoutRoundingProperty, false);
                                //ellipse.SetValue(SnapsToDevicePixelsProperty, true);

                                Canvas.SetLeft(ellipse, x + offset);
                                Canvas.SetTop(ellipse, y + offset);

                                currentCanvas.Children.Add(ellipse);
                            }
                        }
                        break;

                    #endregion

                    #region Grid

                    // grid begin <x> <y> <width> <height>
                    // grud end
                    case "grid":
                        {
                            switch (command.State as string)
                            {
                                case "begin":
                                    {
                                        if (currentGrid != null || currentCanvas == null)
                                            break;

                                        double x = 0.0, y = 0.0, width = 0.0, height = 0.0;
                                        if (double.TryParse(command.X, out x) &&
                                            double.TryParse(command.Y, out y) &&
                                            double.TryParse(command.Width, out width) &&
                                            double.TryParse(command.Height, out height))
                                        {
                                            var grid = new Grid()
                                            {
                                                Width = width,
                                                Height = height,
                                                Background = Brushes.Transparent
                                            };

                                            Canvas.SetLeft(grid, x + offset);
                                            Canvas.SetTop(grid, y + offset);

                                            currentCanvas.Children.Add(grid);

                                            currentGrid = grid;
                                        }
                                    }
                                    break;
                                case "end":
                                    {
                                        if (currentGrid == null)
                                            break;

                                        currentGrid = null;
                                    }
                                    break;
                            }
                        }
                        break;

                    #endregion

                    #region Row

                    // row <height>
                    case "row":
                        {
                            if (currentGrid == null)
                                break;

                            double height;
                            if (double.TryParse(command.Height, out height) && height > 0)
                            {
                                currentGrid.RowDefinitions.Add(new RowDefinition()
                                {
                                    Height = new GridLength(height)
                                });
                            }
                        }
                        break;

                    #endregion

                    #region Rows

                    // rows <count> <height>
                    case "rows":
                        {
                            if (currentGrid == null)
                                break;

                            double height;
                            int count = 0;
                            if (double.TryParse(command.Height, out height) &&
                                int.TryParse(command.Count, out count) &&
                                height > 0 && count > 0)
                            {
                                for (int i = 0; i < count; i++)
                                {
                                    currentGrid.RowDefinitions.Add(new RowDefinition()
                                    {
                                        Height = new GridLength(height)
                                    });
                                }
                            }
                        }
                        break;

                    #endregion

                    #region Column

                    // column <width>
                    case "column":
                        {
                            if (currentGrid == null)
                                break;

                            double width;
                            if (double.TryParse(command.Width, out width) && width > 0)
                            {
                                currentGrid.ColumnDefinitions.Add(new ColumnDefinition
                                {
                                    Width = new GridLength(width)
                                });
                            }
                        }
                        break;

                    #endregion

                    #region Columns

                    // columns <count> <width>
                    case "columns":
                        {
                            if (currentGrid == null)
                                break;

                            if (currentGrid == null)
                                break;

                            double width;
                            int count = 0;
                            if (double.TryParse(command.Width, out width) &&
                                int.TryParse(command.Count, out count) &&
                                width > 0 && count > 0)
                            {
                                for (int i = 0; i < count; i++)
                                {
                                    currentGrid.ColumnDefinitions.Add(new ColumnDefinition
                                    {
                                        Width = new GridLength(width)
                                    });
                                }
                            }
                        }
                        break;

                    #endregion

                    #region Text

                    // Layout: grid
                    // text <row> <column> <row-span> <column-span> <v-alignment> <h-alignment> <font-family> <font-size> <bold> <text>
                    //where:
                    // grid: row, column
                    // grid: row-span, column-span
                    // grid: v-alignment -> top, bottom, center, stretch
                    // grid: h-alignment -> left, right, center, stretch
                    // font-family, font-size
                    // bold -> true, false
                    // text
                    //
                    // example: text 0 0 1 1 center center Arial 10 false SomeText
                    //
                    // Layout: canvas
                    // text <x> <y> <v-alignment> <h-alignment> <font-family> <font-size> <bold> <text>
                    //where:
                    // canvas: x, y
                    // canvas: v-alignment -> top, bottom, center, stretch
                    // canvas: h-alignment -> left, right, center, stretch
                    // font-family, font-size
                    // bold -> true, false
                    // text
                    //
                    // example: text 30 30 bottom right Arial 10 false SomeText
                    case "text":
                        {
                            if (command.Layout == "grid" && currentGrid == null)
                                break;

                            if (command.Layout == "canvas" && currentCanvas == null)
                                break;

                            double fontSize;
                            if (double.TryParse(command.FontSize, out fontSize) == false)
                                break;

                            bool isBold;
                            if (bool.TryParse(command.IsBold, out isBold) == false)
                                break;

                            var tb = new TextBlock()
                            {
                                Text = command.Text,
                                FontFamily = new FontFamily(command.FontFamily),
                                FontWeight = isBold ? FontWeights.Bold : FontWeights.Normal,
                                FontSize = fontSize,
                                Foreground = Brushes.Red
                            };

                            RenderOptions.SetEdgeMode(tb, EdgeMode.Aliased);
                            tb.SetValue(UIElement.SnapsToDevicePixelsProperty, false);

                            switch (command.VerticalAlignment as string)
                            {
                                case "top":
                                    tb.SetValue(FrameworkElement.VerticalAlignmentProperty, VerticalAlignment.Top);
                                    break;
                                case "bottom":
                                    tb.SetValue(FrameworkElement.VerticalAlignmentProperty, VerticalAlignment.Bottom);
                                    break;
                                case "center":
                                    tb.SetValue(FrameworkElement.VerticalAlignmentProperty, VerticalAlignment.Center);
                                    break;
                                case "stretch":
                                    tb.SetValue(FrameworkElement.VerticalAlignmentProperty, VerticalAlignment.Stretch);
                                    break;
                            }

                            switch (command.HorizontalAlignment as string)
                            {
                                case "left":
                                    tb.SetValue(FrameworkElement.HorizontalAlignmentProperty, HorizontalAlignment.Left);
                                    break;
                                case "right":
                                    tb.SetValue(FrameworkElement.HorizontalAlignmentProperty, HorizontalAlignment.Right);
                                    break;
                                case "center":
                                    tb.SetValue(FrameworkElement.HorizontalAlignmentProperty, HorizontalAlignment.Center);
                                    break;
                                case "stretch":
                                    tb.SetValue(FrameworkElement.HorizontalAlignmentProperty, HorizontalAlignment.Stretch);
                                    break;
                            }

                            switch (command.Layout as string)
                            {
                                case "grid":
                                    {
                                        if (currentGrid == null)
                                            break;

                                        int r = 0, c = 0, rs = 1, cs = 1;
                                        if (int.TryParse(command.Row, out r) == false ||
                                            int.TryParse(command.Column, out c) == false ||
                                            int.TryParse(command.RowSpan, out rs) == false ||
                                            int.TryParse(command.ColumnSpan, out cs) == false)
                                            break;

                                        Grid.SetColumn(tb, c);
                                        Grid.SetRow(tb, r);
                                        Grid.SetColumnSpan(tb, cs);
                                        Grid.SetRowSpan(tb, rs);

                                        currentGrid.Children.Add(tb);
                                    }
                                    break;
                                case "canvas":
                                    {
                                        if (currentCanvas == null)
                                            break;

                                        double x = 0, y = 0;
                                        if (double.TryParse(command.X, out x) == false ||
                                            double.TryParse(command.Y, out y) == false)
                                            break;

                                        Canvas.SetLeft(tb, x);
                                        Canvas.SetTop(tb, y);

                                        currentCanvas.Children.Add(tb);

                                    }
                                    break;
                            }
                        }
                        break;

                    #endregion
                }
            }
        }
    }

    #endregion
}
