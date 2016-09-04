
namespace Logic.Model
{
    #region References

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    #endregion

    #region Pin

    public class Pin : Element
    {
        public Pin() : base() { }
        public Pin(PinType type, bool isPinTypeUndefined)
            : this()
        {
            this.Type = type;
            this.IsPinTypeUndefined = isPinTypeUndefined;
        }

        public bool IsPinTypeUndefined { get; set; }
        public PinType Type { get; set; }

        // connection format: Tuple<Pin,bool> where bool is flag Inverted==True|False
        public Tuple<Pin, bool>[] Connections { get; set; }
    }

    #endregion
}
