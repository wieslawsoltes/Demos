// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Sheet.Editor;
using System;
using System.Windows;

namespace Sheet.WPF
{
    public class WpfClipboard : IClipboard
    {
        public void Set(string text)
        {
            Clipboard.SetData(DataFormats.UnicodeText, text);
        }

        public string Get()
        {
            return (string)Clipboard.GetData(DataFormats.UnicodeText);
        }
    }
}
