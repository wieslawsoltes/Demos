// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Logic.Views
{
    #region References

    using Logic.Model;
    using Logic.Services;
    using Logic.Utilities;
    using Logic.ViewModels;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
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
    using System.Windows.Shapes;

    #endregion

    #region MainView

    public partial class MainView : Window
    {
        #region Fields

        private MainViewModel vm = null;

        #endregion

        #region Constructor

        public MainView()
        {
            InitializeComponent();

            Initialize();
        }

        #endregion

        #region Override Events

        protected override void OnClosed(EventArgs e)
        {
            DeInitialize();

            base.OnClosed(e);
        }

        #endregion

        #region Methods

        private void Initialize()
        {
            CreateViewModels();

            LoadOptions();

            CreateModelViews();

            if (vm.Options.EnableRecent)
                LoadRecent();

            if (vm.Options.EnableColors)
                LoadColors();

            // zoom to fit current context
            vm.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == "CurrentContext")
                    Dispatcher.Invoke(() => ZoomToFit());
            };

            this.DataContext = vm;
        }

        private void DeInitialize()
        {
            if (vm.Options.EnableUndoRedo)
                ResetUndoRedo();

            if (vm.Options.EnableRecent)
                SaveRecent();

            if (vm.Options.EnableColors)
                SaveColors();

            SaveOptions();
        }

        private void LoadOptions()
        {
            try
            {
                if (System.IO.File.Exists(Defaults.OptionsFileName) == true)
                {
                    var options = Serializer.OpenJsonNoReference<Options>(Defaults.OptionsFileName);
                    if (options != null)
                    {
                        SetOptions(options);
                    }
                }
                else
                {
                    SetDefaultOptions();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print("Error: {0}", ex.Message);

                SetDefaultOptions();
            }
        }

        private void SetDefaultOptions()
        {
            var options = new Options();

            SetOptions(options);

            Defaults.SetDefaults(options);
        }

        private void SetOptions(Options options)
        {
            if (vm != null)
                vm.Options = options;

            Defaults.Options = options;
        }

        private void SaveOptions()
        {
            try
            {
                if (vm != null && vm.Options != null)
                    Serializer.SaveJsonNoReference<Options>(vm.Options, Defaults.OptionsFileName);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print("Error: {0}", ex.Message);
            }
        }

        private static void LoadColors()
        {
            try
            {
                if (System.IO.File.Exists(Defaults.ColorsFileName) == true)
                {
                    ColorEditor.Load(Defaults.ColorsFileName);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print("Error: {0}", ex.Message);
            }
        }

        private static void SaveColors()
        {
            try
            {
                ColorEditor.Save(Defaults.ColorsFileName);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print("Error: {0}", ex.Message);
            }
        }

        private void LoadRecent()
        {
            try
            {
                if (System.IO.File.Exists(Defaults.RecentFileName) == true)
                {
                    var recentSolutions = Serializer.OpenJsonNoReference<ObservableCollection<Recent>>(Defaults.RecentFileName);
                    if (recentSolutions != null)
                    {
                        // remove files that do not exist
                        var filesDoNotExist = recentSolutions.Where(x => System.IO.File.Exists(x.Path) == false).ToList();

                        foreach (var recent in filesDoNotExist)
                            recentSolutions.Remove(recent);

                        // set recent solutions
                        vm.RecentSolutions = recentSolutions;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print("Error: {0}", ex.Message);
            }
        }

        private void SaveRecent()
        {
            try
            {
                if (vm != null && vm.RecentSolutions != null)
                    Serializer.SaveJsonNoReference<ObservableCollection<Recent>>(vm.RecentSolutions, Defaults.RecentFileName);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print("Error: {0}", ex.Message);
            }
        }

        private void CreateViewModels()
        {
            vm = new MainViewModel(new FileService(), new PrintService());

            vm.Exit = () => Exit();
        }

        private void Exit()
        {
            Application.Current.Shutdown();
        }

        private void CreateModelViews()
        {
            vm.Views.Add(new DashboardModelView("Dashboard"));
            vm.Views.Add(new SolutionModelView("Solution"));
            vm.Views.Add(new ContextModelView("Context"));

            vm.CurrentView = vm.Views.FirstOrDefault(view => view is DashboardModelView);
        }

        private void ZoomToFit()
        {
            if (vm.CurrentContext != null &&
                vm.CurrentProject != null &&
                vm.Options.IsAutoFitEnabled &&
                vm.ZoomManager != null)
            {
                vm.ZoomManager.ZoomToFit();
            }
        }

        private static void ResetUndoRedo()
        {
            Utilities.UndoRedoFramework.Clear();
        }

        #endregion
    }

    #endregion
}
