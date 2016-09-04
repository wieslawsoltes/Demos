// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Autofac;
using Sheet.Editor;
using System;

namespace Sheet.IoC
{
    public class ItemModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ItemController>().As<IItemController>().SingleInstance();
            builder.RegisterType<ItemSerializer>().As<IItemSerializer>().SingleInstance();
        }
    }
}
