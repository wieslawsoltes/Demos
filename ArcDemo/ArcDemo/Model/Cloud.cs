// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;

namespace WpfApplication1
{
    public class Cloud : ObservableObject
    {
        private IList<Point> _points = new ObservableCollection<Point>();

        public IList<Point> Points
        {
            get { return _points; }
            set
            {
                if (value != _points)
                {
                    _points = value;
                    Notify("Points");
                }
            }
        }
    }
}
