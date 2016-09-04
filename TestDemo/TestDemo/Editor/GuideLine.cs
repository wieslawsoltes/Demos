using System;

namespace TestDemo
{
    public class GuideLine
    {
        public GuidePoint Point0 { get; private set; }
        public GuidePoint Point1 { get; private set; }

        public GuideLine(GuidePoint point0, GuidePoint point1)
        {
            Point0 = point0;
            Point1 = point1;
        }
    }
}
