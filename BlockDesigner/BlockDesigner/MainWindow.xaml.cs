// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace BlockDesigner
{
    #region References

    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Markup;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Navigation;
    using System.Windows.Shapes;
    using System.Xml;

    #endregion

    public partial class MainWindow : Window
    {
        #region Contructor

        public MainWindow()
        {
            InitializeComponent();
        }

        #endregion

        #region Methods

        private void LoadCode()
        {
            var dlg = new Microsoft.Win32.OpenFileDialog()
            {
                DefaultExt = "txt",
                Filter = "Block Files (*.txt;*.block)|*.txt;*.block|TXT Files (*.txt)|*.txt|Block Files (*.block)|*.block|All Files (*.*)|*.*",
                FilterIndex = 1,
                FileName = ""
            };

            if (dlg.ShowDialog() == true)
            {
                var sw = System.Diagnostics.Stopwatch.StartNew();

                var textCode = BlockParser.LoadText(dlg.FileName);

                TextCode.Text = textCode;

                sw.Stop();
                System.Diagnostics.Debug.Print("Loaded code in {0}ms", sw.Elapsed.TotalMilliseconds);
            }
        }

        private void SaveCode()
        {
            var dlg = new Microsoft.Win32.SaveFileDialog()
            {
                DefaultExt = "txt",
                Filter = "Block Files (*.txt;*.block)|*.txt;*.block|TXT Files (*.txt)|*.txt|Block Files (*.block)|*.block|All Files (*.*)|*.*",
                FilterIndex = 1,
                FileName = "script1"
            };

            if (dlg.ShowDialog() == true)
            {
                var sw = System.Diagnostics.Stopwatch.StartNew();

                string codeText = TextCode.Text;
                BlockParser.SaveText(dlg.FileName, codeText);

                sw.Stop();
                System.Diagnostics.Debug.Print("Saved code in {0}ms", sw.Elapsed.TotalMilliseconds);
            }
        }

        private void CompileCode()
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();

            var codeText = TextCode.Text;
            var lines = BlockParser.SplitText(codeText);
            var commands = BlockParser.ParseLines(lines);

            // commands compiler output
            var output = new StringBuilder();

            output.AppendLine("Count: " + commands.Count().ToString());
            foreach (var c in commands)
            {
                output.AppendLine("");
                output.AppendLine("[Command]");

                foreach (var property in (IDictionary<String, Object>)c)
                {
                    output.AppendLine(property.Key + ": " + property.Value);
                }
            }

            TextOutput.Text = output.ToString();

            // reset canvas
            CanvasDesignArea.Children.Clear();

            var blocks = BlockCompiler.Compile(commands);

            var resourceDictionary = XamlExporter.GetResourceDictionary(blocks);
            var formattedXaml = XamlExporter.FormatXml(resourceDictionary);

            TextXaml.Text = formattedXaml;

            AddBlocksToDesignArea(blocks);

            sw.Stop();
            System.Diagnostics.Debug.Print("Compiled code in {0}ms", sw.Elapsed.TotalMilliseconds);

            //#if !DEBUG
            //MessageBox.Show("Compiled code in " + sw.Elapsed.TotalMilliseconds.ToString() + "ms");
            //#endif
        }

        private void AddBlocksToDesignArea(IEnumerable<Tuple<string, object>> blocks)
        {
            double offset = 30;
            double nextBlockOffset = offset;

            var sb = new StringBuilder();
            foreach (var tuple in blocks)
            {
                nextBlockOffset += offset + AddBlockToDesignArea(tuple);
            }
        }

        private double AddBlockToDesignArea(Tuple<string, object> tuple)
        {
            // add block to designer canvas
            var ctText = XamlExporter.GetControlTemplate(tuple.Item2, tuple.Item1);
            var ct = (ControlTemplate)XamlReader.Parse(ctText);

            var thumb = new Thumb()
            {
                Template = ct
            };

            thumb.DragDelta += (sender, e) =>
            {
                var t = sender as Thumb;
                double x = Canvas.GetLeft(t) + e.HorizontalChange;
                double y = Canvas.GetTop(t) + e.VerticalChange;
                Canvas.SetLeft(t, x);
                Canvas.SetTop(t, y);
            };

            double blockWidth = (tuple.Item2 as Canvas).Width;
            double blockHeight = (tuple.Item2 as Canvas).Height;

            Canvas.SetLeft(thumb, CanvasDesignArea.ActualWidth / 2.0 - blockWidth / 2.0);
            Canvas.SetTop(thumb, CanvasDesignArea.ActualHeight / 2.0 - blockHeight / 2.0);

            CanvasDesignArea.Children.Add(thumb);

            return (tuple.Item2 as Canvas).Height;
        }

        private void ExportXaml()
        {
            var dlg = new Microsoft.Win32.SaveFileDialog()
            {
                DefaultExt = "xaml",
                Filter = "Xaml Files (*.xaml)|*.xaml;|All Files (*.*)|*.*",
                FilterIndex = 1,
                FileName = "Dictionary1"
            };

            if (dlg.ShowDialog() == true)
            {
                var sw = System.Diagnostics.Stopwatch.StartNew();

                string xamlText = TextXaml.Text;

                XamlExporter.WriteToFile(dlg.FileName, xamlText);

                sw.Stop();
                System.Diagnostics.Debug.Print("Exported xaml in {0}ms", sw.Elapsed.TotalMilliseconds);
            }
        }

        #endregion

        #region Events

        private void MenuFileOpen_Click(object sender, RoutedEventArgs e)
        {
            LoadCode();
        }

        private void MenuFileClose_Click(object sender, RoutedEventArgs e)
        {
            SaveCode();
        }

        private void MenuFileExport_Click(object sender, RoutedEventArgs e)
        {
            ExportXaml();
        }

        private void MenuScriptCompile_Click(object sender, RoutedEventArgs e)
        {
            CompileCode();
        }

        private void MenuFileExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        #endregion
    }
}
