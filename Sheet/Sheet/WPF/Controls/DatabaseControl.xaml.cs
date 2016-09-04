// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Sheet.Core;
using Sheet.Editor;
using Sheet.WPF;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Sheet
{
    public partial class DatabaseControl : UserControl, IDatabaseView
    {
        #region Fields

        private Point dragStartPoint;

        #endregion

        #region Constructor

        public DatabaseControl()
        {
            InitializeComponent();

            Loaded += (sender, e) =>
            {
                Init();
            };

            DataContextChanged += (sender, e) =>
            {
                Init();
            };
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
        }

        #endregion

        #region Init

        private void Init()
        {
            var controller = DataContext as IDatabaseController;
            if (controller != null)
            {
                SetColumns(controller.Columns);
                SetData(controller.Data);
            }
        }

        public void SetColumns(string[] columns)
        {
            Database.View = CreateColumnsView(columns);
        }

        public void SetData(IList<string[]> data)
        {
            Database.ItemsSource = null;
            Database.ItemsSource = data;
        }

        private GridView CreateColumnsView(string[] columns)
        {
            var gv = new GridView();
            int i = 0;
            foreach (var column in columns)
            {
                gv.Columns.Add(new GridViewColumn { Header = column, Width = double.NaN, DisplayMemberBinding = new Binding("[" + i + "]") });
                i++;
            }
            return gv;
        }

        #endregion

        #region Drag

        private void Csv_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            dragStartPoint = e.GetPosition(null);
        }

        private void Csv_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            Point point = e.GetPosition(null);
            Vector diff = dragStartPoint - point;
            if (e.LeftButton == MouseButtonState.Pressed &&
                (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                 Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance))
            {
                var listView = sender as ListView;
                var listViewItem = WpfVisualHelper.FindVisualParent<ListViewItem>((DependencyObject)e.OriginalSource);
                if (listViewItem != null)
                {
                    string[] row = (string[])listView.ItemContainerGenerator.ItemFromContainer(listViewItem);
                    var controller = DataContext as IDatabaseController;
                    var dataItem = new ItemData() { Columns = controller.Columns, Data = row };
                    DataObject dragData = new DataObject("Data", dataItem);
                    DragDrop.DoDragDrop(listViewItem, dragData, DragDropEffects.Move);
                }
            }
        }

        #endregion
    }
}
