using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace RxCanvas.WPF
{
    public partial class SelectedItemControl : UserControl
    {
        public SelectedItemControl()
        {
            InitializeComponent();
        }
    }

    public class XColorValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((ArgbColor)value).ToHtml();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((string)value).FromHtml();
        }
    }

    public class XPointValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((PointShape)value).ToText();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((string)value).FromText();
        }
    }
}
