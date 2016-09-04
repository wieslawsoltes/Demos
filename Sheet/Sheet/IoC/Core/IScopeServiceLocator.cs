// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Sheet.IoC
{
    public interface IScopeServiceLocator
    {
        T GetInstance<T>();
        void CreateScope();
        void ReleaseScope();
    }
}
