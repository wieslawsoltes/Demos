// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;

namespace Sheet.Editor
{
    public interface IDatabaseController
    {
        string Name { get; set; }
        string[] Columns { get; set; }
        IList<string[]> Data { get; set; }
        string[] Get(int index);
        bool Update(int index, string[] item);
        int Add(string[] item);
    }
}
