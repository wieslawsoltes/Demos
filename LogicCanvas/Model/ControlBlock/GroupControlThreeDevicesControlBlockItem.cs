using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using LogicCanvas.Model.Core;

namespace LogicCanvas.Model.ControlBlock
{
    [XmlRoot("GroupControlThreeDevices"), XmlType("GroupControlThreeDevices"), Serializable]
    public class GroupControlThreeDevicesControlBlockItem : Item, IControlBlockItem
    {

    }
}
