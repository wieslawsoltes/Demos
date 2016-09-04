// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.Win32;
using Sheet.Editor;
using System;

namespace Sheet.WPF
{
    public class WpfSaveFileDialog : ISaveFileDialog
    {
        private readonly SaveFileDialog dlg;

        public WpfSaveFileDialog()
        {
            dlg = new SaveFileDialog();
        }

        public string Filter { get; set; }
        public string FileName { get; set; }
        public string[] FileNames { get; set; }
        public int FilterIndex { get; set; }

        public bool ShowDialog()
        {
            FileNames = null;

            dlg.Filter = Filter;
            dlg.FilterIndex = FilterIndex;
            dlg.FileName = FileName;

            var result = dlg.ShowDialog();
            if (result.HasValue && result.Value == true)
            {
                FileName = dlg.FileName;
                FileNames = dlg.FileNames;
                FilterIndex = dlg.FilterIndex;
                return true;
            }
            return false;
        }
    }
}
