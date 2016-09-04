// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.VisualBasic.FileIO;
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

namespace ReadCsv
{
    public static class CsvReader
    {
        public static IEnumerable<string[]> Read(string path, bool skipOverHeaderLine)
        {
            // reference Microsoft.VisualBasic, namespace Microsoft.VisualBasic.FileIO
            using (TextFieldParser parser = new TextFieldParser(path))
            {
                parser.CommentTokens = new string[] { "#" };
                parser.SetDelimiters(new string[] { ";" });
                parser.HasFieldsEnclosedInQuotes = true;

                // skip over header line
                if (skipOverHeaderLine)
                {
                    parser.ReadLine();
                }

                while (!parser.EndOfData)
                {
                    string[] fields = parser.ReadFields();
                    yield return fields;
                }
            }
        }
    }

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void buttonOpen_Click(object sender, RoutedEventArgs e)
        {
            Open();
        }

        private void Open()
        {
            var dlg = new Microsoft.Win32.OpenFileDialog()
            {
                Filter = "Csv Files (*.csv)|*.csv|All Files (*.*)|*.*"
            };

            var result = dlg.ShowDialog();
            if (result.HasValue && result.Value == true)
            {
                Open(dlg.FileName);
            }
        }

        private void Open(string fileName)
        {
            var data = CsvReader.Read(fileName, false).ToList();
            CreateColumns(data.FirstOrDefault());
            SetItemsSource(data.Skip(1));
        }

        private void SetItemsSource(IEnumerable<string[]> data)
        {
            Csv.ItemsSource = null;
            Csv.ItemsSource = data;
        }

        private void CreateColumns(string[] columns)
        {
            var gv = new GridView();
            int i = 0;
            foreach (var column in columns)
            {
                gv.Columns.Add(new GridViewColumn { Header = column, Width = double.NaN, DisplayMemberBinding = new Binding("[" + i++ + "]") });
            }
            Csv.View = gv;
        }
    }
}
