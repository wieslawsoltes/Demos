// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Autofac;
using Sheet.Editor;
using System;

namespace Sheet.IoC
{
    public class SheetModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<SheetPageFactory>().As<IPageFactory>().SingleInstance();
            builder.RegisterType<SheetController>().As<ISheetController>().InstancePerLifetimeScope();
        }
    }
}
