using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Collections.ObjectModel;

namespace LogicCanvas.Model.Core
{
    [XmlRoot("Signals"), XmlType("Signals"), Serializable]
    public class DataItemList : ObservableCollection<DataItem>, ICloneable
    {
        #region ICloneable Implementation

        private static ObservableCollection<T> DeepCopy<T>(IEnumerable<T> list) where T : ICloneable
        {
            return new ObservableCollection<T>(list.Select(x => x.Clone()).Cast<T>());
        }

        public object Clone()
        {
            return DeepCopy(this);
        }

        #endregion
    }
}
