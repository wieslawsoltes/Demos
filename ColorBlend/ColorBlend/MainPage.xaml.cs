//
// ColorBlend (C) 2012 Wiesław Šoltés. All rights reserved.
//
namespace ColorBlend
{
    #region References

    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Xml.Serialization;
    using Windows.Foundation;
    using Windows.Foundation.Collections;
    using Windows.Storage;
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

    #region RGBtoSolidColorBrushConverter

    public class RGBtoSolidColorBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var color = ColorConversion.RGBtoColor(value as RGB);
            if (color != null)
                return new SolidColorBrush(color);
            else
                return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            var brush = value as SolidColorBrush;
            if (brush != null)
                return new RGB(brush.Color.R, brush.Color.G, brush.Color.B);
            else
                return null;
        }
    }

    #endregion

    #region HSVtoSolidColorBrushConverter

    public class HSVtoSolidColorBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var color = ColorConversion.HSVtoColor(value as HSV);
            if (color != null)
                return new SolidColorBrush(color);
            else
                return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            var brush = value as SolidColorBrush;
            if (brush != null)
                return new HSV(new RGB(brush.Color.R, brush.Color.G, brush.Color.B));
            else
                return null;
        }
    }

    #endregion

    #region Main Page

    public sealed partial class MainPage : Page, INotifyPropertyChanged
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

        private const string palettesFileName = "palettes.xml";

        private RGB rgb = new RGB();
        private HSV hsv = new HSV();
        private bool updatingSliders = false;

        private RGB[] vRGB = new RGB[7];
        public RGB[] VRGB
        {
            get { return vRGB; }
            set
            {
                if (value != vRGB)
                {
                    vRGB = value;
                    Notify("VRGB");
                }
            }
        }

        private RGB[] vHSV = new RGB[9];
        public RGB[] VHSV
        {
            get { return vHSV; }
            set
            {
                if (value != vHSV)
                {
                    vHSV = value;
                    Notify("VHSV");
                }
            }
        }

        public HSV[] ZHSV
        {
            get { return palette.hsv; }
            set
            {
                if (value != palette.hsv)
                {
                    palette.hsv = value;
                    Notify("ZHSV");
                }
            }
        }

        private double colorR = 0.0;
        public double ColorR
        {
            get { return colorR; }
            set
            {
                if (value != colorR)
                {
                    colorR = value;
                    Notify("ColorR");

                    if (updatingSliders == false)
                        HandleSliderValueChangedRGB();
                }
            }
        }

        private double colorG = 0.0;
        public double ColorG
        {
            get { return colorG; }
            set
            {
                if (value != colorG)
                {
                    colorG = value;
                    Notify("ColorG");

                    if (updatingSliders == false)
                        HandleSliderValueChangedRGB();
                }
            }
        }

        private double colorB = 0.0;
        public double ColorB
        {
            get { return colorB; }
            set
            {
                if (value != colorB)
                {
                    colorB = value;
                    Notify("ColorB");

                    if (updatingSliders == false)
                        HandleSliderValueChangedRGB();
                }
            }
        }

        private double colorH = 0.0;
        public double ColorH
        {
            get { return colorH; }
            set
            {
                if (value != colorH)
                {
                    colorH = value;
                    Notify("ColorH");

                    if (updatingSliders == false)
                        HandleSliderValueChangedHSV();
                }
            }
        }

        private double colorS = 0.0;
        public double ColorS
        {
            get { return colorS; }
            set
            {
                if (value != colorS)
                {
                    colorS = value;
                    Notify("ColorS");

                    if (updatingSliders == false)
                        HandleSliderValueChangedHSV();
                }
            }
        }

        private double colorV = 0.0;
        public double ColorV
        {
            get { return colorV; }
            set
            {
                if (value != colorV)
                {
                    colorV = value;
                    Notify("ColorV");

                    if (updatingSliders == false)
                        HandleSliderValueChangedHSV();
                }
            }
        }

        private string algorithm = "";
        public string Algorithm 
        {
            get { return algorithm; }
            set
            {
                if (value != null)
                {
                    algorithm = value;
                    Notify("Algorithm");

                    palette = ColorMatch.DoColorMatchAlgorithm(hsv, algorithm);
                    UpdateSwatches();
                    UpdateVariations();
                }
            }
        }

        private ObservableCollection<string> algorrithms = new ObservableCollection<string>();
        public ObservableCollection<string> Algorithms
        {
            get { return algorrithms; }
            set
            {
                if (value != algorrithms)
                {
                    algorrithms = value;
                    Notify("Algorithms");
                }
            }
        }

        private int paletteIndex = 0;
        public int PaletteIndex
        {
            get { return paletteIndex; }
            set
            {
                if (value != paletteIndex)
                {
                    paletteIndex = value;
                    Notify("PaletteIndex");
                }
            }
        }

        private MatchColors palette = new MatchColors();
        public MatchColors Palette
        {
            get { return palette; }
            set
            {
                if (value != palette)
                {
                    palette = value;
                    Notify("Palette");

                    this.UpdateSwatches();
                    this.UpdateVariations();
                }
            }
        }

        private ObservableCollection<MatchColors> palettes = new ObservableCollection<MatchColors>();
        public ObservableCollection<MatchColors> Palettes
        {
            get { return palettes; }
            set
            {
                if (value != palettes)
                {
                    palettes = value;
                    Notify("Palettes");
                }
            }
        }

        #endregion

        #region Palettes

        public async Task<bool> SerializePalettes(string fileName)
        {
            StorageFolder storageFolder = ApplicationData.Current.RoamingFolder;
            XmlSerializer serializer = new XmlSerializer(typeof(ObservableCollection<MatchColors>));
            try
            {
                StorageFile sampleFile = await storageFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
                var file = await sampleFile.OpenAsync(FileAccessMode.ReadWrite);

                return await Task.Run(() =>
                {
                    try
                    {
                        Stream outStream = file.AsStreamForWrite();
                        serializer.Serialize(outStream, Palettes);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex.Message);

                        return false;
                    }

                    return true;
                });
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);

                return false;
            }
        }

        public async Task<bool> DeserializePalettes(string fileName)
        {
            StorageFolder storageFolder = ApplicationData.Current.RoamingFolder;
            XmlSerializer serializer = new XmlSerializer(typeof(ObservableCollection<MatchColors>));
            try
            {
                StorageFile sampleFile = await storageFolder.CreateFileAsync(fileName, CreationCollisionOption.OpenIfExists);
                var file = await sampleFile.OpenAsync(FileAccessMode.Read);

                return await Task.Run(() =>
                {
                    try
                    {
                        Stream inStream = file.AsStreamForRead();
                        Palettes = (ObservableCollection<MatchColors>) serializer.Deserialize(inStream);

                        if (Palettes != null)
                            Palette = Palettes.First();
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex.Message);

                        return false;
                    }

                    return true;
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);

                return false;
            }
        }

        #endregion

        #region Contructor

        public MainPage()
        {
            this.InitializeComponent();

            // fill algoritms list
            algorrithms.Add("classic");
            algorrithms.Add("colorexplorer");
            algorrithms.Add("singlehue");
            algorrithms.Add("complementary");
            algorrithms.Add("splitcomplementary");
            algorrithms.Add("analogue");
            algorrithms.Add("triadic");
            algorrithms.Add("square");

            // set default algorithm
            algorithm = algorrithms.First();

            // initialize default colors
            hsv = new HSV(213, 46, 49);
            rgb = new RGB(hsv);

            Palette = ColorMatch.DoColorMatchAlgorithm(hsv, algorithm);
            Palettes.Add(Palette);
            PaletteIndex = Palettes.Count - 1;

            this.colorR = rgb.r;
            this.colorG = rgb.g;
            this.colorB = rgb.b;

            this.colorH = hsv.h;
            this.colorS = hsv.s;
            this.colorV = hsv.v;

            // update swatches
            this.UpdateSwatches();

            // update variations
            this.UpdateVariations();

            // add new event handlers
            AddEventHandlers();

            this.DataContext = this;
        }

        #endregion

        #region Page Overrides

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //bool res = await DeserializePalettes(palettesFileName);
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            //bool res = await SerializePalettes(palettesFileName);
        }

        #endregion

        #region Swatch Events

        void swatch_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Clicked")
            {
                var swatch = sender as Swatch;
                HandleRectangleClick(swatch.Fill as SolidColorBrush);
            }
        }

        #endregion

        #region Rectangle Events

        private void Rectangle_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            Rectangle r = sender as Rectangle;
            HandleRectangleClick(r.Fill as SolidColorBrush);
        }

        #endregion

        #region Palettes Events

        void paletteAdd_Click(object sender, RoutedEventArgs e)
        {
            Palettes.Add(Palette);
            Palette = Palettes.Last();
            PaletteIndex = Palettes.Count - 1;
        }

        void paletteDel_Click(object sender, RoutedEventArgs e)
        {
            if (Palettes.Count > 1)
            {
                Palettes.Remove(Palette);
                Palette = Palettes.Last();
                PaletteIndex = Palettes.Count - 1;
            }
        }

        #endregion

        #region Handlers

        void AddEventHandlers()
        {
            this.swatch1.PropertyChanged += swatch_PropertyChanged;
            this.swatch2.PropertyChanged += swatch_PropertyChanged;
            this.swatch3.PropertyChanged += swatch_PropertyChanged;
            this.swatch4.PropertyChanged += swatch_PropertyChanged;
            this.swatch5.PropertyChanged += swatch_PropertyChanged;
            this.swatch6.PropertyChanged += swatch_PropertyChanged;

            this.paletteAdd.Click += new RoutedEventHandler(paletteAdd_Click);
            this.paletteDel.Click += new RoutedEventHandler(paletteDel_Click);
        }

        void HandleRectangleClick(SolidColorBrush b)
        {
            rgb = ColorConversion.ColorToRGB(b.Color);
            hsv = rgb.ToHSV();

            updatingSliders = true;
            this.UpdateSliderRGB();
            this.UpdateSliderHSV();
            updatingSliders = false;

            palette = ColorMatch.DoColorMatchAlgorithm(hsv, Algorithm);
            UpdateSwatches();
            UpdateVariations();
        }

        void UpdateSwatches()
        {
            Notify("ZHSV");
        }

        void UpdateSliderRGB()
        {
            this.ColorR = rgb.r;
            this.ColorG = rgb.g;
            this.ColorB = rgb.b;
        }

        void UpdateSliderHSV()
        {
            this.ColorH = hsv.h;
            this.ColorS = hsv.s;
            this.ColorV = hsv.v;
        }

        double addlimit(double x, double d, double min, double max)
        {
            x = x + d;
            if (x < min) return min;
            if (x > max) return max;
            if ((x >= min) && (x <= max)) return x;

            return double.NaN;
        }

        RGB hsvvariation(HSV hsv, double addsat, double addval)
        {
            RGB rgbobj = new RGB();
            HSV hsvobj = new HSV();

            hsvobj.h = hsv.h;
            hsvobj.s = hsv.s;
            hsvobj.v = hsv.v;

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

            Notify("VRGB");
            Notify("VHSV");
        }

        void HandleSliderValueChangedRGB()
        {
            rgb.r = this.ColorR;
            rgb.g = this.colorG;
            rgb.b = this.ColorB;

            hsv = rgb.ToHSV();
            rgb = hsv.ToRGB();

            updatingSliders = true;
            this.UpdateSliderHSV();
            updatingSliders = false;

            palette = ColorMatch.DoColorMatchAlgorithm(hsv, Algorithm);
            UpdateSwatches();
            UpdateVariations();
        }

        void HandleSliderValueChangedHSV()
        {
            hsv.h = this.ColorH;
            hsv.s = this.ColorS;
            hsv.v = this.ColorV;

            rgb = hsv.ToRGB();

            updatingSliders = true;
            this.UpdateSliderRGB();
            updatingSliders = false;

            palette = ColorMatch.DoColorMatchAlgorithm(hsv, Algorithm);
            UpdateSwatches();
            UpdateVariations();
        }

        #endregion
    }

    #endregion

    #region Color Modes

    public static class ColorConversion
    {
        public static RGB ColorToRGB(Color c)
        {
            RGB rgb = new RGB(c.R, c.G, c.B);
            return rgb;
        }

        public static HSV ColorToHSV(Color c)
        {
            return ColorToRGB(c).ToHSV();
        }

        public static Color HSVtoColor(HSV hs)
        {
            RGB rgb = hs.ToRGB();
            return RGBtoColor(rgb);
        }

        public static Color RGBtoColor(RGB rgb)
        {
            return Color.FromArgb(0xFF, (byte)Math.Round(rgb.r), (byte)Math.Round(rgb.g), (byte)Math.Round(rgb.b));
        }
    }

    public class HSV
    {
        public double h;
        public double s;
        public double v;

        public HSV() { }

        public HSV(double h, double s, double v)
        {
            this.h = h;
            this.s = s;
            this.v = v;
        }

        public HSV(HSV hs)
        {
            this.h = hs.h;
            this.s = hs.s;
            this.v = hs.v;
        }

        public HSV(RGB rg)
        {
            HSV hs = rg.ToHSV();
            this.h = hs.h;
            this.s = hs.s;
            this.v = hs.v;
        }

        public RGB ToRGB()
        {
            // Converts an HSV color object to a RGB color object
            RGB rg = new RGB();
            HSV hsx = new HSV(this.h, this.s, this.v);

            if (hsx.s == 0)
            {
                rg.r = rg.g = rg.b = Math.Round(hsx.v * 2.55); return (rg);
            }

            hsx.s = hsx.s / 100;
            hsx.v = hsx.v / 100;
            hsx.h /= 60;

            var i = Math.Floor(hsx.h);
            var f = hsx.h - i;
            var p = hsx.v * (1 - hsx.s);
            var q = hsx.v * (1 - hsx.s * f);
            var t = hsx.v * (1 - hsx.s * (1 - f));

            switch ((int)i)
            {
                case 0: rg.r = hsx.v; rg.g = t; rg.b = p; break;
                case 1: rg.r = q; rg.g = hsx.v; rg.b = p; break;
                case 2: rg.r = p; rg.g = hsx.v; rg.b = t; break;
                case 3: rg.r = p; rg.g = q; rg.b = hsx.v; break;
                case 4: rg.r = t; rg.g = p; rg.b = hsx.v; break;
                default: rg.r = hsx.v; rg.g = p; rg.b = q; break;
            }

            rg.r = Math.Round(rg.r * 255);
            rg.g = Math.Round(rg.g * 255);
            rg.b = Math.Round(rg.b * 255);

            return rg;
        }
    }

    public class RGB
    {
        public double r;
        public double g;
        public double b;

        public RGB() { }

        public RGB(double r, double g, double b)
        {
            this.r = r;
            this.g = g;
            this.b = b;
        }

        public RGB(RGB rg)
        {
            this.r = rg.r;
            this.g = rg.g;
            this.b = rg.b;
        }

        public RGB(HSV hs)
        {
            RGB rg = hs.ToRGB();
            this.r = rg.r;
            this.g = rg.g;
            this.b = rg.b;
        }

        public HSV ToHSV()
        {
            // Converts an RGB color object to a HSV color object
            HSV hs = new HSV();
            RGB rg = new RGB(this.r, this.g, this.b);

            var m = rg.r;
            if (rg.g < m) { m = rg.g; }
            if (rg.b < m) { m = rg.b; }
            var v = rg.r;
            if (rg.g > v) { v = rg.g; }
            if (rg.b > v) { v = rg.b; }
            var value = 100 * v / 255;
            var delta = v - m;
            if (v == 0.0) { hs.s = 0; } else { hs.s = 100 * delta / v; }

            if (hs.s == 0) { hs.h = 0; }
            else
            {
                if (rg.r == v) { hs.h = 60.0 * (rg.g - rg.b) / delta; }
                else if (rg.g == v) { hs.h = 120.0 + 60.0 * (rg.b - rg.r) / delta; }
                else if (rg.b == v) { hs.h = 240.0 + 60.0 * (rg.r - rg.g) / delta; }
                if (hs.h < 0.0) { hs.h = hs.h + 360.0; }
            }

            hs.h = Math.Round(hs.h);
            hs.s = Math.Round(hs.s);
            hs.v = Math.Round(value);

            return hs;
        }
    }

    #endregion

    #region Match Colors

    public class MatchColors
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

        private string name = string.Empty;
        public string Name
        {
            get { return name; }
            set
            {
                if (value != name)
                {
                    name = value;
                    Notify("Name");
                }
            }
        }

        public HSV[] _hsv = new HSV[6];
        public HSV[] hsv
        {
            get { return _hsv; }
            set
            {
                if (value != _hsv)
                {
                    _hsv = value;
                    Notify("hsv");
                }
            }
        }

        #endregion
    }

    #endregion

    #region Color Match

    public static class ColorMatch
    {
        #region Helper Methods

        static double rc(double x, double m)
        {
            if (x > m) { return m; }
            if (x < 0) { return 0; } else { return x; }
        }

        static double hueToWheel(double h)
        {
            if (h <= 120)
            {
                return (Math.Round(h * 1.5));
            }
            else
            {
                return (Math.Round(180 + (h - 120) * 0.75));
            }
        }

        static double wheelToHue(double w)
        {
            if (w <= 180)
            {
                return (Math.Round(w / 1.5));
            }
            else
            {
                return (Math.Round(120 + (w - 180) / 0.75));
            }
        }

        #endregion

        #region Algorithm Methods

        /* Color Matching Algorithm
            "classic"               ColorMatch 5K Classic
            "colorexplorer"         ColorExplorer - "Sweet Spot Offset"
            "singlehue"             Single Hue
            "complementary"         Complementary
            "splitcomplementary"    Split-Complementary
            "analogue"              Analogue
            "triadic"               Triadic
            "square"                Square
        */

        private static void AlgorithmSquare(HSV hs, MatchColors outp)
        {
            var w = hueToWheel(hs.h);
            HSV z = new HSV();

            z.h = wheelToHue((w + 90) % 360);
            z.s = hs.s;
            z.v = hs.v;
            outp.hsv[1] = new HSV(z);

            z.h = wheelToHue((w + 180) % 360);
            z.s = hs.s;
            z.v = hs.v;
            outp.hsv[2] = new HSV(z);

            z.h = wheelToHue((w + 270) % 360);
            z.s = hs.s;
            z.v = hs.v;
            outp.hsv[3] = new HSV(z);

            z.s = 0;
            outp.hsv[4] = new HSV(z);

            z.v = 100 - z.v;
            outp.hsv[5] = new HSV(z);
        }

        private static void AlgorithmTriadic(HSV hs, MatchColors outp)
        {
            var w = hueToWheel(hs.h);
            HSV z = new HSV();

            z.s = hs.s;
            z.h = hs.h;
            z.v = 100 - hs.v;
            outp.hsv[1] = new HSV(z);

            z = new HSV();
            z.h = wheelToHue((w + 120) % 360);
            z.s = hs.s;
            z.v = hs.v;
            outp.hsv[2] = new HSV(z);

            z.v = 100 - z.v;
            outp.hsv[3] = new HSV(z);

            z = new HSV();
            z.h = wheelToHue((w + 240) % 360);
            z.s = hs.s;
            z.v = hs.v;
            outp.hsv[4] = new HSV(z);

            z.v = 100 - z.v;
            outp.hsv[5] = new HSV(z);
        }

        private static void AlgorithmAnalogue(HSV hs, MatchColors outp)
        {
            var w = hueToWheel(hs.h);
            HSV z = new HSV();

            z.h = wheelToHue((w + 30) % 360);
            z.s = hs.s;
            z.v = hs.v;
            outp.hsv[1] = new HSV(z);

            z = new HSV();
            z.h = wheelToHue((w + 60) % 360);
            z.s = hs.s;
            z.v = hs.v;
            outp.hsv[2] = new HSV(z);

            z = new HSV();
            z.s = 0;
            z.h = 0;
            z.v = 100 - hs.v;
            outp.hsv[3] = new HSV(z);

            z.v = Math.Round(hs.v * 1.3) % 100;
            outp.hsv[4] = new HSV(z);

            z.v = Math.Round(hs.v / 1.3) % 100;
            outp.hsv[5] = new HSV(z);
        }

        private static void AlgorithmSplitComplementary(HSV hs, MatchColors outp)
        {
            var w = hueToWheel(hs.h);
            HSV z = new HSV();

            z.h = hs.h;
            z.s = hs.s;
            z.v = hs.v;

            z.h = wheelToHue((w + 150) % 360);
            z.s = hs.s;
            z.v = hs.v;
            outp.hsv[1] = new HSV(z);

            z.h = wheelToHue((w + 210) % 360);
            z.s = hs.s;
            z.v = hs.v;
            outp.hsv[2] = new HSV(z);

            z.s = 0;
            z.v = hs.s;
            outp.hsv[3] = new HSV(z);

            z.s = 0;
            z.v = hs.v;
            outp.hsv[4] = new HSV(z);

            z.s = 0;
            z.v = (100 - hs.v);
            outp.hsv[5] = new HSV(z);
        }

        private static void AlgorithmComplementary(HSV hs, MatchColors outp)
        {
            HSV z = new HSV();

            z.h = hs.h;
            z.s = (hs.s > 50) ? (hs.s * 0.5) : (hs.s * 2);
            z.v = (hs.v < 50) ? (Math.Min(hs.v * 1.5, 100)) : (hs.v / 1.5);
            outp.hsv[1] = new HSV(z);

            var w = hueToWheel(hs.h);
            z.h = wheelToHue((w + 180) % 360);
            z.s = hs.s;
            z.v = hs.v;
            outp.hsv[2] = new HSV(z);

            z.s = (z.s > 50) ? (z.s * 0.5) : (z.s * 2);
            z.v = (z.v < 50) ? (Math.Min(z.v * 1.5, 100)) : (z.v / 1.5);
            outp.hsv[3] = new HSV(z);

            z = new HSV();
            z.s = 0;
            z.h = 0;
            z.v = hs.v;
            outp.hsv[4] = new HSV(z);

            z.v = 100 - hs.v;
            outp.hsv[5] = new HSV(z);
        }

        private static void AlgorithmSingleHue(HSV hs, MatchColors outp)
        {
            HSV z = new HSV();

            z.h = hs.h;
            z.s = hs.s;
            z.v = hs.v + ((hs.v < 50) ? 20 : -20);
            outp.hsv[1] = new HSV(z);

            z.s = hs.s;
            z.v = hs.v + ((hs.v < 50) ? 40 : -40);
            outp.hsv[2] = new HSV(z);

            z.s = hs.s + ((hs.s < 50) ? 20 : -20);
            z.v = hs.v;
            outp.hsv[3] = new HSV(z);

            z.s = hs.s + ((hs.s < 50) ? 40 : -40);
            z.v = hs.v;
            outp.hsv[4] = new HSV(z);

            z.s = hs.s + ((hs.s < 50) ? 40 : -40);
            z.v = hs.v + ((hs.v < 50) ? 40 : -40);
            outp.hsv[5] = new HSV(z);
        }

        private static void AlgorithmColorExplorer(HSV hs, MatchColors outp)
        {
            HSV z = new HSV();

            z.h = hs.h;
            z.s = Math.Round(hs.s * 0.3);
            z.v = Math.Min(Math.Round(hs.v * 1.3), 100);
            outp.hsv[1] = new HSV(z);

            z = new HSV();
            z.h = (hs.h + 300) % 360;
            z.s = hs.s;
            z.v = hs.v;
            outp.hsv[3] = new HSV(z);

            z.s = Math.Min(Math.Round(z.s * 1.2), 100);
            z.v = Math.Min(Math.Round(z.v * 0.5), 100);
            outp.hsv[2] = new HSV(z);

            z.s = 0;
            z.v = (hs.v + 50) % 100;
            outp.hsv[4] = new HSV(z);

            z.v = (z.v + 50) % 100;
            outp.hsv[5] = new HSV(z);
        }

        private static void AlgorithmClassic(HSV hs, MatchColors outp, HSV y, HSV yx)
        {
            y.s = hs.s;
            y.h = hs.h;
            if (hs.v > 70) { y.v = hs.v - 30; } else { y.v = hs.v + 30; };
            outp.hsv[1] = new HSV(y);

            if ((hs.h >= 0) && (hs.h < 30))
            {
                yx.h = y.h = hs.h + 30; yx.s = y.s = hs.s; y.v = hs.v;
                if (hs.v > 70) { yx.v = hs.v - 30; } else { yx.v = hs.v + 30; }
            }

            if ((hs.h >= 30) && (hs.h < 60))
            {
                yx.h = y.h = hs.h + 150;
                y.s = rc(hs.s - 30, 100);
                y.v = rc(hs.v - 20, 100);
                yx.s = rc(hs.s - 50, 100);
                yx.v = rc(hs.v + 20, 100);
            }

            if ((hs.h >= 60) && (hs.h < 180))
            {
                yx.h = y.h = hs.h - 40;
                y.s = yx.s = hs.s;
                y.v = hs.v; if (hs.v > 70) { yx.v = hs.v - 30; } else { yx.v = hs.v + 30; }
            }

            if ((hs.h >= 180) && (hs.h < 220))
            {
                yx.h = hs.h - 170;
                y.h = hs.h - 160;
                yx.s = y.s = hs.s;
                y.v = hs.v;
                if (hs.v > 70) { yx.v = hs.v - 30; } else { yx.v = hs.v + 30; }

            }
            if ((hs.h >= 220) && (hs.h < 300))
            {
                yx.h = y.h = hs.h;
                yx.s = y.s = rc(hs.s - 40, 100);
                y.v = hs.v;
                if (hs.v > 70) { yx.v = hs.v - 30; } else { yx.v = hs.v + 30; }
            }
            if (hs.h >= 300)
            {
                if (hs.s > 50) { y.s = yx.s = hs.s - 40; } else { y.s = yx.s = hs.s + 40; } yx.h = y.h = (hs.h + 20) % 360;
                y.v = hs.v;
                if (hs.v > 70) { yx.v = hs.v - 30; } else { yx.v = hs.v + 30; }
            }

            outp.hsv[2] = new HSV(y);
            outp.hsv[3] = new HSV(yx);

            y.h = 0;
            y.s = 0;
            y.v = 100 - hs.v;
            outp.hsv[4] = new HSV(y);

            y.h = 0;
            y.s = 0;
            y.v = hs.v;
            outp.hsv[5] = new HSV(y);
        }

        #endregion

        #region Match Methods

        public static MatchColors DoColorMatchAlgorithm(RGB rg, string method)
        {
            HSV hs = rg.ToHSV();
            return DoColorMatchAlgorithm(hs, method);
        }

        public static MatchColors DoColorMatchAlgorithm(HSV hs, string method)
        {
            // Color matching algorithm. All work is done in HSV color space, because all
            // calculations are based on hue, saturation and value of the working color.
            // The hue spectrum is divided into sections, are the matching colors are
            // calculated differently depending on the hue of the color.
            // input: hs = a HSV style color object
            MatchColors outp = new MatchColors() { Name = "palette" };
            HSV y = new HSV();
            HSV yx = new HSV();

            outp.hsv[0] = new HSV(hs);

            switch (method)
            {
                case "classic": // colormatch classic
                    {
                        AlgorithmClassic(hs, outp, y, yx);
                    }
                    break;

                case "colorexplorer": // colorexplorer
                    {
                        AlgorithmColorExplorer(hs, outp);
                    }
                    break;

                case "singlehue": // single hue
                    {
                        AlgorithmSingleHue(hs, outp);
                    }
                    break;

                case "complementary": // complementary      
                    {
                        AlgorithmComplementary(hs, outp);
                    }
                    break;

                case "splitcomplementary": // splitcomplementary
                    {
                        AlgorithmSplitComplementary(hs, outp);
                    }
                    break;

                case "analogue": // analogue
                    {
                        AlgorithmAnalogue(hs, outp);
                    }
                    break;

                case "triadic": // triadic
                    {
                        AlgorithmTriadic(hs, outp);
                    }
                    break;

                case "square": // square
                    {
                        AlgorithmSquare(hs, outp);
                    }
                    break;
            }

            return outp;
        }

        public static MatchColors DoMatch(RGB rg)
        {
            HSV hs = rg.ToHSV();
            return DoMatch(hs);
        }

        public static MatchColors DoMatch(HSV hs)
        {
            // Color matching algorithm. All work is done in HSV color space, because all
            // calculations are based on hue, saturation and value of the working color.
            // The hue spectrum is divided into sections, are the matching colors are
            // calculated differently depending on the hue of the color.

            MatchColors z = new MatchColors() { Name = "palette" }; ;
            HSV y = new HSV();
            HSV yx = new HSV();

            z.hsv[0] = new HSV(hs);

            y.s = hs.s;
            y.h = hs.h;

            if (hs.v > 70) { y.v = hs.v - 30; } else { y.v = hs.v + 30; }

            z.hsv[1] = new HSV(y);

            if ((hs.h >= 0) && (hs.h < 30))
            {
                yx.h = y.h = hs.h + 30; yx.s = y.s = hs.s; y.v = hs.v;
                if (hs.v > 70) { yx.v = hs.v - 30; } else { yx.v = hs.v + 30; }
            }

            if ((hs.h >= 30) && (hs.h < 60))
            {
                yx.h = y.h = hs.h + 150;
                y.s = rc(hs.s - 30, 100);
                y.v = rc(hs.v - 20, 100);
                yx.s = rc(hs.s - 50, 100);
                yx.v = rc(hs.v + 20, 100);
            }

            if ((hs.h >= 60) && (hs.h < 180))
            {
                yx.h = y.h = hs.h - 40;
                y.s = yx.s = hs.s;
                y.v = hs.v; if (hs.v > 70) { yx.v = hs.v - 30; } else { yx.v = hs.v + 30; }
            }

            if ((hs.h >= 180) && (hs.h < 220))
            {
                yx.h = hs.h - 170;
                y.h = hs.h - 160;
                yx.s = y.s = hs.s;
                y.v = hs.v;
                if (hs.v > 70) { yx.v = hs.v - 30; } else { yx.v = hs.v + 30; }
            } if ((hs.h >= 220) && (hs.h < 300))
            {
                yx.h = y.h = hs.h;
                yx.s = y.s = rc(hs.s - 40, 100);
                y.v = hs.v;
                if (hs.v > 70) { yx.v = hs.v - 30; } else { yx.v = hs.v + 30; }
            }

            if (hs.h >= 300)
            {
                if (hs.s > 50) { y.s = yx.s = hs.s - 40; } else { y.s = yx.s = hs.s + 40; } yx.h = y.h = (hs.h + 20) % 360;
                y.v = hs.v;
                if (hs.v > 70) { yx.v = hs.v - 30; } else { yx.v = hs.v + 30; }
            }

            z.hsv[2] = new HSV(y);
            z.hsv[3] = new HSV(yx);

            y.h = 0;
            y.s = 0;
            y.v = 100 - hs.v;

            z.hsv[4] = new HSV(y);

            y.h = 0;
            y.s = 0;
            y.v = hs.v;

            z.hsv[5] = new HSV(y);

            return z;
        }

        #endregion
    }

    #endregion
}
