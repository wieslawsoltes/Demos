// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Sheet.Core;
using System;

namespace Sheet.Editor
{
    public class InputArgs
    {
        // Mouse Generic
        public bool OnlyControl { get; set; }
        public bool OnlyShift { get; set; }
        public ItemType SourceType { get; set; }
        public ImmutablePoint SheetPosition { get; set; }
        public ImmutablePoint RootPosition { get; set; }
        public Action<bool> Handled { get; set; }
        // Mouse Wheel
        public int Delta { get; set; }
        // Mouse Down
        public InputButton Button { get; set; }
        public int Clicks { get; set; }
    }
}
