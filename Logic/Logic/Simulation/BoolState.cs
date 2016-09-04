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

    #region BoolState

    public class BoolState : NotifyObject, IBoolState
    {
        #region IState

        public bool? previousState;
        public bool? state;

        public bool? PreviousState
        {
            get { return previousState; }
            set
            {
                if (value != previousState)
                {
                    previousState = value;
                    Notify("PreviousState");
                }
            }
        }

        public bool? State
        {
            get { return state; }
            set
            {
                if (value != state)
                {
                    state = value;
                    Notify("State");
                }
            }
        }

        #endregion
    }

    #endregion
}
