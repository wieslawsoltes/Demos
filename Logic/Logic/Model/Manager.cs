// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Logic.Model
{
    #region References

    using Logic.Model;
    using Logic.Model.Core;
    using Logic.Utilities;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    #endregion

    #region Manager

    public static class Manager
    {
        #region Delete Elements

        public static Wire DeletePin(Context context, Pin pin)
        {
            if (context == null)
                throw new ArgumentNullException();

            if (pin == null)
                throw new ArgumentNullException();

            // find connected wires to pin
            var wires = context.Children.Where(x => x is Wire).Cast<Wire>();
            var connectedWires = wires.Where(w => w.Start == pin || w.End == pin);

            // removed connected wire
            Wire connectedWire = null;

            if (connectedWires.Count() == 1)
            {
                var first = connectedWires.First();

                // remove second pin from 'first' wire if it's not connected to other wires
                if (first.Start == pin)
                {
                    var endPin = first.End;

                    var wiresConnectedToEndPin = wires.Where(w => w.Start == endPin || w.End == endPin);

                    if (wiresConnectedToEndPin.Count() == 1 && (endPin.Parent == null || endPin.Parent is Context))
                        context.Children.Remove(endPin);
                }
                else
                {
                    var startPin = first.Start;

                    var wiresConnectedToStartPin = wires.Where(w => w.Start == startPin || w.End == startPin);

                    if (wiresConnectedToStartPin.Count() == 1 && (startPin.Parent == null || startPin.Parent is Context))
                        context.Children.Remove(startPin);
                }

                // remove pin
                if (pin.Parent == null || pin.Parent is Context)
                    context.Children.Remove(pin);

                connectedWire = first;

                // remove connected wires
                context.Children.Remove(first);
            }
            else if (connectedWires.Count() == 2)
            {
                var first = connectedWires.First();
                var last = connectedWires.Last();

                if (first.Start == pin && last.Start == pin)
                {
                    first.Start = last.End;
                }
                else if (first.End == pin && last.End == pin)
                {
                    first.End = last.Start;
                }
                else if (first.Start == pin && last.End == pin)
                {
                    first.Start = last.Start;
                }
                else if (first.End == pin && last.Start == pin)
                {
                    first.End = last.End;
                }

                if (pin.Parent == null || pin.Parent is Context)
                    context.Children.Remove(pin);

                connectedWire = last;

                context.Children.Remove(last);
            }

            return connectedWire;
        }

        public static void DeleteWire(Context context, Wire wire)
        {
            if (context == null)
                throw new ArgumentNullException();

            if (wire == null)
                throw new ArgumentNullException();

            context.Children.Remove(wire);
        }

        public static void DeleteElement(Context context, Element element)
        {
            if (context == null || element == null)
                throw new ArgumentNullException();

            if (element is Pin && (element.Parent == null || element.Parent is Context))
            {
                Manager.DeletePin(context, element as Pin);
            }
            else if (element is Model.Wire)
            {
                Manager.DeleteWire(context, element as Model.Wire);
            }
            else
            {
                RemoveGenericElement(context, element);
            }
        }

        private static IEnumerable<Pin> GetPinElements(Context context, IEnumerable<Element> elements)
        {
            if (context == null || elements == null)
                throw new ArgumentNullException();

            var pins = elements.Where(x => x is Pin && context.Children.Contains(x) && (x.Parent == null || x.Parent is Context))
                               .Cast<Pin>();

            return pins;
        }

        public static void DeleteElements(Context context, IEnumerable<Element> elements)
        {
            if (context == null || elements == null)
                throw new ArgumentNullException();

            // first pass (some pins may not be removed)
            foreach (var element in elements)
                DeleteElement(context, element);

            // second pass (remove pins)
            var pins = GetPinElements(context, elements);

            foreach (var pin in pins)
                context.Children.Remove(pin);
        }

        #endregion

        #region Remove Elements

        public static IEnumerable<Element> RemoveGenericElement(Context context, Element element)
        {
            if (context == null || element == null)
                throw new ArgumentNullException();

            var list = new List<Element>();

            list.Add(element);

            if (element.Children != null && element.Children.Count > 0)
            {
                list.AddRange(element.Children);

                foreach (var child in element.Children)
                    context.Children.Remove(child);
            }

            context.Children.Remove(element);

            return list;
        }

        public static IEnumerable<Element> RemoveElements(Context context, IEnumerable<Element> elements)
        {
            if (context == null || elements == null)
                throw new ArgumentNullException();

            var list = new List<Element>();

            foreach (var element in elements)
            {
                if (element is Pin && (element.Parent == null || element.Parent is Context))
                {
                    context.Children.Remove(element);

                    list.Add(element);
                }
                else if (element is Model.Wire)
                {
                    context.Children.Remove(element);

                    list.Add(element);
                }
                else
                {
                    var removed = RemoveGenericElement(context, element);

                    list.AddRange(removed);
                }
            }

            return list;
        }

        #endregion

        #region Cut/Copy/Paste Elements

        public static IEnumerable<Element> PasteElements(Context context, IEnumerable<Element> elements)
        {
            if (context == null || elements == null)
                throw new ArgumentNullException();

            // list of cloned elements
            var list = new List<Element>();

            // make shallow copy of all elements
            Dictionary<Element, Element> clones = CreateShallowCopyOfElements(elements);

            // make deep copy of elements that are not pins or lines
            var otherClones = clones.Where(x => !(x.Key is Model.Pin) && !(x.Key is Model.Wire));

            CreateDeepCopyOfOtherElements(context, list, otherClones);

            // make deep copy of pin elements
            var pinClones = clones.Where(x => x.Key is Model.Pin);

            CreateDeepCopyOfPins(context, list, clones, pinClones);

            // make deep copy of lines
            var lineClones = clones.Where(x => x.Key is Model.Wire);

            CreateDeepCopyOfLines(context, list, clones, lineClones);

            // insert cloned elements to context
            AddClonedElementsToContext(context, list);

            return list;
        }

        private static Dictionary<Element, Element> CreateShallowCopyOfElements(IEnumerable<Element> elements)
        {
            if (elements == null)
                throw new ArgumentNullException();

            var clones = new Dictionary<Element, Element>();

            foreach (var element in elements)
            {
                var clone = (Element)element.Clone();

                clone.Children = new ObservableCollection<Element>();

                if (clones.ContainsKey(element) == false)
                    clones.Add(element, clone);
            }

            return clones;
        }

        private static void CreateDeepCopyOfOtherElements(Context context,
                                                          List<Element> list,
                                                          IEnumerable<KeyValuePair<Element, Element>> otherClones)
        {
            if (context == null || list == null || otherClones == null)
                throw new ArgumentNullException();

            foreach (var clone in otherClones)
            {
                var original = clone.Key;
                var copy = clone.Value;

                copy.Id = Guid.NewGuid().ToString();
                copy.IsSelected = false;

                copy.Parent = context;

                list.Add(copy);
            }
        }

        private static void CreateDeepCopyOfPins(Context context,
                                                 List<Element> list,
                                                 Dictionary<Element, Element> clones,
                                                 IEnumerable<KeyValuePair<Element, Element>> pinClones)
        {
            if (context == null || list == null || clones == null || pinClones == null)
                throw new ArgumentNullException();

            foreach (var clone in pinClones)
            {
                var pinOriginal = clone.Key as Model.Pin;
                var pinCopy = clone.Value as Model.Pin;

                if (pinOriginal.Parent != null)
                {
                    if (pinOriginal.Parent is Context)
                    {
                        pinCopy.Parent = context;
                    }
                    else
                    {
                        if (clones.ContainsKey(pinOriginal.Parent))
                        {
                            pinCopy.Parent = clones[pinOriginal.Parent];
                            pinCopy.Parent.Children.Add(pinCopy);
                        }
                        else
                        {
                            pinCopy.Parent = null;
                        }
                    }
                }

                pinCopy.Id = Guid.NewGuid().ToString();
                pinCopy.IsSelected = false;

                list.Add(pinCopy);
            }
        }

        private static void CreateDeepCopyOfLines(Context context,
                                                  List<Element> list,
                                                  Dictionary<Element, Element> clones,
                                                  IEnumerable<KeyValuePair<Element, Element>> lineClones)
        {
            if (context == null || list == null || clones == null || lineClones == null)
                throw new ArgumentNullException();

            foreach (var clone in lineClones)
            {
                var lineOriginal = clone.Key as Model.Wire;
                var lineCopy = clone.Value as Model.Wire;

                if (lineOriginal.Start != null && clones.ContainsKey((Element)lineOriginal.Start))
                {
                    lineCopy.Start = (Pin)clones[(Element)lineOriginal.Start];
                }
                else
                {
                    // create Start pin if missing
                    if (lineCopy.Start != null)
                    {
                        var pin = Factory.CreatePin(context,
                            lineOriginal.Start.X,
                            lineOriginal.Start.Y,
                            Defaults.PinZIndex,
                            true,
                            context, 
                            string.Empty, 
                            string.Empty,
                            Alignment.Undefined);

                        pin.IsLocked = false;

                        lineCopy.Start = pin;
                    }
                    else
                    {
                        throw new NullReferenceException();
                    }
                }

                if (lineOriginal.End != null && clones.ContainsKey((Element)lineOriginal.End))
                {
                    lineCopy.End = (Pin)clones[(Element)lineOriginal.End];
                }
                else
                {
                    // create End pin if missing
                    if (lineCopy.End != null)
                    {
                        var pin = Factory.CreatePin(context,
                            lineOriginal.End.X,
                            lineOriginal.End.Y,
                            Defaults.PinZIndex,
                            true,
                            context,
                            string.Empty,
                            string.Empty,
                            Alignment.Undefined);

                        pin.IsLocked = false;

                        lineCopy.End = pin;
                    }
                    else
                    {
                        throw new NullReferenceException();
                    }
                }

                lineCopy.Id = Guid.NewGuid().ToString();
                lineCopy.IsSelected = false;

                lineCopy.Parent = context;

                list.Add(lineCopy);
            }
        }

        private static void AddClonedElementsToContext(Context context, IEnumerable<Element> elements)
        {
            if (context == null || elements == null)
                throw new ArgumentNullException();

            // add all elements to context
            foreach (var element in elements)
                context.Children.Add(element);

            // initialize lines
            var lines = elements.Where(x => x is Wire).Select(x => x as Wire);
            if (lines != null)
            {
                foreach (var line in lines)
                    line.Initialize();
            }
        }

        #endregion

        #region Select/Deselect Elements

        public static void SelectElements(IEnumerable<Element> elements, bool isSelected)
        {
            if (elements == null)
                return;

            foreach (var element in elements)
            {
                if (element.SelectChildren == true && element.Children.Count > 0)
                {
                    foreach (var child in element.Children)
                        child.IsSelected = isSelected;
                }

                element.IsSelected = isSelected;
            }
        }

        public static void SelectContextElements(Context context, bool isSelected)
        {
            if (context == null)
                throw new ArgumentNullException();

            if (context.SelectedElements != null)
            {
                context.SelectedElements.Clear();
                context.SelectedElements = null;
            }

            if (isSelected == true || context.SelectedElements == null)
                context.SelectedElements = new ObservableCollection<Element>();

            foreach (var element in context.Children)
            {
                if (element.SelectChildren == true && element.Children.Count > 0)
                {
                    foreach (var child in element.Children)
                    {
                        child.IsSelected = isSelected;
                    }
                }

                element.IsSelected = isSelected;

                if (isSelected == true)
                    context.SelectedElements.Add(element);
            }
        }

        public static void ClearSelectedElements(Context context)
        {
            if (context == null)
                throw new ArgumentNullException();

            if (context.SelectedElements == null)
                return;

            var q = context.SelectedElements.Where(x => x.IsSelected == true && (x.Parent == null || x.Parent is Context));

            foreach (var element in q)
            {
                foreach (var child in element.Children)
                {
                    child.IsSelected = false;
                }

                element.IsSelected = false;
            }

            context.SelectedElements = null;
        }

        public static void UpdateSelectedElements(Context context, IEnumerable<Element> elements)
        {
            if (context == null)
                return;

            if (elements == null)
                return;

            if (context.SelectedElements == null)
                context.SelectedElements = new ObservableCollection<Element>();

            var notSelectedElements = elements.Where(x => !context.SelectedElements.Contains(x));

            foreach (var element in notSelectedElements)
            {
                context.SelectedElements.Add(element);

                if ((element.Parent == null || element.Parent is Context) &&
                    !(element is Pin) &&
                    element.SelectChildren == true &&
                    element.Children.Count > 0)
                {
                    foreach (var child in element.Children)
                        context.SelectedElements.Add(child);
                }
            }
        }

        #endregion

        #region Manipulate Elements

        public static void MoveElement(Context context, Element element, double dX, double dY)
        {
            if (context == null || element == null)
                throw new ArgumentNullException();

            var options = Defaults.Options;
            if (options == null)
                throw new Exception();

            var offsetX = options.OffsetX;
            var offsetY = options.OffsetY;
            var snap = options.Snap;
            var enableSnap = options.IsSnapEnabled;

            if (element.Parent == null || element.Parent is Context)
            {
                var x = enableSnap ? SnapToGrid.Snap(element.X + dX, snap, offsetX) : element.X + dX;
                var y = enableSnap ? SnapToGrid.Snap(element.Y + dY, snap, offsetY) : element.Y + dY;
                var deltaX = element.X - x;
                var deltaY = element.Y - y;

                // move element or its parent
                element.X = x;
                element.Y = y;

                //System.Diagnostics.Debug.Print("element.Id {0}", element.Id);

                // move all children of element
                foreach (var child in element.Children)
                {
                    if (child != element)
                    {
                        //System.Diagnostics.Debug.Print("child.Id {0}", child.Id);

                        child.X = child.X - deltaX;
                        child.Y = child.Y - deltaY;
                    }
                }
            }
            else
            {
                var x = enableSnap ? SnapToGrid.Snap(element.Parent.X + dX, snap, offsetX) : element.Parent.X + dX;
                var y = enableSnap ? SnapToGrid.Snap(element.Parent.Y + dY, snap, offsetY) : element.Parent.Y + dY;
                var deltaX = element.Parent.X - x;
                var deltaY = element.Parent.Y - y;

                // move element parent
                element.Parent.X = x;
                element.Parent.Y = y;

                //System.Diagnostics.Debug.Print("element.Parent.Id {0}", element.Parent.Id);

                // move all children of element Parent
                foreach (var child in element.Parent.Children)
                {
                    if (child != element.Parent)
                    {
                        //System.Diagnostics.Debug.Print("child.Id {0}", child.Id);

                        child.X = child.X - deltaX;
                        child.Y = child.Y - deltaY;
                    }
                }
            }
        }

        public static void MoveElements(Context context, IEnumerable<Element> elements, double dX, double dY)
        {
            if (context == null || elements == null)
                throw new ArgumentNullException();

            var q = elements.Where(x => (x.Parent == null || x.Parent is Context) && !(x is Model.Wire));

            foreach (var element in q)
            {
                Manager.MoveElement(context, element, dX, dY);
            }
        }

        public static void MoveSelectedElements(Context context, double dX, double dY)
        {
            if (context == null)
                throw new ArgumentNullException();

            var q = context.SelectedElements.Where(x => (x.Parent == null || x.Parent is Context) && !(x is Model.Wire));

            foreach (var element in q)
            {
                Manager.MoveElement(context, element, dX, dY);
            }
        }

        public static Wire SplitWire(Context context, Wire wire, Pin pin, double x, double y)
        {
            if (context == null || wire == null)
                throw new ArgumentNullException();

            var options = Defaults.Options;

            var newPin = (pin == null) ?
                Factory.CreatePin(context,
                options.IsSnapEnabled ? SnapToGrid.Snap(x, options.Snap, options.OffsetX) : x,
                options.IsSnapEnabled ? SnapToGrid.Snap(y, options.Snap, options.OffsetY) : y,
                Defaults.PinZIndex,
                true,
                context,
                string.Empty,
                string.Empty,
                Alignment.Undefined) : pin;

            newPin.IsLocked = false;

            var newWire = Factory.CreateWire(context, newPin, wire.End);

            newWire.IsLocked = false;

            wire.End = newPin;

            return newWire;
        }

        #endregion
    }

    #endregion
}
