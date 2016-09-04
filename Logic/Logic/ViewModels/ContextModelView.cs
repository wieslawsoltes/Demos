// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Logic.ViewModels
{
    #region References

    using Logic.ViewModels.Core;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    #endregion

    #region ContextModelView

    public class ContextModelView : IView 
    {
        #region Constructor

        public ContextModelView() { }

        public ContextModelView(string name)
            : base()
        {
            this.Name = name;
        }

        #endregion

        #region Properties

        public string Name { get; set; } 

        #endregion
    }

    #endregion
}

