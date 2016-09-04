﻿// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Logic.Elements.Timers
{
    #region References

    using Logic.Elements.Core;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    #endregion

    #region TimerOnElement

    public class TimerOnElement : IElement
    {
        #region IElement Properties

        private string name = "TimerOn";
        public string Name
        {
            get { return name; }
            set
            {
                if (value != name)
                {
                    name = value;
                }
            }
        }

        #endregion
    }

    #endregion
}
