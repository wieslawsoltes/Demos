// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace ListBoxReorderDemo
{
    public partial class Window1 : Window
    {
        private IList<Item> _items = new ObservableCollection<Item>();

        public Window1()
        {
            InitializeComponent();
            
            _items.Add(new Item("1"));
            _items.Add(new Item("2"));
            _items.Add(new Item("3"));
            _items.Add(new Item("4"));
            _items.Add(new Item("5"));
            _items.Add(new Item("6"));

            listBox.DataContext = _items;
        }
    }
}
