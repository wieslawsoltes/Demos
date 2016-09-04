
namespace LineGrid.Model
{
    #region References

    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    #endregion

    #region LineItem

    public class LineItem : NotifyObject
    {
        #region Properties

        private double x1;
        private double y1;
        private double x2;
        private double y2;

        public double X1
        {
            get { return x1; }
            set
            {
                if (value != x1)
                {
                    x1 = value;
                    Notify("X1");
                }
            }
        }

        public double Y1
        {
            get { return y1; }
            set
            {
                if (value != y1)
                {
                    y1 = value;
                    Notify("Y1");
                }
            }
        }

        public double X2
        {
            get { return x2; }
            set
            {
                if (value != x2)
                {
                    x2 = value;
                    Notify("X2");
                }
            }
        }

        public double Y2
        {
            get { return y2; }
            set
            {
                if (value != y2)
                {
                    y2 = value;
                    Notify("Y2");
                }
            }
        }

        #endregion
    }

    #endregion
}
