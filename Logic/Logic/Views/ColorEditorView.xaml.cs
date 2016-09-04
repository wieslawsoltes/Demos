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

    #region ColorEditorView

    public partial class ColorEditorView : UserControl
    {
        #region Constructor

        public ColorEditorView()
        {
            InitializeComponent();
        } 

        #endregion

        #region Methods

        private static void Load()
        {
            var dlg = new Microsoft.Win32.OpenFileDialog()
            {
                DefaultExt = "json",
                Filter = "JSON Files (*.json)|*.json|All Files (*.*)|*.*",
                FilterIndex = 1,
                FileName = ""
            };

            if (dlg.ShowDialog() == true)
            {
                ColorEditor.Load(dlg.FileName);
            }
        }

        private static void Save()
        {
            var dlg = new Microsoft.Win32.SaveFileDialog()
            {
                DefaultExt = "json",
                Filter = "JSON Files (*.json)|*.json|All Files (*.*)|*.*",
                FilterIndex = 1,
                FileName = "colors"
            };

            if (dlg.ShowDialog() == true)
            {
                ColorEditor.Save(dlg.FileName);
            }
        }

        public static void DefaultColors()
        {
            var resources = App.Current.Resources;

            resources["BackgroundColorKey"] = new SolidColorBrush(Colors.Black);
            resources["GridColorKey"] = new SolidColorBrush(Color.FromRgb(0x21, 0x21, 0x21));
            resources["PageColorKey"] = new SolidColorBrush(Color.FromRgb(0x55, 0x55, 0x55));
            resources["LogicColorKey"] = new SolidColorBrush(Color.FromRgb(0x56, 0xCA, 0xD6));
            resources["LogicSelectedColorKey"] = new SolidColorBrush(Colors.Violet);
            resources["HelperColorKey"] = new SolidColorBrush(Color.FromRgb(0x46, 0x46, 0x46));
            resources["LogicMouseOverColorKey"] = new SolidColorBrush(Colors.Yellow);
            resources["LogicTrueStateColorKey"] = new SolidColorBrush(Color.FromRgb(0xFF, 0x00, 0x00));
            resources["LogicFalseStateColorKey"] = new SolidColorBrush(Color.FromRgb(0x43, 0xE8, 0x4E));
            resources["LogicNullStateColorKey"] = new SolidColorBrush(Colors.Yellow);
        }

        public static void PrintColors()
        {
            var resources = App.Current.Resources;

            resources["BackgroundColorKey"] = new SolidColorBrush( Colors.White);
            resources["GridColorKey"] = new SolidColorBrush(Colors.Transparent);
            resources["PageColorKey"] = new SolidColorBrush(Colors.Black);
            resources["LogicColorKey"] = new SolidColorBrush(Colors.Black);
            resources["LogicSelectedColorKey"] = new SolidColorBrush(Colors.Black);
            resources["HelperColorKey"] = new SolidColorBrush(Colors.Transparent);
            resources["LogicMouseOverColorKey"] = new SolidColorBrush(Colors.Transparent);
            resources["LogicTrueStateColorKey"] = new SolidColorBrush(Colors.Black);
            resources["LogicFalseStateColorKey"] = new SolidColorBrush(Colors.Black);
            resources["LogicNullStateColorKey"] = new SolidColorBrush(Colors.Black);
        }

        public void UpdateColors()
        {
            var resources = App.Current.Resources;

            resources["BackgroundColorKey"] = new SolidColorBrush((BackgroundColorRect.Fill as SolidColorBrush).Color);
            resources["GridColorKey"] = new SolidColorBrush((GridColorKeyRect.Fill as SolidColorBrush).Color);
            resources["PageColorKey"] = new SolidColorBrush((PageColorKeyRect.Fill as SolidColorBrush).Color);
            resources["LogicColorKey"] = new SolidColorBrush((LogicColorKeyRect.Fill as SolidColorBrush).Color);
            resources["LogicSelectedColorKey"] = new SolidColorBrush((LogicSelectedColorKeyRect.Fill as SolidColorBrush).Color);
            resources["HelperColorKey"] = new SolidColorBrush((HelperColorKeyRect.Fill as SolidColorBrush).Color);
            resources["LogicMouseOverColorKey"] = new SolidColorBrush((LogicMouseOverColorKeyRect.Fill as SolidColorBrush).Color);
            resources["LogicTrueStateColorKey"] = new SolidColorBrush((LogicTrueStateColorKeyRect.Fill as SolidColorBrush).Color);
            resources["LogicFalseStateColorKey"] = new SolidColorBrush((LogicFalseStateColorKeyRect.Fill as SolidColorBrush).Color);
            resources["LogicNullStateColorKey"] = new SolidColorBrush((LogicNullStateColorKeyRect.Fill as SolidColorBrush).Color);
        }

        #endregion

        #region Events

        private void ButtonDefaults_Click(object sender, RoutedEventArgs e)
        {
            DefaultColors();
        }

        private void ButtonPrint_Click(object sender, RoutedEventArgs e)
        {
            PrintColors();
        }

        private void ButtonUpdate_Click(object sender, RoutedEventArgs e)
        {
            UpdateColors();
        }

        private void ButtonLoad_Click(object sender, RoutedEventArgs e)
        {
            Load();
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            Save();
        }

        #endregion
    } 

    #endregion
}
