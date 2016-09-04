// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Logic.Simulation
{
    #region References

    using Logic.Model.Core;
    using Logic.Simulation.Core;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    #endregion

    #region Clock

    public class Clock : IClock
    {
        public Clock()
            : base()
        {
        }

        public Clock(long cycle, int resolution)
            : this()
        {
            this.Cycle = cycle;
            this.Resolution = resolution;
        }

        public long Cycle { get; set; }
        public int Resolution { get; set; }
    }

    #endregion
}
