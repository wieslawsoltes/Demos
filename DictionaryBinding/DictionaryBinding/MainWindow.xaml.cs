// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace DictionaryBinding
{
    #region References

    using Newtonsoft.Json;
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

    #endregion

    #region MainWindow

    public partial class MainWindow : Window
    {
        #region Fields

        private Context context; 
        
        #endregion

        #region Constructor

        public MainWindow()
        {
            InitializeComponent();

            this.context = new Context();

            this.context.Properties.Add("Date", new Property("2013-05-11"));
            this.context.Properties.Add("Revision", new Property("0"));

            this.DataContext = this.context;
        } 

        #endregion

        #region Events

        private void ButtonSerialize_Click(object sender, RoutedEventArgs e)
        {
            string json = JsonConvert.SerializeObject(context.Properties, Formatting.Indented);

            JsonTextBox.Text = json;
        }

        private void ButtonDeserialize_Click(object sender, RoutedEventArgs e)
        {
            var properties = JsonConvert.DeserializeObject<Dictionary<string, Property>>(JsonTextBox.Text);

            this.context.Properties = properties;
        }

        //private void ButtonCreateProperty_Click(object sender, RoutedEventArgs e)
        //{
        //    string name = this.PropertyNameTextBox.Text;
        //    string value = this.PropertyValueTextBox.Text;

        //    try
        //    {
        //        this.context.Properties.Add(name, new Property(value));
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }
        //}

        #endregion
    } 

    #endregion

    #region NotifyObject
    
    public class NotifyObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void Notify(string propertyName)
        {
            var handle = PropertyChanged;

            if (handle != null)
            {
                handle(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    } 

    #endregion

    #region Property

    public class Property : NotifyObject
    {
        #region Constructor

        public Property()
            : base()
        {
        }

        public Property(object data)
            : this()
        {
            this.data = data;
        } 

        #endregion

        #region Properties

        private object data;

        public object Data
        {
            get { return data; }
            set
            {
                if (value != data)
                {
                    data = value;
                    Notify("Data");
                }
            }
        } 

        #endregion
    } 

    #endregion

    #region Context

    public class Context : NotifyObject
    {
        private Dictionary<string, Property> properties = new Dictionary<string, Property>();

        public Dictionary<string, Property> Properties
        {
            get { return properties; }
            set
            {
                if (value != properties)
                {
                    properties = value;
                    Notify("Properties");
                }
            }
        }
    } 

    #endregion
}
