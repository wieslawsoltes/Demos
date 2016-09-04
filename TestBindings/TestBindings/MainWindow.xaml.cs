// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
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
using Test2d;
using TestEDITOR;

namespace TestBindings
{
    public partial class MainWindow : Window
    {
        public LayerElement Layer { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            Initialize();




        }

        private void Initialize()
        {
            int numberOfElements = 5;
            double width = 600;
            double height = 600;
            
            Layer = new LayerElement();
            Layer.Test = new Test()
            {
                Values = ImmutableArray.Create<Value>(),
                Points = ImmutableArray.Create<XPoint>(),
                Bindings = ImmutableArray.Create<XBinding>()
            };
            canvas.Children.Add(Layer);
            
            XBinding.Create(Layer, "Elapsed", this, "Title", XBindingMode.OneWay);
            
            DataContext = this;
            
            Task.Run(
                () =>
                {
                    InitializeLayer(numberOfElements, width, height);
                    Dispatcher.Invoke(Invalidate);
                });
        }
        
        private void Invalidate()
        {
            DataContext = null;
            DataContext = this;
            Layer.InvalidateVisual();
        }

        private void LayerObserver(object sender, PropertyChangedEventArgs e)
        {
            Layer.InvalidateVisual();
        }
        
        private void InitializeLayer(int numberOfElements, double width, double height)
        {
            var rand = new Random(Guid.NewGuid().GetHashCode());
            var mode = XBindingMode.TwoWay;

            var values = Layer.Test.Values.ToBuilder();
            var points = Layer.Test.Points.ToBuilder();
            var bidnings = Layer.Test.Bindings.ToBuilder();

            for (int i = 0; i < numberOfElements; i++)
            {
                double x = rand.NextDouble() * width;
                double y = rand.NextDouble() * height;

                var p = new XPoint();
                var vx = new Value() { Content = x.ToString(CultureInfo.InvariantCulture) };
                var vy = new Value() { Content = y.ToString(CultureInfo.InvariantCulture) };

                var bx = XBinding.Create(vx, "Content", p, "X", mode);
                var by = XBinding.Create(vy, "Content", p, "Y", mode);

                vx.PropertyChanged += LayerObserver;
                vy.PropertyChanged += LayerObserver;
                p.PropertyChanged += LayerObserver;

                values.Add(vx);
                values.Add(vy);
                points.Add(p);

                bidnings.Add(bx);
                bidnings.Add(by);
            }

            Layer.Test.Values = values.ToImmutable();
            Layer.Test.Points = points.ToImmutable();
            Layer.Test.Bindings = bidnings.ToImmutable();
        }

        private void TestSerialization()
        {
            var serializer = new NewtonsoftSerializer();

            var json = serializer.ToJson(Layer.Test);
            var test = serializer.FromJson<Test>(json);

            foreach (var binding in test.Bindings)
            {
                binding.StartObserving();
            }

            Layer.Test = test;

            Invalidate();
        }
    }
}
