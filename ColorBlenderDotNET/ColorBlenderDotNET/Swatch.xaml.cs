using System;
using System.Windows.Controls;
using System.Windows.Data;

namespace ColorBlenderDotNET
{
    public class HtmlColorConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter,  System.Globalization.CultureInfo culture)
        {
            string color = "#";
            foreach (byte val in values)
                color += val.ToString("X2");
            return color;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter,  System.Globalization.CultureInfo culture)
        {
            object[] bytes = new object[3];
            string html = value as string;
            bytes[0] = byte.Parse(html.Substring(1, 2), System.Globalization.NumberStyles.HexNumber);
            bytes[1] = byte.Parse(html.Substring(3, 2), System.Globalization.NumberStyles.HexNumber);
            bytes[2] = byte.Parse(html.Substring(5, 2), System.Globalization.NumberStyles.HexNumber);
            return bytes;
        }
    }

    /// <summary>
    /// Interaction logic for Swatch.xaml
    /// </summary>
    public partial class Swatch : UserControl
    {
        public Swatch()
        {
            InitializeComponent();
        }
    }
}
