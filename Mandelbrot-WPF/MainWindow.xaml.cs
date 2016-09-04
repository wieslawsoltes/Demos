// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
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

namespace Mandelbrot_WPF
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void CreateMandelbrot_Click(object sender, RoutedEventArgs e)
        {
            MandelbrotImage.Source = CreateMandelbrotBitmap(1500, 1000, 150);
        }

        private void SaveMandelbrot_Click(object sender, RoutedEventArgs e)
        {
            Save(MandelbrotImage.Source as BitmapImage);
        }

        private BitmapImage CreateMandelbrotBitmap(uint width, uint height, int iterations)
        {
            var image = new BitmapImage();

            using (var ms = new System.IO.MemoryStream())
            {
                Mandelbrot.M(ms, width, height, iterations);
                ms.Position = 0;
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = null;
                image.StreamSource = ms;
                image.EndInit();
            }

            image.Freeze();
            return image;
        }

        private void Save(BitmapImage image)
        {
            var dlg = new Microsoft.Win32.SaveFileDialog()
            {
                Filter = "BMP Files (*.bmp)|*.bmp",
                FileName = "M"
            };

            if (dlg.ShowDialog() == true)
            {
                var enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(image));

                using (var stream = dlg.OpenFile())
                {
                    enc.Save(stream);
                }
            }
        }
    } 
}
