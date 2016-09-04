// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TestPATH;

namespace PathDemo
{
    // Source (Path) markup syntax:
    // https://msdn.microsoft.com/en-us/library/ms752293(v=vs.110).aspx

    // SVG icons source:
    // https://github.com/google/material-design-icons/

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Initialize();
        }

        private void Initialize()
        {
            canvas.Paths = new List<XPath>();

            /*
            {
                var s = "M9.64 7.64c.23-.5.36-1.05.36-1.64 0-2.21-1.79-4-4-4S2 3.79 2 6s1.79 4 4 4c.59 0 1.14-.13 1.64-.36L10 12l-2.36 2.36C7.14 14.13 6.59 14 6 14c-2.21 0-4 1.79-4 4s1.79 4 4 4 4-1.79 4-4c0-.59-.13-1.14-.36-1.64L12 14l7 7h3v-1L9.64 7.64zM6 8c-1.1 0-2-.89-2-2s.9-2 2-2 2 .89 2 2-.9 2-2 2zm0 12c-1.1 0-2-.89-2-2s.9-2 2-2 2 .89 2 2-.9 2-2 2zm6-7.5c-.28 0-.5-.22-.5-.5s.22-.5.5-.5.5.22.5.5-.22.5-.5.5zM19 3l-6 6 2 2 7-7V3z";
                var xpg = s.ToXPathGeometry();
                Debug.Print(xpg.ToSource());
                var t = XTransform.Create(scaleX: 2.0, scaleY: 2.0, offsetX: 90.0, offsetY: 90.0);
                var path = XPath.Create("Cut", s, xpg, t, false, true);
                canvas.Paths.Add(path);
            }
            //*/

            tree.ItemsSource = canvas.Paths;

            redraw.Click += (s, e) =>
                {
                    canvas.InvalidateVisual();
                };

            clear.Click += (s, e) =>
                {
                    canvas.Paths.Clear();
                    canvas.InvalidateVisual();

                    tree.ItemsSource = null;
                    tree.ItemsSource = canvas.Paths;
                };

            canvas.PreviewMouseLeftButtonDown += (s, e) =>
                {
                    var p = e.GetPosition(canvas);
                    Insert(p.X, p.Y);
                };

            import.Click += (s, e) =>
                {
                    var dlg = new OpenFileDialog()
                    {
                        Filter = "Txt (*.txt)|*.txt|All (*.*)|*.*"
                    };

                    if (dlg.ShowDialog() == true)
                    {
                        var output = dlg.FileName;

                        try
                        {
                            var sources = MaterialDesignIconsImporter.ToSources(output);

                            /*
                            foreach (var source in sources)
                            {
                                var path = ToXPath(source, 0, 0);
                                canvas.Paths.Add(path);
                            }
                            tree.ItemsSource = null;
                            tree.ItemsSource = canvas.Paths;
                            canvas.InvalidateVisual();
                            //*/

                            list.ItemsSource = sources;
                        }
                        catch(Exception ex)
                        {
                            Debug.Print(ex.Message);
                            Debug.Print(ex.StackTrace);
                        }
                    }
                };

            list.MouseDoubleClick += (s, e) =>
                {
                    Insert(90, 90);
                };
        }

        private void Insert(double x, double y)
        {
            var item = list.SelectedItem;
            if (item != null)
            {
                try
                {
                    var source = item as Source;
                    var path = ToXPath(source, x, y);
                    canvas.Paths.Add(path);

                    tree.ItemsSource = null;
                    tree.ItemsSource = canvas.Paths;
                    canvas.InvalidateVisual();
                }
                catch (Exception ex)
                {
                    Debug.Print(ex.Message);
                    Debug.Print(ex.StackTrace);
                }
            }
        }

        private XPath ToXPath(Source source, double x, double y)
        {
            var xpg = source.Value.ToXPathGeometry();
            var t = XTransform.Create(scaleX: 1.0, scaleY: 1.0, offsetX: x, offsetY: y);
            var path = XPath.Create(source.Name, source.Value, xpg, t, false, true);
            return path;
        }
    }
}
