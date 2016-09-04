// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Autofac;
using Sheet.Editor;
using System;

namespace Sheet.IoC
{
    public class BlockModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<BlockController>().As<IBlockController>().SingleInstance();
            builder.RegisterType<BlockSerializer>().As<IBlockSerializer>().SingleInstance();
            builder.RegisterType<PointController>().As<IPointController>().SingleInstance();
        }
    }
}
