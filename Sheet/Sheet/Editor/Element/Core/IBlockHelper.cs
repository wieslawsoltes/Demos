// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Sheet.Core;
using System;

namespace Sheet.Editor
{
    public interface IBlockHelper
    {
        bool HitTest(XElement element, ImmutableRect rect);
        bool HitTest(XElement element, ImmutableRect rect, object relativeTo);

        void SetIsSelected(XElement element, bool value);
        bool GetIsSelected(XElement element);

        bool IsSelected(XPoint point);
        bool IsSelected(XLine line);
        bool IsSelected(XRectangle rectangle);
        bool IsSelected(XEllipse ellipse);
        bool IsSelected(XText text);
        bool IsSelected(XImage image);

        void Deselect(XPoint point);
        void Deselect(XLine line);
        void Deselect(XRectangle rectangle);
        void Deselect(XEllipse ellipse);
        void Deselect(XText text);
        void Deselect(XImage image);

        void Select(XPoint point);
        void Select(XLine line);
        void Select(XRectangle rectangle);
        void Select(XEllipse ellipse);
        void Select(XText text);
        void Select(XImage image);

        void SetZIndex(XElement element, int index);

        void ToggleFill(XRectangle rectangle);
        void ToggleFill(XEllipse ellipse);
        void ToggleFill(XPoint point);

        double GetLeft(XElement element);
        double GetTop(XElement element);
        double GetWidth(XElement element);
        double GetHeight(XElement element);
        void SetLeft(XElement element, double left);
        void SetTop(XElement element, double top);
        void SetWidth(XElement element, double width);
        void SetHeight(XElement element, double height);

        double GetX1(XLine line);
        double GetY1(XLine line);
        double GetX2(XLine line);
        double GetY2(XLine line);
        ArgbColor GetStroke(XLine line);
        void SetX1(XLine line, double x1);
        void SetY1(XLine line, double y1);
        void SetX2(XLine line, double x2);
        void SetY2(XLine line, double y2);
        void SetStrokeThickness(XLine line, double thickness);
        double GetStrokeThickness(XLine line);

        ArgbColor GetStroke(XRectangle rectangle);
        ArgbColor GetFill(XRectangle rectangle);
        bool IsTransparent(XRectangle rectangle);
        void SetStrokeThickness(XRectangle rectangle, double thickness);
        double GetStrokeThickness(XRectangle rectangle);

        ArgbColor GetStroke(XEllipse ellipse);
        ArgbColor GetFill(XEllipse ellipse);
        bool IsTransparent(XEllipse ellipse);
        void SetStrokeThickness(XEllipse ellipse, double thickness);
        double GetStrokeThickness(XEllipse ellipse);

        ArgbColor GetBackground(XText text);
        ArgbColor GetForeground(XText text);

        string GetText(XText text);
        int GetHAlign(XText text);
        int GetVAlign(XText text);
        double GetSize(XText text);

        byte[] GetData(XImage image);
    }
}
