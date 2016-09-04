using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Xml.Serialization;

namespace LogicCanvas.Model.Core
{
    [XmlRoot("Notify"), XmlType("Notify"), Serializable]
    public class NotifyObject : INotifyPropertyChanged
    {
        public virtual void Notify(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
