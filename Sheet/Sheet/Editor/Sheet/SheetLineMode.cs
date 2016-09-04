// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Sheet.Core;
using Sheet.IoC;
using System;

namespace Sheet.Editor
{
    public class SheetLineMode
    {
        #region IoC

        private readonly IServiceLocator _serviceLocator;
        private readonly IBlockController _blockController;
        private readonly IBlockFactory _blockFactory;
        private readonly IBlockHelper _blockHelper;
        private readonly IItemController _itemController;
        private readonly IPointController _pointController;

        private readonly SheetState _state;

        public SheetLineMode(IServiceLocator serviceLocator, SheetState state)
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

        private XLine TempLine;
        private XEllipse TempStartEllipse;
        private XEllipse TempEndEllipse;

        #endregion

        #region Methods

        public void Init(ImmutablePoint p, XPoint start)
        {
            double x = _itemController.Snap(p.X, _state.Options.SnapSize);
            double y = _itemController.Snap(p.Y, _state.Options.SnapSize);

            TempLine = _blockFactory.CreateLine(_state.Options.LineThickness / _state.ZoomController.Zoom, x, y, x, y, ArgbColors.Black);

            if (start != null)
            {
                TempLine.Start = start;
            }

            TempStartEllipse = _blockFactory.CreateEllipse(_state.Options.LineThickness / _state.ZoomController.Zoom, x - 4.0, y - 4.0, 8.0, 8.0, true, ArgbColors.Black, ArgbColors.Black);
            TempEndEllipse = _blockFactory.CreateEllipse(_state.Options.LineThickness / _state.ZoomController.Zoom, x - 4.0, y - 4.0, 8.0, 8.0, true, ArgbColors.Black, ArgbColors.Black);

            _state.OverlaySheet.Add(TempLine);
            _state.OverlaySheet.Add(TempStartEllipse);
            _state.OverlaySheet.Add(TempEndEllipse);
            _state.OverlaySheet.Capture();
        }

        public void Move(ImmutablePoint p)
        {
            double x = _itemController.Snap(p.X, _state.Options.SnapSize);
            double y = _itemController.Snap(p.Y, _state.Options.SnapSize);
            double x2 = _blockHelper.GetX2(TempLine);
            double y2 = _blockHelper.GetY2(TempLine);
            if (Math.Round(x, 1) != Math.Round(x2, 1)
                || Math.Round(y, 1) != Math.Round(y2, 1))
            {
                _blockHelper.SetX2(TempLine, x);
                _blockHelper.SetY2(TempLine, y);
                _blockHelper.SetLeft(TempEndEllipse, x - 4.0);
                _blockHelper.SetTop(TempEndEllipse, y - 4.0);
            }
        }

        public void Finish(XPoint end)
        {
            double x1 = _blockHelper.GetX1(TempLine);
            double y1 = _blockHelper.GetY1(TempLine);
            double x2 = _blockHelper.GetX2(TempLine);
            double y2 = _blockHelper.GetY2(TempLine);

            if (Math.Round(x1, 1) == Math.Round(x2, 1) && Math.Round(y1, 1) == Math.Round(y2, 1))
            {
                Cancel();
            }
            else
            {
                if (end != null)
                {
                    TempLine.End = end;
                }

                _state.OverlaySheet.ReleaseCapture();
                _state.OverlaySheet.Remove(TempLine);
                _state.OverlaySheet.Remove(TempStartEllipse);
                _state.OverlaySheet.Remove(TempEndEllipse);

                _state.HistoryController.Register("Create Line");

                if (TempLine.Start != null)
                {
                    _pointController.ConnectStart(TempLine.Start, TempLine);
                }

                if (TempLine.End != null)
                {
                    _pointController.ConnectEnd(TempLine.End, TempLine);
                }

                _state.ContentBlock.Lines.Add(TempLine);
                _state.ContentSheet.Add(TempLine);

                TempLine = null;
                TempStartEllipse = null;
                TempEndEllipse = null;
            }
        }

        public void Cancel()
        {
            _state.OverlaySheet.ReleaseCapture();
            _state.OverlaySheet.Remove(TempLine);
            _state.OverlaySheet.Remove(TempStartEllipse);
            _state.OverlaySheet.Remove(TempEndEllipse);
            TempLine = null;
            TempStartEllipse = null;
            TempEndEllipse = null;
        }

        public void Reset()
        {
            if (TempLine != null)
            {
                _state.OverlaySheet.Remove(TempLine);
                TempLine = null;
            }

            if (TempStartEllipse != null)
            {
                _state.OverlaySheet.Remove(TempStartEllipse);
                TempLine = null;
            }

            if (TempEndEllipse != null)
            {
                _state.OverlaySheet.Remove(TempEndEllipse);
                TempEndEllipse = null;
            }
        }

        public void Adjust(double zoom)
        {
            double lineThicknessZoomed = _state.Options.LineThickness / zoom;

            if (TempLine != null)
            {
                _blockHelper.SetStrokeThickness(TempLine, lineThicknessZoomed);
            }

            if (TempStartEllipse != null)
            {
                _blockHelper.SetStrokeThickness(TempStartEllipse, lineThicknessZoomed);
            }

            if (TempEndEllipse != null)
            {
                _blockHelper.SetStrokeThickness(TempEndEllipse, lineThicknessZoomed);
            }
        }

        public void ToggleFill()
        {
        }

        #endregion
    }
}
