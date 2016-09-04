// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Logic.Elements.Core
{
    #region References

    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    #endregion

    #region IElements

    [InheritedExport]
    public interface IElement
    {
        string Name { get; set; }
    } 

    #endregion
}
