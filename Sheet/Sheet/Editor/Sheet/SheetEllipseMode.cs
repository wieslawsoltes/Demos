// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Sheet.Core;
using Sheet.IoC;
using System;

namespace Sheet.Editor
{
    public class SheetEllipseMode
    {
        #region IoC

        private readonly IServiceLocator _serviceLocator;
        private readonly IBlockController _blockController;
        private readonly IBlockFactory _blockFactory;
        private readonly IBlockHelper _blockHelper;
        private readonly IItemController _itemController;
        private readonly IPointController _pointController;

        private readonly SheetState _state;

        public SheetEllipseMode(IServiceLocator serviceLocator, SheetState state)
        {
            this._serviceLocator = serviceLocator;
            this._blockController = serviceLocator.GetInstance<IBlockController>();
            this._blockFactory = serviceLocator.GetInstance<IBlockFactory>();
            this._blockHelper = serviceLocator.GetInstance<IBlockHelper>();
            this._itemController = serviceLocator.GetInstance<IItemController>();
            this._pointController = serviceLocator.GetInstance<IPointController>();

            this._state = state;
        }

        #endregion

        #region Fields

        private XEllipse TempEllipse;
        private ImmutablePoint SelectionStartPoint;

        #endregion

        #region Methods

        public void Init(ImmutablePoint p)
        {
            double x = _itemController.Snap(p.X, _state.Options.SnapSize);
            double y = _itemController.Snap(p.Y, _state.Options.SnapSize);
            SelectionStartPoint = new ImmutablePoint(x, y);
            TempEllipse = _blockFactory.CreateEllipse(_state.Options.LineThickness / _state.ZoomController.Zoom, x, y, 0.0, 0.0, false, ArgbColors.Black, ArgbColors.Transparent);
            _state.OverlaySheet.Add(TempEllipse);
            _state.OverlaySheet.Capture();
        }

        public void Move(ImmutablePoint p)
        {
            double sx = SelectionStartPoint.X;
            double sy = SelectionStartPoint.Y;
            double x = _itemController.Snap(p.X, _state.Options.SnapSize);
            double y = _itemController.Snap(p.Y, _state.Options.SnapSize);
            _blockHelper.SetLeft(TempEllipse, Math.Min(sx, x));
            _blockHelper.SetTop(TempEllipse, Math.Min(sy, y));
            _blockHelper.SetWidth(TempEllipse, Math.Abs(sx - x));
            _blockHelper.SetHeight(TempEllipse, Math.Abs(sy - y));
        }

        public void Finish()
        {
            double x = _blockHelper.GetLeft(TempEllipse);
            double y = _blockHelper.GetTop(TempEllipse);
            double width = _blockHelper.GetWidth(TempEllipse);
            double height = _blockHelper.GetHeight(TempEllipse);
            if (width == 0.0 || height == 0.0)
            {
                Cancel();
            }
            else
            {
                _state.OverlaySheet.ReleaseCapture();
                _state.OverlaySheet.Remove(TempEllipse);
                _state.HistoryController.Register("Create Ellipse");
                _state.ContentBlock.Ellipses.Add(TempEllipse);
                _state.ContentSheet.Add(TempEllipse);
                TempEllipse = null;
            }
        }

        public void Cancel()
        {
            _state.OverlaySheet.ReleaseCapture();
            _state.OverlaySheet.Remove(TempEllipse);
            TempEllipse = null;
        }

        public void Reset()
        {
            if (TempEllipse != null)
            {
                _state.OverlaySheet.Remove(TempEllipse);
                TempEllipse = null;
            }
        }

        public void Adjust(double zoom)
        {
            double lineThicknessZoomed = _state.Options.LineThickness / zoom;

            if (TempEllipse != null)
            {
                _blockHelper.SetStrokeThickness(TempEllipse, lineThicknessZoomed);
            }
        }

        public void ToggleFill()
        {
            if (TempEllipse != null)
            {
                _blockController.ToggleFill(TempEllipse);
            }
        }

        #endregion
    }
}
