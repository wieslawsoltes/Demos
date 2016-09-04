#region References

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;

#endregion

namespace CustomDrawing.Core
{
    #region Editor

    public class Editor
    {
        #region Properties

        public Painter Painter { get; set; }
        public Factory Factory { get; set; }
        public IPointer Pointer { get; set; }
        public Action Redraw { get; set; }

        #endregion

        #region Fields

        public int CurrentEditMode = EditMode.Create;
        public int CurrentCreateMode = CreateMode.Line;
        private Point previous;
        private Line line = null;
        private Rectangle rect = null;
        private Text text = null;

        #endregion

        #region Mode

        public void SetCreateModeToLine()
        {
            CurrentCreateMode = CreateMode.Line;
        }

        public void SetCreateModeToText()
        {
            CurrentCreateMode = CreateMode.Text;
        }

        public void SetCreateModeToRectangle()
        {
            CurrentCreateMode = CreateMode.Rectangle;
        }

        public void SetEditModeToHitTest()
        {
            CurrentEditMode = EditMode.HitTest;
        }

        public void SetEditModeToCreate()
        {
            CurrentEditMode = EditMode.Create;
        }

        public void SetEditModeToMove()
        {
            CurrentEditMode = EditMode.Move;
        }

        #endregion

        #region Edit

        private void RedrawIfDirty()
        {
            if (Painter.IsDirty())
                Redraw();
        }

        public void LeftDown(Point p)
        {
            if (CurrentEditMode == EditMode.HitTest)
            {
                Pointer.Capture();
                StartHitTest(p, this.Redraw);
            }
            else if (CurrentEditMode == EditMode.Create)
            {
                StartEdit(p);
            }
            else if (CurrentEditMode == EditMode.Move)
            {
                FinishEdit();
                RedrawIfDirty();
            }
        }

        public void LeftUp()
        {
            if (CurrentEditMode == EditMode.HitTest)
                Pointer.Release();
        }

        public void Move(Point p)
        {
            if (CurrentEditMode == EditMode.HitTest)
            {
                if (Pointer.IsCaptured() && Painter.Selected != null)
                    MoveSelected(p);
                else
                    StartHitTest(p, this.RedrawIfDirty);
            }
            else if (CurrentEditMode == EditMode.Create)
            {
                StartHitTest(p, this.RedrawIfDirty);
            }
            else if (CurrentEditMode == EditMode.Move)
            {
                if (Pointer.IsCaptured())
                    MovedCurrentElement(p);
            }
        }

        private void MoveSelected(Point p)
        {
            Painter.Selected.X += (float)(p.X - previous.X);
            Painter.Selected.Y += (float)(p.Y - previous.Y);
            previous = p;
            Redraw();
        }

        private void MovedCurrentElement(Point p)
        {
            if (CurrentCreateMode == CreateMode.Line)
            {
                line.End.X = (float)p.X;
                line.End.Y = (float)p.Y;
                StartHitTest(p, this.Redraw);
            }
            else if (CurrentCreateMode == CreateMode.Rectangle)
            {
                rect.BottomRight.X = (float)p.X;
                rect.BottomRight.Y = (float)p.Y;
                StartHitTest(p, this.Redraw);
            }
            else if (CurrentCreateMode == CreateMode.Text)
            {
                text.Origin.X = (float)p.X;
                text.Origin.Y = (float)p.Y;
                StartHitTest(p, this.Redraw);
            }
        }

        public void RightDown()
        {
            if (CurrentEditMode == EditMode.Move)
                CancelEdit();
        }

        public void ToggleSnapMode(PinTypes type)
        {
            PinTypes current = Painter.SnapMode;
            Painter.SnapMode = ((current & type) == type) ? current &= ~type : current |= type;
        }

        public void MoveTextLeft()
        {
            if (CurrentCreateMode == CreateMode.Text && text != null)
            {
                switch (text.HAlign)
                {
                    case HAlign.Left: text.HAlign = HAlign.Right; break;
                    case HAlign.Center: text.HAlign = HAlign.Left; break;
                    case HAlign.Right: text.HAlign = HAlign.Center; break;
                }
                Redraw();
            }
        }

        public void MoveTextRight()
        {
            if (CurrentCreateMode == CreateMode.Text && text != null)
            {
                switch (text.HAlign)
                {
                    case HAlign.Left: text.HAlign = HAlign.Center; break;
                    case HAlign.Center: text.HAlign = HAlign.Right; break;
                    case HAlign.Right: text.HAlign = HAlign.Left; break;
                }
                Redraw();
            }
        }

        public void MoveTextDown()
        {
            if (CurrentCreateMode == CreateMode.Text && text != null)
            {
                switch (text.VAlign)
                {
                    case VAlign.Top: text.VAlign = VAlign.Center; break;
                    case VAlign.Center: text.VAlign = VAlign.Bottom; break;
                    case VAlign.Bottom: text.VAlign = VAlign.Top; break;
                }
                Redraw();
            }
        }

        public void MoveTextUp()
        {
            if (CurrentCreateMode == CreateMode.Text && text != null)
            {
                switch (text.VAlign)
                {
                    case VAlign.Top: text.VAlign = VAlign.Bottom; break;
                    case VAlign.Center: text.VAlign = VAlign.Top; break;
                    case VAlign.Bottom: text.VAlign = VAlign.Center; break;
                }
                Redraw();
            }
        }

        private void StartHitTest(Point p, Action redraw)
        {
            Painter.ResetSelected();
            previous = p;
            Painter.HitX = (float)p.X;
            Painter.HitY = (float)p.Y;
            Painter.DoHitTest = true;
            redraw();
        }

        private void StartEdit(Point p)
        {
            CreateElement(p);
            SetEditModeToMove();
            Pointer.Capture();
            Painter.DoHitTest = false;
            Redraw();
        }

        private void CreateElement(Point p)
        {
            if (CurrentCreateMode == CreateMode.Line)
            {
                var start = Painter.Selected != null ? Painter.Selected : Factory.CreatePin(null, (float)p.X, (float)p.Y, PinTypes.Snap);
                var end = Factory.CreatePin(null, (float)start.TransX, (float)start.TransY, PinTypes.Snap);
                line = Factory.CreateLine(null, start, end);
                line.UseTransforms = true;
                Painter.Elements.Add(line);
            }
            else if (CurrentCreateMode == CreateMode.Rectangle)
            {
                var tl = Painter.Selected != null ? Painter.Selected : Factory.CreatePin(null, (float)p.X, (float)p.Y, PinTypes.Snap);
                var br = Factory.CreatePin(null, (float)tl.TransX, (float)tl.TransY, PinTypes.Snap);
                rect = Factory.CreateRectangle(null, tl, br);
                rect.UseTransforms = true;
                Painter.Elements.Add(rect);
            }
            else if (CurrentCreateMode == CreateMode.Text)
            {
                var origin = Painter.Selected != null ? Painter.Selected : Factory.CreatePin(null, (float)p.X, (float)p.Y, PinTypes.Snap);
                text = Factory.CreateText(null, origin, "text", HAlign.Center, VAlign.Center);
                text.UseTransforms = true;
                Painter.Elements.Add(text);
            }
        }

        private void FinishEdit()
        {
            if (CurrentCreateMode == CreateMode.Line)
                ResetCurrentLine();
            else if (CurrentCreateMode == CreateMode.Rectangle)
                ResetCurrentRectangle();
            else if (CurrentCreateMode == CreateMode.Text)
                ResetCurrentText();

            SetEditModeToCreate();
            Pointer.Release();
        }

        private void ResetCurrentLine()
        {
            if (Painter.Selected != null)
            {
                line.End = Painter.Selected;
                Painter.ResetSelected();
            }

            line = null;
        }

        private void ResetCurrentRectangle()
        {
            if (Painter.Selected != null)
            {
                rect.BottomRight = Painter.Selected;
                Painter.ResetSelected();
            }

            rect = null;
        }

        private void ResetCurrentText()
        {
            if (Painter.Selected != null)
            {
                text.Origin = Painter.Selected;
                Painter.ResetSelected();
            }

            text = null;
        }

        public void CancelEdit()
        {
            RemoveCurrentElement();
            FinishEdit();
            Redraw();
        }

        private void RemoveCurrentElement()
        {
            switch (CurrentCreateMode)
            {
                case CreateMode.Line: Painter.Elements.Remove(line); break;
                case CreateMode.Rectangle: Painter.Elements.Remove(rect); break;
                case CreateMode.Text: Painter.Elements.Remove(text); break;
            }
        }

        #endregion
    }

    #endregion
}
