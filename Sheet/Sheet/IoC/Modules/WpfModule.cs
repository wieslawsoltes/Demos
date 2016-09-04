// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Autofac;
using Sheet.Editor;
using Sheet.WPF;
using System;

namespace Sheet.IoC
{
    public class WpfModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<WpfClipboard>().As<IClipboard>().SingleInstance();

            builder.RegisterType<WpfOpenFileDialog>().As<IOpenFileDialog>().InstancePerDependency();
            builder.RegisterType<WpfSaveFileDialog>().As<ISaveFileDialog>().InstancePerDependency();

            builder.RegisterType<WpfBlockFactory>().As<IBlockFactory>().SingleInstance();
            builder.RegisterType<WpfBlockHelper>().As<IBlockHelper>().SingleInstance();
            builder.RegisterType<WpfCanvasSheet>().As<ISheet>().InstancePerDependency();

            builder.RegisterType<SheetHistoryController>().As<IHistoryController>().InstancePerDependency();

            builder.RegisterType<LibraryControl>()
                .As<ILibraryView>()
                .As<ILibraryController>()
                .SingleInstance();

            builder.RegisterType<SheetControl>()
                .As<ISheetView>()
                .As<IZoomController>()
                .As<ICursorController>()
                .InstancePerLifetimeScope();

            builder.RegisterType<SheetWindow>().As<IMainWindow>().InstancePerLifetimeScope();
        }
    } 
}
