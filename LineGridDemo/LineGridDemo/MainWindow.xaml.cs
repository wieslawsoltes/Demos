
namespace LineGridDemo
{
    #region References

    using LineGrid.Model;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
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

    #endregion

    #region MainWindow

    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void Notify(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region Properties

        private LineGridContext context;

        public LineGridContext Context
        {
            get { return context; }
            set
            {
                if (value != context)
                {
                    context = value;
                    Notify("Context");
                }
            }
        }

        #endregion

        #region Constructor

        public MainWindow()
        {
            InitializeComponent();

            this.Context = new LineGridContext();

            this.DataContext = this.Context;
        }

        #endregion

    } 

    #endregion
}
