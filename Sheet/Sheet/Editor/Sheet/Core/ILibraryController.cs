// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Sheet.Core;
using System;
using System.Collections.Generic;

namespace Sheet.Editor
{
    public interface ILibraryController
    {
        ItemBlock GetSelected();
        void SetSelected(ItemBlock block);
        IEnumerable<ItemBlock> GetSource();
        void SetSource(IEnumerable<ItemBlock> source);
    }
}
