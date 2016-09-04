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

var BIG    = 1.0e10;
var SMALL  = 1.0e-5;  /* suggested by Edd Biddulph; originally 1.0e-3 */
var DULL   = 0;
var BRIGHT = 1;
var MIRROR = 2;


/*
======================================================================
rtmain()

Conventional entry point for the raytracer.  This renders correctly,
but it prevents the browser from doing anything until the render is
complete.
====================================================================== */

function rtmain()
{
   var line = {org:[0,0,0],dir:[0,0,0],t:0};
   var brite = [];
   var o = {};
   var w = {};
   var i, j;
   
   setup( o, w );
   initsc( o );
   
   for ( j = 0; j < o.ny; j++ ) {
      for ( i = 0; i < o.nx; i++ ) {
         pixline( line, o, i, j );
         raytrace( brite, line, w );
         ham( i, j, brite, o );
      }
   }
   cleanup( o );
}

/*
======================================================================
rtinit()
rtloop()

A more browser-friendly implementation of the rendering loop.

Initialize the scene, create the canvas, and enter the rendering loop.
Every 16 scanlines, rtloop() yields control back to the browser,
allowing it to update the canvas and render other parts of the page.
====================================================================== */

var pixel_i = 0, pixel_j = 0;
var observer = {};
var world = {};
var requestAnimFrame;


function rtinit()
{
   setup( observer, world );
   initsc( observer );
   requestAnimFrame = window.requestAnimationFrame ||
                      window.mozRequestAnimationFrame ||
                      window.webkitRequestAnimationFrame ||
                      window.msRequestAnimationFrame;
   pixel_i = pixel_j = 0;
   rtloop();
}


function rtloop()
{
   var line = {org:[0,0,0],dir:[0,0,0],t:0};
   var brite = [];

   while ( true ) {
      pixline( line, observer, pixel_i, pixel_j );
      raytrace( brite, line, world );
      ham( pixel_i, pixel_j, brite, observer );
      ++pixel_i;
      if ( pixel_i == observer.nx ) {
         pixel_i = 0;
         ++pixel_j;
         if ( pixel_j % 16 == 0 || pixel_j == observer.ny ) {
            observer.ctx.putImageData( observer.imgdata, 0, 0 );
            if ( pixel_j < observer.ny )
               requestAnimFrame( rtloop );
            break;
         }
      }
   }
}


/*
======================================================================
raytrace()

Given a camera ray and a world (or scene), calculate the color seen in
the ray direction.
====================================================================== */

function raytrace( brite, line, w )
{
   var ptch = {pos:[0,0,0],normal:[0,0,0],color:[0,0,0]}, pos = [], k;

   var tmin = BIG;
   var spnear = -1;                       /* do we see a sphere? */
   for ( k = 0; k < w.numsp; k++ )
      if ( intsplin( line, w.sp[ k ])) {
         if ( line.t < tmin ) {
            tmin = line.t;
            spnear = k;
         }
      }

   var lmpnear = -1;                      /* are we looking at a lamp? */
   for ( k = 0; k < w.numlmp; k++ )
      if ( intsplin( line, w.lmp[ k ])) {
         if ( line.t < tmin ) {
            tmin = line.t;
            lmpnear = k;
         }
      }

   if ( lmpnear != -1 ) {                 /* we see a lamp! */
      r = w.lmp[ lmpnear ].radius * w.lmp[ lmpnear ].radius;
      for ( k = 0; k < 3; k++ )
         brite[ k ] = w.lmp[ lmpnear ].color[ k ] / r;
      return;
   }

   if ( inthor( line ))                   /* do we see the ground? */
      if ( line.t < tmin ) {
         point( pos, line.t, line );
         k = gingham( pos );
         w.horizon[ k ].pos = pos.slice();
         pixbrite( brite, w.horizon[ k ], w, -1 );
         return;
      }

   if ( spnear != -1 ) {                  /* we see a sphere */
      point( ptch.pos, tmin, line );
      setnorm( ptch, w.sp[ spnear ] );
      ptch.color = w.sp[ spnear ].color.slice();
      switch ( w.sp[ spnear ].type ) {
         case BRIGHT:  if ( glint( brite, ptch, w, spnear, line )) break;
         case DULL:    pixbrite( brite, ptch, w, spnear );  break;
         case MIRROR:  mirror( brite, ptch, w, line );  break;
      }
      return;
   }
   skybrite( brite, line, w );            /* nothing else, must be sky */
}


/*
======================================================================
skybrite()

Calculate the sky color in the ray direction by interpolating between
colors at the zenith and horizon.  The interpolation parameter, sin2,
comes from sin^2(a) = z^2 / r^2, where (x,y,z) is a point on a sphere
of radius r, and a is the elevation angle above the ground plane.
====================================================================== */

function skybrite( brite, line, w )
{
   var sin2, cos2;
   sin2 = line.dir[ 2 ] * line.dir[ 2 ];
   sin2 /= ( line.dir[ 0 ] * line.dir[ 0 ] + line.dir[ 1 ] * line.dir[ 1 ] + sin2 );
   cos2 = 1.0 - sin2;
   for ( var k = 0; k < 3; k++ )
      brite[ k ] = cos2 * w.skyhor[ k ] + sin2 * w.skyzen[ k ];
}


/*
======================================================================
pixline()

Calculate the ray for pixel i, j and observer o.
====================================================================== */

function pixline( line, o, i, j )
{
   var x, y, tp = [];
   y = ( 0.5 * o.ny - j ) * o.py;
   x = ( i - 0.5 * o.nx ) * o.px;
   for ( var k = 0; k < 3; k++ )
      tp[ k ] = o.viewdir[ k ] * o.fl + y * o.vhat[ k ] +
         x * o.uhat[ k ] + o.obspos[ k ];
   genline( line, o.obspos, tp );
}


/*
======================================================================
vecsub()

a = b - c for vectors.
====================================================================== */

function vecsub( a, b, c )
{
   for ( var k = 0; k < 3; k++ )
      a[ k ] = b[ k ] - c[ k ];
}


/*
======================================================================
intsplin()

Returns true if the line intersects the sphere, and calculates the
intersection point.  Eric's code returned the intersection point in a
pointer-to-t argument.  We put it into the line's t member.
====================================================================== */

function intsplin( line, sp )
{
   var a, b, c, d, p, q;
   a = b = 0.0;
   c = sp.radius;
   c = -c * c;
   for ( var k = 0; k < 3; k++ ) {
      p = line.org[ k ] - sp.pos[ k ];
      q = line.dir[ k ];
      a += q * q;
      b += 2.0 * p * q;
      c += p * p;
   }
   d = b * b - 4.0 * a * c;
   if ( d <= 0 ) return false;
   d = Math.sqrt( d );
   line.t = -( b + d ) / ( 2.0 * a );
   if ( line.t < SMALL ) line.t = ( d - b ) / ( 2.0 * a );
   return line.t > SMALL;
}


/*
======================================================================
inthor()

Returns true if the line intersects the ground plane, and calculates
the point of intersection.  Eric's code returned the parameter t in a
pointer-to-t argument.  We put it into the line's t member.
====================================================================== */

function inthor( line )
{
   if ( line.dir[ 2 ] == 0.0 ) return false;
   line.t = -line.org[ 2 ] / line.dir[ 2 ];
   return line.t > SMALL;
}


/*
======================================================================
genline()

Create a ray through the points a and b.  Eric's original code
represented a line as an array of 6 doubles, with the origin in
elements 0, 2, 4 and the direction in 1, 3, 5.
====================================================================== */

function genline( line, a, b )
{
   for ( var k = 0; k < 3; k++ ) {
      line.org[ k ] = a[ k ];
      line.dir[ k ] = b[ k ] - a[ k ];
   }
   line.t = 0.0;
}


/*
======================================================================
dot()

Dot product of 2 vectors.
====================================================================== */

function dot( a, b )
{
   return a[ 0 ] * b[ 0 ] + a[ 1 ] * b[ 1 ] + a[ 2 ] * b[ 2 ];
}


/*
======================================================================
point()

Calculate the position of a point on the line with parameter t.
====================================================================== */

function point( pos, t, line )
{
   for ( var k = 0; k < 3; k++ )
      pos[ k ] = line.org[ k ] + line.dir[ k ] * t;
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

function glint( brite, p, w, spk, incident )
{
   var k, li, t, r, lp = [], cosi, incvec = [], refvec = [], ref2;
   var line = {org:[0,0,0],dir:[0,0,0],t:0};
   var minglint = 0.95;
   var firstlite = true;

   lamploop:  for ( li = 0; li < w.numlmp; li++ ) {
      vecsub( lp, w.lmp[ li ].pos, p.pos );
      cosi = dot( lp, p.normal );      /* cosine of the incidence angle */
      if ( cosi <= 0.0 ) continue;     /* facing away from the lamp */

      genline( line, p.pos, w.lmp[ li ].pos );
      for ( k = 0; k < w.numsp; k++ ) {
         if ( k == spk ) continue;     /* sphere can't shadow itself */
         if ( intsplin( line, w.sp[ k ]))
            continue lamploop;         /* shadowed by another sphere */
      }

      if ( firstlite ) {
         incvec = incident.dir.slice();
         reflect( refvec, p.normal, incvec );
         ref2 = dot( refvec, refvec );
         firstlite = false;
      }
      r = dot( lp, lp );
      t = dot( lp, refvec );
      t *= t / ( dot( lp, lp ) * ref2 );
      if ( t > minglint ) {            /* it's a highlight */
         for ( k = 0; k < 3; k++ )
            brite[ k ] = 1.0;
            return true;
      }
   }
   return false;
}


/*
======================================================================
mirror()

Shader for mirror patch p.  The patch is the point hit by the current
camera ray.  The shaded color is returned in brite.
====================================================================== */

function mirror( brite, p, w, incident )
{
   var k, refvec = [], t;
   var line = {org:[0,0,0],dir:[0,0,0],t:0};

   t = dot( p.normal, incident.dir );
   if ( t >= 0 ) {               /* we're inside a sphere, it's dark */
      for ( k = 0; k < 3; k++ )
         brite[ k ] = 0.0;
      return false;
   }
   reflect( refvec, p.normal, incident.dir );
   line.org = p.pos.slice();
   line.dir = refvec.slice();
   raytrace( brite, line, w );   /* recursion saves the day */
   for ( k = 0; k < 3; k++ )
      brite[ k ] *= p.color[ k ];
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

function pixbrite( brite, p, w, spk )
{
   var zenith = [ 0.0, 0.0, 1.0 ];
   var f1 = 1.5;
   var f2 = 0.4;
   var k, li, r, lp = [], cosi, diffuse;
   var line = {org:[0,0,0],dir:[0,0,0],t:0};

   /* ambient sky light */
   var diffuse = ( dot( zenith, p.normal ) + f1 ) * f2;
   for ( k = 0; k < 3; k++ )
      brite[ k ] = diffuse * w.illum[ k ] * p.color[ k ];

   /* light from each lamp */
   lamploop:  for ( li = 0; li < w.numlmp; li++ ) {
      vecsub( lp, w.lmp[ li ].pos, p.pos );
      cosi = dot( lp, p.normal );      /* cosine of the incidence angle */
      if ( cosi <= 0.0 ) continue;     /* facing away from the lamp */

      genline( line, p.pos, w.lmp[ li ].pos );
      for ( k = 0; k < w.numsp; k++ ) {
         if ( k == spk ) continue;     /* sphere can't shadow itself */
         if ( intsplin( line, w.sp[ k ]))
            continue lamploop;         /* shadowed by another sphere */
      }
      r = Math.sqrt( dot( lp, lp ));   /* light distance */
      cosi /= ( r * r * r );           /* light falloff as cube of distance */
      for ( k = 0; k < 3; k++ )
         brite[ k ] = brite[ k ] + cosi * p.color[ k ] * w.lmp[ li ].color[ k ];
   }
}


/*
======================================================================
setnorm()

Calculate the normal at patch p, a point on sphere s.
====================================================================== */

function setnorm( p, s )
{
   vecsub( p.normal, p.pos, s.pos );
   p.normal[ 0 ] /= s.radius;
   p.normal[ 1 ] /= s.radius;
   p.normal[ 2 ] /= s.radius;
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

function gingham( pos )
{
   var x = pos[ 0 ];
   var y = pos[ 1 ];
   var kx = 0;
   var ky = 0;
   if ( x < 0 ) { x = -x; ++kx; }
   if ( y < 0 ) { y = -y; ++ky; }
   return ( Math.floor(( x + kx ) / 3 ) + Math.floor(( y + ky ) / 3 )) % 2;
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

function reflect( y, n, x )
{
   var k, d, xx = [];
   d = dot( x, n );
   for ( k = 0; k < 3; k++ )
      y[ k ] = x[ k ] - 2 * d * n[ k ];
}
