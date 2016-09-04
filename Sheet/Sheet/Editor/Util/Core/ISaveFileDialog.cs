// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Sheet.Editor
{
    public interface ISaveFileDialog
    {
        string Filter { get; set; }
        string FileName { get; set; }
        string[] FileNames { get; set; }
        int FilterIndex { get; set; }
        bool ShowDialog();
    }
}
