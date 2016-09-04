// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Logic.Utilities
{
    public static class UndoRedoFramework
    {
        #region Properties

        private static Stack<UndoRedoAction> undoActions = new Stack<UndoRedoAction>();
        private static Stack<UndoRedoAction> redoActions = new Stack<UndoRedoAction>();
        private static UndoRedoState state = new UndoRedoState();

        public static Stack<UndoRedoAction> UndoActions
        {
            get { return undoActions; }
        }

        public static Stack<UndoRedoAction> RedoActions
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
}
