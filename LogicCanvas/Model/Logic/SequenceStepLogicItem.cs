using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using LogicCanvas.Model.Core;

namespace LogicCanvas.Model.Logic
{
    [XmlRoot("SequenceStep"), XmlType("SequenceStep"), Serializable]
    public class SequenceStepLogicItem : Item, ILogicItem
    {

    }
}
