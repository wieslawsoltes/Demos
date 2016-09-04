
namespace PathGridGenerator
{
    #region References

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Printing;
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

    public partial class MainWindow : Window
    {
        #region Constructor

        public MainWindow()
        {
            InitializeComponent();
        }

        #endregion

        private void Print()
        {
            var dlg = new PrintDialog();

            dlg.PrintTicket = dlg.PrintQueue.DefaultPrintTicket;
            dlg.PrintTicket.PageOrientation = PageOrientation.Landscape;
            dlg.PrintTicket.OutputQuality = OutputQuality.High;
            dlg.PrintTicket.TrueTypeFontMode = TrueTypeFontMode.DownloadAsNativeTrueTypeFont;

            if (dlg.ShowDialog() == true)
            {
                dlg.PrintVisual(canvas, "canvas");
            }
        }

        private void Reset()
        {
            textGrid.Text = "";
        }

        private void Generate(int width, int height, int size)
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();

            StringBuilder sb = new StringBuilder();

            int originX = 0;
            int originY = 0;

            int startX = size;
            int startY = size;

            // horizontal lines
            for (int y = startY + originY /* originY + size */; y < height + originY; y += size)
            {
                sb.AppendFormat("M{0},{1}", originX, y);
                sb.AppendFormat("L{0},{1}", width + originX, y);
            }

            // vertical lines
            for (int x = startX + originX /* originX + size */; x < width + originX; x += size)
            {
                sb.AppendFormat("M{0},{1}", x, originY);
                sb.AppendFormat("L{0},{1}", x, height + originY);
            }

            string s = sb.ToString();

            //pathGrid.Data = Geometry.Parse(s);
            textGrid.Text = s;

            sw.Stop();
            System.Diagnostics.Debug.Print("Generate() in {0}ms", sw.Elapsed.TotalMilliseconds);
        }

        #region Events

        private void ButtonGenerate_Click(object sender, RoutedEventArgs e)
        {
            int width = int.Parse(textGridWidth.Text);
            int height = int.Parse(textGridHeight.Text);
            int size = int.Parse(textGridSize.Text);

            Reset();

            Generate(width, height, size);
        }

        private void ButtonPrint_Click(object sender, RoutedEventArgs e)
        {
            Print();
        }

        private void ButtonReset_Click(object sender, RoutedEventArgs e)
        {
            Reset();
        }

        #endregion
    }
}
