// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace PanAndZoom
{
    #region References

    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
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

    #region ImageViewer

    public partial class ImageViewer : UserControl, INotifyPropertyChanged
    {
        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private void Notify(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region Properties

        private string title;
        public string Title
        {
            get { return title; }
            set
            {
                if (value != title)
                {
                    title = value;
                    Notify("Title");
                }
            }
        }

        #endregion

        #region Constructor

        public ImageViewer()
        {
            InitializeComponent();

            Initialize();
        }

        #endregion

        #region Initialize

        private void Initialize()
        {
            this.Title = "PanAndZoom";
            this.AllowDrop = true;

            LinkedList<string> fileNames = new LinkedList<string>();
            LinkedListNode<string> current = null;
            BitmapImage old = null;

            #region Action: Set Current Image

            Action setCurrentImage = () =>
            {
                if (current == null)
                    return;

                try
                {
                    byte[] buffer = System.IO.File.ReadAllBytes(current.Value);
                    System.IO.MemoryStream ms = new System.IO.MemoryStream(buffer);

                    BitmapImage bi = new BitmapImage();
                    bi.BeginInit();
                    bi.StreamSource = ms;
                    bi.EndInit();
                    bi.Freeze();

                    image.Source = bi;

                    if (old != null)
                        old.StreamSource.Dispose();

                    old = bi;

                    //image.Source = new BitmapImage(new Uri(current.Value));

                    Title = "PanAndZoom : " + System.IO.Path.GetFileNameWithoutExtension(current.Value);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.Print(ex.Message);
                }
            };

            #endregion

            #region Action: Set File Names

            Action<string[]> setFileNames = (fileNamesArray) =>
            {
                try
                {
                    // System.IO.FileInfo fi = new System.IO.FileInfo(path);
                    // fi.DirectoryName //Directory

                    var files = fileNamesArray.Where(x => (System.IO.File.GetAttributes(x) & System.IO.FileAttributes.Directory) != System.IO.FileAttributes.Directory)
                                              .OrderBy(f => f);

                    fileNames = new LinkedList<string>(files);
                    current = fileNames.First;

                    setCurrentImage();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.Print(ex.Message);
                }
            };

            #endregion

            #region Action: Open Images

            Action openImages = () =>
            {
                var dlg = new Microsoft.Win32.OpenFileDialog()
                {
                    DefaultExt = "",
                    Filter = "All Files (*.*)|*.*",
                    FilterIndex = 1,
                    FileName = "",
                    Multiselect = true
                };

                if (dlg.ShowDialog() == true)
                {
                    setFileNames(dlg.FileNames);
                }

                this.Focus();
            };

            #endregion

            #region Action: Close Images

            Action closeImages = () =>
            {
                image.Source = null;

                if (old != null)
                {
                    old.StreamSource.Dispose();
                    old = null;
                }

                current = null;
                fileNames.Clear();

                Title = "PandAndZoom";
            };

            #endregion

            #region Action: Save Image

            Action saveImage = () =>
            {
                if (old == null || current == null)
                    return;

                var dlg = new Microsoft.Win32.SaveFileDialog()
                {
                    DefaultExt = "",
                    Filter = "JPG Files (*.jpg)|*.jpg|PNG Files (*.png)|*.png|BMP Files (*.bmp)|*.bmp|GIF Files (*.gif)|*.gif|TIFF Files (*.tiff)|*.tiff",
                    FilterIndex = 1,
                    FileName = System.IO.Path.GetFileNameWithoutExtension(current.Value),

                };

                if (dlg.ShowDialog() == true)
                {
                    try
                    {
                        BitmapEncoder encoder = null;

                        switch (dlg.FilterIndex)
                        {
                            case 1:
                                encoder = new JpegBitmapEncoder();
                                break;
                            case 2:
                                encoder = new PngBitmapEncoder();
                                break;
                            case 3:
                                encoder = new BmpBitmapEncoder();
                                break;
                            case 4:
                                encoder = new GifBitmapEncoder();
                                break;
                            case 5:
                                encoder = new TiffBitmapEncoder();
                                break;
                            default:
                                return;
                        }

                        if (encoder == null)
                            return;

                        encoder.Frames.Add(BitmapFrame.Create(old));
                        using (var stream = dlg.OpenFile())
                        {
                            encoder.Save(stream);
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.Print(ex.Message);
                    }
                }

                this.Focus();
            };

            #endregion

            #region Action: Handle Key

            Action<Key> handleKey = (key) =>
            {
                bool haveNewFileName = false;
                switch (key)
                {
                    case Key.R:
                    case Key.Escape:
                        {
                            zoomBorder.ResetZoom();
                        }
                        break;
                    case Key.O:
                        {
                            openImages();
                        }
                        break;
                    case Key.C:
                        {
                            closeImages();
                        }
                        break;
                    case Key.S:
                        {
                            saveImage();
                        }
                        break;
                    case Key.Space:
                    case Key.Right:
                    case Key.Next:
                        {
                            if (fileNames.Count <= 0)
                                break;

                            var next = current.Next;
                            if (next == null)
                                next = current.List.First;

                            if (next != null)
                            {
                                current = next;
                                haveNewFileName = true;
                            }
                        }
                        break;
                    case Key.Left:
                    case Key.Prior:
                        {
                            if (fileNames.Count <= 0)
                                break;

                            var previous = current.Previous;
                            if (previous == null)
                                previous = current.List.Last;

                            if (previous != null)
                            {
                                current = previous;
                                haveNewFileName = true;
                            }
                        }
                        break;
                    case Key.Home:
                        {
                            if (fileNames.Count <= 0)
                                break;

                            var first = current.List.First;

                            if (first != null)
                            {
                                current = first;
                                haveNewFileName = true;
                            }
                        }
                        break;
                    case Key.End:
                        {
                            if (fileNames.Count <= 0)
                                break;

                            var last = current.List.Last;

                            if (last != null)
                            {
                                current = last;
                                haveNewFileName = true;
                            }
                        }
                        break;
                }

                if (haveNewFileName)
                    setCurrentImage();
            };

            #endregion

            #region Events

            this.PreviewDrop += (sender, e) =>
            {
                if (e.Data.GetDataPresent(DataFormats.FileDrop, true))
                {
                    string[] droppedFilePaths = e.Data.GetData(DataFormats.FileDrop, true) as string[];

                    setFileNames(droppedFilePaths);
                }
            };

            this.PreviewMouseDown += (sender, e) =>
            {
                if (e.ChangedButton == MouseButton.Middle)
                {
                    openImages();
                }
            };

            this.PreviewKeyDown += (sender, e) =>
            {
                handleKey(e.Key);
            };

            this.Loaded += (sender, e) =>
            {
                this.Focus();

                try
                {
                    if (Application.Current.Properties["AppArgs"] != null)
                    {
                        string[] argFilePaths = (string[])Application.Current.Properties["AppArgs"];

                        setFileNames(argFilePaths);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.Print(ex.Message);
                }
            };

            #endregion
        }

        #endregion
    }

    #endregion
}
