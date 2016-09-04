// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Autofac;
using Sheet.Editor;
using Sheet.IoC;
using System;
using System.Diagnostics;
using System.Windows;

namespace Sheet
{
    public partial class App : Application
    {
        public static IContainer Container { get; private set; }
        public static ILifetimeScope Scope { get; private set; }

        public App()
        {
            if (Environment.UserInteractive)
            {
                Debug.Listeners.Add(new TextWriterTraceListener(System.Console.Out));
            }

            Init();
        }

        private void Init()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<AppServiceLocator>().As<IServiceLocator>().SingleInstance();
            builder.RegisterType<AppScopeServiceLocator>().As<IScopeServiceLocator>().InstancePerDependency();

            builder.RegisterModule<UtilModule>();
            builder.RegisterModule<BlockModule>();
            builder.RegisterModule<ItemModule>();
            builder.RegisterModule<SheetModule>();
            builder.RegisterModule<WpfModule>();

            ShowMainWindow(builder);
        }

        private void ShowMainWindow(ContainerBuilder builder)
        {
            using (Container = builder.Build())
            {
                using (Scope = Container.BeginLifetimeScope())
                {
                    using (var window = Scope.Resolve<IMainWindow>())
                    {
                        window.ShowDialog();
                    }
                }
            }
        }
    }
}
