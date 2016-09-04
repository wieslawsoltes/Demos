/*
======================================================================
rt1.js

Ernie Wright  23 Feb 2014

Eric Graham's RT raytracer, translated into Javascript.  The original
rt1.c included the following notice.

RT1.C    Ray tracing program
Copyright 1987 Eric Graham
Permission is granted to copy and modify this file, provided that this
notice is retained.
====================================================================== */

using System;
using System.Threading.Tasks;

namespace rt
{
    public static class rt1
    {
        public const double BIG = 1.0e10;
        public const double SMALL = 1.0e-5;  /* suggested by Edd Biddulph; originally 1.0e-3 */
        public const int DULL = 0;
        public const int BRIGHT = 1;
        public const int MIRROR = 2;


        /*
        ======================================================================
        rtmain()
        
        Conventional entry point for the raytracer.  This renders correctly,
        but it prevents the browser from doing anything until the render is
        complete.
        ====================================================================== */

        public static Observer rtmain()
        {
            var line = new Line() { org = Vector.Zero, dir = Vector.Zero, t = 0 };
            var brite = Vector.Zero;
            var o = new Observer();
            var w = new World();

            rt2.setup(ref o, ref w);
            rt3.initsc(ref o);

            for (int j = 0; j < o.ny; j++)
            {
                for (int i = 0; i < o.nx; i++)
                {
                    pixline(ref line, ref o, i, j);
                    raytrace(ref brite, ref line, ref w);
                    rt3.ham(i, j, ref brite, ref o);
                }
            }

            return o;
        }

        public static Observer rtmain_mt(int nthreads)
        {
            var o = new Observer();
            var w = new World();

            rt2.setup(ref o, ref w);
            rt3.initsc(ref o);

            Task[] t = new Task[nthreads];

            for (uint n = 0; n < nthreads; n++)
            {
                var _n = n;

                t[_n] = Task.Factory.StartNew(() =>
                {
                    var line = new Line() { org = Vector.Zero, dir = Vector.Zero, t = 0 };
                    var brite = Vector.Zero;

                    int start = (int)((o.ny / nthreads) * _n);
                    int end = (int)(o.ny - (o.ny / nthreads) * (nthreads - 1 - _n));

                    //System.Diagnostics.Debug.Print("{0} {1}", start, end);

                    for (int j = start; j < end; j++)
                    {
                        for (int i = 0; i < o.nx; i++)
                        {
                            pixline(ref line, ref o, i, j);
                            raytrace(ref brite, ref line, ref w);
                            rt3.ham(i, j, ref brite, ref o);
                        }
                    }
                });
            }

            Task.WaitAll(t);

            return o;
        }


        /*
        ======================================================================
        raytrace()
        
        Given a camera ray and a world (or scene), calculate the color seen in
        the ray direction.
        ====================================================================== */

        public static void raytrace(ref Vector brite, ref Line line, ref World w)
        {
            var ptch = new Patch() { pos = Vector.Zero, normal = Vector.Zero, color = Vector.Zero };
            var pos = Vector.Zero;
            int k;

            var tmin = BIG;
            var spnear = -1;                       /* do we see a sphere? */
            for (k = 0; k < w.numsp; k++)
                if (intsplin(ref line, ref w.sp[k]))
                {
                    if (line.t < tmin)
                    {
                        tmin = line.t;
                        spnear = k;
                    }
                }

            var lmpnear = -1;                      /* are we looking at a lamp? */
            for (k = 0; k < w.numlmp; k++)
            {
                if (intsplin(ref line, ref w.lmp[k].shape))
                {
                    if (line.t < tmin)
                    {
                        tmin = line.t;
                        lmpnear = k;
                    }
                }
            }

            if (lmpnear != -1)
            {                 /* we see a lamp! */
                double r = w.lmp[lmpnear].shape.radius * w.lmp[lmpnear].shape.radius;
                brite.X = w.lmp[lmpnear].shape.color.X / r;
                brite.Y = w.lmp[lmpnear].shape.color.Y / r;
                brite.Z = w.lmp[lmpnear].shape.color.Z / r;
                return;
            }

            if (inthor(ref line))                   /* do we see the ground? */
                if (line.t < tmin)
                {
                    point(ref pos, line.t, ref line);
                    k = gingham(ref pos);
                    /* use 'pos2' instead of updating 'w.horizon[k].pos' to enable multithreaded raytracing */
                    var pos2 = pos.slice(); /* w.horizon[k].pos = pos.slice(); */
                    pixbrite(ref brite, ref w.horizon[k], ref pos2, ref w, -1);
                    return;
                }

            if (spnear != -1)
            {                  /* we see a sphere */
                point(ref  ptch.pos, tmin, ref line);
                setnorm(ref ptch, ref w.sp[spnear]);
                ptch.color = w.sp[spnear].color.slice();
                switch (w.sp[spnear].type)
                {
                    case BRIGHT:
                        if (glint(ref brite, ref ptch, ref w, spnear, ref line))
                            break;
                        else
                            pixbrite(ref brite, ref ptch, ref ptch.pos, ref w, spnear);
                        break;
                    case DULL: pixbrite(ref brite, ref ptch, ref ptch.pos, ref w, spnear); break;
                    case MIRROR: mirror(ref brite, ref ptch, ref w, ref line); break;
                }
                return;
            }
            skybrite(ref brite, ref line, ref w);            /* nothing else, must be sky */
        }


        /*
        ======================================================================
        skybrite()
        
        Calculate the sky color in the ray direction by interpolating between
        colors at the zenith and horizon.  The interpolation parameter, sin2,
        comes from sin^2(a) = z^2 / r^2, where (x,y,z) is a point on a sphere
        of radius r, and a is the elevation angle above the ground plane.
        ====================================================================== */

        public static void skybrite(ref Vector brite, ref Line line, ref World w)
        {
            double sin2, cos2;
            sin2 = line.dir.Z * line.dir.Z;
            sin2 /= (line.dir.X * line.dir.X + line.dir.Y * line.dir.Y + sin2);
            cos2 = 1.0 - sin2;
            brite.X = cos2 * w.skyhor.X + sin2 * w.skyzen.X;
            brite.Y = cos2 * w.skyhor.Y + sin2 * w.skyzen.Y;
            brite.Z = cos2 * w.skyhor.Z + sin2 * w.skyzen.Z;
        }


        /*
        ======================================================================
        pixline()
        
        Calculate the ray for pixel i, j and observer o.
        ====================================================================== */

        public static void pixline(ref Line line, ref Observer o, int i, int j)
        {
            double x, y;
            var tp = Vector.Zero;
            y = (0.5 * o.ny - j) * o.py;
            x = (i - 0.5 * o.nx) * o.px;
            tp.X = o.viewdir.X * o.fl + y * o.vhat.X + x * o.uhat.X + o.obspos.X;
            tp.Y = o.viewdir.Y * o.fl + y * o.vhat.Y + x * o.uhat.Y + o.obspos.Y;
            tp.Z = o.viewdir.Z * o.fl + y * o.vhat.Z + x * o.uhat.Z + o.obspos.Z;
            genline(ref line, ref o.obspos, ref tp);
        }


        /*
        ======================================================================
        vecsub()
        
        a = b - c for vectors.
        ====================================================================== */

        public static void vecsub(ref Vector a, ref Vector b, ref Vector c)
        {
            a.X = b.X - c.X;
            a.Y = b.Y - c.Y;
            a.Z = b.Z - c.Z;
        }


        /*
        ======================================================================
        intsplin()
        
        Returns true if the line intersects the sphere, and calculates the
        intersection point.  Eric's code returned the intersection point in a
        pointer-to-t argument.  We put it into the line's t member.
        ====================================================================== */

        public static bool intsplin(ref Line line, ref Sphere sp)
        {
            double a, b, c, d, p, q;
            a = b = 0.0;
            c = sp.radius;
            c = -c * c;

            p = line.org.X - sp.pos.X;
            q = line.dir.X;
            a += q * q;
            b += 2.0 * p * q;
            c += p * p;

            p = line.org.Y - sp.pos.Y;
            q = line.dir.Y;
            a += q * q;
            b += 2.0 * p * q;
            c += p * p;

            p = line.org.Z - sp.pos.Z;
            q = line.dir.Z;
            a += q * q;
            b += 2.0 * p * q;
            c += p * p;

            d = b * b - 4.0 * a * c;
            if (d <= 0)
                return false;

            d = Math.Sqrt(d);
            line.t = -(b + d) / (2.0 * a);
            if (line.t < SMALL)
                line.t = (d - b) / (2.0 * a);

            return line.t > SMALL;
        }


        /*
        ======================================================================
        inthor()
        
        Returns true if the line intersects the ground plane, and calculates
        the point of intersection.  Eric's code returned the parameter t in a
        pointer-to-t argument.  We put it into the line's t member.
        ====================================================================== */

        public static bool inthor(ref Line line)
        {
            if (line.dir.Z == 0.0)
                return false;
            line.t = -line.org.Z / line.dir.Z;
            return line.t > SMALL;
        }


        /*
        ======================================================================
        genline()
        
        Create a ray through the points a and b.  Eric's original code
        represented a line as an array of 6 doubles, with the origin in
        elements 0, 2, 4 and the direction in 1, 3, 5.
        ====================================================================== */

        public static void genline(ref Line line, ref Vector a, ref Vector b)
        {
            line.org.X = a.X;
            line.org.Y = a.Y;
            line.org.Z = a.Z;
            line.dir.X = b.X - a.X;
            line.dir.Y = b.Y - a.Y;
            line.dir.Z = b.Z - a.Z;
            line.t = 0.0;
        }


        /*
        ======================================================================
        dot()
        
        Dot product of 2 vectors.
        ====================================================================== */

        public static double dot(ref Vector a, ref Vector b)
        {
            return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
        }


        /*
        ======================================================================
        point()
        
        Calculate the position of a point on the line with parameter t.
        ====================================================================== */

        public static void point(ref Vector pos, double t, ref Line line)
        {
            pos.X = line.org.X + line.dir.X * t;
            pos.Y = line.org.Y + line.dir.Y * t;
            pos.Z = line.org.Z + line.dir.Z * t;
        }


        /*
        ======================================================================
        glint()
        
        Shader for shiny patch p.  The patch is the point hit by the current
        camera ray.  p lies on the sphere with index spk.  The shaded color is
        returned in brite.  Returns true if p is within a specular highlight,
        otherwise false.
        
        Eric's code passed a pointer to the sphere, but this was only used to
        skip this sphere in the shadow calculations.
        ====================================================================== */

        public static bool glint(ref Vector brite, ref Patch p, ref World w, int spk, ref Line incident)
        {
            int k, li;
            double t, r;
            var lp = Vector.Zero;
            double cosi;
            var incvec = Vector.Zero;
            var refvec = Vector.Zero;
            double ref2 = default(double);
            var line = new Line() { org = Vector.Zero, dir = Vector.Zero, t = 0 };
            const double minglint = 0.95;
            bool firstlite = true;

            for (li = 0; li < w.numlmp; li++)
            {
                vecsub(ref lp, ref w.lmp[li].shape.pos, ref p.pos);
                cosi = dot(ref lp, ref p.normal);      /* cosine of the incidence angle */
                if (cosi <= 0.0) continue;     /* facing away from the lamp */

                genline(ref line, ref p.pos, ref w.lmp[li].shape.pos);
                for (k = 0; k < w.numsp; k++)
                {
                    if (k == spk) continue;     /* sphere can't shadow itself */
                    if (intsplin(ref line, ref w.sp[k]))
                        goto end; /* shadowed by another sphere */
                }

                if (firstlite)
                {
                    incvec = incident.dir.slice();
                    reflect(ref refvec, ref p.normal, ref incvec);
                    ref2 = dot(ref refvec, ref refvec);
                    firstlite = false;
                }
                r = dot(ref lp, ref lp);
                t = dot(ref lp, ref refvec);
                t *= t / (dot(ref lp, ref lp) * ref2);
                if (t > minglint)
                {            /* it's a highlight */
                    brite.X = 1.0;
                    brite.Y = 1.0;
                    brite.Z = 1.0;
                    return true;
                }

            end: continue;
            }
            return false;
        }


        /*
        ======================================================================
        mirror()
        
        Shader for mirror patch p.  The patch is the point hit by the current
        camera ray.  The shaded color is returned in brite.
        ====================================================================== */

        public static bool mirror(ref Vector brite, ref Patch p, ref World w, ref Line incident)
        {
            var refvec = Vector.Zero;
            double t;
            var line = new Line() { org = Vector.Zero, dir = Vector.Zero, t = 0 };

            t = dot(ref p.normal, ref incident.dir);
            if (t >= 0)
            {               /* we're inside a sphere, it's dark */
                brite.X = 0.0;
                brite.Y = 0.0;
                brite.Z = 0.0;
                return false;
            }
            reflect(ref refvec, ref p.normal, ref incident.dir);
            line.org = p.pos.slice();
            line.dir = refvec.slice();
            raytrace(ref brite, ref line, ref w);   /* recursion saves the day */
            brite.X *= p.color.X;
            brite.Y *= p.color.Y;
            brite.Z *= p.color.Z;
            return true;
        }


        /*
        ======================================================================
        pixbrite()
        
        Shader for dull (matte, not shiny) patch p.  The patch is the point
        hit by the current camera ray.  p lies on the sphere with index spk.
        The shaded color is returned in brite.
        
        Eric's code passed a pointer to the sphere, but this was only used to
        skip this sphere in the shadow calculations.
        ====================================================================== */

        public static void pixbrite(ref Vector brite, ref Patch p, ref Vector ppos, ref World w, int spk)
        {
            var zenith = new Vector(0.0, 0.0, 1.0);
            const double f1 = 1.5;
            const double f2 = 0.4;
            int k, li;
            double r;
            var lp = Vector.Zero;
            double cosi;
            var line = new Line() { org = Vector.Zero, dir = Vector.Zero, t = 0 };

            /* ambient sky light */
            double diffuse = (dot(ref zenith, ref p.normal) + f1) * f2;
            brite.X = diffuse * w.illum.X * p.color.X;
            brite.Y = diffuse * w.illum.Y * p.color.Y;
            brite.Z = diffuse * w.illum.Z * p.color.Z;

            /* light from each lamp */
            for (li = 0; li < w.numlmp; li++)
            {
                vecsub(ref lp, ref w.lmp[li].shape.pos, ref ppos);
                cosi = dot(ref lp, ref p.normal);      /* cosine of the incidence angle */
                if (cosi <= 0.0) continue;     /* facing away from the lamp */

                genline(ref line, ref ppos, ref w.lmp[li].shape.pos);
                for (k = 0; k < w.numsp; k++)
                {
                    if (k == spk) continue;     /* sphere can't shadow itself */
                    if (intsplin(ref line, ref w.sp[k]))
                        goto end; /* shadowed by another sphere */
                }
                r = Math.Sqrt(dot(ref lp, ref lp));   /* light distance */
                cosi /= (r * r * r);           /* light falloff as cube of distance */
                brite.X = brite.X + cosi * p.color.X * w.lmp[li].shape.color.X;
                brite.Y = brite.Y + cosi * p.color.Y * w.lmp[li].shape.color.Y;
                brite.Z = brite.Z + cosi * p.color.Z * w.lmp[li].shape.color.Z;

            end: continue;
            }
        }


        /*
        ======================================================================
        setnorm()
        
        Calculate the normal at patch p, a point on sphere s.
        ====================================================================== */

        public static void setnorm(ref Patch p, ref Sphere s)
        {
            vecsub(ref p.normal, ref p.pos, ref s.pos);
            p.normal.X /= s.radius;
            p.normal.Y /= s.radius;
            p.normal.Z /= s.radius;
        }

        /*
        ======================================================================
        gingham()
        
        Returns which of the two ground plane tile colors is at the given
        position.  Tiles are 3 x 3 squares arranged in a checkered pattern,
        but with the pattern reflected about the X and Y axes.  The effect of
        the reflection is to make the tile at the origin 6 x 6, tiles on the
        X axis 3 x 6, and tiles on the Y axis 6 x 3.
        ====================================================================== */

        public static int gingham(ref Vector pos)
        {
            var x = pos.X;
            var y = pos.Y;
            var kx = 0;
            var ky = 0;
            if (x < 0) { x = -x; ++kx; }
            if (y < 0) { y = -y; ++ky; }
            return (int)((Math.Floor((x + kx) / 3) + Math.Floor((y + ky) / 3)) % 2);
        }


        /*
        ======================================================================
        reflect()
        
        Calculate the reflection ray y (incoming ray x reflected about the
        surface normal n).  Eric's code had some wacky cross-product stuff
        going on, with a special case for x || n.  I've replaced it with the
        standard calculation.  See for example
        
        http://paulbourke.net/geometry/reflected/
        ====================================================================== */

        public static void reflect(ref Vector y, ref Vector n, ref Vector x)
        {
            double d = dot(ref x, ref n);
            y.X = x.X - 2 * d * n.X;
            y.Y = x.Y - 2 * d * n.Y;
            y.Z = x.Z - 2 * d * n.Z;
        }
    }
}
