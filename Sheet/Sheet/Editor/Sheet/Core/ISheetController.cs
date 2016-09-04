// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Sheet.Core;
using System;
using System.Threading.Tasks;

namespace Sheet.Editor
{
    public interface ISheetController
    {
        // Properties
        SheetState State { get; set; }

        // Init
        void Init();

        // Clipboard
        void CutText();
        void CopyText();
        void PasteText();
        void CutJson();
        void CopyJson();
        void PasteJson();

        // Delete
        void Delete(XBlock block);
        void Delete();

        // Select All
        void SelecteAll();

        // Deselect All
        void DeselectAll();

        // Toggle Fill
        void ToggleFill();

        // Insert Mode
        void CreateBlock();
        void BreakBlock();

        // Move Mode
        void MoveUp();
        void MoveDown();
        void MoveLeft();
        void MoveRight();

        // Pan & Zoom Mode
        void SetAutoFitSize(double finalWidth, double finalHeight);
        void AdjustBackThickness(double zoom);
        void AdjustPageThickness(double zoom);

        // Data Binding
        bool BindDataToBlock(ImmutablePoint p, ItemData dataItem);
        bool BindDataToBlock(XBlock block, ItemData dataItem);
        void TryToBindData(ImmutablePoint p, ItemData dataItem);

        // New Page
        void NewPage();

        // Open Page
        Task OpenTextPage(string path);
        Task OpenJsonPage(string path);
        void OpenPage();

        // Save Page
        void SaveTextPage(string path);
        void SaveJsonPage(string path);
        void SavePage();

        // Library
        void Insert(ImmutablePoint p);
        XBlock Insert(ItemBlock blockItem, ImmutablePoint p, bool select);
        Task LoadLibrary(string path);
        void LoadLibrary();

        // Input
        void LeftDown(InputArgs args);
        void LeftUp(InputArgs args);
        void Move(InputArgs args);
        void RightDown(InputArgs args);
        void RightUp(InputArgs args);
        void Wheel(int delta, ImmutablePoint position);
        void Down(InputArgs args);

        // Page
        void SetPage(string text);
        string GetPage();
        ItemBlock SerializePage();
        void DeserializePage(ItemBlock page);
        void ResetPage();
        void ResetPageContent();
    }
}
