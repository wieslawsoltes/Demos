using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace GraphDemo
{
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
            
            Loaded += (s, e) => graph.Focus();
        }
    }
}
