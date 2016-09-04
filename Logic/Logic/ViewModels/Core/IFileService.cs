// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Logic.ViewModels.Core
{
    #region References

    using Logic.Model;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    #endregion

    #region IFileService

    public interface IFileService
    {
        bool HaveFileName { get; set; }
        string FileName { get; set; }
        Task<Tuple<Solution, string>> Open();
        Task<Tuple<Solution, string>> Open(string fileName);
        Task<string> Save(Solution solution);
        Task<string> SaveAs(Solution solution);
        void Reset();
    }

    #endregion
}
