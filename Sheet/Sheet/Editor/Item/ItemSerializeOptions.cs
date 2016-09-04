// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Sheet.Editor
{
    public class ItemSerializeOptions
    {
        public string LineSeparator { get; set; }
        public string ModelSeparator { get; set; }
        public char[] LineSeparators { get; set; }
        public char[] ModelSeparators { get; set; }
        public char[] WhiteSpace { get; set; }
        public string IndentWhiteSpace { get; set; }

        public static ItemSerializeOptions Default
        {
            get
            {
                return new ItemSerializeOptions()
                {
                    LineSeparator = "\r\n",
                    ModelSeparator = ";",
                    LineSeparators = new char[] { '\r', '\n' },
                    ModelSeparators = new char[] { ';' },
                    WhiteSpace = new char[] { ' ', '\t' },
                    IndentWhiteSpace = "    "
                };
            }
        }
    }
}
