// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Sheet.Core
{
    public static class ArgbColors
    {
        public static ArgbColor Transparent { get { return new ArgbColor(0, 255, 255, 255); } }
        public static ArgbColor White { get { return new ArgbColor(255, 255, 255, 255); } }
        public static ArgbColor Black { get { return new ArgbColor(255, 0, 0, 0); } }
        public static ArgbColor Red { get { return new ArgbColor(255, 255, 0, 0); } }
        public static ArgbColor LightGray { get { return new ArgbColor(255, 211, 211, 211); } }
        public static ArgbColor DarkGray { get { return new ArgbColor(255, 169, 169, 169); } }
    }
}
