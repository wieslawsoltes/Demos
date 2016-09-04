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
using System.Printing;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Xml.Serialization;
using System.IO;
using Microsoft.Win32;
using System.Diagnostics;
using System.Windows.Controls.Primitives;

using LogicCanvas.Helpers;
using LogicCanvas.Adorners;
using LogicCanvas.Extensions;

using LogicCanvas.Model.Core;
using LogicCanvas.Model.Logic;
using LogicCanvas.Model.ControlBlock;
using LogicCanvas.Model.Page;
using System.Collections;

namespace LogicCanvas
{
    public partial class MainWindow : Window
    {
        #region MainWindow Data

        private Point RightClick = new Point(0, 0);
        private Point WindowRightClick = new Point(0, 0);
        private double GridSize = UnitConverter.CmToDip(0.5);
        private double GridSizeCreate = UnitConverter.CmToDip(1.0);
        private double GridOffsetLeft = UnitConverter.CmToDip(0.2);
        private double GridOffsetTop = UnitConverter.CmToDip(0.1);
		private double GridOffsetDataBlockLeft = UnitConverter.CmToDip(-0.1);
		
        private static string[] Dictionaries = 
        {
                "Dictionaries/StyleDictionary.xaml",
                "Dictionaries/PageDictionary.xaml",
                "Dictionaries/LogicDictionary.xaml",
                "Dictionaries/ControlBlockDictionary.xaml",
                "Dictionaries/ItemTemplateDictionary.xaml",
                "Dictionaries/PropertiesDictionary.xaml"
        };

        #endregion

        #region MainWindow Constructor

        public MainWindow()
        {
            InitializeComponent();

            this.KeyDown += new KeyEventHandler(MainWindow_KeyDown);
            this.PreviewMouseRightButtonDown += new MouseButtonEventHandler(MainWindow_PreviewMouseRightButtonDown);

            // page view
            var Page = new PageItem()
            {
                IsPrinting = false,
                IsGridVisible = true,
                IsDeletedVisible = false,
                Table = new TableItem()
                {
                    DrawnBy = "User",
                    CheckedBy = "User",
                    Date = "05.2014",
                    Type = "DIAGRAM",
                    Description1 = "Description1",
                    Description2 = "Description2",
                    Description3 = "Description3",
                    Revision = "E",
                    Format = "A4",
                    Page = "04",
                    Pages = "12",
                    Status = "-",
                    Project = "Project",
                    OrderNo = "",
                    DocumentNo1 = "-",
                    DocumentNo2 = "-",
                    Revisions = new RevisionList()
                    {
                        new RvisionItem() { Revision = "", Date = "", Author = "" },
                        new RvisionItem() { Revision = "", Date = "", Author = "" },
                        new RvisionItem() { Revision = "", Date = "", Author = "" },
                        new RvisionItem() { Revision = "", Date = "", Author = "" },
                        new RvisionItem() { Revision = "", Date = "", Author = "" },
                        new RvisionItem() { Revision = "", Date = "", Author = "" }
                    },
                    Logo1 = new BitmapImage(), // Logo Width=4.5cm, Height=2.8cm
                    Logo2 = new BitmapImage(),  // Logo Width=4.5cm, Height=2.8cm
                },
                Items = new ItemList(),
                Signals = new DataItemList()
                {
                    new DataItem()
                    {
                        Id = Guid.NewGuid(),
                        Designation = "Designation",
                        Signal = "Signal",
                        Description = "Description",
                        Condition = "Condition",
                    },
                    new DataItem()
                    {
                        Id = Guid.NewGuid(),
                        Designation = "Designation",
                        Signal = "Signal",
                        Description = "Description",
                        Condition = "Condition",
                    },
                },
                SelectedItem = null
            };

            //Page.Items.Add(new LineLogicItem() 
            //{ 
            //    X1 = UnitConverter.CmToDip(14.2), Y1 = UnitConverter.CmToDip(11.6), X2 = UnitConverter.CmToDip(14.7), Y2 = UnitConverter.CmToDip(11.6),
            //    IsStartInverted = false, IsEndInverted = true,
            //    X = 0, Y = 0, Z = 0,
            //});

            //Page.Items.Add(new LineLogicItem() { X1 = UnitConverter.CmToDip(14.2), Y1 = UnitConverter.CmToDip(12.6), X2 = UnitConverter.CmToDip(14.7), Y2 = UnitConverter.CmToDip(12.6), Z = 0 });
            //Page.Items.Add(new LineLogicItem() { X1 = UnitConverter.CmToDip(14.7), Y1 = UnitConverter.CmToDip(11.6), X2 = UnitConverter.CmToDip(14.7), Y2 = UnitConverter.CmToDip(17.1), Z = 0 });
            //Page.Items.Add(new LineLogicItem() { X1 = UnitConverter.CmToDip(14.7), Y1 = UnitConverter.CmToDip(18.1), X2 = UnitConverter.CmToDip(14.7), Y2 = UnitConverter.CmToDip(19.1), Z = 0 });

            //Page.Items.Add(new MemoryResetPriorityLogicItem() { X = UnitConverter.CmToDip(17.2), Y = UnitConverter.CmToDip(8.1), Z = 1 });
            //Page.Items.Add(new MemorySetPriorityLogicItem() { X = UnitConverter.CmToDip(23.2), Y = UnitConverter.CmToDip(8.1), Z = 1 });

            //Page.Items.Add(new AndGateLogicItem() { X = UnitConverter.CmToDip(14.2), Y = UnitConverter.CmToDip(17.1), Z = 1 });
            //Page.Items.Add(new OrGateLogicItem() { X = UnitConverter.CmToDip(16.2), Y = UnitConverter.CmToDip(17.1), Z = 1 });
            //Page.Items.Add(new SequenceControlControlBlockItem() { X = UnitConverter.CmToDip(13.2), Y = UnitConverter.CmToDip(19.1), Z = 1 });
            //Page.Items.Add(new SequenceStepLogicItem() { X = UnitConverter.CmToDip(20.2), Y = UnitConverter.CmToDip(24.1), Z = 1 });

            //Page.Items.Add(new DataBlockLogicItem()
            //{
            //    Designation = "Designation",
            //    Signal = "Signal",
            //    Description = "Description",
            //    Condition = "Condition",
            //    X = UnitConverter.CmToDip(1.3),
            //    Y = UnitConverter.CmToDip(11.1),
            //    Z = 1
            //});

            this.DataContext = Page;

            // page list view
            /*
            PageList Pages = new PageList();

            for (int i = 0; i < 3; i++)
            {
                Pages.Add(new PageItem()
                {
                    IsPrinting = false,
                    IsGridVisible = true,
                    IsDeletedVisible = false,
                    Table = new TableItem()
                    {
                        DrawnBy = "",
                        CheckedBy = "",
                        Date = "",
                        Type = "LOGIC DIAGRAMS",
                        Description1 = "",
                        Description2 = "",
                        Description3 = "",
                        Revision = "A",
                        Format = "A4",
                        Page = "",
                        Pages = "",
                        Status = "-",
                        Project = "",
                        OrderNo = "",
                        DocumentNo1 = "",
                        DocumentNo2 = "",
                        Revisions = new RevisionList(),
                        Logo1 = new BitmapImage(),
                        Logo2 = new BitmapImage()
                    },
                    Items = new ItemList()
                });
            }

            this.DataContext = Pages;
            */
        }

        #endregion

        #region Helper Functions

        void ShowItemProperties(Item item)
        {
            string info = string.Format("Type: {0}", item.GetType()) + System.Environment.NewLine +
                string.Format("IsNew: {0}", item.IsNew) + System.Environment.NewLine +
                string.Format("IsModified: {0}", item.IsNew) + System.Environment.NewLine +
                string.Format("IsDeleted: {0}", item.IsNew) + System.Environment.NewLine +
                string.Format("X: {0}cm", UnitConverter.DipToCm(item.X)) + System.Environment.NewLine +
                string.Format("Y: {0}cm", UnitConverter.DipToCm(item.Y)) + System.Environment.NewLine +
                string.Format("Z: {0}", item.Z);

            if (item is LineLogicItem)
            {
                var line = item as ILine;

                info += System.Environment.NewLine +
                    string.Format("IsStartInverted: {0}", line.IsStartInverted) + System.Environment.NewLine +
                    string.Format("IsEndInverted: {0}", line.IsEndInverted) + System.Environment.NewLine +
                    string.Format("X1: {0}cm", UnitConverter.DipToCm(line.X1)) + System.Environment.NewLine +
                    string.Format("Y1: {0}cm", UnitConverter.DipToCm(line.Y1)) + System.Environment.NewLine +
                    string.Format("X2: {0}cm", UnitConverter.DipToCm(line.X2)) + System.Environment.NewLine +
                    string.Format("Y2: {0}cm", UnitConverter.DipToCm(line.Y2));
            }

            MessageBox.Show(info, "Item Properties", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        Point GetClickLocation(double offsetLeft, double offsetTop)
        {
            return new Point()
            {
                X = SnapGrid.Snap(RightClick.X, GridSizeCreate, offsetLeft),
                Y = SnapGrid.Snap(RightClick.Y, GridSizeCreate, offsetTop)
            };
        }

        void ChangeColorTheme(string path)
        {
            try
            {
                Application.Current.Resources.BeginInit();

                Application.Current.Resources.MergedDictionaries.Clear();

                ResourceDictionary colorDict = Application.LoadComponent(new Uri(path, UriKind.Relative)) as ResourceDictionary;
                Application.Current.Resources.MergedDictionaries.Add(colorDict);

                foreach (string s in Dictionaries)
                {
                    ResourceDictionary dict = Application.LoadComponent(new Uri(s, UriKind.Relative)) as ResourceDictionary;
                    Application.Current.Resources.MergedDictionaries.Add(dict);
                }

                Application.Current.Resources.EndInit();
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
            }
        }

        #endregion

        #region Implementation

        void Open(PageItem Page)
        {
            try
            {
                OpenFileDialog dlg = new OpenFileDialog()
                {
                    FileName = "",
                    Filter = "All Files (*.*)|*.*|Xml Files (*.xml)|*.xml",
                    FilterIndex = 2
                };

                if (dlg.ShowDialog() == true)
                {
                    bool IsPrinting = Page.IsPrinting;
                    bool IsGridVisible = Page.IsGridVisible;
                    bool IsDeletedVisible = Page.IsDeletedVisible;

                    using (TextReader reader = new StreamReader(dlg.FileName))
                    {
                        XmlSerializer serializer = new XmlSerializer(Page.GetType(), Page.GetTypes());

                        PageItem newPage = new PageItem();
                        newPage = (PageItem)serializer.Deserialize(reader);

                        // restore not serialized valules
                        newPage.IsPrinting = IsPrinting;
                        newPage.IsGridVisible = IsGridVisible;
                        newPage.IsDeletedVisible = IsDeletedVisible;

                        // newPage.Signals = ??


                        // get all DataBlockLogicItem items
                        //var list = from item in newPage.Items 
                        //           where item is DataBlockLogicItem 
                        //           select item;


                        this.DataContext = newPage;
                        Page = null;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
            }
        }

        void Save(PageItem Page)
        {
            try
            {
                SaveFileDialog dlg = new SaveFileDialog()
                {
                    FileName = "Page",
                    Filter = "All Files (*.*)|*.*|Xml Files (*.xml)|*.xml",
                    FilterIndex = 2
                };

                if (dlg.ShowDialog() == true)
                {
                    using (TextWriter writer = new StreamWriter(dlg.FileName))
                    {
                        XmlSerializer serializer = new XmlSerializer(Page.GetType(), Page.GetTypes());
                        serializer.Serialize(writer, Page);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
            }
        }

        void Print(PageItem Page)
        {
            try
            {
                Page.IsGridVisible = false;
                Page.IsPrinting = true;

                ContentControl cc = new ContentControl();
                cc.ContentTemplate = this.FindResource("LogicPageItemTemplate1") as DataTemplate;
                cc.Content = Page;

                PrintDialog dlg = new System.Windows.Controls.PrintDialog();
                dlg.PrintQueue = LocalPrintServer.GetDefaultPrintQueue();
                dlg.PrintTicket = dlg.PrintQueue.DefaultPrintTicket;
                dlg.PrintTicket.PageOrientation = PageOrientation.Landscape;
                dlg.PrintTicket.OutputQuality = OutputQuality.High;
                //dlg.PrintTicket.OutputColor = OutputColor.Monochrome;
                dlg.PrintTicket.TrueTypeFontMode = TrueTypeFontMode.DownloadAsNativeTrueTypeFont;

                Nullable<bool> result = dlg.ShowDialog();
                if (result == true)
                {
                    System.Printing.PrintCapabilities capabilities = dlg.PrintQueue.GetPrintCapabilities(dlg.PrintTicket);

                    double scale = Math.Min(capabilities.PageImageableArea.ExtentWidth / UnitConverter.CmToDip(42.0),
                        capabilities.PageImageableArea.ExtentHeight / UnitConverter.CmToDip(29.7));

                    cc.LayoutTransform = new ScaleTransform(scale, scale);

                    Size sz = new Size(capabilities.PageImageableArea.ExtentWidth, capabilities.PageImageableArea.ExtentHeight);

                    cc.Measure(sz);
                    cc.Arrange(new Rect(new Point(capabilities.PageImageableArea.OriginWidth, capabilities.PageImageableArea.OriginHeight), sz));
                    cc.UpdateLayout();

                    dlg.PrintVisual(cc, "Page");
                }

                Page.IsGridVisible = true;
                Page.IsPrinting = false;
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
            }
        }

        void ToggleIsPrinting(PageItem Page)
        {
            Page.IsPrinting = Page.IsPrinting ? false : true;
            Page.IsGridVisible = Page.IsPrinting ? false : true;
        }

        void ToggleIsGridVisible(PageItem Page)
        {
            Page.IsGridVisible = Page.IsGridVisible ? false : true;
        }

        void ToggleIsDeletedVisible(PageItem Page)
        {
            Page.IsDeletedVisible = Page.IsDeletedVisible ? false : true;
        }

        #endregion

        #region MainWindow Events

        void MainWindow_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            RightClick = e.GetPosition(PageView);
            WindowRightClick = e.GetPosition(this);
        }

        void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            var Page = this.DataContext as PageItem;
            bool IsControl = Keyboard.Modifiers == ModifierKeys.Control;
            bool IsAlt = Keyboard.Modifiers == ModifierKeys.Alt;
            switch (e.Key)
            {
                case Key.NumPad0:
                case Key.D0:
                    {
                        if (IsControl)
                        {
                            //PageZoomView.ResetZoom();
                            PageZoomView.Reset();
                        }
                    }
                    break;
                case Key.Escape:
                    {
                       PageZoomView.Reset();
                    }
                    break;
                case Key.O:
                    {
                        if (IsControl)
                            Open(Page);
                    }
                    break;
                case Key.S:
                    {
                        if (IsControl)
                            Save(Page);
                    }
                    break;
                case Key.P:
                    {
                        if (IsControl)
                            Print(Page);
                    }
                    break;
                case Key.F5:
                    {
                        ToggleIsGridVisible(Page);
                    }
                    break;
                case Key.F6:
                    {
                        ToggleIsPrinting(Page);
                    }
                    break;
                case Key.F7:
                    {
                        ToggleIsDeletedVisible(Page);
                    }
                    break;

                case Key.Enter:
                    {
                        if (IsAlt)
                        {
                            // TODO: item properties
                        }
                    }
                    break;
                case Key.X:
                    {
                        if (IsControl)
                        {
                            // TODO: cut item
                        }
                    }
                    break;
                case Key.C:
                    {
                        if (IsControl)
                        {
                            // TODO: copy item
                        }
                    }
                    break;
                case Key.V:
                    {
                        if (IsControl)
                        {
                            Paste();
                        }
                    }
                    break;
                case Key.Delete:
                    {
                        if (IsControl)
                        {
                            // TODO: delete permanently item
                        }
                        else
                        {
                            // TODO: delete item
                        }
                    }
                    break;
            }
        }

        #endregion

        #region Thumb Events

        private void Thumb_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var item = (sender as FrameworkElement).DataContext as Item;
            ShowItemProperties(item);
        }

        private void Thumb_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            //if (e.ChangedButton == MouseButton.Left)
            //    (sender as Thumb).Cursor = Cursors.Hand;
        }

        private void Thumb_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            //if (e.ChangedButton == MouseButton.Left)
            //    (sender as Thumb).Cursor = Cursors.Arrow;
        }

        void Thumb_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // set currently selected page item
            var item = (sender as FrameworkElement).DataContext as Item;
            var Page = this.DataContext as PageItem;
            Page.SelectedItem = item;

            // NOTE: to remove item use 'Left Mouse Click + Shift'
            if (Keyboard.Modifiers == ModifierKeys.Shift)
            {
                Page.Items.Remove(item);
            }
        }

        public enum DataBlockSnapMode
        {
            Input,
            Output,
            Auto
        }

        void SnapDataBlockLogicItem(ILocation locacion, double dX, double dY, DataBlockSnapMode mode)
        {
            double left = SnapGrid.Snap(locacion.X + dX, UnitConverter.CmToDip(0.1));
            //double top = SnapGrid.Snap(locacion.Y + dY, UnitConverter.CmToDip(1.0), UnitConverter.CmToDip(0.1));
            double top = UnitConverter.CmToDip(Math.Floor(UnitConverter.DipToCm(locacion.Y + dY)) + 0.1);

            switch (mode)
            {
                case DataBlockSnapMode.Input:
                    {
                        left = UnitConverter.CmToDip(1.3);
                    }
                    break;
                case DataBlockSnapMode.Output:
                    {
                        left = UnitConverter.CmToDip(31.2);
                    }
                    break;
                case DataBlockSnapMode.Auto:
                    {
                        if (left < UnitConverter.CmToDip(1.3))
                        {
                            // X < 1.3cm
                            left = UnitConverter.CmToDip(1.3);
                        }
                        else if (left >= UnitConverter.CmToDip(1.3) && left < UnitConverter.CmToDip(11.2))
                        {
                            // 1.3cm >= X > 11.2cm
                            left = UnitConverter.CmToDip(1.3);
                        }
                        else if (left >= UnitConverter.CmToDip(11.2) && left < UnitConverter.CmToDip(31.2))
                        {
                            // 11.2cm >= X > 31.2cm
                            if (left <= UnitConverter.CmToDip(21.2))
                            {
                                // move to inputs
                                left = UnitConverter.CmToDip(1.3);
                            }
                            else
                            {
                                // move to outputs
                                left = UnitConverter.CmToDip(31.2);
                            }
                        }
                        else
                        {
                            // X >= 31.2cm
                            left = UnitConverter.CmToDip(31.2);
                        }
                    }
                    break;
            }

            if (top <= UnitConverter.CmToDip(2.1))
            {
                // Y <= 2.1cm
                top = UnitConverter.CmToDip(2.1);
            }
            else if (top >= UnitConverter.CmToDip(25.1))
            {
                // Y >= 25.1
                top = UnitConverter.CmToDip(25.1);
            }
            else
            {
                // 2.1cm < Y < 25.1cm
                //top = top + UnitConverter.CmToDip(0.1);
            }

            locacion.X = left;
            locacion.Y = top;
        }

        private void Thumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            var item = (sender as FrameworkElement).DataContext as Item;

            if (item is LineLogicItem)
            {
                // ILine line = item as ILine;
            }
			else if(item is DataBlockLogicItem)
			{
                SnapDataBlockLogicItem(item, e.HorizontalChange, e.VerticalChange, DataBlockSnapMode.Auto);
                //item.IsModified = true;
			}
            else
            {
                double left = item.X + e.HorizontalChange;
                double top = item.Y + e.VerticalChange;

                item.X = SnapGrid.Snap(left, GridSize, GridOffsetLeft);
                item.Y = SnapGrid.Snap(top, GridSize, GridOffsetTop);

                //item.IsModified = true;
            }
        }

        #endregion

        #region Canvas ContextMenu Events

        private void menuItemFileOpen_Click(object sender, RoutedEventArgs e)
        {
            var Page = this.DataContext as PageItem;
            Page.SelectedItem = null;
            Open(Page);
        }

        private void menuItemFileSave_Click(object sender, RoutedEventArgs e)
        {
            var Page = this.DataContext as PageItem;
            Save(Page);
        }

        private void menuItemFilePrint_Click(object sender, RoutedEventArgs e)
        {
            var Page = this.DataContext as PageItem;
            Print(Page);
        }

        private void menuItemViewGrid_Click(object sender, RoutedEventArgs e)
        {
            var Page = this.DataContext as PageItem;
            ToggleIsGridVisible(Page);
        }

        private void menuItemViewDeleted_Click(object sender, RoutedEventArgs e)
        {
            var Page = this.DataContext as PageItem;
            ToggleIsDeletedVisible(Page);
        }

        private void menuItemViewPrint_Click(object sender, RoutedEventArgs e)
        {
            var Page = this.DataContext as PageItem;
            ToggleIsPrinting(Page);
        }

        private void menuItemFileExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void menuItemInsertOneWayMotorControlBlock_Click(object sender, RoutedEventArgs e)
        {
            var Page = this.DataContext as PageItem;
            Point p = GetClickLocation(GridOffsetLeft, GridOffsetTop);
            Page.Items.Add(new OneWayMotorControlBlockItem() 
            {
                IsNew = false,
                IsModified = false,
                IsDeleted = false,
                X = p.X, 
                Y = p.Y,
                Z = 1
            });
        }

        private void menuItemInsertTwoWayMotorControlBlock_Click(object sender, RoutedEventArgs e)
        {
            var Page = this.DataContext as PageItem;
            Point p = GetClickLocation(GridOffsetLeft, GridOffsetTop);
            Page.Items.Add(new TwoWayMotorControlBlockItem()
            {
                IsNew = false,
                IsModified = false,
                IsDeleted = false,
                X = p.X,
                Y = p.Y,
                Z = 1
            });
        }

        private void menuItemInsertSolenoidValveControlBlock_Click(object sender, RoutedEventArgs e)
        {
            var Page = this.DataContext as PageItem;
            Point p = GetClickLocation(GridOffsetLeft, GridOffsetTop);
            Page.Items.Add(new SolenoidValveControlBlockItem()
            {
                IsNew = false,
                IsModified = false,
                IsDeleted = false,
                X = p.X,
                Y = p.Y,
                Z = 1
            });
        }

        private void menuItemInsertThrottlingValveControlBlock_Click(object sender, RoutedEventArgs e)
        {
            var Page = this.DataContext as PageItem;
            Point p = GetClickLocation(GridOffsetLeft, GridOffsetTop);
            Page.Items.Add(new ThrottlingValveControlBlockItem()
            {
                IsNew = false,
                IsModified = false,
                IsDeleted = false,
                X = p.X,
                Y = p.Y,
                Z = 1
            });
        }

        private void menuItemInsertControlValveControlBlock_Click(object sender, RoutedEventArgs e)
        {
            var Page = this.DataContext as PageItem;
            Point p = GetClickLocation(GridOffsetLeft, GridOffsetTop);
            Page.Items.Add(new ControlValveControlBlockItem()
            {
                IsNew = false,
                IsModified = false,
                IsDeleted = false,
                X = p.X,
                Y = p.Y,
                Z = 1
            });
        }

        private void menuItemInsertFrequencyConverterControlBlock_Click(object sender, RoutedEventArgs e)
        {
            var Page = this.DataContext as PageItem;
            Point p = GetClickLocation(GridOffsetLeft, GridOffsetTop);
            Page.Items.Add(new FrequencyConverterControlBlockItem()
            {
                IsNew = false,
                IsModified = false,
                IsDeleted = false,
                X = p.X,
                Y = p.Y,
                Z = 1
            });
        }

        private void menuItemInsertSequenceControlControlBlock_Click(object sender, RoutedEventArgs e)
        {
            var Page = this.DataContext as PageItem;
            Point p = GetClickLocation(GridOffsetLeft, GridOffsetTop);
            Page.Items.Add(new SequenceControlControlBlockItem()
            {
                IsNew = false,
                IsModified = false,
                IsDeleted = false,
                X = p.X,
                Y = p.Y,
                Z = 1
            });
        }

        private void menuItemInsertGroupControlTwoDevicesControlBlock_Click(object sender, RoutedEventArgs e)
        {
            var Page = this.DataContext as PageItem;
            Point p = GetClickLocation(GridOffsetLeft, GridOffsetTop);
            Page.Items.Add(new GroupControlTwoDevicesControlBlockItem()
            {
                IsNew = false,
                IsModified = false,
                IsDeleted = false,
                X = p.X,
                Y = p.Y,
                Z = 1
            });
        }

        private void menuItemInsertGroupControlThreeDevicesControlBlock_Click(object sender, RoutedEventArgs e)
        {
            var Page = this.DataContext as PageItem;
            Point p = GetClickLocation(GridOffsetLeft, GridOffsetTop);
            Page.Items.Add(new GroupControlThreeDevicesControlBlockItem()
            {
                IsNew = false,
                IsModified = false,
                IsDeleted = false,
                X = p.X,
                Y = p.Y,
                Z = 1
            });
        }

        private void menuItemInsertAndGateLogic_Click(object sender, RoutedEventArgs e)
        {
            var Page = this.DataContext as PageItem;
            Point p = GetClickLocation(GridOffsetLeft, GridOffsetTop);
            Page.Items.Add(new AndGateLogicItem()
            {
                IsNew = false,
                IsModified = false,
                IsDeleted = false,
                X = p.X,
                Y = p.Y,
                Z = 1
            });
        }

        private void menuItemInsertOrGateLogic_Click(object sender, RoutedEventArgs e)
        {
            var Page = this.DataContext as PageItem;
            Point p = GetClickLocation(GridOffsetLeft, GridOffsetTop);
            Page.Items.Add(new OrGateLogicItem()
            {
                IsNew = false,
                IsModified = false,
                IsDeleted = false,
                X = p.X,
                Y = p.Y,
                Z = 1
            });
        }

        private void menuItemInsertMemoryResetPriorityLogic_Click(object sender, RoutedEventArgs e)
        {
            var Page = this.DataContext as PageItem;
            Point p = GetClickLocation(GridOffsetLeft, GridOffsetTop);
            Page.Items.Add(new MemoryResetPriorityLogicItem()
            {
                IsNew = false,
                IsModified = false,
                IsDeleted = false,
                X = p.X,
                Y = p.Y,
                Z = 1
            });
        }

        private void menuItemInsertMemorySetPriorityLogic_Click(object sender, RoutedEventArgs e)
        {
            var Page = this.DataContext as PageItem;
            Point p = GetClickLocation(GridOffsetLeft, GridOffsetTop);
            Page.Items.Add(new MemorySetPriorityLogicItem()
            {
                IsNew = false,
                IsModified = false,
                IsDeleted = false,
                X = p.X,
                Y = p.Y,
                Z = 1
            });
        }

        private void menuItemInsertSequenceStepLogic_Click(object sender, RoutedEventArgs e)
        {
            var Page = this.DataContext as PageItem;
            Point p = GetClickLocation(GridOffsetLeft, GridOffsetTop);
            Page.Items.Add(new SequenceStepLogicItem()
            {
                IsNew = false,
                IsModified = false,
                IsDeleted = false,
                X = p.X,
                Y = p.Y,
                Z = 1
            });
        }

        void InsertDataBlockLogic(PageItem page, DataBlockSnapMode mode)
        {
            //Point p = GetClickLocation(0.0, 0.0);
            Point p = new Point(RightClick.X, RightClick.Y);

            var item = new DataBlockLogicItem()
            {
                IsNew = false,
                IsModified = false,
                IsDeleted = false,
                Designation = "Designation",
                Signal = "Signal",
                Description = "Description",
                Condition = "Condition",
                X = p.X,
                Y = p.Y,
                Z = 1
            };

            SnapDataBlockLogicItem(item, 0.0, 0.0, mode);
            page.Items.Add(item);
        }

        private void menuItemInsertDataBlockLogicAsInput_Click(object sender, RoutedEventArgs e)
        {
            var Page = this.DataContext as PageItem;
            InsertDataBlockLogic(Page, DataBlockSnapMode.Input);
        }

        private void menuItemInsertDataBlockLogicAsOutput_Click(object sender, RoutedEventArgs e)
        {
            var Page = this.DataContext as PageItem;
            InsertDataBlockLogic(Page, DataBlockSnapMode.Output);
        }

        private void menuItemInsertDataBlockLogicAsAuto_Click(object sender, RoutedEventArgs e)
        {
            var Page = this.DataContext as PageItem;
            InsertDataBlockLogic(Page, DataBlockSnapMode.Auto);
        }

        private void menuItemThemeLight_Click(object sender, RoutedEventArgs e)
        {
            ChangeColorTheme("/Dictionaries/ColorDictionary.Light.xaml");
        }

        private void menuItemThemeDark_Click(object sender, RoutedEventArgs e)
        {
            ChangeColorTheme("/Dictionaries/ColorDictionary.Dark.xaml");
        }

        #endregion

        #region Item ContextMenu Events

        private Item cloneItem = null;

        private void itemContextMenuCut_Click(object sender, RoutedEventArgs e)
        {
            var Page = this.DataContext as PageItem;
            var item = (sender as FrameworkElement).DataContext as Item;
            cloneItem = (Item)item.Clone();
            cloneItem.IsModified = false;
            cloneItem.IsNew = false;
            cloneItem.IsDeleted = false;
            Page.Items.Remove(item);
        }

        private void itemContextMenuCopy_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as FrameworkElement).DataContext as Item;
            cloneItem = (Item)item.Clone();
        }

        void Paste()
        {
            if (cloneItem != null)
            {
                var Page = this.DataContext as PageItem;
                Point p = GetClickLocation(GridOffsetLeft, GridOffsetTop);

                Item item = (Item)cloneItem.Clone();
                item.X = p.X;
                item.Y = p.Y;

                if (item is DataBlockLogicItem)
                {
                    SnapDataBlockLogicItem(item, 0.0, 0.0, DataBlockSnapMode.Auto);
                }
                else if (item is LineLogicItem)
                {
                    LineLogicItem line = (LineLogicItem)item;
                    
                    // TODO:
                }

                Page.Items.Add(item);
            }
        }

        private void itemContextMenuPaste_Click(object sender, RoutedEventArgs e)
        {
            Paste();
        }

        private void itemContextMenuDelete_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as FrameworkElement).DataContext as Item;
            var Page = this.DataContext as PageItem;
            Page.SelectedItem = null;
            item.IsDeleted = true;
        }

        private void itemContextMenuDeletePermanently_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as FrameworkElement).DataContext as Item;
            var Page = this.DataContext as PageItem;
            Page.SelectedItem = null;
            Page.Items.Remove(item);
        }

        private void itemContextMenuStatusIsNew_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as FrameworkElement).DataContext as Item;
            item.IsNew = item.IsNew ? false : true;
        }

        private void itemContextMenuStatusIsModified_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as FrameworkElement).DataContext as Item;
            item.IsModified = item.IsModified ? false : true;
        }

        private void itemContextMenuStatusIsDeleted_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as FrameworkElement).DataContext as Item;
            item.IsDeleted = item.IsDeleted ? false : true;
        }

        private void itemContextMenuIsStartInverted_Click(object sender, RoutedEventArgs e)
        {
            var line = (sender as FrameworkElement).DataContext as ILine;
            line.IsStartInverted = line.IsStartInverted ? false : true;
        }

        private void itemContextMenuIsEndInverted_Click(object sender, RoutedEventArgs e)
        {
            var line = (sender as FrameworkElement).DataContext as ILine;
            line.IsEndInverted = line.IsEndInverted ? false : true;
        }

        private void itemContextMenuProperties_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as FrameworkElement).DataContext as Item;
            ShowItemProperties(item);
        }

        #endregion

        #region Page ContextMenu Events

        private void pageContextMenuDeleteAll_Click(object sender, RoutedEventArgs e)
        {
            var Page = this.DataContext as PageItem;
            Page.SelectedItem = null;
            Page.Items.Clear();
        }

        private void pageContextMenuPaste_Click(object sender, RoutedEventArgs e)
        {
            Paste();
        }

        private void pageContextMenuStatusResetAll_Click(object sender, RoutedEventArgs e)
        {
            var Page = this.DataContext as PageItem;
            foreach (var item in Page.Items)
            {
                item.IsNew = false;
                item.IsModified = false;
                item.IsDeleted = false;
            }
        }

        private void pageContextMenuSetAllIsNew_Click(object sender, RoutedEventArgs e)
        {
            var Page = this.DataContext as PageItem;
            foreach (var item in Page.Items)
                item.IsNew = true;
        }

        private void pageContextMenuResetAllIsNew_Click(object sender, RoutedEventArgs e)
        {
            var Page = this.DataContext as PageItem;
            foreach (var item in Page.Items)
                item.IsNew = false;
        }

        private void pageContextMenuSetAllIsModified_Click(object sender, RoutedEventArgs e)
        {
            var Page = this.DataContext as PageItem;
            foreach (var item in Page.Items)
                item.IsModified = true;
        }

        private void pageContextMenuResetAllIsModified_Click(object sender, RoutedEventArgs e)
        {
            var Page = this.DataContext as PageItem;
            foreach (var item in Page.Items)
                item.IsModified = false;
        }

        private void pageContextMenuSetAllIsDeleted_Click(object sender, RoutedEventArgs e)
        {
            var Page = this.DataContext as PageItem;
            foreach (var item in Page.Items)
                item.IsDeleted = true;
        }

        private void pageContextMenuResetAllIsDeleted_Click(object sender, RoutedEventArgs e)
        {
            var Page = this.DataContext as PageItem;
            foreach (var item in Page.Items)
                item.IsDeleted = false;
        }

        #endregion

        #region ListBox Events

        private void ListBox_KeyDown(object sender, KeyEventArgs e)
        {
            var Page = this.DataContext as PageItem;
            var listBox = sender as ListBox;
            switch (e.Key)
            {
                case Key.Delete:
                    {
                        ItemList toDelete = new ItemList();
                        foreach (var item in listBox.SelectedItems)
                        {
                            toDelete.Add(item as Item);
                        }

                        foreach (var item in toDelete)
                        {
                            Page.Items.Remove(item);
                        }
                    }
                    break;
            }
        }

        #endregion

        #region Drag & Drop Signals

        private Point dragStartPoint;

        public static T FindVisualParent<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);
            if (parentObject == null)
                return null;

            T parent = parentObject as T;
            if (parent != null)
            {
                return parent;
            }
            else
            {
                return FindVisualParent<T>(parentObject);
            }
        }

        private void ListBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            dragStartPoint = e.GetPosition(null);
        }

        private void ListBox_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            Point mousePos = e.GetPosition(null);
            Vector diff = dragStartPoint - mousePos;
            if (e.LeftButton == MouseButtonState.Pressed && (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance || Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance)) 
            {
                ListBox listBox = sender as ListBox;
                ListBoxItem listBoxItem = FindVisualParent<ListBoxItem>((DependencyObject)e.OriginalSource);
                if (listBoxItem != null)
                {
                    DataItem dataItem = (DataItem)listBox.ItemContainerGenerator.ItemFromContainer(listBoxItem);
                    DataObject dragData = new DataObject("DataItemFormat", dataItem);
                    DragDrop.DoDragDrop(listBoxItem, dragData, DragDropEffects.Move);
                }
            } 
        }

        private void PageView_DragEnter(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent("DataItemFormat") || sender == e.Source)
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private void PageView_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("DataItemFormat"))
            {
                var dataItem = e.Data.GetData("DataItemFormat") as DataItem;

                if (dataItem != null)
                {
                    var parent = (ContentControl)sender;
                    var page = parent.DataContext as PageItem;
                    Point p = e.GetPosition(parent);

                    var item = new DataBlockLogicItem()
                    {
                        Id = dataItem.Id,
                        Designation = dataItem.Designation,
                        Signal = dataItem.Signal,
                        Description = dataItem.Description,
                        Condition = dataItem.Condition,
                        IsNew = false,
                        IsModified = false,
                        IsDeleted = false,
                        X = p.X,
                        Y = p.Y,
                        Z = 1
                    };

                    SnapDataBlockLogicItem(item, 0.0, 0.0, DataBlockSnapMode.Auto);

                    page.Items.Add(item);
                }
            }
        }

        #endregion
    }
}
