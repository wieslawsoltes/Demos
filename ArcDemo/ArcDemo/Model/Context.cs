// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace WpfApplication1
{
    public class Context : ObservableObject
    {
        private IList<Cloud> _clouds = new ObservableCollection<Cloud>();
        private Cloud _currentCloud = null;

        public IList<Cloud> Clouds
        {
            get { return _clouds; }
            set
            {
                if (value != _clouds)
                {
                    _clouds = value;
                    Notify("Clouds");
                }
            }
        }

        public Cloud CurrentCloud
        {
            get { return _currentCloud; }
            set
            {
                if (value != _currentCloud)
                {
                    _currentCloud = value;
                    Notify("CurrentCloud");
                }
            }
        }
    }
}
