// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Sheet.Core;
using Sheet.Editor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Sheet
{
    public partial class SheetControl : UserControl, ISheetView, IZoomController, ICursorController
    {
        #region IoC

        private readonly ISheetController _sheetController;

        public SheetControl(ISheetController sheetController)
        {
            InitializeComponent();

            this._sheetController = sheetController;

            Loaded += (sender, e) =>
            {
                _sheetController.State.EditorSheet.SetParent(EditorCanvas);
                _sheetController.State.BackSheet.SetParent(Root.Back);
                _sheetController.State.ContentSheet.SetParent(Root.Sheet);
                _sheetController.State.OverlaySheet.SetParent(Root.Overlay);
                _sheetController.Init();
            };
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
        }

        #endregion

        #region AutoFit

        private int FindZoomIndex(double factor)
        {
            int index = -1;

            if (_sheetController == null || _sheetController.State.Options == null)
            {
                return index;
            }

            for (int i = 0; i < _sheetController.State.Options.ZoomFactors.Length; i++)
            {
                if (_sheetController.State.Options.ZoomFactors[i] > factor)
                {
                    index = i;
                    break;
                }
            }

            index = Math.Max(0, index);
            index = Math.Min(index, _sheetController.State.Options.MaxZoomIndex);

            return index;
        }

        private void AutoFit(double finalWidth,  double finalHeight, double desiredWidth, double desiredHeight)
        {
            if (_sheetController == null || _sheetController.State.Options == null)
            {
                return;
            }

            // calculate factor
            double fwidth = finalWidth / _sheetController.State.Options.PageWidth;
            double fheight = finalHeight / _sheetController.State.Options.PageHeight;
            double factor = Math.Min(fwidth, fheight);
            double panX = (finalWidth - (_sheetController.State.Options.PageWidth * factor)) / 2.0;
            double panY = (finalHeight - (_sheetController.State.Options.PageHeight * factor)) / 2.0;
            double dx = Math.Max(0, (finalWidth - desiredWidth) / 2.0);
            double dy = Math.Max(0, (finalHeight - desiredHeight) / 2.0);

            // adjust zoom
            ZoomIndex = FindZoomIndex(factor);
            Zoom = factor;

            // adjust pan
            PanX = panX - dx;
            PanY = panY - dy;
        }

        #endregion

        #region IZoomController

        private int zoomIndex = -1;
        public int ZoomIndex
        {
            get { return zoomIndex; }
            set
            {
                if (_sheetController == null || _sheetController.State.Options == null)
                {
                    return;
                }

                if (value >= 0 && value <= _sheetController.State.Options.MaxZoomIndex)
                {
                    zoomIndex = value;
                    Zoom = _sheetController.State.Options.ZoomFactors[zoomIndex];
                }
            }
        }

        public double Zoom
        {
            get { return zoom.ScaleX; }
            set
            {
                if (_sheetController == null || _sheetController.State.Options == null)
                {
                    return;
                }

                if (IsLoaded)
                {
                    _sheetController.AdjustPageThickness(value);
                }

                zoom.ScaleX = value;
                zoom.ScaleY = value;
                _sheetController.State.Options.Zoom = value;
            }
        }

        public double PanX
        {
            get { return pan.X; }
            set
            {
                if (_sheetController == null || _sheetController.State.Options == null)
                {
                    return;
                }

                pan.X = value;
                _sheetController.State.Options.PanX = value;
            }
        }

        public double PanY
        {
            get { return pan.Y; }
            set
            {
                if (_sheetController == null || _sheetController.State.Options == null)
                {
                    return;
                }

                pan.Y = value;
                _sheetController.State.Options.PanY = value;
            }
        }

        public void AutoFit()
        {
            AutoFit(_sheetController.State.LastFinalWidth, _sheetController.State.LastFinalHeight, DesiredSize.Width, DesiredSize.Height);
        }

        public void ActualSize()
        {
            zoomIndex = _sheetController.State.Options.DefaultZoomIndex;
            Zoom = _sheetController.State.Options.ZoomFactors[zoomIndex];
            PanX = 0.0;
            PanY = 0.0;
        }

        #endregion

        #region ICursorController

        public void Set(SheetCursor cursor)
        {
            switch(cursor)
            {
                case SheetCursor.Normal:
                    Cursor = Cursors.Arrow;
                    break;
                case SheetCursor.Move:
                    Cursor = Cursors.SizeAll;
                    break;
                case SheetCursor.Pan:
                    Cursor = Cursors.ScrollAll;
                    break;
                default:
                    break;
            }
        }

        public SheetCursor Get()
        {
            return SheetCursor.Unknown;
        }

        #endregion

        #region Events
        
        private void LeftDown(MouseButtonEventArgs e)
        {
            bool onlyCtrl = Keyboard.Modifiers == ModifierKeys.Control;
            bool onlyShift = Keyboard.Modifiers == ModifierKeys.Shift;
            bool sourceIsThumb = ((e.OriginalSource as FrameworkElement).TemplatedParent) is Thumb;
            Point sheetPoint = e.GetPosition(_sheetController.State.OverlaySheet.GetParent() as FrameworkElement);
            ImmutablePoint sheetPosition = new ImmutablePoint(sheetPoint.X, sheetPoint.Y);
            Point rootPoint = e.GetPosition(this);
            ImmutablePoint rootPosition = new ImmutablePoint(rootPoint.X, rootPoint.Y);

            var args = new InputArgs()
            {
                OnlyControl = onlyCtrl,
                OnlyShift = onlyShift,
                SourceType = sourceIsThumb ? ItemType.Thumb : ItemType.None,
                SheetPosition = sheetPosition,
                RootPosition = rootPosition,
                Handled = (handled) => e.Handled = handled,
                Delta = 0,
                Button = InputButton.Left,
                Clicks = 1
            };

            _sheetController.LeftDown(args);
        }

        private void LeftUp(MouseButtonEventArgs e)
        {
            bool onlyCtrl = Keyboard.Modifiers == ModifierKeys.Control;
            bool onlyShift = Keyboard.Modifiers == ModifierKeys.Shift;
            bool sourceIsThumb = ((e.OriginalSource as FrameworkElement).TemplatedParent) is Thumb;
            Point sheetPoint = e.GetPosition(_sheetController.State.OverlaySheet.GetParent() as FrameworkElement);
            ImmutablePoint sheetPosition = new ImmutablePoint(sheetPoint.X, sheetPoint.Y);
            Point rootPoint = e.GetPosition(this);
            ImmutablePoint rootPosition = new ImmutablePoint(rootPoint.X, rootPoint.Y);

            var args = new InputArgs()
            {
                OnlyControl = onlyCtrl,
                OnlyShift = onlyShift,
                SourceType = sourceIsThumb ? ItemType.Thumb : ItemType.None,
                SheetPosition = sheetPosition,
                RootPosition = rootPosition,
                Handled = (handled) => e.Handled = handled,
                Delta = 0,
                Button = InputButton.Left,
                Clicks = 1
            };

            _sheetController.LeftUp(args);
        }

        private void Move(MouseEventArgs e)
        {
            bool onlyCtrl = Keyboard.Modifiers == ModifierKeys.Control;
            bool onlyShift = Keyboard.Modifiers == ModifierKeys.Shift;
            bool sourceIsThumb = ((e.OriginalSource as FrameworkElement).TemplatedParent) is Thumb;
            Point sheetPoint = e.GetPosition(_sheetController.State.OverlaySheet.GetParent() as FrameworkElement);
            ImmutablePoint sheetPosition = new ImmutablePoint(sheetPoint.X, sheetPoint.Y);
            Point rootPoint = e.GetPosition(this);
            ImmutablePoint rootPosition = new ImmutablePoint(rootPoint.X, rootPoint.Y);

            var args = new InputArgs()
            {
                OnlyControl = onlyCtrl,
                OnlyShift = onlyShift,
                SourceType = sourceIsThumb ? ItemType.Thumb : ItemType.None,
                SheetPosition = sheetPosition,
                RootPosition = rootPosition,
                Handled = (handled) => e.Handled = handled,
                Delta = 0,
                Button = InputButton.Left,
                Clicks = 1
            };

            _sheetController.Move(args);
        }

        private void RightDown(MouseButtonEventArgs e)
        {
            bool onlyCtrl = Keyboard.Modifiers == ModifierKeys.Control;
            bool onlyShift = Keyboard.Modifiers == ModifierKeys.Shift;
            bool sourceIsThumb = ((e.OriginalSource as FrameworkElement).TemplatedParent) is Thumb;
            Point sheetPoint = e.GetPosition(_sheetController.State.OverlaySheet.GetParent() as FrameworkElement);
            ImmutablePoint sheetPosition = new ImmutablePoint(sheetPoint.X, sheetPoint.Y);
            Point rootPoint = e.GetPosition(this);
            ImmutablePoint rootPosition = new ImmutablePoint(rootPoint.X, rootPoint.Y);

            var args = new InputArgs()
            {
                OnlyControl = onlyCtrl,
                OnlyShift = onlyShift,
                SourceType = sourceIsThumb ? ItemType.Thumb : ItemType.None,
                SheetPosition = sheetPosition,
                RootPosition = rootPosition,
                Handled = (handled) => e.Handled = handled,
                Delta = 0,
                Button = InputButton.Left,
                Clicks = 1
            };

            _sheetController.RightDown(args);
        }

        private void RightUp(MouseButtonEventArgs e)
        {
            bool onlyCtrl = Keyboard.Modifiers == ModifierKeys.Control;
            bool onlyShift = Keyboard.Modifiers == ModifierKeys.Shift;
            bool sourceIsThumb = ((e.OriginalSource as FrameworkElement).TemplatedParent) is Thumb;
            Point sheetPoint = e.GetPosition(_sheetController.State.OverlaySheet.GetParent() as FrameworkElement);
            ImmutablePoint sheetPosition = new ImmutablePoint(sheetPoint.X, sheetPoint.Y);
            Point rootPoint = e.GetPosition(this);
            ImmutablePoint rootPosition = new ImmutablePoint(rootPoint.X, rootPoint.Y);

            var args = new InputArgs()
            {
                OnlyControl = onlyCtrl,
                OnlyShift = onlyShift,
                SourceType = sourceIsThumb ? ItemType.Thumb : ItemType.None,
                SheetPosition = sheetPosition,
                RootPosition = rootPosition,
                Handled = (handled) => e.Handled = handled,
                Delta = 0,
                Button = InputButton.Left,
                Clicks = 1
            };

            _sheetController.RightUp(args);
        }

        private void Wheel(MouseWheelEventArgs e)
        {
            int d = e.Delta;
            var p = e.GetPosition(Layout);
            _sheetController.Wheel(d, new ImmutablePoint(p.X, p.Y));
        }

        private void Down(MouseButtonEventArgs e)
        {
            bool onlyCtrl = Keyboard.Modifiers == ModifierKeys.Control;
            bool onlyShift = Keyboard.Modifiers == ModifierKeys.Shift;
            bool sourceIsThumb = ((e.OriginalSource as FrameworkElement).TemplatedParent) is Thumb;
            Point sheetPoint = e.GetPosition(_sheetController.State.OverlaySheet.GetParent() as FrameworkElement);
            ImmutablePoint sheetPosition = new ImmutablePoint(sheetPoint.X, sheetPoint.Y);
            Point rootPoint = e.GetPosition(this);
            ImmutablePoint rootPosition = new ImmutablePoint(rootPoint.X, rootPoint.Y);

            var args = new InputArgs()
            {
                OnlyControl = onlyCtrl,
                OnlyShift = onlyShift,
                SourceType = sourceIsThumb ? ItemType.Thumb : ItemType.None,
                SheetPosition = sheetPosition,
                RootPosition = rootPosition,
                Handled = (handled) => e.Handled = handled,
                Delta = 0,
                Clicks = e.ClickCount
            };

            switch(e.ChangedButton)
            {
                case MouseButton.Left: args.Button = InputButton.Left; break;
                case MouseButton.Middle: args.Button = InputButton.Middle; break;
                case MouseButton.Right: args.Button = InputButton.Right; break;
                default: args.Button = InputButton.None; break;
            }

            _sheetController.Down(args);
        }

        private void UserControl_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Focus();
            LeftDown(e);
        }

        private void UserControl_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            LeftUp(e);
        }

        private void UserControl_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            Move(e);
        }

        private void UserControl_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Focus();
            RightDown(e);
        }

        private void UserControl_PreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            RightUp(e);
        }

        private void UserControl_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            Wheel(e);
        }

        private void UserControl_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Down(e);
        }

        #endregion

        #region Drop

        public const string BlockDropFormat = "Block";
        public const string DataDropFormat = "Data";

        private void UserControl_DragEnter(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(BlockDropFormat) || !e.Data.GetDataPresent(DataDropFormat) || sender == e.Source)
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private void UserControl_Drop(object sender, DragEventArgs e)
        {
            Point point = e.GetPosition(_sheetController.State.OverlaySheet.GetParent() as FrameworkElement);
            ImmutablePoint position = new ImmutablePoint(point.X, point.Y);

            if (e.Data.GetDataPresent(BlockDropFormat))
            {
                var blockItem = e.Data.GetData(BlockDropFormat) as ItemBlock;
                if (blockItem != null)
                {
                    _sheetController.Insert(blockItem, position, true);
                    e.Handled = true;
                }
            }
            else if (e.Data.GetDataPresent(DataDropFormat))
            {
                var dataItem = e.Data.GetData(DataDropFormat) as ItemData;
                if (dataItem != null)
                {
                    _sheetController.TryToBindData(position, dataItem);
                    e.Handled = true;
                }
            }
        }

        #endregion
    }
}
