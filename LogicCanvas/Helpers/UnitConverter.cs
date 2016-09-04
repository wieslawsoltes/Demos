using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LogicCanvas.Helpers
{
    public static class UnitConverter
    {
        public static double Dpi = 96.0;

        public static double CmToDip(double cm)
        {
            // Device Independent Pixel (DIP)
            // 96 DIP correspond to 1 inch (2.54 cm)
            return cm * Dpi / 2.54;
        }

        public static double DipToCm(double dip)
        {
            // Device Independent Pixel (DIP)
            // 96 DIP correspond to 1 inch (2.54 cm)
            return dip / Dpi * 2.54;
        }
    }
}
