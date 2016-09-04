// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Sheet.Core;
using System;

namespace Sheet.Editor
{
    public class SheetState
    {
        public IHistoryController HistoryController { get; set; }
        public ILibraryController LibraryController { get; set; }
        public IZoomController ZoomController { get; set; }
        public ICursorController CursorController { get; set; }

        public SheetOptions Options { get; set; }

        public ISheet EditorSheet { get; set; }
        public ISheet BackSheet { get; set; }
        public ISheet ContentSheet { get; set; }
        public ISheet OverlaySheet { get; set; }

        public ISheetView View { get; set; }

        public double LastFinalWidth { get; set; }
        public double LastFinalHeight { get; set; }

        public SheetMode Mode { get; set; }
        public SheetMode TempMode { get; set; }

        public XBlock SelectedBlock { get; set; }
        public XBlock ContentBlock { get; set; }
        public XBlock FrameBlock { get; set; }
        public XBlock GridBlock { get; set; }

        public XRectangle TempSelectionRect { get; set; }
        public bool IsFirstMove { get; set; }
        public ImmutablePoint PanStartPoint { get; set; }
        public ImmutablePoint SelectionStartPoint { get; set; }
        public ItemType SelectedType { get; set; }
        public XLine SelectedLine { get; set; }
        public XThumb LineThumbStart { get; set; }
        public XThumb LineThumbEnd { get; set; }
        public XElement SelectedElement { get; set; }
        public XThumb ThumbTopLeft { get; set; }
        public XThumb ThumbTopRight { get; set; }
        public XThumb ThumbBottomLeft { get; set; }
        public XThumb ThumbBottomRight { get; set; }
    }
}
