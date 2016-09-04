// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Logic.ViewModels.Core
{
    #region References

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Logic.Model;
    using Logic.Model.Core;

    #endregion

    #region IExportService

    public interface IExportService
    {
        void Export(Element element);
        void AutoRename(Element element);
    }

    #endregion
}
