﻿// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Logic.Simulation.Core
{
    #region References

    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Threading.Tasks;

    #endregion

    #region IState

    public interface IBoolState
    {
        bool? PreviousState { get; set; }
        bool? State { get; set; }
    }

    #endregion
}