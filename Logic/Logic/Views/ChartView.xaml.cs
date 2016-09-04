// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Logic.Views
{
    #region References

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
    using System.Windows.Shapes;

    #endregion

    #region ChartView

    public partial class ChartView : Window
    {
        public ChartView()
        {
            InitializeComponent();

            this.DataContextChanged += ChartView_DataContextChanged;
        }

        void ChartView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            UpdateGrid();
        }

        private void UpdateGrid()
        {
            var vm = this.DataContext as MainViewModel;

            if (vm != null)
            {
                if (vm.Charts != null)
                    UpdateGrid(vm);

                vm.Options.PropertyChanged += (sender, e) =>
                {
                    if (e.PropertyName == "SimulationIsRunning")
                        UpdateGrid(vm);
                };
            }
        }

        private void UpdateGrid(MainViewModel vm)
        {
            if (vm.Charts != null)
            {
                int count = vm.Charts.Count;
                int width = (int)this.Width;
                int height = count * 28;

                GenerateGrid(width, height, 20, 28, 103, 0);
            }
            else
            {
                ChartViewGrid.Data = null;
            }
        }

        private void GenerateGrid(int width, int height, int sizeX, int sizeY, int originX, int originY)
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();

            StringBuilder sb = new StringBuilder();

            // horizontal lines
            for (int y = originY + sizeY; y < height + originY; y += sizeY)
            {
                sb.AppendFormat("M{0},{1}", originX, y);
                sb.AppendFormat("L{0},{1}\n", width + originX, y);
            }

            // vertical lines
            for (int x = originX + sizeX; x < width + originX; x += sizeX)
            {
                sb.AppendFormat("M{0},{1}", x, originY);
                sb.AppendFormat("L{0},{1}\n", x, height + originY);
            }

            string s = sb.ToString();

            ChartViewGrid.Data = Geometry.Parse(s);
        }
    } 

    #endregion
}
