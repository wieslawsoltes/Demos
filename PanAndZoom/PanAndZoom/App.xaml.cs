// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace PanAndZoom
{
    #region References

    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Linq;
    using System.Windows;

    #endregion

    #region App

    public partial class App : Application
    {
        #region Events

        protected override void OnStartup(StartupEventArgs e)
        {
            if (e.Args != null && e.Args.Count() > 0)
            {
                this.Properties["AppArgs"] = e.Args;
            }
            
            base.OnStartup(e);
        }

        #endregion
    }

    #endregion
}
