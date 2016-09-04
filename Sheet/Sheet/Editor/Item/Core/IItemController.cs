// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Threading.Tasks;
using Sheet.Core;

namespace Sheet.Editor
{
    public interface IItemController
    {
        Task<string> OpenText(string fileName);
        void SaveText(string fileName, string text);

        void ResetPosition(ItemBlock block, double originX, double originY, double width, double height);

        double Snap(double val, double snap);
    }
}
