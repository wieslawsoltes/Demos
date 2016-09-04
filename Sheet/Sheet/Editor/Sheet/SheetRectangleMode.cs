// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Sheet.Core;
using Sheet.IoC;
using System;

namespace Sheet.Editor
{
    public class SheetRectangleMode
    {
        #region IoC

        private readonly IServiceLocator _serviceLocator;
        private readonly IBlockController _blockController;
        private readonly IBlockFactory _blockFactory;
        private readonly IBlockHelper _blockHelper;
        private readonly IItemController _itemController;
        private readonly IPointController _pointController;

        private readonly SheetState _state;

        public SheetRectangleMode(IServiceLocator serviceLocator, SheetState state)
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

        private XRectangle TempRectangle;
        private ImmutablePoint SelectionStartPoint;

        #endregion

        #region Methods

        public void Init(ImmutablePoint p)
        {
            double x = _itemController.Snap(p.X, _state.Options.SnapSize);
            double y = _itemController.Snap(p.Y, _state.Options.SnapSize);
            SelectionStartPoint = new ImmutablePoint(x, y);
            TempRectangle = _blockFactory.CreateRectangle(_state.Options.LineThickness / _state.ZoomController.Zoom, x, y, 0.0, 0.0, false, ArgbColors.Black, ArgbColors.Transparent);
            _state.OverlaySheet.Add(TempRectangle);
            _state.OverlaySheet.Capture();
        }

        public void Move(ImmutablePoint p)
        {
            double sx = SelectionStartPoint.X;
            double sy = SelectionStartPoint.Y;
            double x = _itemController.Snap(p.X, _state.Options.SnapSize);
            double y = _itemController.Snap(p.Y, _state.Options.SnapSize);
            _blockHelper.SetLeft(TempRectangle, Math.Min(sx, x));
            _blockHelper.SetTop(TempRectangle, Math.Min(sy, y));
            _blockHelper.SetWidth(TempRectangle, Math.Abs(sx - x));
            _blockHelper.SetHeight(TempRectangle, Math.Abs(sy - y));
        }

        public void Finish()
        {
            double x = _blockHelper.GetLeft(TempRectangle);
            double y = _blockHelper.GetTop(TempRectangle);
            double width = _blockHelper.GetWidth(TempRectangle);
            double height = _blockHelper.GetHeight(TempRectangle);
            if (width == 0.0 || height == 0.0)
            {
                Cancel();
            }
            else
            {
                _state.OverlaySheet.ReleaseCapture();
                _state.OverlaySheet.Remove(TempRectangle);
                _state.HistoryController.Register("Create Rectangle");
                _state.ContentBlock.Rectangles.Add(TempRectangle);
                _state.ContentSheet.Add(TempRectangle);
                TempRectangle = null;
            }
        }

        public void Cancel()
        {
            _state.OverlaySheet.ReleaseCapture();
            _state.OverlaySheet.Remove(TempRectangle);
            TempRectangle = null;
        }

        public void Reset()
        {
            if (TempRectangle != null)
            {
                _state.OverlaySheet.Remove(TempRectangle);
                TempRectangle = null;
            }
        }

        public void Adjust(double zoom)
        {
            double lineThicknessZoomed = _state.Options.LineThickness / zoom;

            if (TempRectangle != null)
            {
                _blockHelper.SetStrokeThickness(TempRectangle, lineThicknessZoomed);
            }
        }

        public void ToggleFill()
        {
            if (TempRectangle != null)
            {
                _blockController.ToggleFill(TempRectangle);
            }
        }

        #endregion
    }
}
