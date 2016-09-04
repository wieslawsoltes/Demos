// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Logic.Views
{
    #region References

    using Logic.Model;
    using Logic.ViewModels;
    using Logic.ViewModels.Core;
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

    #region Context View

    public partial class ContextView : UserControl
    {
        #region Properties

        public IZoomService ZoomManager
        {
            get { return (IZoomService)GetValue(ZoomManagerProperty); }
            set { SetValue(ZoomManagerProperty, value); }
        }

        public static readonly DependencyProperty ZoomManagerProperty =
            DependencyProperty.Register("ZoomManager", typeof(IZoomService), typeof(ContextView), new PropertyMetadata(null));

        #endregion

        #region Construtor

        public ContextView()
        {
            InitializeComponent();

            this.Loaded += ContextView_Loaded;

            this.DataContextChanged += ContextView_DataContextChanged;
        }

        #endregion

        #region Events

        void ContextView_Loaded(object sender, RoutedEventArgs e)
        {
            Initialize();
        }

        void ContextView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            UpdateDataProvider();
        }

        #endregion

        #region Methods

        private void Initialize()
        {
            MainViewModel vm = this.DataContext as MainViewModel;

            if (vm != null)
            {
                // set zoom manager
                vm.ZoomManager = this.ZoomManager;

                // autofit context view
                if (vm.Options.IsAutoFitEnabled && vm.ZoomManager != null)
                    vm.ZoomManager.ZoomToFit();
            }
        }

        private void UpdateDataProvider()
        {
            ObjectDataProvider dataProvider = this.TryFindResource("OptionsDataProvider") as ObjectDataProvider;
            if (dataProvider != null)
            {
                dataProvider.ObjectInstance = Defaults.Options;
            }
        }

        #endregion
    }

    #endregion
}
