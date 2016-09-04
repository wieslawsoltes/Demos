// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Logic.Views
{
    #region References

    using Logic.ViewModels;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Navigation;
    using System.Windows.Shapes;

    #endregion

    #region SolutionTreeView

    public partial class SolutionTreeView : UserControl
    {
        #region Constructor

        public SolutionTreeView()
        {
            InitializeComponent();
        }

        #endregion

        #region TreeView Events

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var vm = this.DataContext as MainViewModel;

            if (vm == null)
                return;

            if (vm.Options.Sync == true)
                return;

            var value = e.NewValue;

            Task.Factory.StartNew(() =>
            {
                vm.UpdateSelection(value);
            });
        }

        private void TreeViewItem_MouseRightButtonDown(object sender, MouseEventArgs e)
        {
            TreeViewItem item = sender as TreeViewItem;
            if (item != null)
            {
                item.IsSelected = true;
                //item.Focus();
                item.BringIntoView();
                
                e.Handled = true;
            }
        }

        #endregion
    }

    #endregion
}
