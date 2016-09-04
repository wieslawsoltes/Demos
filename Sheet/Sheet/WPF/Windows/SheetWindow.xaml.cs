// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Sheet.Editor;
using Sheet.IoC;
using Sheet.WPF;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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

namespace Sheet
{
    public partial class SheetWindow : Window, IMainWindow
    {
        #region IoC

        private readonly IServiceLocator _serviceLocator;
        private ISheetController _sheetController;
        private List<ISheetController> _sheetControllers;
        private List<IScopeServiceLocator> _scopeServiceLocators;

        public SheetWindow(IServiceLocator serviceLocator)
        {
            InitializeComponent();

            this._serviceLocator = serviceLocator;
            _scopeServiceLocators = new List<IScopeServiceLocator>();
            _sheetControllers = new List<ISheetController>();

            SinglePage();
            //MultiPage();

            var library = _serviceLocator.GetInstance<ILibraryView>();
            Library.Content = library;

            Init();

            Loaded += (sender, e) => _sheetController.State.View.Focus();
        }

        private void SinglePage()
        {
            var sheet = CreateSheetView();
            Sheet.Content = sheet;
            _sheetController = _sheetControllers.FirstOrDefault();
        }

        private void MultiPage()
        {
            //for (int i = 0; i < 5; i++)
            //{
            //    var sheet = CreateSheetView();
            //    var contentControl = new ContentControl();
            //    contentControl.Content = sheet;
            //    Sheets.Children.Add(contentControl);
            //}
            //_sheetController = _sheetControllers.FirstOrDefault();
        }

        private ISheetView CreateSheetView()
        {
            var locator = _serviceLocator.GetInstance<IScopeServiceLocator>();
            _scopeServiceLocators.Add(locator);

            var controller = locator.GetInstance<ISheetController>();
            _sheetControllers.Add(controller);

            controller.State.HistoryController = locator.GetInstance<IHistoryController>();
            controller.State.LibraryController = locator.GetInstance<ILibraryController>();
            controller.State.ZoomController = locator.GetInstance<IZoomController>();
            controller.State.CursorController = locator.GetInstance<ICursorController>();

            controller.State.EditorSheet = locator.GetInstance<ISheet>();
            controller.State.BackSheet = locator.GetInstance<ISheet>();
            controller.State.ContentSheet = locator.GetInstance<ISheet>();
            controller.State.OverlaySheet = locator.GetInstance<ISheet>();

            controller.State.View = locator.GetInstance<ISheetView>();

            return controller.State.View;
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            if (_scopeServiceLocators != null)
            {
                foreach (var locator in _scopeServiceLocators)
                {
                    locator.ReleaseScope();
                }
            }
        } 

        #endregion

        #region Fields

        private ObservableCollection<IDatabaseController> _databaseControllers;

        #endregion

        #region Init

        private void Init()
        {
            InitSizeBorder();
            InitDrop();
            UpdateModeMenu();
            InitDatabases();
        }

        private void InitSizeBorder()
        {
            SizeBorder.ExecuteUpdateSize = (size) => _sheetController.SetAutoFitSize(size.Width, size.Height);
            SizeBorder.ExecuteSizeChanged = () => _sheetController.State.ZoomController.AutoFit();
        }

        private void InitDrop()
        {
            AllowDrop = true;

            PreviewDrop += async (sender, e) =>
            {
                try
                {
                    if (e.Data.GetDataPresent(DataFormats.FileDrop, true))
                    {
                        string[] paths = e.Data.GetData(DataFormats.FileDrop, true) as string[];
                        await Open(paths);
                    }
                }
                catch (Exception ex)
                {
                    Debug.Print(ex.Message);
                    Debug.Print(ex.StackTrace);
                }
            };
        }

        private void InitDatabases()
        {
            _databaseControllers = new ObservableCollection<IDatabaseController>();
            Databases.Tabs.ItemsSource = _databaseControllers;

            CreateTestDatabase();
        }

        #endregion

        #region Database

        private void CreateTestDatabase()
        {
            string[] columns = { "Index", "Designation", "Description", "Signal", "Condition" };

            var data = new List<string[]>();
            for (int i = 0; i < 10; i++)
            {
                string[] item = { i.ToString(), "Designation", "Description", "Signal", "Condition" };
                data.Add(item);
            }

            var controller = CreateDatabaseController("Test", columns, data);
            _databaseControllers.Add(controller);
        }

        public async void OpenDatabase()
        {
            var dlg = _serviceLocator.GetInstance<IOpenFileDialog>();
            dlg.Filter = FileDialogSettings.DatabaseFilter;
            dlg.FilterIndex = 1;
            dlg.FileName = "";

            if (dlg.ShowDialog() == true)
            {
                try
                {
                    await OpenDatabase(dlg.FileName);
                }
                catch (Exception ex)
                {
                    Debug.Print(ex.Message);
                    Debug.Print(ex.StackTrace);
                }
            }
        }

        public async Task OpenDatabase(string fileName)
        {
            var reader = new CsvDataReader();
            var fields = await Task.Run(() => reader.Read(fileName));
            var name = System.IO.Path.GetFileName(fileName);

            var controller = CreateDatabaseController(name, fields.FirstOrDefault(), fields.Skip(1).ToList());
            _databaseControllers.Add(controller);
        }

        private CsvDatabaseController CreateDatabaseController(string name, string[] columns, List<string[]> data)
        {
            var controller = new CsvDatabaseController(name);

            controller.Columns = columns;
            controller.Data = data;

            return controller;
        }

        #endregion

        #region Open

        private async Task Open(string[] paths)
        {
            var files = paths.Where(x => (System.IO.File.GetAttributes(x) & System.IO.FileAttributes.Directory) != System.IO.FileAttributes.Directory).OrderBy(f => f);
            string path = files.FirstOrDefault();
            string ext = System.IO.Path.GetExtension(path);

            if (string.Compare(ext, FileDialogSettings.PageExtension, true) == 0)
            {
                await _sheetController.OpenTextPage(path);
            }
            else if (string.Compare(ext, FileDialogSettings.JsonPageExtension, true) == 0)
            {
                await _sheetController.OpenJsonPage(path);
            }
            else if (string.Compare(ext, FileDialogSettings.LibraryExtension, true) == 0)
            {
                await _sheetController.LoadLibrary(path);
            }
            else if (string.Compare(ext, FileDialogSettings.DatabaseExtension, true) == 0)
            {
                await OpenDatabase(path);
            }
        }

        #endregion

        #region Mode Menu

        private void UpdateModeMenu()
        {
            var mode = _sheetController.State.Mode;
            ModeNone.IsChecked = mode == SheetMode.None ? true : false;
            ModeSelection.IsChecked = mode == SheetMode.Selection ? true : false;
            ModeInsert.IsChecked = mode == SheetMode.Insert ? true : false;
            ModePoint.IsChecked = mode == SheetMode.Point ? true : false;
            ModeLine.IsChecked = mode == SheetMode.Line ? true : false;
            ModeRectangle.IsChecked = mode == SheetMode.Rectangle ? true : false;
            ModeEllipse.IsChecked = mode == SheetMode.Ellipse ? true : false;
            ModeText.IsChecked = mode == SheetMode.Text ? true : false;
            ModeImage.IsChecked = mode == SheetMode.Image ? true : false;
        }

        #endregion

        #region Key Events

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (_sheetController.State.Mode == SheetMode.TextEditor)
            {
                return;
            }

            bool none = Keyboard.Modifiers == ModifierKeys.None;
            bool onlyCtrl = Keyboard.Modifiers == ModifierKeys.Control;
            bool onlyShift = Keyboard.Modifiers == ModifierKeys.Shift;
            bool ctrlShift = Keyboard.Modifiers == (ModifierKeys.Control | ModifierKeys.Shift);

            switch (e.Key)
            {
                // Ctrl+0: Zoom to Page Level
                case Key.D0:
                case Key.NumPad0:
                    if (onlyCtrl)
                    {
                        _sheetController.State.ZoomController.AutoFit();
                    }
                    break;
                // Ctrl+1: Actual Size
                case Key.D1:
                case Key.NumPad1:
                    if (onlyCtrl)
                    {
                        _sheetController.State.ZoomController.ActualSize();
                    }
                    break;
                // N: Mode None
                // Ctrl+N: New Page
                case Key.N:
                    if (none)
                    {
                        _sheetController.State.Mode = SheetMode.None;
                        UpdateModeMenu();
                    }
                    if (onlyCtrl)
                    {
                        _sheetController.NewPage();
                    }
                    break;
                // Ctrl+O: Open Page
                case Key.O:
                    if (onlyCtrl)
                    {
                        _sheetController.OpenPage();
                    }
                    break;
                // Ctrl+S: Save Page
                // S: Mode Selection
                case Key.S:
                    if (onlyCtrl)
                    {
                        _sheetController.SavePage();
                    }
                    else if (none)
                    {
                        _sheetController.State.Mode = SheetMode.Selection;
                        UpdateModeMenu();
                    }
                    break;
                // E: Mode Ellipse
                case Key.E:
                    if (none)
                    {
                        _sheetController.State.Mode = SheetMode.Ellipse;
                        UpdateModeMenu();
                    }
                    break;
                // Ctrl+L: Library
                // L: Mode Line
                case Key.L:
                    if (onlyCtrl)
                    {
                        _sheetController.LoadLibrary();
                    }
                    else if (none)
                    {
                        _sheetController.State.Mode = SheetMode.Line;
                        UpdateModeMenu();
                    }
                    break;
                // Ctrl+D: Database
                // D: Mode Image
                case Key.D:
                    if (onlyCtrl)
                    {
                        OpenDatabase();
                    }
                    else if (none)
                    {
                        _sheetController.State.Mode = SheetMode.Image;
                        UpdateModeMenu();
                    }
                    break;
                // Ctrl+Z: Undo
                case Key.Z:
                    if (onlyCtrl)
                    {
                        _sheetController.State.HistoryController.Undo();
                    }
                    break;
                // Ctrl+Y: Redo
                case Key.Y:
                    if (onlyCtrl)
                    {
                        _sheetController.State.HistoryController.Redo();
                    }
                    break;
                // Ctrl+X: Cut
                // Ctrl+Shift+X: Cut as Json
                case Key.X:
                    if (onlyCtrl)
                    {
                        _sheetController.CutText();
                    }
                    else if (ctrlShift)
                    {
                        _sheetController.CutJson();
                    }
                    break;
                // Ctrl+C: Copy
                // Ctrl+Shift+C: Copy as Json
                case Key.C:
                    if (onlyCtrl)
                    {
                        _sheetController.CopyText();
                    }
                    else if (ctrlShift)
                    {
                        _sheetController.CopyJson();
                    }
                    break;
                // Ctrl+V: Paste
                // Ctrl+Shift+V: Paste as Json
                case Key.V:
                    if (onlyCtrl)
                    {
                        _sheetController.PasteText();
                    }
                    break;
                // Del: Delete
                // Ctrl+Del: Reset
                case Key.Delete:
                    if (onlyCtrl)
                    {
                        _sheetController.State.HistoryController.Register("Reset");
                        _sheetController.ResetPage();
                    }
                    else if (none)
                    {
                        _sheetController.Delete();
                    }
                    break;
                // Ctrl+A: Select All
                case Key.A:
                    if (onlyCtrl)
                    {
                        _sheetController.SelecteAll();
                    }
                    break;
                // B: Create Block
                // Ctrl+B: Break Block
                case Key.B:
                    if (onlyCtrl)
                    {
                        _sheetController.BreakBlock();
                    }
                    else if (none)
                    {
                        e.Handled = true;
                        _sheetController.CreateBlock();
                    }
                    break;
                // Up: Move Up
                case Key.Up:
                    if (none && _sheetController.State.View.IsFocused)
                    {
                        _sheetController.MoveUp();
                        e.Handled = true;
                    }
                    break;
                // Down: Move Down
                case Key.Down:
                    if (none && _sheetController.State.View.IsFocused)
                    {
                        _sheetController.MoveDown();
                        e.Handled = true;
                    }
                    break;
                // Left: Move Left
                case Key.Left:
                    if (none && _sheetController.State.View.IsFocused)
                    {
                        _sheetController.MoveLeft();
                        e.Handled = true;
                    }
                    break;
                // Right: Move Right
                case Key.Right:
                    if (none && _sheetController.State.View.IsFocused)
                    {
                        _sheetController.MoveRight();
                        e.Handled = true;
                    }
                    break;
                // F: Toggle Fill
                case Key.F:
                    if (none)
                    {
                        _sheetController.ToggleFill();
                    }
                    break;
                // I: Mode Insert
                case Key.I:
                    if (none)
                    {
                        _sheetController.State.Mode = SheetMode.Insert;
                        UpdateModeMenu(); 
                    }
                    break;
                // P: Mode Point
                case Key.P:
                    if (none)
                    {
                        _sheetController.State.Mode = SheetMode.Point;
                        UpdateModeMenu(); 
                    }
                    break;
                // R: Mode Rectangle
                case Key.R:
                    if (none)
                    {
                        _sheetController.State.Mode = SheetMode.Rectangle;
                        UpdateModeMenu(); 
                    }
                    break;
                // T: Mode Text
                case Key.T:
                    if (none)
                    {
                        _sheetController.State.Mode = SheetMode.Text;
                        UpdateModeMenu(); 
                    }
                    break;
            }
        }

        #endregion

        #region File Menu Events

        private void FileNewPage_Click(object sender, RoutedEventArgs e)
        {
            _sheetController.NewPage();
        }

        private void FileOpenPage_Click(object sender, RoutedEventArgs e)
        {
            _sheetController.OpenPage();
        }

        private void FileSavePage_Click(object sender, RoutedEventArgs e)
        {
            _sheetController.SavePage();
        }

        private void FileLibrary_Click(object sender, RoutedEventArgs e)
        {
            _sheetController.LoadLibrary();
        }

        private void FileDatabase_Click(object sender, RoutedEventArgs e)
        {
            OpenDatabase();
        }

        private void FileExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        #endregion

        #region Edit Menu Events

        private void EditUndo_Click(object sender, RoutedEventArgs e)
        {
            _sheetController.State.HistoryController.Undo();
        }

        private void EditRedo_Click(object sender, RoutedEventArgs e)
        {
            _sheetController.State.HistoryController.Redo();
        }

        private void EditCut_Click(object sender, RoutedEventArgs e)
        {
            _sheetController.CutText();
        }

        private void EditCopy_Click(object sender, RoutedEventArgs e)
        {
            _sheetController.CopyText();
        }

        private void EditPaste_Click(object sender, RoutedEventArgs e)
        {
            _sheetController.PasteText();
        }

        private void EditDelete_Click(object sender, RoutedEventArgs e)
        {
            _sheetController.Delete();
        }

        private void EditReset_Click(object sender, RoutedEventArgs e)
        {
            _sheetController.State.HistoryController.Register("Reset");
            _sheetController.ResetPage();
        }

        private void EditSelectAll_Click(object sender, RoutedEventArgs e)
        {
            _sheetController.SelecteAll();
        }

        private void EditDeselectAll_Click(object sender, RoutedEventArgs e)
        {
            _sheetController.DeselectAll();
        }

        private void EditCreateBlock_Click(object sender, RoutedEventArgs e)
        {
            _sheetController.CreateBlock();
        }

        private void EditBreakBlock_Click(object sender, RoutedEventArgs e)
        {
            _sheetController.BreakBlock();
        }

        private void EditMoveUp_Click(object sender, RoutedEventArgs e)
        {
            if (_sheetController.State.View.IsFocused)
            {
                _sheetController.MoveUp();
            }
        }

        private void EditMoveDown_Click(object sender, RoutedEventArgs e)
        {
            if (_sheetController.State.View.IsFocused)
            {
                _sheetController.MoveDown();
            }
        }

        private void EditMoveLeft_Click(object sender, RoutedEventArgs e)
        {
            if (_sheetController.State.View.IsFocused)
            {
                _sheetController.MoveLeft();
            }
        }

        private void EditMoveRight_Click(object sender, RoutedEventArgs e)
        {
            if (_sheetController.State.View.IsFocused)
            {
                _sheetController.MoveRight();
            }
        }

        private void EditToggleFill_Click(object sender, RoutedEventArgs e)
        {
            _sheetController.ToggleFill();
        }

        #endregion

        #region View Menu Events

        private void ViewZoomToPageLevel_Click(object sender, RoutedEventArgs e)
        {
            _sheetController.State.ZoomController.AutoFit();
        }

        private void ViewZoomActualSize_Click(object sender, RoutedEventArgs e)
        {
            _sheetController.State.ZoomController.ActualSize();
        }

        #endregion

        #region Mode Menu Events

        private void ModeNone_Click(object sender, RoutedEventArgs e)
        {
            _sheetController.State.Mode = SheetMode.None;
            UpdateModeMenu();
        }

        private void ModeSelection_Click(object sender, RoutedEventArgs e)
        {
            _sheetController.State.Mode = SheetMode.Selection;
            UpdateModeMenu();
        }

        private void ModeInsert_Click(object sender, RoutedEventArgs e)
        {
            _sheetController.State.Mode = SheetMode.Insert;
            UpdateModeMenu();
        }

        private void ModePoint_Click(object sender, RoutedEventArgs e)
        {
            _sheetController.State.Mode = SheetMode.Point;
            UpdateModeMenu();
        }
        
        private void ModeLine_Click(object sender, RoutedEventArgs e)
        {
            _sheetController.State.Mode = SheetMode.Line;
            UpdateModeMenu();
        }

        private void ModeRectangle_Click(object sender, RoutedEventArgs e)
        {
            _sheetController.State.Mode = SheetMode.Rectangle;
            UpdateModeMenu();
        }

        private void ModeEllipse_Click(object sender, RoutedEventArgs e)
        {
            _sheetController.State.Mode = SheetMode.Ellipse;
            UpdateModeMenu();
        }

        private void ModeText_Click(object sender, RoutedEventArgs e)
        {
            _sheetController.State.Mode = SheetMode.Text;
            UpdateModeMenu();
        }
        
        private void ModeImage_Click(object sender, RoutedEventArgs e)
        {
            _sheetController.State.Mode = SheetMode.Image;
            UpdateModeMenu();
        }

        #endregion
    }
}
