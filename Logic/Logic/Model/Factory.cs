// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Logic.Model
{
    #region References

    using Logic.Model;
    using Logic.Model.Core;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    #endregion

    #region Factory

    public static class Factory
    {
        #region Create Wire

        public static Wire CreateWire(Context context, Pin start, Pin end)
        {
            if (context == null)
                return null;

            var element = new Wire()
            {
                Start = start,
                End = end,
                X = 0,
                Y = 0,
                Z = Defaults.LineZIndex,
                Id = Guid.NewGuid().ToString(),
                IsLocked = true,
                Parent = context,
                SelectChildren = false
            };

            element.Initialize();

            context.Children.Add(element);

            return element;
        }

        #endregion

        #region Create Pin

        public static Pin CreatePin(Context context, 
            double x, 
            double y, 
            double z, 
            bool isLocked, 
            Element parent, 
            string name, 
            string factoryName,
            Alignment alignment,
            PinType type = PinType.Undefined, 
            bool isPinTypeUndefined = true)
        {
            if (context == null || double.IsNaN(x) || double.IsNaN(y))
                return null;

            var element = new Pin()
            {
                Name = name,
                FactoryName = factoryName,
                X = x,
                Y = y,
                Z = z,
                Id = Guid.NewGuid().ToString(),
                IsLocked = isLocked,
                Parent = parent,
                IsEditable = parent == null ? true : false,
                Alignment = alignment,
                Type = type,
                IsPinTypeUndefined = isPinTypeUndefined
            };

            if (parent != null && !(parent is Context))
            {
                parent.Children.Add(element);
            }

            context.Children.Add(element);

            return element;
        }

        #endregion

        #region Create Signal

        public static Signal CreateSignal(Context context, double x, double y, string name = "")
        {
            if (context == null || double.IsNaN(x) || double.IsNaN(y))
                return null;

            var element = new Signal()
            {                
                Name = name,
                Id = Guid.NewGuid().ToString(),
                X = x,
                Y = y,
                Z = Defaults.SignalZIndex,
                IsSelected = false,
                IsLocked = false,
                Tag = null,
                Parent = context
            };

            context.Children.Add(element);

            // left, Input: Children[0]
            CreatePin(context, x, y + 15, Defaults.PinZIndex, false, element, "I", "I", Alignment.Left, PinType.Input, false);

            // right, Output: Children[1]
            CreatePin(context, x + 285, y + 15, Defaults.PinZIndex, false, element, "O", "O", Alignment.Right, PinType.Output, false);

            return element;
        }

        #endregion

        #region Create Gates

        public static AndGate CreateAndGate(Context context, double x, double y, string name = "")
        {
            if (context == null || double.IsNaN(x) || double.IsNaN(y))
                return null;

            var element = new AndGate()
            {
                Name = name,
                Id = Guid.NewGuid().ToString(),
                X = x,
                Y = y,
                Z = Defaults.ElementZIndex,
                IsSelected = false,
                IsLocked = false,
                Parent = context
            };

            context.Children.Add(element);

            // left
            CreatePin(context, x, y + 15, Defaults.PinZIndex, false, element, "L", "L", Alignment.Left);
            // right
            CreatePin(context, x + 30, y + 15, Defaults.PinZIndex, false, element, "R", "R", Alignment.Right);
            // top
            CreatePin(context, x + 15, y, Defaults.PinZIndex, false, element, "T", "T", Alignment.Top);
            // bottom
            CreatePin(context, x + 15, y + 30, Defaults.PinZIndex, false, element, "B", "B", Alignment.Bottom);

            return element;
        }

        public static OrGate CreateOrGate(Context context, double x, double y, string name = "")
        {
            if (context == null || double.IsNaN(x) || double.IsNaN(y))
                return null;

            var element = new OrGate()
            {
                Name = name,
                Id = Guid.NewGuid().ToString(),
                X = x,
                Y = y,
                Z = Defaults.ElementZIndex,
                IsSelected = false,
                IsLocked = false,
                Parent = context
            };

            context.Children.Add(element);

            // left
            CreatePin(context, x, y + 15, Defaults.PinZIndex, false, element, "L", "L", Alignment.Left);
            // right
            CreatePin(context, x + 30, y + 15, Defaults.PinZIndex, false, element, "R", "R", Alignment.Right);
            // top
            CreatePin(context, x + 15, y, Defaults.PinZIndex, false, element, "T", "T", Alignment.Top);
            // bottom
            CreatePin(context, x + 15, y + 30, Defaults.PinZIndex, false, element, "B", "B", Alignment.Bottom);

            return element;
        }

        public static NotGate CreateNotGate(Context context, double x, double y, string name = "")
        {
            if (context == null || double.IsNaN(x) || double.IsNaN(y))
                return null;

            var element = new NotGate()
            {
                Name = name,
                Id = Guid.NewGuid().ToString(),
                X = x,
                Y = y,
                Z = Defaults.ElementZIndex,
                IsSelected = false,
                IsLocked = false,
                Parent = context
            };

            context.Children.Add(element);

            // left
            CreatePin(context, x, y + 15, Defaults.PinZIndex, false, element, "L", "L", Alignment.Left);
            // right
            CreatePin(context, x + 40, y + 15, Defaults.PinZIndex, false, element, "R", "R", Alignment.Right);
            // top
            CreatePin(context, x + 15, y, Defaults.PinZIndex, false, element, "T", "T", Alignment.Top);
            // bottom
            CreatePin(context, x + 15, y + 30, Defaults.PinZIndex, false, element, "B", "B", Alignment.Bottom);

            return element;
        }

        public static BufferGate CreateBufferGate(Context context, double x, double y, string name = "")
        {
            if (context == null || double.IsNaN(x) || double.IsNaN(y))
                return null;

            var element = new BufferGate()
            {
                Name = name,
                Id = Guid.NewGuid().ToString(),
                X = x,
                Y = y,
                Z = Defaults.ElementZIndex,
                IsSelected = false,
                IsLocked = false,
                Parent = context
            };

            context.Children.Add(element);

            // left
            CreatePin(context, x, y + 15, Defaults.PinZIndex, false, element, "L", "L", Alignment.Left);
            // right
            CreatePin(context, x + 30, y + 15, Defaults.PinZIndex, false, element, "R", "R", Alignment.Right);
            // top
            CreatePin(context, x + 15, y, Defaults.PinZIndex, false, element, "T", "T", Alignment.Top);
            // bottom
            CreatePin(context, x + 15, y + 30, Defaults.PinZIndex, false, element, "B", "B", Alignment.Bottom);

            return element;
        }

        public static NandGate CreateNandGate(Context context, double x, double y, string name = "")
        {
            if (context == null || double.IsNaN(x) || double.IsNaN(y))
                return null;

            var element = new NandGate()
            {
                Name = name,
                Id = Guid.NewGuid().ToString(),
                X = x,
                Y = y,
                Z = Defaults.ElementZIndex,
                IsSelected = false,
                IsLocked = false,
                Parent = context
            };

            context.Children.Add(element);

            // left
            CreatePin(context, x, y + 15, Defaults.PinZIndex, false, element, "L", "L", Alignment.Left);
            // right
            CreatePin(context, x + 40, y + 15, Defaults.PinZIndex, false, element, "R", "R", Alignment.Right);
            // top
            CreatePin(context, x + 15, y, Defaults.PinZIndex, false, element, "T", "T", Alignment.Top);
            // bottom
            CreatePin(context, x + 15, y + 30, Defaults.PinZIndex, false, element, "B", "B", Alignment.Bottom);

            return element;
        }

        public static NorGate CreateNorGate(Context context, double x, double y, string name = "")
        {
            if (context == null || double.IsNaN(x) || double.IsNaN(y))
                return null;

            var element = new NorGate()
            {
                Name = name,
                Id = Guid.NewGuid().ToString(),
                X = x,
                Y = y,
                Z = Defaults.ElementZIndex,
                IsSelected = false,
                IsLocked = false,
                Parent = context
            };

            context.Children.Add(element);

            // left
            CreatePin(context, x, y + 15, Defaults.PinZIndex, false, element, "L", "L", Alignment.Left);
            // right
            CreatePin(context, x + 40, y + 15, Defaults.PinZIndex, false, element, "R", "R", Alignment.Right);
            // top
            CreatePin(context, x + 15, y, Defaults.PinZIndex, false, element, "T", "T", Alignment.Top);
            // bottom
            CreatePin(context, x + 15, y + 30, Defaults.PinZIndex, false, element, "B", "B", Alignment.Bottom);

            return element;
        }

        public static XorGate CreateXorGate(Context context, double x, double y, string name = "")
        {
            if (context == null || double.IsNaN(x) || double.IsNaN(y))
                return null;

            var element = new XorGate()
            {
                Name = name,
                Id = Guid.NewGuid().ToString(),
                X = x,
                Y = y,
                Z = Defaults.ElementZIndex,
                IsSelected = false,
                IsLocked = false,
                Parent = context
            };

            context.Children.Add(element);

            // left
            CreatePin(context, x, y + 15, Defaults.PinZIndex, false, element, "L", "L", Alignment.Left);
            // right
            CreatePin(context, x + 30, y + 15, Defaults.PinZIndex, false, element, "R", "R", Alignment.Right);
            // top
            CreatePin(context, x + 15, y, Defaults.PinZIndex, false, element, "T", "T", Alignment.Top);
            // bottom
            CreatePin(context, x + 15, y + 30, Defaults.PinZIndex, false, element, "B", "B", Alignment.Bottom);

            return element;
        }

        public static XnorGate CreateXnorGate(Context context, double x, double y, string name = "")
        {
            if (context == null || double.IsNaN(x) || double.IsNaN(y))
                return null;

            var element = new XnorGate()
            {
                Name = name,
                Id = Guid.NewGuid().ToString(),
                X = x,
                Y = y,
                Z = Defaults.ElementZIndex,
                IsSelected = false,
                IsLocked = false,
                Parent = context
            };

            context.Children.Add(element);

            // left
            CreatePin(context, x, y + 15, Defaults.PinZIndex, false, element, "L", "L", Alignment.Left);
            // right
            CreatePin(context, x + 40, y + 15, Defaults.PinZIndex, false, element, "R", "R", Alignment.Right);
            // top
            CreatePin(context, x + 15, y, Defaults.PinZIndex, false, element, "T", "T", Alignment.Top);
            // bottom
            CreatePin(context, x + 15, y + 30, Defaults.PinZIndex, false, element, "B", "B", Alignment.Bottom);

            return element;
        }

        #endregion

        #region Create Memory

        public static MemorySetPriority CreateMemorySetPriority(Context context, double x, double y, string name = "")
        {
            if (context == null || double.IsNaN(x) || double.IsNaN(y))
                return null;

            var element = new MemorySetPriority()
            {
                Name = name,
                Id = Guid.NewGuid().ToString(),
                X = x,
                Y = y,
                Z = Defaults.ElementZIndex,
                IsSelected = false,
                IsLocked = false,
                Parent = context
            };

            context.Children.Add(element);

            // S
            CreatePin(context, x + 15, y, Defaults.PinZIndex, false, element, "S", "S", Alignment.Top);
            // R
            CreatePin(context, x + 45, y, Defaults.PinZIndex, false, element, "R", "R", Alignment.Top);
            // Q
            CreatePin(context, x + 15, y + 30, Defaults.PinZIndex, false, element, "Q", "Q", Alignment.Bottom);
            // Q'
            CreatePin(context, x + 45, y + 30, Defaults.PinZIndex, false, element, "NQ", "NQ", Alignment.Bottom);

            return element;
        }

        public static MemoryResetPriority CreateMemoryResetPriority(Context context, double x, double y, string name = "")
        {
            if (context == null || double.IsNaN(x) || double.IsNaN(y))
                return null;

            var element = new MemoryResetPriority()
            {
                Name = name,
                Id = Guid.NewGuid().ToString(),
                X = x,
                Y = y,
                Z = Defaults.ElementZIndex,
                IsSelected = false,
                IsLocked = false,
                Parent = context
            };

            context.Children.Add(element);

            // S
            CreatePin(context, x + 15, y, Defaults.PinZIndex, false, element, "S", "S", Alignment.Top);
            // R
            CreatePin(context, x + 45, y, Defaults.PinZIndex, false, element, "R", "R", Alignment.Top);
            // Q
            CreatePin(context, x + 15, y + 30, Defaults.PinZIndex, false, element, "Q", "Q", Alignment.Bottom);
            // Q'
            CreatePin(context, x + 45, y + 30, Defaults.PinZIndex, false, element, "NQ", "NQ", Alignment.Bottom);

            return element;
        }

        #endregion

        #region Create Timers

        public static TimerPulse CreateTimerPulse(Context context, double x, double y, string name = "")
        {
            if (context == null || double.IsNaN(x) || double.IsNaN(y))
                return null;

            var element = new TimerPulse()
            {
                Name = name,
                Id = Guid.NewGuid().ToString(),
                X = x,
                Y = y,
                Z = Defaults.ElementZIndex,
                IsSelected = false,
                IsLocked = false,
                Parent = context
            };

            context.Children.Add(element);

            // left
            CreatePin(context, x, y + 15, Defaults.PinZIndex, false, element, "L", "L", Alignment.Left);
            // right
            CreatePin(context, x + 30, y + 15, Defaults.PinZIndex, false, element, "R", "R", Alignment.Right);
            // top
            CreatePin(context, x + 15, y, Defaults.PinZIndex, false, element, "T", "T", Alignment.Top);
            // bottom
            CreatePin(context, x + 15, y + 30, Defaults.PinZIndex, false, element, "B", "B", Alignment.Bottom);

            return element;
        }

        public static TimerOn CreateTimerOn(Context context, double x, double y, string name = "")
        {
            if (context == null || double.IsNaN(x) || double.IsNaN(y))
                return null;

            var element = new TimerOn()
            {
                Name = name,
                Id = Guid.NewGuid().ToString(),
                X = x,
                Y = y,
                Z = Defaults.ElementZIndex,
                IsSelected = false,
                IsLocked = false,
                Parent = context
            };

            context.Children.Add(element);

            // left
            CreatePin(context, x, y + 15, Defaults.PinZIndex, false, element, "L", "L", Alignment.Left);
            // right
            CreatePin(context, x + 30, y + 15, Defaults.PinZIndex, false, element, "R", "R", Alignment.Right);
            // top
            CreatePin(context, x + 15, y, Defaults.PinZIndex, false, element, "T", "T", Alignment.Top);
            // bottom
            CreatePin(context, x + 15, y + 30, Defaults.PinZIndex, false, element, "B", "B", Alignment.Bottom);

            return element;
        }

        public static TimerOff CreateTimerOff(Context context, double x, double y, string name = "")
        {
            if (context == null || double.IsNaN(x) || double.IsNaN(y))
                return null;

            var element = new TimerOff()
            {
                Name = name,
                Id = Guid.NewGuid().ToString(),
                X = x,
                Y = y,
                Z = Defaults.ElementZIndex,
                IsSelected = false,
                IsLocked = false,
                Parent = context
            };

            context.Children.Add(element);

            // left
            CreatePin(context, x, y + 15, Defaults.PinZIndex, false, element, "L", "L", Alignment.Left);
            // right
            CreatePin(context, x + 30, y + 15, Defaults.PinZIndex, false, element, "R", "R", Alignment.Right);
            // top
            CreatePin(context, x + 15, y, Defaults.PinZIndex, false, element, "T", "T", Alignment.Top);
            // bottom
            CreatePin(context, x + 15, y + 30, Defaults.PinZIndex, false, element, "B", "B", Alignment.Bottom);

            return element;
        }

        #endregion

        #region Create From Type

        public static Element CreateElementFromType(Context context, string type, double x, double y)
        {
            if (context == null || type == string.Empty)
                return null;

            Element element = null;

            switch (type)
            {
                case "Signal":
                    element = CreateSignal(context, x, y);
                    break;
                case "AndGate":
                    element = CreateAndGate(context, x, y);
                    break;
                case "OrGate":
                    element = CreateOrGate(context, x, y);
                    break;
                case "NotGate":
                    element = CreateNotGate(context, x, y);
                    break;
                case "BufferGate":
                    element = CreateBufferGate(context, x, y);
                    break;
                case "NandGate":
                    element = CreateNandGate(context, x, y);
                    break;
                case "NorGate":
                    element = CreateNorGate(context, x, y);
                    break;
                case "XorGate":
                    element = CreateXorGate(context, x, y);
                    break;
                case "XnorGate":
                    element = CreateXnorGate(context, x, y);
                    break;
                case "MemorySetPriority":
                    element = CreateMemorySetPriority(context, x, y);
                    break;
                case "MemoryResetPriority":
                    element = CreateMemoryResetPriority(context, x, y);
                    break;
                case "TimerPulse":
                    element = CreateTimerPulse(context, x, y);
                    break;
                case "TimerOn":
                    element = CreateTimerOn(context, x, y);
                    break;
                case "TimerOff":
                    element = CreateTimerOff(context, x, y);
                    break;
                default:
                    throw new ArgumentException();
            };

            return element;
        }

        #endregion
    }

    #endregion
}
