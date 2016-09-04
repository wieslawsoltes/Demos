// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Sheet.Core;

namespace Sheet.Editor
{
    public interface IItemSerializer
    {
        string SerializeContents(ItemBlock block, ItemSerializeOptions options);
        string SerializeContents(ItemBlock block);
        ItemBlock DeserializeContents(string model, ItemSerializeOptions options);
        ItemBlock DeserializeContents(string model);
    }
}
