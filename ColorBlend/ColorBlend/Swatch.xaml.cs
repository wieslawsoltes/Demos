//
// ColorBlend (C) 2012 Wiesław Šoltés. All rights reserved.
//
namespace ColorBlend
{
    #region References

    using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

    #endregion

    #region SolidColorBrushToRgbStringConverter

    public class SolidColorBrushToRgbStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var brush = value as SolidColorBrush;
            if (brush != null)
            {
                string color = string.Format("{0},{1},{2}",
                    brush.Color.R.ToString(),
                    brush.Color.G.ToString(),
                    brush.Color.B.ToString());

                return color;
            }
            else
                return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            var color = value as string;

            if (color != null)
            {
                var colors = color.Split(',');

                if (colors.Count() == 3 && colors.All(x => x.Length >= 1 && x.Length <= 3))
                {
                    byte r = 0x00;
                    byte g = 0x00;
                    byte b = 0x00;

                    if (byte.TryParse(colors[0], out r) && byte.TryParse(colors[1], out g) && byte.TryParse(colors[2], out b))
                        return new SolidColorBrush(Color.FromArgb(0xFF, r, g, b));
                    else return null;
                }
                else
                    return null;
            }
            else
                return null;
        }
    }

    #endregion

    #region SolidColorBrushToHtmlStringConverter

    public class SolidColorBrushToHtmlStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var brush = value as SolidColorBrush;
            if (brush != null)
            {
                string color = string.Format("#{0}{1}{2}",
                    brush.Color.R.ToString("X2"),
                    brush.Color.G.ToString("X2"),
                    brush.Color.B.ToString("X2"));

                return color;
            }
            else
                return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            var color = value as string;

            if (color != null)
            {
                byte r = 0x00;
                byte g = 0x00;
                byte b = 0x00;

                if (color.StartsWith("#") && color.Length == 7)
                {
                    bool rOK = byte.TryParse(color.Substring(1, 2),
                        System.Globalization.NumberStyles.AllowHexSpecifier,
                        System.Globalization.NumberFormatInfo.CurrentInfo,
                        out r);

                    bool gOK = byte.TryParse(color.Substring(3, 2),
                        System.Globalization.NumberStyles.AllowHexSpecifier,
                        System.Globalization.NumberFormatInfo.CurrentInfo,
                        out g);

                    bool bOK = byte.TryParse(color.Substring(5, 2),
                        System.Globalization.NumberStyles.AllowHexSpecifier,
                        System.Globalization.NumberFormatInfo.CurrentInfo,
                        out b);

                    if (rOK && gOK && bOK)
                        return new SolidColorBrush(Color.FromArgb(0xFF, r, g, b));
                    else 
                        return null;
                }
                else return null;
            }
            else
                return null;




            //throw new NotImplementedException();
        }
    }

    #endregion

    #region Swatch

    public sealed partial class Swatch : UserControl, INotifyPropertyChanged
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

        public static readonly DependencyProperty FillProperty =
            DependencyProperty.Register("Fill", typeof(Brush), typeof(Swatch), new PropertyMetadata(new SolidColorBrush(Colors.Gray)));

        public Brush Fill
        {
            get { return (Brush)GetValue(FillProperty); }
            set { SetValue(FillProperty, value); }
        }

        public bool Clicked
        {
            set { Notify("Clicked"); }
        }

        #endregion

        #region Contructor

        public Swatch()
        {
            this.InitializeComponent();

            this.rectangle.PointerPressed += (sender, e) =>
                {
                    this.Clicked = true;
                };
        }

        #endregion
    }

    #endregion
}
