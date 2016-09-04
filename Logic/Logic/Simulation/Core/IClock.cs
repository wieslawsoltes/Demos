// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Logic.Simulation.Core
{
    #region References

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    #endregion

    #region IClock

    public interface IClock
    {
        long Cycle { get; set; }
        int Resolution { get; set; }
    }

    #endregion
}
