
namespace Logic.WPF
{
    #region References

    using System;
    using System.Collections.Generic;
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
    using Logic.Model;

    #endregion

    #region MainWindow

    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        #region INotifyPropertyChanged Implementation

        public virtual void Notify(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Properties

        private string text;
        public string Text
        {
            get { return text; }
            set
            {
                if (value != text)
                {
                    text = value;
                    Notify("Text");
                }
            }
        }

        #endregion

        #region Commands

        public UICommand TextBoxReturnKeyCommand { get; private set; }
        private object _sync = new object();

        private void ExecuteTextBoxReturnKeyCommand(object parameter)
        {
            if (text == null)
                return;

            Task.Factory.StartNew(() =>
            {
                lock (_sync)
                {
                    string[] lines = text.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                    foreach (var line in lines)
                    {
                        TextParser.ExecuteCommand(line);
                    }

                    Dispatcher.Invoke(() =>
                    {
                        Text = string.Empty;
                    });
                }
            }); 
        }

        #endregion

        #region Constructor

        public MainWindow()
        {
            InitializeComponent();

            TextBoxReturnKeyCommand = new UICommand(ExecuteTextBoxReturnKeyCommand);

            DataContext = this;

            Initialize();

            System.IO.TextWriter writer = new TextBoxStreamWriter(consoleOut);
            Console.SetOut(writer);

            consoleIn.Focus();
        }

        private static void Initialize()
        {
            // create simulation context
            SimulationFactory.CurrentSimulationContext = new SimulationContext()
            {
                Cache = null,
                SimulationClock = new Clock()
            };

            SimulationFactory.IsConsole = false;

            // disable Console debug output
            SimulationSettings.EnableDebug = false;

            // disable Log for Run()
            SimulationSettings.EnableLog = false;
        }

        #endregion
    }

    #endregion
}
