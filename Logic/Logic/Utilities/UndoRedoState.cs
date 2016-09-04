// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Logic.Utilities
{
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
}
