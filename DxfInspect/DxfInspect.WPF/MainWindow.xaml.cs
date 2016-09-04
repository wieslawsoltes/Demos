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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Dxf;

namespace DxfInspect.WPF
{
    public partial class MainWindow : Window
    {
        private string path = null;
        private string text = null;
        private List<DxfRawTag> sections = null;

        public MainWindow()
        {
            InitializeComponent();
        }

        private string ConvertSectionsToDxf()
        {
            var sb = new StringBuilder();
            foreach (var section in sections)
            {
                sb.Append(section.Dxf);
            }
            return sb.ToString();
        }

        private async void FileOpenDxf_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog()
            {
                Filter = "Dxf Files (*.dxf)|*.dxf|All Files (*.*)|*.*"
            };

            var result = dlg.ShowDialog();
            if (result.HasValue && result.Value)
            {
                path = dlg.FileName;
                text = await Dxf.DxfInspect.Read(path);
                await Task.Run(() => sections = Dxf.DxfInspect.Parse(text));
                DataContext = null;
                DataContext = sections;
            }
        }

        private void FileSaveDxfAs_Click(object sender, RoutedEventArgs e)
        {
            if (path == null)
            {
                return;
            }

            var dlg = new Microsoft.Win32.SaveFileDialog()
            {
                FileName = System.IO.Path.GetFileNameWithoutExtension(path),
                Filter = "Dxf Files (*.dxf)|*.dxf|All Files (*.*)|*.*"
            };

            var result = dlg.ShowDialog();
            if (result.HasValue && result.Value)
            {
                Task.Run(() => Dxf.DxfInspect.Save(dlg.FileName, ConvertSectionsToDxf()));
            }
        }

        private void FileInspectAsHtml_Click(object sender, RoutedEventArgs e)
        {
            if (path == null)
            {
                return;
            }

            var dlg = new Microsoft.Win32.SaveFileDialog()
            {
                FileName = System.IO.Path.GetFileNameWithoutExtension(path),
                Filter = "Html Files (*.html)|*.html|All Files (*.*)|*.*"
            };

            var result = dlg.ShowDialog();
            if (result.HasValue && result.Value)
            {
                Task.Run(() =>
                {
                    var fileName = dlg.FileName;
                    var html = Dxf.DxfInspect.ToHtml(sections, System.IO.Path.GetFileName(path));
                    Dxf.DxfInspect.Save(fileName, html);
                    Process.Start(fileName);
                });
            }
        }

        private void FileClose_Click(object sender, RoutedEventArgs e)
        {
            path = null;
            text = null;
            sections = null;
            DataContext = null;
        }

        private void FileExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
