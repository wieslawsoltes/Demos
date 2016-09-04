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
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    #endregion

    #region UndoRedoActions

    public static class UndoRedoActions
    {
        #region Remove Elements

        public static void RemoveElements(Context context, IEnumerable<Element> elements)
        {
            if (context == null || elements == null)
                throw new ArgumentNullException();

            System.Diagnostics.Debug.Print("UndoRedo.RemoveElements(Context context, IEnumerable<Element> elements)");

            Action undoAction = () =>
            {
                //System.Diagnostics.Debug.Print("---");
                //System.Diagnostics.Debug.Print("UNDO context {0} | elements.Count {0}", context.Name, elements.Count());

                // clear currently selected elements in target context
                Manager.ClearSelectedElements(context);

                // add all elements to context
                foreach (var element in elements)
                    context.Children.Add(element);

                // select pasted elements
                Manager.SelectElements(elements, true);
                Manager.UpdateSelectedElements(context, elements);
            };

            Action redoAction = () =>
            {
                //System.Diagnostics.Debug.Print("---");
                //System.Diagnostics.Debug.Print("REDO context {0} | elements.Count {0}", context.Name, elements.Count());

                // remove all elements from context
                foreach (var element in elements)
                    context.Children.Remove(element);
            };

            var action = new UndoRedoAction(undoAction, redoAction, "Remove Elements");

            UndoRedoFramework.Add(action);
        }

        #endregion

        #region Cut/Paste Elements

        public static void CutElements(Context context, IEnumerable<Element> elements)
        {
            if (context == null || elements == null)
                throw new ArgumentNullException();

            System.Diagnostics.Debug.Print("UndoRedo.CutElements(Context context, IEnumerable<Element> elements)");

            Action undoAction = () =>
            {
                //System.Diagnostics.Debug.Print("---");
                //System.Diagnostics.Debug.Print("UNDO context {0} | elements.Count {0}", context.Name, elements.Count());

                // clear currently selected elements in target context
                Manager.ClearSelectedElements(context);

                // add all elements to context
                foreach (var element in elements)
                    context.Children.Add(element);

                // select pasted elements
                Manager.SelectElements(elements, true);
                Manager.UpdateSelectedElements(context, elements);
            };

            Action redoAction = () =>
            {
                //System.Diagnostics.Debug.Print("---");
                //System.Diagnostics.Debug.Print("REDO context {0} | elements.Count {0}", context.Name, elements.Count());

                // remove all elements from context
                foreach (var element in elements)
                    context.Children.Remove(element);
            };

            var action = new UndoRedoAction(undoAction, redoAction, "Cut Elements");

            UndoRedoFramework.Add(action);
        }

        public static void PasteElements(Context context, IEnumerable<Element> elements)
        {
            if (context == null || elements == null)
                throw new ArgumentNullException();

            System.Diagnostics.Debug.Print("UndoRedo.PasteElements(Context context, IEnumerable<Element> elements)");

            Action undoAction = () =>
            {
                //System.Diagnostics.Debug.Print("---");
                //System.Diagnostics.Debug.Print("UNDO context {0} | elements.Count {0}", context.Name, elements.Count());

                // remove all elements from context
                foreach (var element in elements)
                    context.Children.Remove(element);
            };

            Action redoAction = () =>
            {
                //System.Diagnostics.Debug.Print("---");
                //System.Diagnostics.Debug.Print("REDO context {0} | elements.Count {0}", context.Name, elements.Count());

                // clear currently selected elements in target context
                Manager.ClearSelectedElements(context);

                // add all elements to context
                foreach (var element in elements)
                    context.Children.Add(element);

                // select pasted elements
                Manager.SelectElements(elements, true);
                Manager.UpdateSelectedElements(context, elements);
            };

            var action = new UndoRedoAction(undoAction, redoAction, "Paste Elements");

            UndoRedoFramework.Add(action);
        }

        #endregion

        #region New Element

        public static void NewElement(Context context, Element element)
        {
            if (context == null || element == null)
                throw new ArgumentNullException();

            System.Diagnostics.Debug.Print("UndoRedo.NewElement(Context context, Element element)");

            List<Element> children = null;

            Action undoAction = () =>
            {
                //System.Diagnostics.Debug.Print("---");
                //System.Diagnostics.Debug.Print("UNDO element {0} | {1},{2}", element.Id, element.X, element.Y);
                //foreach (var child in element.Children)
                //{
                //    System.Diagnostics.Debug.Print("UNDO child {0} parent {1} | {2},{3}", 
                //        child.Id, child.Parent.Id,
                //        child.X, child.Y);
                //}

                children = new List<Element>(element.Children);

                Manager.DeleteElement(context, element);
            };

            Action redoAction = () =>
            {
                context.Children.Add(element);

                //System.Diagnostics.Debug.Print("---");
                //System.Diagnostics.Debug.Print("REDO element {0} | {1},{2}", element.Id, element.X, element.Y);

                foreach (var child in children)
                {
                    context.Children.Add(child);

                    //System.Diagnostics.Debug.Print("REDO child {0} parent {1} | {2},{3}", 
                    //    child.Id, child.Parent.Id,
                    //    child.X, child.Y);
                }
            };

            var action = new UndoRedoAction(undoAction, redoAction, "New Element");

            UndoRedoFramework.Add(action);
        }

        #endregion

        #region New Wire

        public static void NewWire(Context context, Wire wire)
        {
            if (context == null || wire == null)
                throw new ArgumentNullException();

            System.Diagnostics.Debug.Print("UndoRedo.NewWire(Context context, Wire wire)");

            Action undoAction = () =>
            {
                //System.Diagnostics.Debug.Print("---");
                //System.Diagnostics.Debug.Print("UNDO wire {0}", wire.Id);
                //System.Diagnostics.Debug.Print("UNDO wire.Start {0}", wire.Start.Id);
                //System.Diagnostics.Debug.Print("UNDO wire.End {0}", wire.End.Id);

                Manager.DeleteElement(context, wire);
            };

            Action redoAction = () =>
            {
                context.Children.Add(wire);

                //System.Diagnostics.Debug.Print("---");
                //System.Diagnostics.Debug.Print("REDO wire {0}", wire.Id);
                //System.Diagnostics.Debug.Print("REDO wire.Start {0}", wire.Start.Id);
                //System.Diagnostics.Debug.Print("REDO wire.End {0}", wire.End.Id);
            };

            var action = new UndoRedoAction(undoAction, redoAction, "New Wire");

            UndoRedoFramework.Add(action);
        }

        public static void NewWire(Context context, Wire wire, Pin pin)
        {
            if (context == null || wire == null || pin == null)
                throw new ArgumentNullException();

            System.Diagnostics.Debug.Print("UndoRedo.NewWire(Context context, Wire wire, Pin pin)");

            Wire connectedWire = null;

            Action undoAction = () =>
            {
                //System.Diagnostics.Debug.Print("---");
                //System.Diagnostics.Debug.Print("UNDO wire {0}", wire.Id);
                //System.Diagnostics.Debug.Print("UNDO wire.Start {0}", wire.Start.Id);
                //System.Diagnostics.Debug.Print("UNDO wire.End {0}", wire.End.Id);

                Manager.DeleteElement(context, wire);

                connectedWire = Manager.DeletePin(context, pin);

                //System.Diagnostics.Debug.Print("UNDO connectedWire {0}", connectedWire != null ? connectedWire.Id : "<null>");
            };

            Action redoAction = () =>
            {
                context.Children.Add(pin);

                //System.Diagnostics.Debug.Print("---");
                //System.Diagnostics.Debug.Print("REDO wire {0}", wire.Id);
                //System.Diagnostics.Debug.Print("REDO wire.Start {0}", wire.Start.Id);
                //System.Diagnostics.Debug.Print("REDO wire.End {0}", wire.End.Id);
                //System.Diagnostics.Debug.Print("REDO connectedWire {0}", connectedWire != null ? connectedWire.Id : "<null>");

                if (connectedWire != null)
                    context.Children.Add(connectedWire);
                else
                    context.Children.Add(wire);
            };

            var action = new UndoRedoAction(undoAction, redoAction, "New Wire");

            UndoRedoFramework.Add(action);
        }

        public static void NewWire(Context context, Wire wire, Pin pin, Pin endPin, Wire splitWire)
        {
            if (context == null || wire == null || pin == null || endPin == null || splitWire == null)
                throw new ArgumentNullException();

            System.Diagnostics.Debug.Print("UndoRedo.NewWire(Context context, Wire wire, Pin pin, Pin endPin, Wire splitWire)");

            Pin wireEndPin = wire.End;
            Pin splitWireStartPin = splitWire.Start;
            Wire connectedWire = null;

            Action undoAction = () =>
            {
                //System.Diagnostics.Debug.Print("---");
                //System.Diagnostics.Debug.Print("UNDO splitWire {0}", splitWire.Id);
                //System.Diagnostics.Debug.Print("UNDO splitWire.Start {0}", splitWire.Start.Id);
                //System.Diagnostics.Debug.Print("UNDO splitWire.End {0}", splitWire.End.Id);

                wire.End = endPin;

                Manager.DeleteElement(context, splitWire);

                connectedWire = Manager.DeletePin(context, pin);

                //System.Diagnostics.Debug.Print("UNDO connectedWire {0}", connectedWire.Id);
            };

            Action redoAction = () =>
            {
                wire.End = wireEndPin;

                context.Children.Add(pin);

                splitWire.Start = splitWireStartPin;

                context.Children.Add(connectedWire);

                context.Children.Add(splitWire);

                //System.Diagnostics.Debug.Print("---");
                //System.Diagnostics.Debug.Print("REDO splitWire {0}", splitWire.Id);
                //System.Diagnostics.Debug.Print("REDO splitWire.Start {0}", splitWire.Start.Id);
                //System.Diagnostics.Debug.Print("REDO splitWire.End {0}", splitWire.End.Id);
                //System.Diagnostics.Debug.Print("REDO connectedWire {0}", connectedWire.Id);
            };

            var action = new UndoRedoAction(undoAction, redoAction, "New Wire");

            UndoRedoFramework.Add(action);
        }

        #endregion

        #region Move Element(s)

        public static void MoveElement(Context context, Element element, double dX, double dY)
        {
            if (context == null)
                throw new ArgumentNullException();

            if (element == null)
                throw new ArgumentNullException();

            System.Diagnostics.Debug.Print("UndoRedo.MoveElement(Context context, Element element, double dX, double dY)");

            Action undoAction = () =>
            {
                Manager.MoveElement(context, element, dX, dY);
            };

            Action redoAction = () =>
            {
                Manager.MoveElement(context, element, -dX, -dY);
            };

            var action = new UndoRedoAction(undoAction, redoAction, "Move Element");

            UndoRedoFramework.Add(action);
        }

        public static void MoveElements(Context context, IEnumerable<Element> elements, double dX, double dY)
        {
            if (context == null)
                throw new ArgumentNullException();

            if (elements == null)
                throw new ArgumentNullException();

            System.Diagnostics.Debug.Print("UndoRedo.MoveElements(Context context, List<Element> elements, double dX, double dY)");

            Action undoAction = () =>
            {
                Manager.MoveElements(context, elements, dX, dY);
            };

            Action redoAction = () =>
            {
                Manager.MoveElements(context, elements, -dX, -dY);
            };

            var action = new UndoRedoAction(undoAction, redoAction, "Move Elements");

            UndoRedoFramework.Add(action);
        }

        #endregion
    }

    #endregion
}
