// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Logic.Views
{
    #region References

    using Logic.Elements.Core;
    using Logic.Model;
    using Logic.ViewModels;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
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

    #endregion

    #region ToolboxView

    public partial class ToolboxView : UserControl
    {
        #region Fields

        private Point dragStartPoint;

        #endregion

        #region Contructor

        public ToolboxView()
        {
            InitializeComponent();
        }

        #endregion

        #region Drag & Drop

        private void RadioButton_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            dragStartPoint = e.GetPosition(null);
        }

        private void RadioButton_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            Point mousePos = e.GetPosition(null);
            Vector diff = dragStartPoint - mousePos;

            if (e.LeftButton == MouseButtonState.Pressed && 
                (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance || Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance))
            {
                var item = sender as RadioButton;

                if (item != null && e.OriginalSource is RadioButton && item.IsChecked == true)
                {
                    e.Handled = true;

                    var data = (item.DataContext as Lazy<IElement>).Value.Name;
                    var dragData = new DataObject(Defaults.DataFormat, data);
                    DragDrop.DoDragDrop(e.OriginalSource as DependencyObject, dragData, DragDropEffects.Move);
                }
            }
        }

        #endregion
    }

    #endregion
}
