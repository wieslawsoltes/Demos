// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
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

namespace BooleanPulseChart
{
    public partial class MainWindow : Window
    {
        private System.Threading.Timer _timer;
        private bool[] _toggles;

        public MainWindow()
        {
            InitializeComponent();

            Initialize();

            Closed += (s, e) =>
            {
                if (_timer != null)
                {
                    _timer.Dispose();
                }
            };
        }

        private void Initialize()
        {
            // settings
            int period = 100; // milliseconds
            int displayedStates = 80;
            int displayedCharts = 25;
            double pulseWidth = 10;
            double pulseHeight = 12;
            double chartMargin = 4;

            // grid
            var grid = new ChartGrid(
                new Size(
                    displayedStates * pulseWidth,
                    displayedCharts * (pulseHeight + 2 * chartMargin)),
                new Size(
                    pulseWidth,
                    pulseHeight + 2 * chartMargin),
                1.0,
                Brushes.LightGray);

            var host = new ChartGridHost(grid)
            {
                Width = displayedStates * pulseWidth,
                Height = displayedCharts * (pulseHeight + 2 * chartMargin)
            };
            layout.Children.Add(host);

            // panel
            var panel = new StackPanel()
            {
                Width = displayedStates * pulseWidth + 2 * chartMargin,
                Height = displayedCharts * (pulseHeight + 2 * chartMargin)
            };
            layout.Children.Add(panel);

            // charts
            var charts = new List<BooleanPulseChartElement>();
            for (int i = 0; i < displayedCharts; i++)
            {
                var chart = new BooleanPulseChartElement()
                {
                    Width = displayedStates * pulseWidth,
                    Height = pulseHeight,
                    Margin = new Thickness(chartMargin)
                };

                chart.States = new Queue<bool>();
                chart.DiplayedStates = displayedStates;
                chart.LimitStatesBufferSize = true;
                chart.OffsetX = 0.0;
                chart.OffsetY = 0.0;
                chart.PulseWidth = pulseWidth;
                chart.PulseHeight = pulseHeight;

                charts.Add(chart);
                panel.Children.Add(chart);
            }

            // timer
            _toggles = new bool[displayedCharts];
            byte[] result = new byte[4];
            new RNGCryptoServiceProvider().GetBytes(result);
            int seed = BitConverter.ToInt32(result, 0);
            Random random = new Random(seed);
            for (int i = 0; i < _toggles.Length; i++)
            {
                _toggles[i] = random.NextDouble() >= 0.5 ? true : false;
            }

            _timer = new System.Threading.Timer((state) =>
            {
                Dispatcher.Invoke(() =>
                {
                    for (int i = 0; i < charts.Count; i++)
                    {
                        var chart = charts[i];
                        _toggles[i] = random.NextDouble() >= 0.5 ? true : false;
                        chart.Add(_toggles[i]);
                        chart.InvalidateVisual();
                    }
                });
            },
            null, 0, period);
        }
    }
}
