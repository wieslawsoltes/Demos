// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Sheet.Core;
using Sheet.IoC;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Sheet.Editor
{
    public class PointController : IPointController
    {
        #region IoC

        private readonly IServiceLocator _serviceLocator;
        private readonly IBlockHelper _blockHelper;

        public PointController(IServiceLocator serviceLocator)
        {
            this._serviceLocator = serviceLocator;
            this._blockHelper = _serviceLocator.GetInstance<IBlockHelper>();
        }

        #endregion

        #region Get

        private IEnumerable<KeyValuePair<int, XPoint>> GetAllPoints(IList<XBlock> blocks)
        {
            foreach (var block in blocks)
            {
                foreach (var point in block.Points)
                {
                    yield return new KeyValuePair<int, XPoint>(point.Id, point);
                }

                foreach (var kvp in GetAllPoints(block.Blocks))
                {
                    yield return kvp;
                }
            }
        }

        private IEnumerable<XLine> GetAllLines(IList<XBlock> blocks)
        {
            foreach (var block in blocks)
            {
                foreach (var line in block.Lines)
                {
                    yield return line;
                }

                foreach (var line in GetAllLines(block.Blocks))
                {
                    yield return line;
                }
            }
        }

        #endregion

        #region Connect

        public void ConnectStart(XPoint point, XLine line)
        {
            Action<XElement, XPoint> update = (element, p) =>
            {
                _blockHelper.SetX1(element as XLine, p.X);
                _blockHelper.SetY1(element as XLine, p.Y);
            };
            point.Connected.Add(new Dependency(line, update));
        }

        public void ConnectEnd(XPoint point, XLine line)
        {
            Action<XElement, XPoint> update = (element, p) =>
            {
                _blockHelper.SetX2(element as XLine, p.X);
                _blockHelper.SetY2(element as XLine, p.Y);
            };
            point.Connected.Add(new Dependency(line, update));
        }

        #endregion

        #region Dependencies

        public void UpdateDependencies(IList<XBlock> blocks, IList<XPoint> points, IList<XLine> lines)
        {
            var ps = GetAllPoints(blocks).ToDictionary(x => x.Key, x => x.Value);

            foreach (var point in points)
            {
                ps.Add(point.Id, point);
            }

            var ls = GetAllLines(blocks).ToList();

            foreach (var line in lines)
            {
                ls.Add(line);
            }

            foreach (var line in ls)
            {
                if (line.StartId >= 0)
                {
                    XPoint point;
                    if (ps.TryGetValue(line.StartId, out point))
                    {
                        line.Start = point;
                        ConnectStart(line.Start, line);
                    }
                    else
                    {
                        Debug.Print("Invalid line Start point Id.");
                    }
                }

                if (line.EndId >= 0)
                {
                    XPoint point;
                    if (ps.TryGetValue(line.EndId, out point))
                    {
                        line.End = point;
                        ConnectEnd(line.End, line);
                    }
                    else
                    {
                        Debug.Print("Invalid line End point Id.");
                    }
                }
            }
        }

        #endregion
    }
}
