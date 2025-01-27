﻿// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Logic.Views
{
    #region References

    using Logic.Model;
    using Logic.Model.Core;
    using System;
    using System.Collections.Generic;
    using System.Linq;
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

    #region SignalView

    public partial class SignalView : UserControl
    {
        #region Constructor

        public SignalView()
        {
            InitializeComponent();
        }

        #endregion

        #region Filter

        private void ChildrenViewSource_Filter(object sender, FilterEventArgs e)
        {
            var element = e.Item as Element;
            if (element != null)
            {
                if (element is Signal)
                    e.Accepted = true;
                else
                    e.Accepted = false;
            }
        }

        #endregion
    }

    #endregion
}
