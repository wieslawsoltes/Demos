// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Logic.Utilities
{
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
}
