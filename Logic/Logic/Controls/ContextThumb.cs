// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Logic.Controls
{
    #region References

    using Logic.Model;
    using Logic.Model.Core;
    using Logic.Utilities;
    using Logic.ViewModels;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;

    #endregion

    #region Context Thumb

    public class ContextThumb : Thumb
    {
        #region Constructor

        public ContextThumb()
            : base()
        {
            this.DragStarted += ContextThumb_DragStarted;
            this.DragDelta += ContextThumb_DragDelta;
            this.DragCompleted += ContextThumb_DragCompleted;
        }

        #endregion

        #region Fields

        private Point? previous = null;
        private Point? original = null;
        private Point? final = null;
        private Thumb previousThumb = null;

        #endregion

        #region Events

        private static Point GetElementLocation(FrameworkElement element)
        {
            var location = (element.DataContext as ILocation);

            if (location == null)
                throw new Exception();

            return new Point(location.X, location.Y);
        }

        void ContextThumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            if (Defaults.Options.SimulationIsRunning == true)
            {
                e.Handled = false;
                return;
            }

            var thumb = sender as Thumb;

            if (thumb != null)
            {
                original = GetElementLocation(thumb);

                this.Cursor = Cursors.SizeAll;
            }
        }

        void ContextThumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            if (Defaults.Options.SimulationIsRunning == true)
            {
                e.Handled = false;
                return;
            }

            this.Cursor = Cursors.Arrow;

            // get thumb datacontext
            var element = (sender as FrameworkElement).DataContext as Element;
            if (element == null)
            {
                e.Handled = false;
                return;
            }

            var context = element.Parent as Context;
            if (context == null)
            {
                e.Handled = false;
                return;
            }

            if (context.IsLocked == true)
            {
                e.Handled = false;
                return;
            }

            var thumb = sender as Thumb;
            final = GetElementLocation(thumb);

            if (original != final)
            {
                // calculate delta
                var dX = original.Value.X - final.Value.X;
                var dY = original.Value.Y - final.Value.Y;

                var selectedElements = context.SelectedElements != null ? new List<Element>(context.SelectedElements) : null;

                // TODO: undo
                if (selectedElements != null && selectedElements.Count > 0)
                    UndoRedoActions.MoveElements(context, selectedElements, dX, dY);
                else
                    UndoRedoActions.MoveElement(context, element, dX, dY);
            }

            // reset drag context
            previous = null;
            original = null;
            final = null;
        }

        void ContextThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (Defaults.Options.SimulationIsRunning == true)
            {
                e.Handled = false;
                return;
            }

            // get thumb datacontext
            var element = (sender as FrameworkElement).DataContext as Element;
            if (element == null)
            {
                e.Handled = false;
                return;
            }

            var context = element.Parent as Context;
            if (context == null)
            {
                e.Handled = false;
                return;
            }

            if (context.IsLocked == true)
            {
                e.Handled = false;
                return;
            }

            var thumb = sender as Thumb;

            var dX = e.HorizontalChange;
            var dY = e.VerticalChange;
            var p = GetElementLocation(thumb);

            if (Defaults.Options.IsSnapEnabled)
            {
                p.X = SnapToGrid.Snap(p.X + dX, Defaults.Options.Snap);
                p.Y = SnapToGrid.Snap(p.Y + dY, Defaults.Options.Snap);
            }
            else
            {
                p.X = p.X + dX;
                p.Y = p.Y + dY;
            }

            if (previous == null)
            {
                previous = new Point(p.X, p.Y);
                previousThumb = thumb;
            }
            else
            {
                if ((Math.Round(p.X, 1) != Math.Round(previous.Value.X, 1) || Math.Round(p.Y, 1) != Math.Round(previous.Value.Y, 1))
                    && thumb == previousThumb)
                {
                    previous = new Point(p.X, p.Y);
                }
                else if (thumb != previousThumb)
                {
                    previous = new Point(p.X, p.Y);
                    previousThumb = thumb;
                }
            }

            // move element(s)
            if (context.SelectedElements != null && context.SelectedElements.Count > 0 && element.IsSelected == true)
                Manager.MoveSelectedElements(context, dX, dY);
            else
                Manager.MoveElement(context, element, dX, dY);
        }

        #endregion
    }

    #endregion
}
