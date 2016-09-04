// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Windows;
using System.Windows.Controls;

namespace Sheet
{
    public class SizeBorder : Border
    {
        #region Properties

        public Action<Size> ExecuteUpdateSize { get; set; }
        public Action ExecuteSizeChanged { get; set; }

        #endregion

        #region Constructor

        public SizeBorder()
        {
            SizeChanged += (sender, e) =>
            {
                if (Child != null && ExecuteSizeChanged != null)
                {
                    ExecuteSizeChanged();
                }
            };
        }

        #endregion

        #region ArrangeOverride

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (ExecuteUpdateSize != null)
            {
                ExecuteUpdateSize(finalSize);
            }
            return base.ArrangeOverride(finalSize);
        }

        #endregion
    }
}
