// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Autofac;
using System;

namespace Sheet.IoC
{
    public class AppServiceLocator : IServiceLocator
    {
        public T GetInstance<T>()
        {
            return App.Scope.Resolve<T>();
        }
    }
}
