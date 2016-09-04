// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Logic.Views
{
    #region References

    using Logic.Model;
    using Logic.ViewModels;
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

    #region SolutionView

    public partial class SolutionView : UserControl
    {
        #region Properties

        private MainViewModel vm = null;

        #endregion

        #region Constructor

        public SolutionView()
        {
            InitializeComponent();

            this.Loaded += SolutionView_Loaded;
        }

        void SolutionView_Loaded(object sender, RoutedEventArgs e)
        {
           Initialize();
        }

        private void Initialize()
        {
            if (vm == null)
            {
                vm = this.DataContext as MainViewModel;

                vm.ShowHelp = () => Help();
                vm.ShowAbout = () => About();
                vm.ShowCharts = () => ShowChart();
                vm.Exit = () => Exit();
            }
        }

        #endregion

        #region Methods

        private void About()
        {
            MessageBox.Show("Logic\nCopyright © Wieslaw Soltes 2012-2013\nAll rights reserved.",
                "About Logic",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void Help()
        {
            MessageBox.Show("Tips for using application with mouse and keyboard:\n" +
                "[ Pan & Zoom View ]\n" +
                " - Press 'Esc' key to Zoom To Fit\n" +
                " - Press Shift + 'Esc' key to Reset Zoom\n" +
                " - Mouse Wheel to zoom in & out\n" +
                " - Mouse Right Button Click on canvas and hold to pan\n" +
                " - Mouse Middle Button Click anywhere and hold to pan\n" +
                "[ Create Elements ]\n" +
                " - Drag & Drop elements to canvas from Toolbox\n" +
                " - Mouse Left Button on pins to draw lines (and canvas)\n" +
                " - [Ctrl +]  Mouse Left Button Click on canvas to create Signal\n" +
                "[ Mnipulate Elements ]\n" +
                " - Mouse Left Button on elements to drag elements\n" +
                " - [Ctrl +]  A to select all elements\n" +
                " - Mouse Left Button click and move to select elements\n" +
                " - Delete to remove selected elements\n" +
                "[ Auto Connect Elements ]\n" +
                " - [Ctrl +] Mouse Left Button click on first element\n" +
                "   and next Ctrl + Mouse Left Button click on second element\n" +
                "[ Split/Connect Lines ]\n" +
                " - [Ctrl +] Mouse Left Button Click on line to split\n" +
                " - [Ctrl +] Mouse Left  Button Click on pin to connect line\n"
                ,
                "Help",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        public void ShowChart()
        {
            if (vm != null && vm.Options != null && vm.Options.EnableCharts)
            {
                ChartView chart = new ChartView();

                chart.DataContext = vm;

                chart.Owner = Window.GetWindow(this);

                chart.Show();
            }
        }

        private void Exit()
        {
            if (vm != null && vm.Options.SimulationIsRunning)
                vm.StopSimulation();

            Application.Current.Shutdown();
        }

        #endregion
    }

    #endregion
}
