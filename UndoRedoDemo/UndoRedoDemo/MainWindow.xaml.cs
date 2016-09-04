// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace UndoRedoDemo
{
    #region References

    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Navigation;
    using System.Windows.Shapes;

    #endregion

    #region MainWindow

    public partial class MainWindow : Window
    {
        #region Properties

        private double snap = 30;
        private Point? previous = null;
        private Point? original = null;
        private Point? final = null;
        private Thumb previousThumb = null;

        #endregion

        #region Constructor

        public MainWindow()
        {
            InitializeComponent();
        }

        #endregion

        #region Element Location

        private Point GetElementLocation(UIElement element)
        {
            var left = Canvas.GetLeft(element);
            var top = Canvas.GetTop(element);

            return new Point(left, top);
        }

        private void SetElementLocation(UIElement element, Point point)
        {
            Canvas.SetLeft(element, point.X);
            Canvas.SetTop(element, point.Y);
        }

        #endregion

        #region Thumb Events

        private void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            var thumb = sender as Thumb;

            var dX = e.HorizontalChange;
            var dY = e.VerticalChange;

            var p = GetElementLocation(thumb);

            //System.Diagnostics.Debug.Print("p0={0}", p);

            p.X = SnapToGrid.Snap(p.X + dX, snap);
            p.Y = SnapToGrid.Snap(p.Y + dY, snap);

            if (previous == null)
            {
                SetElementLocation(thumb, p);

                previous = new Point(p.X, p.Y);
                previousThumb = thumb;

                System.Diagnostics.Debug.Print("Thumb_DragDelta: p1={0} (previous is null)", p);  
            }
            else
            {
                if ((Math.Round(p.X, 1) != Math.Round(previous.Value.X, 1) || Math.Round(p.Y, 1) != Math.Round(previous.Value.Y, 1))
                    && thumb == previousThumb)
                {
                    SetElementLocation(thumb, p);

                    previous = new Point(p.X, p.Y);

                    System.Diagnostics.Debug.Print("Thumb_DragDelta: p1={0} (previous in not null)", p);
                }
                else if (thumb != previousThumb)
                {
                    SetElementLocation(thumb, p);

                    previous = new Point(p.X, p.Y);
                    previousThumb = thumb;

                    System.Diagnostics.Debug.Print("Thumb_DragDelta: p1={0} (new thumb)", p);
                }
            } 
        }

        private void Thumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            var thumb = sender as Thumb;

            original = GetElementLocation(thumb);

            System.Diagnostics.Debug.Print("Thumb_DragStarted: {0}, {1} | original: {2}", e.HorizontalOffset, e.VerticalOffset, original.Value);
        }

        private void Thumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            var thumb = sender as Thumb;

            final = GetElementLocation(thumb);

            System.Diagnostics.Debug.Print("Thumb_DragCompleted: {0}, {1} | original: {2} | final: {3}", e.HorizontalChange, e.VerticalChange, original.Value, final);

            if (original != final)
            {
                // undo action
                var undoPoint = new Point(original.Value.X, original.Value.Y);
                Action undoAction = () =>
                {
                    System.Diagnostics.Debug.Print("Undo: {0} | tag: {1}", undoPoint, thumb.Tag);

                    SetElementLocation(thumb, undoPoint);
                };

                // redo action
                var redoPoint = new Point(final.Value.X, final.Value.Y);
                Action redoAction = () =>
                {
                    System.Diagnostics.Debug.Print("Redo: {0} | tag: {1}", redoPoint, thumb.Tag);
                    SetElementLocation(thumb, redoPoint);
                };

                // register undo/redo action
                var action = new UndoRedoAction(undoAction, redoAction, "Move Element");
                UndoRedoFramework.Add(action);
            }

            // reset drag context
            previous = null;
            original = null;
            final = null;
        }

        #endregion

        #region Button Events

        private void buttonUndo_Click(object sender, RoutedEventArgs e)
        {
            UndoRedoFramework.Undo();
        }

        private void buttonRedo_Click(object sender, RoutedEventArgs e)
        {
            UndoRedoFramework.Redo();
        }

        private void buttonReset_Click(object sender, RoutedEventArgs e)
        {
            UndoRedoFramework.Clear();
        }

        private void buttonReplay_Click(object sender, RoutedEventArgs e)
        {
            if (timer == null)
                this.StartTimer();
            else
                this.StopTimer();
        }

        #endregion

        #region Replay Timer

        private System.Timers.Timer timer = null;
        private bool doRedo = false;

        private void StartTimer()
        {
            if (timer == null)
            {
                timer = new System.Timers.Timer();
                timer.Interval = 100;
                timer.Elapsed += timer_Elapsed;
                timer.Start();
            }
        }

        private void StopTimer()
        {
            if (timer != null)
            {
                timer.Dispose();
                timer = null;
            }
        }

        private void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                if (doRedo == false && UndoRedoFramework.State.CanUndo)
                {
                    UndoRedoFramework.Undo();
                }
                else if (doRedo == false && !UndoRedoFramework.State.CanUndo)
                {
                    doRedo = true;
                }
                else if (UndoRedoFramework.State.CanRedo && doRedo)
                {
                    UndoRedoFramework.Redo();
                }
                else if (!UndoRedoFramework.State.CanRedo && doRedo)
                {
                    doRedo = false;
                }
            });
        }

        #endregion
    }

    #endregion
}
