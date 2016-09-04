
namespace SignalChart
{
    #region References

    using Logic.Chart;
    using Logic.Chart.Model;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
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
    using System.Windows.Threading; 

    #endregion

    #region MainWindow

    public partial class MainWindow : Window
    {
        #region Fields

        private ObservableCollection<ChartContext> charts;

        #endregion

        #region Constructor

        public MainWindow()
        {
            InitializeComponent();

            charts = new ObservableCollection<ChartContext>();

            for (int i = 1; i < 10; i++)
            {
                charts.Add(new ChartContext("s" + i));
            }

            this.DataContext = charts;
        }

        #endregion

        #region Events

        private void ButtonLow_Click(object sender, RoutedEventArgs e)
        {
            //var sw = System.Diagnostics.Stopwatch.StartNew();

            foreach (var c in charts)
                c.Low();

            //sw.Stop();
            //System.Diagnostics.Debug.Print("Low(): {0}ms", sw.Elapsed.TotalMilliseconds);
        }

        private void ButtonHigh_Click(object sender, RoutedEventArgs e)
        {
            //var sw = System.Diagnostics.Stopwatch.StartNew();

            foreach (var c in charts)
                c.High();

            //sw.Stop();
            //System.Diagnostics.Debug.Print("High(): {0}ms", sw.Elapsed.TotalMilliseconds);
        }

        private void ButtonUndefined_Click(object sender, RoutedEventArgs e)
        {
            //var sw = System.Diagnostics.Stopwatch.StartNew();

            foreach (var c in charts)
                c.Undefined();

            //sw.Stop();
            //System.Diagnostics.Debug.Print("High(): {0}ms", sw.Elapsed.TotalMilliseconds);
        }

        private void ButtonReset_Click(object sender, RoutedEventArgs e)
        {
            //foreach (var c in charts)
            //    c.Reset();
        }

        #endregion
    } 

    #endregion
}
