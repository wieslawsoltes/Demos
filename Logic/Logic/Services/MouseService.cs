// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Logic.Services
{
    #region References

    using Logic.Model;
    using Logic.Model.Core;
    using Logic.Utilities;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks; 

    #endregion

    #region MouseService

    public class MouseService
    {
        #region Fields

        public Pin FirstPin = null;
        public Pin PreviousPin = null;
        public Pin TempPin = null;
        public int PinCount = 0;
        public Wire PreviousWire = null;
        public Element SrcElement = null;
        public Element DstElement = null; 

        #endregion

        #region Methods

        public void RelaseMouse(Context context)
        {
            System.Diagnostics.Debug.Print("RelaseMouse");

            context.Children.Remove(this.PreviousWire);
            context.Children.Remove(this.TempPin);

            if (this.PreviousPin != null)
            {
                this.PreviousPin.Z = Defaults.PinZIndex;
            }

            if (this.PinCount == 2)
            {
                context.Children.Remove(this.FirstPin);
            }

            // reset mouse capture context
            this.FirstPin = null;
            this.PinCount = 0;
            this.PreviousPin = null;
            this.PreviousWire = null;
            this.TempPin = null;

            // reset elements
            this.SrcElement = null;
            this.DstElement = null;
        }

        public void MouseMove(Options options, double x, double y)
        {
            if (this.TempPin != null && (SrcElement == null && DstElement == null))
            {
                this.TempPin.X = options.IsSnapEnabled ?
                    SnapToGrid.Snap(x, options.Snap, options.OffsetX) : x;

                this.TempPin.Y = options.IsSnapEnabled ?
                    SnapToGrid.Snap(y, options.Snap, options.OffsetY) : y;
            }
        }

        public void NotCapturedLeftButtonPin(Options options, Context context, Pin pin)
        {
            System.Diagnostics.Debug.Print("NotCapturedLeftButtonPin, pin: {0}", pin.Alignment);

            this.FirstPin = null;

            this.TempPin = Factory.CreatePin(context, pin.X, pin.Y, Defaults.NewPinZIndex, true, context, string.Empty, string.Empty, Alignment.Undefined);
            PinCount++;

            this.PreviousWire = Factory.CreateWire(context, pin, this.TempPin);

            this.PreviousPin = TempPin;

            // capture mouse
            options.CaptureMouse = true;
        }

        public void NotCapturedLeftButtonAutoConnect(Options options, Context context, Element element)
        {
            System.Diagnostics.Debug.Print("NotCapturedLeftButtonAutoConnect");

            if (SrcElement == null && DstElement == null)
            {
                SrcElement = element;

                System.Diagnostics.Debug.Print("srcElement: {0}, position: {1},{2}", SrcElement.GetType(), SrcElement.X, SrcElement.Y);
            }
            else if (SrcElement != null && DstElement == null)
            {
                DstElement = element;

                System.Diagnostics.Debug.Print("dstElement: {0}, position: {1},{2}", DstElement.GetType(), DstElement.X, DstElement.Y);
            }

            if (SrcElement != null && DstElement != null)
            {
                var srcPins = SrcElement.Children.Where(x => x is Pin).Cast<Pin>();
                var dstPins = DstElement.Children.Where(x => x is Pin).Cast<Pin>();

                // RIGHT - LEFT

                if (SrcElement.X < DstElement.X && SrcElement.Y == DstElement.Y)
                {
                    var p1 = srcPins.Where(x => x.Alignment == Alignment.Right).FirstOrDefault();
                    var p2 = dstPins.Where(x => x.Alignment == Alignment.Left).FirstOrDefault();

                    if (p1 != null && p2 != null)
                    {
                        var wire = Factory.CreateWire(context, p1, p2);

                        // TODO: undo
                        UndoRedoActions.NewWire(context, wire);
                    }
                }

                // LEFT - RIGHT

                else if (SrcElement.X > DstElement.X && SrcElement.Y == DstElement.Y)
                {
                    var p1 = dstPins.Where(x => x.Alignment == Alignment.Right).FirstOrDefault();
                    var p2 = srcPins.Where(x => x.Alignment == Alignment.Left).FirstOrDefault();

                    if (p1 != null && p2 != null)
                    {
                        var wire = Factory.CreateWire(context, p1, p2);

                        // TODO: undo
                        UndoRedoActions.NewWire(context, wire);
                    }
                }

                // BOTTOM - TOP

                else if (SrcElement.X == DstElement.X && SrcElement.Y < DstElement.Y)
                {
                    var p1 = srcPins.Where(x => x.Alignment == Alignment.Bottom).FirstOrDefault();
                    var p2 = dstPins.Where(x => x.Alignment == Alignment.Top).FirstOrDefault();

                    if (p1 != null && p2 != null)
                    {
                        var wire = Factory.CreateWire(context, p1, p2);

                        // TODO: undo
                        UndoRedoActions.NewWire(context, wire);
                    }
                }

                // TOP - BOTTOM

                else if (SrcElement.X == DstElement.X && SrcElement.Y > DstElement.Y)
                {
                    var p1 = dstPins.Where(x => x.Alignment == Alignment.Bottom).FirstOrDefault();
                    var p2 = srcPins.Where(x => x.Alignment == Alignment.Top).FirstOrDefault();

                    if (p1 != null && p2 != null)
                    {
                        var wire = Factory.CreateWire(context, p1, p2);

                        // TODO: undo
                        UndoRedoActions.NewWire(context, wire);
                    }
                }

                // RIGHT - TOP

                else if (SrcElement.X < DstElement.X && SrcElement.Y < DstElement.Y)
                {
                    var p1 = srcPins.Where(x => x.Alignment == Alignment.Right).FirstOrDefault();
                    var p2 = dstPins.Where(x => x.Alignment == Alignment.Top).FirstOrDefault();

                    if (p1 != null && p2 != null)
                    {
                        this.NotCapturedLeftButtonPin(options, context, p1);

                        this.CapturedLeftButtonContext(options, context, p2.X, p1.Y);

                        this.CapturedLeftButtonPin(options, context, p2);

                        this.RelaseMouse(context);
                    }
                }

                // TOP - RIGHT

                else if (SrcElement.X > DstElement.X && SrcElement.Y > DstElement.Y)
                {
                    var p1 = dstPins.Where(x => x.Alignment == Alignment.Right).FirstOrDefault();
                    var p2 = srcPins.Where(x => x.Alignment == Alignment.Top).FirstOrDefault();

                    if (p1 != null && p2 != null)
                    {
                        this.NotCapturedLeftButtonPin(options, context, p1);

                        this.CapturedLeftButtonContext(options, context, p2.X, p1.Y);

                        this.CapturedLeftButtonPin(options, context, p2);

                        this.RelaseMouse(context);
                    }
                }

                // BOTTOM - RIGHT

                else if (SrcElement.X > DstElement.X && SrcElement.Y < DstElement.Y)
                {
                    var p1 = dstPins.Where(x => x.Alignment == Alignment.Right).FirstOrDefault();
                    var p2 = srcPins.Where(x => x.Alignment == Alignment.Bottom).FirstOrDefault();

                    if (p1 != null && p2 != null)
                    {
                        this.NotCapturedLeftButtonPin(options, context, p1);

                        this.CapturedLeftButtonContext(options, context, p2.X, p1.Y);

                        this.CapturedLeftButtonPin(options, context, p2);

                        this.RelaseMouse(context);
                    }
                }

                // RIGHT - BOTTOM

                else if (SrcElement.X < DstElement.X && SrcElement.Y > DstElement.Y)
                {
                    var p1 = srcPins.Where(x => x.Alignment == Alignment.Right).FirstOrDefault();
                    var p2 = dstPins.Where(x => x.Alignment == Alignment.Bottom).FirstOrDefault();

                    if (p1 != null && p2 != null)
                    {
                        this.NotCapturedLeftButtonPin(options, context, p1);

                        this.CapturedLeftButtonContext(options, context, p2.X, p1.Y);

                        this.CapturedLeftButtonPin(options, context, p2);

                        this.RelaseMouse(context);
                    }
                }

                // reset elements after connecting
                SrcElement = null;
                DstElement = null;
            }
        }

        public void CapturedLeftButtonPin(Options options, Context context, Pin pin)
        {
            System.Diagnostics.Debug.Print("CapturedLeftButtonPin, pin: {0}", pin.Alignment);

            // connect to existing pin
            this.PreviousWire.End = pin;
            this.PreviousWire.IsLocked = false;

            // TODO: undo
            UndoRedoActions.NewWire(context, PreviousWire);

            // reset temp variables
            this.FirstPin = null;
            this.PreviousWire = null;

            // release mouse capture
            options.CaptureMouse = false;
        }

        public void CapturedLeftButtonWire(Options options, Context context, Wire line, double x, double y)
        {
            System.Diagnostics.Debug.Print("CapturedLeftButtonWire, point: {0},{1}", x, y);

            // TODO: undo
            Pin lineEndPin = line.End;

            // split line
            Wire splitLine = Manager.SplitWire(context, line, this.TempPin, x, y);

            // TODO: undo
            UndoRedoActions.NewWire(context, line, this.TempPin, lineEndPin, splitLine);

            // connect to existing pin (added after line split)
            this.PreviousWire.End = line.End;
            this.PreviousWire.IsLocked = false;
            this.FirstPin = null;
            this.PreviousWire = null;
            this.TempPin = null;

            // release mouse capture
            options.CaptureMouse = false;
        }

        public void CapturedLeftButtonContext(Options options, Context context, double x, double y)
        {
            System.Diagnostics.Debug.Print("CapturedLeftButtonContext, point: {0},{1}", x, y);

            // create new pin
            var pin = Factory.CreatePin(context,
                options.IsSnapEnabled ? SnapToGrid.Snap(x, options.Snap, options.OffsetX) : x,
                options.IsSnapEnabled ? SnapToGrid.Snap(y, options.Snap, options.OffsetY) : y,
                Defaults.NewPinZIndex,
                true,
                context,
                string.Empty,
                string.Empty,
                Alignment.Undefined);

            this.PinCount++;

            if (this.FirstPin == null)
            {
                this.FirstPin = pin;
                pin.IsLocked = false;
            }

            if (this.PreviousWire != null)
            {
                this.PreviousWire.IsLocked = false;
            }

            // create new line
            if (this.PreviousPin != null)
            {
                this.PreviousPin.IsLocked = false;

                // update previous pin position
                this.PreviousPin.X = pin.X;
                this.PreviousPin.Y = pin.Y;
                this.PreviousPin.Z = Defaults.PinZIndex;

                this.PreviousWire = Factory.CreateWire(context, this.PreviousPin, pin);

                // TODO: undo
                UndoRedoActions.NewWire(context, this.PreviousWire, this.PreviousPin);

                // set previous and temp pin reference
                this.PreviousPin = pin;
                this.TempPin = pin;
            }
            else
            {
                this.TempPin = Factory.CreatePin(context,
                    options.IsSnapEnabled ? SnapToGrid.Snap(x, options.Snap, options.OffsetX) : x,
                    options.IsSnapEnabled ? SnapToGrid.Snap(y, options.Snap, options.OffsetY) : y,
                    Defaults.NewPinZIndex,
                    true,
                    context,
                    string.Empty,
                    string.Empty,
                    Alignment.Undefined);

                this.PinCount++;

                pin.Z = Defaults.PinZIndex;

                this.PreviousWire = Factory.CreateWire(context, pin, this.TempPin);

                this.PreviousPin = this.TempPin;
            }

            this.TempPin.IsLocked = true;
        } 

        #endregion
    } 

    #endregion
}
