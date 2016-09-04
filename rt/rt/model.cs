using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace rt
{
    public struct Vector
    {
        public double X;
        public double Y;
        public double Z;
        public static Vector Zero { get { return new Vector(0.0, 0.0, 0.0); } }
        public Vector(double x, double y, double z) { X = x; Y = y; Z = z; }
        public Vector slice() { return new Vector(X, Y, Z); }
    }

    public struct Sphere
    {
        public Vector pos;
        public Vector color;
        public double radius;
        public int type;
    }

    public struct Lamp
    {
        public Sphere shape;
    }

    public struct Patch
    {
        public Vector pos;
        public Vector normal;
        public Vector color;
    }

    public struct Line
    {
        public Vector org;
        public Vector dir;
        public double t;
    }

    public struct Observer
    {
        public Vector obspos;
        public int nx;
        public int ny;
        public double hratio;
        public double alt;
        public double az;
        public double fl;
        public double px;
        public double py;
        public Vector viewdir;
        public Vector uhat;
        public Vector vhat;
        public byte[] imgdata;
    }

    public struct World
    {
        public int numsp;
        public Sphere[] sp;
        public int numlmp;
        public Lamp[] lmp;
        public Patch[] horizon;
        public Vector illum;
        public Vector skyzen;
        public Vector skyhor;
    }
}
