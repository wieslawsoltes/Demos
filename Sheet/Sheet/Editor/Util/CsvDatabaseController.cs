// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Linq;
using System.Collections.Generic;

namespace Sheet.Editor
{
    public class CsvDatabaseController : IDatabaseController
    {
        private string _name = null;
        private string[] _columns = null;
        private IList<string[]> _data = null;

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
            }
        }

        public string[] Columns
        {
            get { return _columns; }
            set
            {
                _columns = value;
            }
        }

        public IList<string[]> Data
        {
            get { return _data; }
            set
            {
                _data = value;
            }
        }

        public CsvDatabaseController(string name)
        {
            Name = name;
        }

        public string[] Get(int index)
        {
            return _data.Where(x => int.Parse(x[0]) == index).FirstOrDefault();
        }

        public bool Update(int index, string[] item)
        {
            for (int i = 0; i < _data.Count; i++)
            {
                if (int.Parse(_data[i][0]) == index)
                {
                    _data[i] = item;
                    return true;
                }
            }
            return false;
        }

        public int Add(string[] item)
        {
            int index = _data.Max((x) => int.Parse(x[0])) + 1;
            item[0] = index.ToString();
            _data.Add(item);
            return index;
        }
    }
}
