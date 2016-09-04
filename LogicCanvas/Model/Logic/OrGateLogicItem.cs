using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using LogicCanvas.Model.Core;

namespace LogicCanvas.Model.Logic
{
    [XmlRoot("OrGate"), XmlType("OrGate"), Serializable]
    public class OrGateLogicItem : Item, ILogicItem
    {

    }
}
