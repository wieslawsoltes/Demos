// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Logic.Controls
{
    #region References

    using Logic.Model;
    using Logic.Model.Core;
    using Logic.Services;
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

    #region Context Canvas

    public class ContextCanvas : Canvas
    {
        #region Fields

        private SelectionService selectionService = null;
        private MouseService mouseService = null;

        #endregion

        #region Constructor

        public ContextCanvas()
            : base()
        {
            this.selectionService = new SelectionService(this);

            this.mouseService = new MouseService();

            this.InitializeEvents();
        }

        #endregion

        #region Options

        private Options GetOptions()
        {
            var context = this.DataContext as Context;
            if (context == null)
                return null;

            var options = Defaults.Options;
            if (options == null)
                return null;

            if (options.Sync == true || options.SimulationIsRunning == true)
                return null;

            if (context.PageType == PageType.Title || context.IsLocked == true)
                return null;

            return options;
        }

        #endregion

        #region Initialize Events

        private void InitializeMouseMove()
        {
            // process mouse move events with Rx
            var mouseMove = Observable.FromEventPattern<MouseEventHandler, MouseEventArgs>(eh => MouseMove += eh, eh => MouseMove -= eh);

            mouseMove.ObserveOnDispatcher().Subscribe(x =>
            {
                var e = x.EventArgs;
                var sender = x.Sender;

                var options = this.GetOptions();
                if (options == null)
                    return;

                #region Selection Adorner

                if (this.IsMouseCaptured && options.CaptureMouse == false)
                {
                    var p = e.GetPosition(this);

                    selectionService.Move(p);
                }

                #endregion

                #region Line & Pin

                else
                {
                    if (options.CaptureMouse == true)
                    {
                        var point = e.GetPosition(sender as FrameworkElement);

                        mouseService.MouseMove(options, point.X, point.Y);
                    }
                }

                #endregion
            });
        }

        private void InitializeEvents()
        {
            InitializeMouseMove();

            // mouse events
            this.PreviewMouseLeftButtonDown += ContextCanvas_PreviewMouseLeftButtonDown;
            this.MouseLeftButtonDown += ContextCanvas_MouseLeftButtonDown;
            this.MouseLeftButtonUp += ContextCanvas_MouseLeftButtonUp;
            this.PreviewMouseRightButtonDown += ContextCanvas_PreviewMouseRightButtonDown;
            this.MouseRightButtonDown += ContextCanvas_MouseRightButtonDown;

            // drag & drop events
            this.DragEnter += ContextCanvas_DragEnter;
            this.Drop += ContextCanvas_Drop;
        }

        #endregion

        #region Mouse Events

        void ContextCanvas_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            #region Options

            var options = this.GetOptions();
            if (options == null)
                return;

            #endregion

            #region Line & Pin

            var context = this.DataContext as Context;
            var source = e.OriginalSource as FrameworkElement;
            var parent = (source.DataContext as Element).Parent;

            if (options.CaptureMouse == false &&
                source.DataContext is Pin &&
                ((parent == null || parent is Context) && Keyboard.Modifiers != ModifierKeys.Control))
            {
                return;
            }

            if (source.DataContext is ISelected && (source.DataContext as ISelected).IsSelected)
            {
                if (source.DataContext is Pin && (parent != null || !(parent is Context)))
                    e.Handled = true;

                return;
            }

            if (options.CaptureMouse == false)
            {
                if (source.DataContext is Pin)
                {
                    mouseService.NotCapturedLeftButtonPin(options, context, source.DataContext as Pin);

                    e.Handled = true;
                }

                // TODO: connect two elements using Ctrl key
                // - left click on first element while holding Ctrl key
                // - left click on second element while holding Ctrl key
                else if (!(source.DataContext is Context) && Keyboard.Modifiers == ModifierKeys.Control)
                {
                    mouseService.NotCapturedLeftButtonAutoConnect(options, context, source.DataContext as Element);

                    e.Handled = true;
                }

                // TODO: left click + Ctrl to create Signal
                else if (source.DataContext is Context && Keyboard.Modifiers == ModifierKeys.Control)
                {
                    if (context.PageType == PageType.Title || context.IsLocked)
                        return;

                    var point = e.GetPosition(sender as FrameworkElement);

                    double x = options.IsSnapEnabled ? SnapToGrid.Snap(point.X, options.Snap, options.OffsetX) : point.X;
                    double y = options.IsSnapEnabled ? SnapToGrid.Snap(point.Y, options.Snap, options.OffsetY) : point.Y;

                    Element element = Factory.CreateElementFromType(context, options.CurrentElement, x, y);

                    if (element != null)
                    {
                        // TODO: undo
                        UndoRedoActions.NewElement(context, element);
                    }
                }
            }
            else
            {
                if (source.DataContext is Pin)
                {
                    var pin = source.DataContext as Pin;
                    if (pin != mouseService.TempPin)
                    {
                        mouseService.CapturedLeftButtonPin(options, context, pin);

                        // handle mouse release
                        mouseService.RelaseMouse(context);

                        e.Handled = true;
                    }
                }
                else if (source.DataContext is Wire)
                {
                    var line = source.DataContext as Wire;
                    var point = e.GetPosition(sender as FrameworkElement);

                    mouseService.CapturedLeftButtonWire(options, context, line, point.X, point.Y);

                    // handle mouse release
                    mouseService.RelaseMouse(context);

                    e.Handled = true;
                }
            }

            #endregion
        }

        void ContextCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            #region Options

            var options = this.GetOptions();
            if (options == null)
                return; 

            #endregion

            #region Selection Adorner

            if (!this.IsMouseCaptured && options.CaptureMouse == false && Keyboard.Modifiers != ModifierKeys.Control)
            {
                selectionService.Capture();
            }

            #endregion
        }

        void ContextCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            #region Options

            var options = this.GetOptions();
            if (options == null)
                return;

            #endregion

            #region Selection Adorner

            if (this.IsMouseCaptured == true)
            {
                this.ReleaseMouseCapture();

                selectionService.Relase();
            }

            #endregion

            #region Line & Pin

            else
            {
                var context = this.DataContext as Context;
                var source = e.OriginalSource as FrameworkElement;
                var point = e.GetPosition(sender as FrameworkElement);

                if (options.CaptureMouse == true)
                {
                    if (source.DataContext is Context)
                    {
                        // handle new line
                        mouseService.CapturedLeftButtonContext(options, context, point.X, point.Y);

                        e.Handled = true;
                    }
                }
            }
            #endregion
        }

        void ContextCanvas_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            #region Options

            var options = this.GetOptions();
            if (options == null)
                return;

            #endregion

            #region Line & Pin

            if (options.CaptureMouse == true)
            {
                // TODO: ignore mouse right click with Control key
                if (Keyboard.Modifiers == ModifierKeys.Control)
                    return;

                var context = this.DataContext as Context;

                // release mouse capture
                options.CaptureMouse = false;

                // handle mouse release
                mouseService.RelaseMouse(context);

                e.Handled = true;
            }

            #endregion
        }

        void ContextCanvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            #region Options

            var options = this.GetOptions();
            if (options == null)
                return;

            #endregion

            #region Selection Adorner

            if (options.CaptureMouse == false)
            {
                selectionService.ReleaseAndClear();
            }

            #endregion

            #region Line & Pin

            else
            {
                // TODO: ignore mouse right click with Control key
                if (Keyboard.Modifiers == ModifierKeys.Control)
                    return;

                var context = this.DataContext as Context;

                // release mouse capture
                options.CaptureMouse = false;

                // handle mouse release
                mouseService.RelaseMouse(context);

            }
            #endregion
        }

        #endregion

        #region Drag Events

        void ContextCanvas_DragEnter(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(Defaults.DataFormat) || sender == e.Source)
            {
                e.Effects = DragDropEffects.None;
            }
        }

        void ContextCanvas_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(Defaults.DataFormat))
            {
                var type = e.Data.GetData(Defaults.DataFormat) as string;

                if (type != null)
                {
                    var canvas = sender as Canvas;
                    if (canvas == null)
                        return;

                    var context = canvas.DataContext as Context;
                    if (context == null)
                        return;

                    if (context.PageType == PageType.Title || context.IsLocked)
                        return;

                    var project = context.Parent as Project;
                    if (project == null)
                        return;

                    // get element position
                    var p = e.GetPosition(canvas);

                    var options = Defaults.Options;

                    double x = options.IsSnapEnabled ? SnapToGrid.Snap(p.X, options.Snap, options.OffsetX) : p.X;
                    double y = options.IsSnapEnabled ? SnapToGrid.Snap(p.Y, options.Snap, options.OffsetY) : p.Y;

                    Element element = Factory.CreateElementFromType(context, type, x, y);

                    if (element != null)
                    {
                        // TODO: undo
                        UndoRedoActions.NewElement(context, element);
                    }
                }
            }
        }

        #endregion
    }

    #endregion
}
