// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Sheet.Editor;
using System;
using System.Collections.Generic;
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
    public partial class TextControl : UserControl, ITextView, ITextController
    {
        #region Fields

        private Action<string> okAction = null;
        private Action cancelAction = null; 

        #endregion

        #region Constructor

        public TextControl()
        {
            InitializeComponent();

            Loaded += (sender, e) =>
            {
                TextValue.Focus();
                TextValue.SelectAll();
            };
        } 

        #endregion

        #region IDisposable

        public void Dispose()
        {
        }

        #endregion

        #region XTextController

        public void Set(Action<string> ok, Action cancel, string title, string label, string text)
        {
            okAction = ok;
            cancelAction = cancel;
            TextTitle.Text = title;
            TextLabel.Text = label;
            TextValue.Text = text;
        }

        #endregion

        #region Events

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Hidden;

            if (okAction != null)
            {
                okAction(TextValue.Text);
            }

            okAction = null;
            cancelAction = null;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Hidden;

            if (cancelAction != null)
            {
                cancelAction();
            }

            okAction = null;
            cancelAction = null;
        }

        private void ThumbDrag_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            Drag(e.HorizontalChange, e.VerticalChange);
        }

        #endregion

        #region Drag

        private void Drag(double dx, double dy)
        {
            double x = Canvas.GetLeft(this);
            double y = Canvas.GetTop(this);
            Canvas.SetLeft(this, x + dx);
            Canvas.SetTop(this, y + dy);
        }

        #endregion
    }
}
