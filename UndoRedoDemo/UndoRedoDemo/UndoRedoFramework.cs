// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace UndoRedoDemo
{
    #region References

    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;

    #endregion

    #region Undo/Redo Framework

    public class ObservableStack<T> : Stack<T>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        #region Constructor

        public ObservableStack() { }

        public ObservableStack(IEnumerable<T> collection)
        {
            foreach (var item in collection)
                base.Push(item);
        }

        public ObservableStack(List<T> list)
        {
            foreach (var item in list)
                base.Push(item);
        }

        #endregion

        #region Stack<T>

        public new virtual void Clear()
        {
            base.Clear();
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public new virtual T Pop()
        {
            var item = base.Pop();
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, 0));
            return item;
        }

        public new virtual void Push(T item)
        {
            base.Push(item);
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, 0));
        }

        #endregion

        #region INotifyCollectionChanged

        public virtual event NotifyCollectionChangedEventHandler CollectionChanged;

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            this.RaiseCollectionChanged(e);
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            this.RaisePropertyChanged(e);
        }

        #endregion

        #region INotifyPropertyChanged

        protected virtual event PropertyChangedEventHandler PropertyChanged;

        private void RaiseCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (this.CollectionChanged != null)
                this.CollectionChanged(this, e);
        }

        private void RaisePropertyChanged(PropertyChangedEventArgs e)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, e);
        }

        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add { this.PropertyChanged += value; }
            remove { this.PropertyChanged -= value; }
        }

        #endregion
    }

    public class UndoRedoAction : INotifyPropertyChanged
    {
        #region Constructor

        public UndoRedoAction() { }

        public UndoRedoAction(Action undoAction, Action redoAction, string name)
        {
            this.undoAction = undoAction;
            this.redoAction = redoAction;
            this.name = name;
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void Notify(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region Properties

        private Action undoAction = null;
        private Action redoAction = null;
        private string name = string.Empty;

        public Action UndoAction
        {
            get { return undoAction; }
            set
            {
                if (value != undoAction)
                {
                    undoAction = value;
                    Notify("UndoAction");
                }
            }
        }

        public Action RedoAction
        {
            get { return redoAction; }
            set
            {
                if (value != redoAction)
                {
                    redoAction = value;
                    Notify("RedoAction");
                }
            }
        }

        public string Name
        {
            get { return name; }
            set
            {
                if (value != name)
                {
                    name = value;
                    Notify("Name");
                }
            }
        }

        #endregion
    }

    public class UndoRedoState : INotifyPropertyChanged
    {
        #region Constructor

        public UndoRedoState() { }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void Notify(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region Properties

        private bool canUndo = false;
        private bool canRedo = false;

        public bool CanUndo
        {
            get { return canUndo; }
            private set
            {
                if (value != canUndo)
                {
                    canUndo = value;
                    Notify("CanUndo");
                }
            }
        }

        public bool CanRedo
        {
            get { return canRedo; }
            private set
            {
                if (value != canRedo)
                {
                    canRedo = value;
                    Notify("CanRedo");
                }
            }
        }

        #endregion

        #region Methods

        public void SetUndoState(bool state)
        {
            this.CanUndo = state;
        }

        public void SetRedoState(bool state)
        {
            this.CanRedo = state;
        }

        #endregion
    }

    public static class UndoRedoFramework
    {
        #region Properties

        private static ObservableStack<UndoRedoAction> undoActions = new ObservableStack<UndoRedoAction>();
        private static ObservableStack<UndoRedoAction> redoActions = new ObservableStack<UndoRedoAction>();
        private static UndoRedoState state = new UndoRedoState();

        public static ObservableStack<UndoRedoAction> UndoActions
        {
            get { return undoActions; }
        }

        public static ObservableStack<UndoRedoAction> RedoActions
        {
            get { return redoActions; }
        }

        public static UndoRedoState State
        {
            get { return state; }
        }

        #endregion

        #region Methods

        private static void UpdateUndoState()
        {
            state.SetUndoState(undoActions.Count <= 0 ? false : true);
        }

        private static void UpdateRedoState()
        {
            state.SetRedoState(redoActions.Count <= 0 ? false : true);
        }

        private static void ClearUndo()
        {
            if (undoActions.Count <= 0)
                return;

            undoActions.Clear();

            UpdateUndoState();
        }

        private static void ClearRedo()
        {
            if (redoActions.Count <= 0)
                return;

            redoActions.Clear();

            UpdateRedoState();
        }

        public static void Clear()
        {
            ClearUndo();

            ClearRedo();
        }

        public static void Undo()
        {
            if (undoActions.Count <= 0)
                return;

            var undo = undoActions.Pop();

            UpdateUndoState();

            if (undo != null && undo.UndoAction != null)
            {
                // execute undo
                undo.UndoAction();

                // register redo
                if (undo.RedoAction != null)
                {
                    var action = new UndoRedoAction(undo.UndoAction, undo.RedoAction, undo.Name);

                    redoActions.Push(action);
                    UpdateRedoState();
                }
            }
        }

        public static void Redo()
        {
            if (redoActions.Count <= 0)
                return;

            var redo = redoActions.Pop();

            UpdateRedoState();

            if (redo != null && redo.RedoAction != null)
            {
                // execute redo
                redo.RedoAction();

                // register undo
                if (redo.UndoAction != null)
                {
                    var action = new UndoRedoAction(redo.UndoAction, redo.RedoAction, redo.Name);

                    undoActions.Push(action);
                    UpdateUndoState();
                }
            }
        }

        public static void Add(UndoRedoAction action)
        {
            ClearRedo();

            undoActions.Push(action);

            UpdateUndoState();
        }

        #endregion
    }

    #endregion
}
