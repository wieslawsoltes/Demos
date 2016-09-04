// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Autofac;
using System;

namespace Sheet.IoC
{
    public class AppScopeServiceLocator : IScopeServiceLocator
    {
        ILifetimeScope _scope;

        public AppScopeServiceLocator()
        {
            CreateScope();
        }

        public T GetInstance<T>()
        {
            return _scope.Resolve<T>();
        }

        public void CreateScope()
        {
            _scope = App.Scope.BeginLifetimeScope();
        }

        public void ReleaseScope()
        {
            _scope.Dispose();
        }
    }
}
