// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace BlockDesigner
{
    #region References

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Dynamic;

    #endregion

    #region BlockParser

    public class BlockParser
    {
        public static string LoadText(string fileName)
        {
            var sb = new StringBuilder();

            using (var stream = new System.IO.StreamReader(fileName, Encoding.UTF8))
            {
                string line;
                while ((line = stream.ReadLine()) != null)
                {
                    sb.AppendLine(line);
                }
            }

            return sb.ToString();
        }

        public static void SaveText(string fileName, string text)
        {
            using (var stream = new System.IO.StreamWriter(fileName, false, Encoding.UTF8))
            {
                stream.Write(text);
            }
        }

        private static char[] SplitChar = { ' ', '\t' };

        public static List<string[]> LoadLines(string fileName)
        {
            var lines = new List<string[]>();

            using (var stream = new System.IO.StreamReader(fileName))
            {
                string line;
                while ((line = stream.ReadLine()) != null)
                {
                    lines.Add(line.Split(SplitChar, StringSplitOptions.RemoveEmptyEntries));
                }
            }

            return lines;
        }

        public static IEnumerable<string[]> SplitText(string text)
        {
            var lines = new List<string[]>();

            using (System.IO.StringReader reader = new System.IO.StringReader(text))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    lines.Add(line.Split(SplitChar, StringSplitOptions.RemoveEmptyEntries));
                }
            }

            return lines;
        }

        public static IEnumerable<dynamic> ParseLines(IEnumerable<string[]> lines)
        {
            var commands = new List<dynamic>();
            
            foreach (var l in lines)
            {
                // skip empty lines
                if (l.Length <= 0)
                    continue;

                switch(l[0])
                {
                    // execute <path>
                    case "execute":
                        {
                            if (l.Length == 2)
                            {
                                dynamic command = new ExpandoObject();
                                command.Version = "1.0";
                                command.Command = l[0];
                                command.Path = l[1];
                                commands.Add(command);
                            }
                        }
                        break;
                    // block begin <name> <width> <height>
                    // block end
                    case "block":
                        {
                            if (l.Length == 5)
                            {
                                dynamic command = new ExpandoObject();
                                command.Version = "1.0";
                                command.Command = l[0];
                                command.State = l[1];
                                command.Name = l[2];
                                command.Width = l[3];
                                command.Height = l[4];
                                commands.Add(command);
                            }
                            else if (l.Length == 2)
                            {
                                dynamic command = new ExpandoObject();
                                command.Version = "1.0";
                                command.Command = l[0];
                                command.State = l[1];
                                commands.Add(command);
                            }
                        }
                        break;
                    // simulation <path>
                    case "simulation":
                        {
                            dynamic command = new ExpandoObject();
                            command.Version = "1.0";
                            command.Command = l[0];
                            command.Path = l[1];
                            commands.Add(command);
                        }
                        break;
                    // path <state>
                    // path begin
                    // path end
                    case "path":
                        {
                            if (l.Length == 2)
                            {
                                dynamic command = new ExpandoObject();
                                command.Version = "1.0";
                                command.Command = l[0];
                                command.State = l[1];
                                commands.Add(command);
                            }
                        }
                        break;
                    // line <x1,y1> <x2,y2> [<x3,y3> ... <xn,yn>] [close]
                    case "line":
                        {
                            if (l.Length >= 3)
                            {
                                dynamic command = new ExpandoObject();
                                command.Version = "1.0";
                                command.Command = l[0];
                                command.Start = l[1];
                                command.End = l[2];

                                var last = l.Last();
                                bool isClosed = string.Compare(last, "close", StringComparison.OrdinalIgnoreCase) == 0;

                                if (l.Length > 4)
                                {
                                    var pointsLength = isClosed ? l.Length - 4 : l.Length - 3;

                                    var points = new string[pointsLength];

                                    for (int i = 0; i < pointsLength; i++)
                                        points[i] = l[i + 3];

                                    command.Points = points;
                                }
                                else
                                {
                                    command.Points = new string[0];
                                }

                                command.IsClosed = isClosed;

                                commands.Add(command);
                            }
                        }
                        break;
                    // pin <name> <x> <y>
                    case "pin":
                        {
                            if (l.Length == 4)
                            {
                                dynamic command = new ExpandoObject();
                                command.Version = "1.0";
                                command.Command = l[0];
                                command.Name = l[1];
                                command.X = l[2];
                                command.Y = l[3];
                                commands.Add(command);
                            }
                        }
                        break;
                    // grid begin <x> <y> <width> <height>
                    // grud end
                    case "grid":
                        {
                            if (l.Length == 6)
                            {
                                dynamic command = new ExpandoObject();
                                command.Version = "1.0";
                                command.Command = l[0];
                                command.State = l[1];
                                command.X = l[2];
                                command.Y = l[3];
                                command.Width = l[4];
                                command.Height = l[5];
                                commands.Add(command);
                            }
                            else if (l.Length == 2)
                            {
                                dynamic command = new ExpandoObject();
                                command.Version = "1.0";
                                command.Command = l[0];
                                command.State = l[1];
                                commands.Add(command);
                            }
                        }
                        break;
                    // row <height>
                    case "row":
                        {
                            if (l.Length == 2)
                            {
                                dynamic command = new ExpandoObject();
                                command.Version = "1.0";
                                command.Command = l[0];
                                command.Height = l[1];
                                commands.Add(command);
                            }
                        }
                        break;
                    // rows <count> <height>
                    case "rows":
                        {
                            if (l.Length == 3)
                            {
                                dynamic command = new ExpandoObject();
                                command.Version = "1.0";
                                command.Command = l[0];
                                command.Count = l[1];
                                command.Height = l[2];
                                commands.Add(command);
                            }
                        }
                        break;
                    // column <width>
                    case "column":
                        {
                            if (l.Length == 2)
                            {
                                dynamic command = new ExpandoObject();
                                command.Version = "1.0";
                                command.Command = l[0];
                                command.Width = l[1];
                                commands.Add(command);
                            }
                        }
                        break;
                    // columns <count> <width>
                    case "columns":
                        {
                            if (l.Length == 3)
                            {
                                dynamic command = new ExpandoObject();
                                command.Version = "1.0";
                                command.Command = l[0];
                                command.Count = l[1];
                                command.Width = l[2];
                                commands.Add(command);
                            }
                        }
                        break;
                    // text <row> <column> <row-span> <column-span> <v-alignment> <h-alignment> <font-family> <font-size> <bold> <text>
                    case "text":
                        {
                            if (l.Length == 9)
                            {
                                dynamic command = new ExpandoObject();
                                command.Version = "1.0";
                                command.Command = l[0];
                                command.Layout = "canvas";
                                command.X = l[1];
                                command.Y = l[2];
                                command.VerticalAlignment = l[3];
                                command.HorizontalAlignment = l[4];
                                command.FontFamily = l[5];
                                command.FontSize = l[6];
                                command.IsBold = l[7];
                                command.Text = l[8];
                                commands.Add(command);
                            }
                            else if (l.Length == 11)
                            {
                                dynamic command = new ExpandoObject();
                                command.Version = "1.0";
                                command.Command = l[0];
                                command.Layout = "grid";
                                command.Row = l[1];
                                command.Column = l[2];
                                command.RowSpan = l[3];
                                command.ColumnSpan = l[4];
                                command.VerticalAlignment = l[5];
                                command.HorizontalAlignment = l[6];
                                command.FontFamily = l[7];
                                command.FontSize = l[8];
                                command.IsBold = l[9];
                                command.Text = l[10];
                                commands.Add(command);
                            }
                        }
                        break;
                };
            }

            return commands;
        }
    }

    #endregion
}
