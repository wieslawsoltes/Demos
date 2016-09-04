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

namespace rt
{
    public partial class Window1 : Window
    {
        private int nthreads = 4;

        private void WriteHeader(System.IO.Stream stream, Observer o)
        {
            uint size = (uint)(o.nx * o.ny) * 3 + 26;

            byte[] header = 
            { 
                //Bitmap file header
                66, 77,
                (byte)(size & 255), (byte)((size >> 8) & 255), (byte)((size >> 16) & 255), (byte)(size >> 24), 
                0, 0, 0, 0, 
                26, 0, 0, 0, 
                // BITMAPCOREHEADER
                12, 0, 0, 0, 
                (byte)((uint)o.nx & 255), (byte)(o.nx >> 8), 
                (byte)((uint)o.ny & 255), (byte)(o.ny >> 8), 
                1, 0, 
                24, 0 
            };

            stream.Write(header, 0, 26);
        }

        public Window1()
        {
            InitializeComponent();

            rtImage.MouseLeftButtonDown += (s, e) => Save(rtImage.Source as BitmapImage);

            rtImage.Source = GetImage();
        }

        private BitmapImage GetImage()
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();

            var image = new BitmapImage();

            using (var ms = new System.IO.MemoryStream())
            {
                var o = rt1.rtmain_mt(nthreads); //var o = rt1.rtmain();

                WriteHeader(ms, o);
                ms.Write(o.imgdata, 0, o.imgdata.Length);

                ms.Position = 0;
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = null;
                image.StreamSource = ms;
                image.EndInit();
            }

            image.Freeze();

            sw.Stop();
            Title = sw.Elapsed.TotalMilliseconds.ToString() + "ms";

            return image;
        }

        private void Save(BitmapImage image)
        {
            var dlg = new Microsoft.Win32.SaveFileDialog()
            {
                Filter = "BMP Files (*.bmp)|*.bmp",
                FileName = "rt"
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