using System;

namespace TestDemo
{
    public class Argb
    {
        public readonly byte A;
        public readonly byte R;
        public readonly byte G;
        public readonly byte B;
        
        public Argb(byte a = 255, byte r = 0, byte g = 0, byte b = 0)
        {
            A = a;
            R = r;
            G = g;
            B = b;
        }
    }
}
