// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Sheet.Editor
{
    public interface ITextController
    {
        void Set(Action<string> ok, Action cancel, string title, string label, string text);
    }
}
