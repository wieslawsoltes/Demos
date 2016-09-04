// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Logic.Views.Selectors
{
    #region References

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;

    #endregion

    #region ModelContainerStyleSelector

    public class ModelContainerStyleSelector : StyleSelector
    {
        public override Style SelectStyle(object item, DependencyObject container)
        {
            try
            {
                var key = string.Format("{0}{1}", item.GetType().Name, "ContainerStyleKey");
                var element = container as FrameworkElement;
                var st = (Style)element.TryFindResource(key);

                if (st == null)
                {
                    System.Diagnostics.Debug.Print("Could not find resource: {0}", key);
                }

                return st;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print("Exception: {0}", ex.Message);
            }

            return base.SelectStyle(item, container);
        }
    }

    #endregion
}
