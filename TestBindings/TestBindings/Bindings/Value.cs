// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Test2d
{
    public class Value : ObservableObject
    {
        private string _content;

        public string Content
        {
            get { return _content; }
            set { Update(ref _content, value); }
        }
    }
}
