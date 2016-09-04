// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Logic.Views
{
    #region References

    using Logic.Utilities;
    using System;
    using System.Collections.Generic;
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

    #region ColorEditor

    public static class ColorEditor
    {
        #region Fields

        public static string[] ResourceColors = 
        {
            "BackgroundColorKey",
            "GridColorKey",
            "PageColorKey",
            "LogicColorKey",
            "LogicSelectedColorKey",
            "HelperColorKey",
            "LogicMouseOverColorKey",
            "LogicTrueStateColorKey",
            "LogicFalseStateColorKey",
            "LogicNullStateColorKey"
        };

        #endregion

        #region Methods


        public static Dictionary<string, string> GetColors()
        {
            var resources = App.Current.Resources;
            var dict = new Dictionary<string, string>();

            foreach (var c in ResourceColors)
            {
                dict.Add(c, (resources[c] as SolidColorBrush).Color.ToString());
            }

            return dict;
        }

        public static void SetColors(Dictionary<string, string> dict)
        {
            var resources = App.Current.Resources;

            foreach (var c in dict)
            {
                Color color = (Color)ColorConverter.ConvertFromString(c.Value);

                resources[c.Key] = new SolidColorBrush(color);
            }
        }

        public static void Load(string fileName)
        {
            var dict = Serializer.OpenJsonNoReference<Dictionary<string, string>>(fileName);

            SetColors(dict);
        }

        public static void Save(string fileName)
        {
            var dict = GetColors();
            Serializer.SaveJsonNoReference<Dictionary<string, string>>(dict, fileName);
        }

        #endregion
    }

    #endregion
}
