using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Collections.ObjectModel;
using LogicCanvas.Model.Core;

namespace LogicCanvas.Model.ControlBlock
{
    [XmlRoot("ControlValve"), XmlType("ControlValve"), Serializable]
    public class ControlValveControlBlockItem : Item, IControlBlockItem
    {

    }
}
