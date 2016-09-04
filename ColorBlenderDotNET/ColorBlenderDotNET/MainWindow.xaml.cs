using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using System.IO;

namespace ColorBlenderDotNET
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Based on sources from:
        // http://www.colorblender.com/
        // About: ColorBlender is free online tool for color matching and palette design
        // - old version of ColorBlender can be found here: http://www.colormatch5k.com/
        // - new version of ColorBlender can be found here: http://www.colorexplorer.com/colormatch.aspx

        RGB rgb;
        HSV hsv;
        MatchColors z;
        RGB[] vRGB = new RGB[7];
        RGB[] vHSV = new RGB[9];
        bool updatingSliders = false;
        string Algorithm { get { return (algorithm.SelectedItem as ComboBoxItem).Content.ToString(); } }

        List<MatchColors> Palettes = new List<MatchColors>();

        bool LoadPalettes(string fileName)
        {
            try
            {
                System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(List<MatchColors>));
                using (System.IO.TextReader outputStream = new StreamReader(fileName))
                {
                    Palettes = (List<MatchColors>)serializer.Deserialize(outputStream);
                }
            }
            catch { return false; }

            if (Palettes.Count > 0)
                return true;
            else
                return false;
        }

        bool SavePalettes(string fileName)
        {
            try
            {
                System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(List<MatchColors>));
                using (System.IO.TextWriter outputStream = new StreamWriter(fileName))
                {
                    serializer.Serialize(outputStream, Palettes);
                }
            }
            catch { return false; }

            return true;
        }

        public MainWindow()
        {
            InitializeComponent();

            //this.LoadPalettes(palettesFileName);

            // initialize window with default color
            hsv = new HSV(213, 46, 49);
            rgb = new RGB(hsv);
            z = ColorMatch.DoColorMatchAlgorithm(hsv, Algorithm);

            // update all UI controls
            this.UpdateSliderRGB();
            this.UpdateSliderHSV();
            this.UpdateSwatches();
            this.UpdateVariations();

            // add new event handlers
            AddEventHandlers();
        }

        void AddEventHandlers()
        {
            this.Closing += new System.ComponentModel.CancelEventHandler(MainWindow_Closing);

            this.sliderR.ValueChanged += new RoutedPropertyChangedEventHandler<double>(sliderRGB_ValueChanged);
            this.sliderG.ValueChanged += new RoutedPropertyChangedEventHandler<double>(sliderRGB_ValueChanged);
            this.sliderB.ValueChanged += new RoutedPropertyChangedEventHandler<double>(sliderRGB_ValueChanged);

            this.sliderH.ValueChanged += new RoutedPropertyChangedEventHandler<double>(sliderHSV_ValueChanged);
            this.sliderS.ValueChanged += new RoutedPropertyChangedEventHandler<double>(sliderHSV_ValueChanged);
            this.sliderV.ValueChanged += new RoutedPropertyChangedEventHandler<double>(sliderHSV_ValueChanged);

            rgbvar1.MouseLeftButtonDown += new MouseButtonEventHandler(rectangle_MouseLeftButtonDown);
            rgbvar2.MouseLeftButtonDown += new MouseButtonEventHandler(rectangle_MouseLeftButtonDown);
            rgbvar3.MouseLeftButtonDown += new MouseButtonEventHandler(rectangle_MouseLeftButtonDown);
            rgbvar4.MouseLeftButtonDown += new MouseButtonEventHandler(rectangle_MouseLeftButtonDown);
            rgbvar5.MouseLeftButtonDown += new MouseButtonEventHandler(rectangle_MouseLeftButtonDown);
            rgbvar6.MouseLeftButtonDown += new MouseButtonEventHandler(rectangle_MouseLeftButtonDown);
            rgbvar7.MouseLeftButtonDown += new MouseButtonEventHandler(rectangle_MouseLeftButtonDown);

            hsvvar1.MouseLeftButtonDown += new MouseButtonEventHandler(rectangle_MouseLeftButtonDown);
            hsvvar2.MouseLeftButtonDown += new MouseButtonEventHandler(rectangle_MouseLeftButtonDown);
            hsvvar3.MouseLeftButtonDown += new MouseButtonEventHandler(rectangle_MouseLeftButtonDown);
            hsvvar4.MouseLeftButtonDown += new MouseButtonEventHandler(rectangle_MouseLeftButtonDown);
            hsvvar5.MouseLeftButtonDown += new MouseButtonEventHandler(rectangle_MouseLeftButtonDown);
            hsvvar6.MouseLeftButtonDown += new MouseButtonEventHandler(rectangle_MouseLeftButtonDown);
            hsvvar7.MouseLeftButtonDown += new MouseButtonEventHandler(rectangle_MouseLeftButtonDown);
            hsvvar8.MouseLeftButtonDown += new MouseButtonEventHandler(rectangle_MouseLeftButtonDown);
            hsvvar9.MouseLeftButtonDown += new MouseButtonEventHandler(rectangle_MouseLeftButtonDown);

            this.swatch1.col.MouseLeftButtonDown += new MouseButtonEventHandler(rectangle_MouseLeftButtonDown);
            this.swatch2.col.MouseLeftButtonDown += new MouseButtonEventHandler(rectangle_MouseLeftButtonDown);
            this.swatch3.col.MouseLeftButtonDown += new MouseButtonEventHandler(rectangle_MouseLeftButtonDown);
            this.swatch4.col.MouseLeftButtonDown += new MouseButtonEventHandler(rectangle_MouseLeftButtonDown);
            this.swatch5.col.MouseLeftButtonDown += new MouseButtonEventHandler(rectangle_MouseLeftButtonDown);
            this.swatch6.col.MouseLeftButtonDown += new MouseButtonEventHandler(rectangle_MouseLeftButtonDown);

            this.algorithm.SelectionChanged += new SelectionChangedEventHandler(algorithm_SelectionChanged);

            paletteAdd.Click += new RoutedEventHandler(paletteAdd_Click);
            paletteDel.Click += new RoutedEventHandler(paletteDel_Click);
            palettes.SelectionChanged += new SelectionChangedEventHandler(palettes_SelectionChanged);
        }

        void palettes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdatePalette();
        }

        void paletteAdd_Click(object sender, RoutedEventArgs e)
        {
            AddPalette();
        }

        void paletteDel_Click(object sender, RoutedEventArgs e)
        {
            DelPalette();
        }

        void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //this.SavePalettes(palettesFileName);
        }

        void algorithm_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            z = ColorMatch.DoColorMatchAlgorithm(hsv, Algorithm);
            UpdateSwatches();
            UpdateVariations();
        }

        void sliderRGB_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (updatingSliders == false)
                HandleSliderValueChangedRGB();
        }

        void sliderHSV_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (updatingSliders == false)
                HandleSliderValueChangedHSV();
        }

        void rectangle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Rectangle r = sender as Rectangle;
            HandlevRectangleClick(r.Fill as SolidColorBrush);
        }

        void AddPalette()
        {
            Palettes.Add(z);
            ComboBoxItem item = new ComboBoxItem();
            item.Content = paletteName.Text;
            palettes.Items.Add(item);
            palettes.SelectedIndex = palettes.Items.Count - 1;
        }

        void DelPalette()
        {
            if (palettes.SelectedIndex != -1)
            {
                int index = palettes.SelectedIndex;
                palettes.Items.RemoveAt(index);
                Palettes.RemoveAt(index);
            }
        }

        void UpdatePalette()
        {
            if (palettes.SelectedIndex != -1)
            {
                z = Palettes[palettes.SelectedIndex];
                UpdateSwatches();
                UpdateVariations();
            }
        }

        void HandlevRectangleClick(SolidColorBrush b)
        {
            rgb = ColorConversion.ColorToRGB(b.Color);
            hsv = rgb.ToHSV();

            updatingSliders = true;
            this.UpdateSliderRGB();
            this.UpdateSliderHSV();
            updatingSliders = false;

            z = ColorMatch.DoColorMatchAlgorithm(hsv, Algorithm);
            UpdateSwatches();
            UpdateVariations();
        }

        void UpdateSwatches()
        {
            this.swatch1.col.Fill = new SolidColorBrush(ColorConversion.HSVtoColor(z.hsv[0]));
            this.swatch2.col.Fill = new SolidColorBrush(ColorConversion.HSVtoColor(z.hsv[1]));
            this.swatch3.col.Fill = new SolidColorBrush(ColorConversion.HSVtoColor(z.hsv[2]));
            this.swatch4.col.Fill = new SolidColorBrush(ColorConversion.HSVtoColor(z.hsv[3]));
            this.swatch5.col.Fill = new SolidColorBrush(ColorConversion.HSVtoColor(z.hsv[4]));
            this.swatch6.col.Fill = new SolidColorBrush(ColorConversion.HSVtoColor(z.hsv[5]));
        }

        void UpdateSliderRGB()
        {
            this.sliderR.Value = rgb.r;
            this.sliderG.Value = rgb.g;
            this.sliderB.Value = rgb.b;
        }

        void UpdateSliderHSV()
        {
            this.sliderH.Value = hsv.h;
            this.sliderS.Value = hsv.s;
            this.sliderV.Value = hsv.v;
        }

        double addlimit(double x, double d, double min, double max) 
        {
            x = x + d;
            if(x < min) return min;
            if(x > max) return max;
            if((x >= min) && (x <= max)) return x;

            return double.NaN;
        }

        RGB hsvvariation(HSV hsv, double addsat, double addval)
        {
            RGB rgbobj = new RGB();
            HSV hsvobj = new HSV();

            hsvobj.h=hsv.h;
            hsvobj.s=hsv.s;
            hsvobj.v=hsv.v;

            hsvobj.s = addlimit(hsvobj.s, addsat, 0, 99);
            hsvobj.v = addlimit(hsvobj.v, addval, 0, 99);

            rgbobj = hsvobj.ToRGB();

            return rgbobj;
        }

        void UpdateVariationsRGB()
        {
            double vv = 20;
            double vw = 10;

            vRGB[0] = new RGB(addlimit(rgb.r, -vw, 0, 255), addlimit(rgb.g, vv, 0, 255), addlimit(rgb.b, -vw, 0, 255));
            vRGB[1] = new RGB(addlimit(rgb.r, vw, 0, 255), addlimit(rgb.g, vw, 0, 255), addlimit(rgb.b, -vv, 0, 255));
            vRGB[2] = new RGB(addlimit(rgb.r, -vv, 0, 255), addlimit(rgb.g, vw, 0, 255), addlimit(rgb.b, vw, 0, 255));
            vRGB[3] = new RGB(rgb.r, rgb.g, rgb.b);
            vRGB[4] = new RGB(addlimit(rgb.r, vv, 0, 255), addlimit(rgb.g, -vw, 0, 255), addlimit(rgb.b, -vw, 0, 255));
            vRGB[5] = new RGB(addlimit(rgb.r, -vw, 0, 255), addlimit(rgb.g, -vw, 0, 255), addlimit(rgb.b, vv, 0, 255));
            vRGB[6] = new RGB(addlimit(rgb.r, vw, 0, 255), addlimit(rgb.g, -vv, 0, 255), addlimit(rgb.b, vw, 0, 255));
        }

        void UpdateVariationsHSV()
        {
            double vv = 10;

            vHSV[0] = hsvvariation(hsv, -vv, vv);
            vHSV[1] = hsvvariation(hsv, 0, vv);
            vHSV[2] = hsvvariation(hsv, vv, vv);
            vHSV[3] = hsvvariation(hsv, -vv, 0);
            vHSV[4] = hsv.ToRGB();
            vHSV[5] = hsvvariation(hsv, vv, 0);
            vHSV[6] = hsvvariation(hsv, -vv, -vv);
            vHSV[7] = hsvvariation(hsv, 0, -vv);
            vHSV[8] = hsvvariation(hsv, vv, -vv);
        }

        void UpdateVariations()
        {
            UpdateVariationsRGB();
            UpdateVariationsHSV();

            rgbvar1.Fill = new SolidColorBrush(ColorConversion.RGBtoColor(vRGB[0]));
            rgbvar2.Fill = new SolidColorBrush(ColorConversion.RGBtoColor(vRGB[1]));
            rgbvar3.Fill = new SolidColorBrush(ColorConversion.RGBtoColor(vRGB[2]));
            rgbvar4.Fill = new SolidColorBrush(ColorConversion.RGBtoColor(vRGB[3]));
            rgbvar5.Fill = new SolidColorBrush(ColorConversion.RGBtoColor(vRGB[4]));
            rgbvar6.Fill = new SolidColorBrush(ColorConversion.RGBtoColor(vRGB[5]));
            rgbvar7.Fill = new SolidColorBrush(ColorConversion.RGBtoColor(vRGB[6]));

            hsvvar1.Fill = new SolidColorBrush(ColorConversion.RGBtoColor(vHSV[0]));
            hsvvar2.Fill = new SolidColorBrush(ColorConversion.RGBtoColor(vHSV[1]));
            hsvvar3.Fill = new SolidColorBrush(ColorConversion.RGBtoColor(vHSV[2]));
            hsvvar4.Fill = new SolidColorBrush(ColorConversion.RGBtoColor(vHSV[3]));
            hsvvar5.Fill = new SolidColorBrush(ColorConversion.RGBtoColor(vHSV[4]));
            hsvvar6.Fill = new SolidColorBrush(ColorConversion.RGBtoColor(vHSV[5]));
            hsvvar7.Fill = new SolidColorBrush(ColorConversion.RGBtoColor(vHSV[6]));
            hsvvar8.Fill = new SolidColorBrush(ColorConversion.RGBtoColor(vHSV[7]));
            hsvvar9.Fill = new SolidColorBrush(ColorConversion.RGBtoColor(vHSV[8]));
        }

        void HandleSliderValueChangedRGB()
        {
            rgb.r = sliderR.Value;
            rgb.g = sliderG.Value;
            rgb.b = sliderB.Value;

            hsv = rgb.ToHSV();
            rgb = hsv.ToRGB();

            updatingSliders = true;
            this.UpdateSliderHSV();
            updatingSliders = false;

            z = ColorMatch.DoColorMatchAlgorithm(hsv, Algorithm);
            UpdateSwatches();
            UpdateVariations();
        }

        void HandleSliderValueChangedHSV()
        {
            hsv.h = sliderH.Value;
            hsv.s = sliderS.Value;
            hsv.v = sliderV.Value;

            rgb = hsv.ToRGB();

            updatingSliders = true;
            this.UpdateSliderRGB();
            updatingSliders = false;

            z = ColorMatch.DoColorMatchAlgorithm(hsv, Algorithm);
            UpdateSwatches();
            UpdateVariations();
        }
    }
}
