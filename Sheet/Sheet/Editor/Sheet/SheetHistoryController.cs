// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Sheet.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Sheet.Editor
{
    public class SheetHistoryController : IHistoryController
    {
        #region IoC

        private readonly ISheetController _sheetController;
        private readonly IItemSerializer _itemSerializer;

        public SheetHistoryController(ISheetController sheetController, IItemSerializer itemSerializer)
        {
            this._sheetController = sheetController;
            this._itemSerializer = itemSerializer;
        }

        #endregion

        #region Fields

        private Stack<ItemChange> undos = new Stack<ItemChange>();
        private Stack<ItemChange> redos = new Stack<ItemChange>();

        #endregion

        #region Factory

        private async Task<ItemChange> CreateChange(string message)
        {
            var block = _sheetController.SerializePage();
            var text = await Task.Run(() => _itemSerializer.SerializeContents(block));
            var change = new ItemChange()
            {
                Message = message,
                Model = text
            };
            return change;
        }

        #endregion

        #region IHistoryController

        public async void Register(string message)
        {
            var change = await CreateChange(message);
            undos.Push(change);
            redos.Clear();
        }

        public void Reset()
        {
            undos.Clear();
            redos.Clear();
        }

        public async void Undo()
        {
            if (undos.Count > 0)
            {
                try
                {
                    var change = await CreateChange("Redo");
                    redos.Push(change);
                    var undo = undos.Pop();
                    var block = await Task.Run(() => _itemSerializer.DeserializeContents(undo.Model));
                    _sheetController.ResetPage();
                    _sheetController.DeserializePage(block);
                }
                catch (Exception ex)
                {
                    Debug.Print(ex.Message);
                    Debug.Print(ex.StackTrace);
                }
            }
        }

        public async void Redo()
        {
            if (redos.Count > 0)
            {
                try
                {
                    var change = await CreateChange("Undo");
                    undos.Push(change);
                    var redo = redos.Pop();
                    var block = await Task.Run(() => _itemSerializer.DeserializeContents(redo.Model));
                    _sheetController.ResetPage();
                    _sheetController.DeserializePage(block);
                }
                catch (Exception ex)
                {
                    Debug.Print(ex.Message);
                    Debug.Print(ex.StackTrace);
                }
            }
        }

        #endregion
    }
}
