/*
======================================================================
rt2.js

Ernie Wright  23 Feb 2014

Initialize the raytracer's observer and world.  This is a Javascript
version of Eric Graham's rt2.c raytracer source file.  I've added the
object and scene data from his robots.dat, and I've generalized the
setup() function so that it can be used with other scenes.

The original rt2.c contained the following notice.

RT2.C
Copyright 1987 Eric Graham
All rights reserved.
This file may not be copied, modified or uploaded to a bulletin
board system, except as provided below.
Permission is granted to make a reasonable number of backup copies,
in order that it may be used to generate executable code for use
on a single computer system.
Permission is granted to modify this code and use the modified code
for non commercial use by the original purchaser of this software,
and provided that this notice is included in the modified version.
====================================================================== */

// objects are made of spheres
// types are dull (0), shiny (1), mirror (2)

var rtspheres = [
{pos: [-0.9,-2.1,5.3], color: [0.9,0.9,0.9], radius: 0.6, type: 2},
{pos: [-1.1,1.9,5.9], color: [0.9,0.9,0.9], radius: 0.6, type: 2},
{pos: [-0.4,-1.2,6.8], color: [0.9,0.9,0.9], radius: 0.6, type: 2},
{pos: [0,0,6.1], color: [1,0.7,0.7], radius: 0.5, type: 1},
{pos: [0.02,0,6.12], color: [0.2,0.1,0.1], radius: 0.5, type: 1},
{pos: [-0.4,0.2,6.1], color: [0.1,0.1,1], radius: 0.15, type: 1},
{pos: [-0.4,-0.2,6.1], color: [0.1,0.1,1], radius: 0.15, type: 1},
{pos: [0,0,5.5], color: [1,0.7,0.7], radius: 0.2, type: 1},
{pos: [0,0,4.6], color: [1,0.1,0.1], radius: 0.8, type: 1},
{pos: [0,0,4.34], color: [1,0.1,0.1], radius: 0.76, type: 1},
{pos: [0,0,4.08], color: [1,0.1,0.1], radius: 0.72, type: 1},
{pos: [0,0,3.82], color: [1,0.1,0.1], radius: 0.68, type: 1},
{pos: [0,0,3.56], color: [1,0.1,0.1], radius: 0.64, type: 1},
{pos: [0,0,3.3], color: [1,0.1,0.1], radius: 0.6, type: 1},
{pos: [0,0.6,2.9], color: [1,0.7,0.7], radius: 0.2, type: 1},
{pos: [-0.1,0.6,2.68333], color: [1,0.7,0.7], radius: 0.2, type: 1},
{pos: [-0.2,0.6,2.46667], color: [1,0.7,0.7], radius: 0.2, type: 1},
{pos: [-0.3,0.6,2.25], color: [1,0.7,0.7], radius: 0.2, type: 1},
{pos: [-0.4,0.6,2.03333], color: [1,0.7,0.7], radius: 0.2, type: 1},
{pos: [-0.5,0.6,1.81667], color: [1,0.7,0.7], radius: 0.2, type: 1},
{pos: [-0.6,0.6,1.6], color: [1,0.7,0.7], radius: 0.2, type: 1},
{pos: [-0.571429,0.6,1.37143], color: [1,0.7,0.7], radius: 0.185714, type: 1},
{pos: [-0.542857,0.6,1.14286], color: [1,0.7,0.7], radius: 0.171429, type: 1},
{pos: [-0.514286,0.6,0.914286], color: [1,0.7,0.7], radius: 0.157143, type: 1},
{pos: [-0.485714,0.6,0.685714], color: [1,0.7,0.7], radius: 0.142857, type: 1},
{pos: [-0.457143,0.6,0.457143], color: [1,0.7,0.7], radius: 0.128571, type: 1},
{pos: [-0.428571,0.6,0.228571], color: [1,0.7,0.7], radius: 0.114286, type: 1},
{pos: [-0.4,0.6,0], color: [1,0.7,0.7], radius: 0.1, type: 1},
{pos: [0,-0.6,2.9], color: [1,0.7,0.7], radius: 0.2, type: 1},
{pos: [0.0333333,-0.6,2.68333], color: [1,0.7,0.7], radius: 0.2, type: 1},
{pos: [0.0666667,-0.6,2.46667], color: [1,0.7,0.7], radius: 0.2, type: 1},
{pos: [0.1,-0.6,2.25], color: [1,0.7,0.7], radius: 0.2, type: 1},
{pos: [0.133333,-0.6,2.03333], color: [1,0.7,0.7], radius: 0.2, type: 1},
{pos: [0.166667,-0.6,1.81667], color: [1,0.7,0.7], radius: 0.2, type: 1},
{pos: [0.2,-0.6,1.6], color: [1,0.7,0.7], radius: 0.2, type: 1},
{pos: [0.228571,-0.6,1.37143], color: [1,0.7,0.7], radius: 0.185714, type: 1},
{pos: [0.257143,-0.6,1.14286], color: [1,0.7,0.7], radius: 0.171429, type: 1},
{pos: [0.285714,-0.6,0.914286], color: [1,0.7,0.7], radius: 0.157143, type: 1},
{pos: [0.314286,-0.6,0.685714], color: [1,0.7,0.7], radius: 0.142857, type: 1},
{pos: [0.342857,-0.6,0.457143], color: [1,0.7,0.7], radius: 0.128571, type: 1},
{pos: [0.371429,-0.6,0.228571], color: [1,0.7,0.7], radius: 0.114286, type: 1},
{pos: [0.4,-0.6,0], color: [1,0.7,0.7], radius: 0.1, type: 1},
{pos: [0,-0.7,5.1], color: [1,0.7,0.7], radius: 0.2, type: 1},
{pos: [-0.0333333,-0.783333,4.95], color: [1,0.7,0.7], radius: 0.2, type: 1},
{pos: [-0.0666667,-0.866667,4.8], color: [1,0.7,0.7], radius: 0.2, type: 1},
{pos: [-0.1,-0.95,4.65], color: [1,0.7,0.7], radius: 0.2, type: 1},
{pos: [-0.133333,-1.03333,4.5], color: [1,0.7,0.7], radius: 0.2, type: 1},
{pos: [-0.166667,-1.11667,4.35], color: [1,0.7,0.7], radius: 0.2, type: 1},
{pos: [-0.2,-1.2,4.2], color: [1,0.7,0.7], radius: 0.2, type: 1},
{pos: [-0.328571,-1.31429,4.18571], color: [1,0.7,0.7], radius: 0.185714, type: 1},
{pos: [-0.457143,-1.42857,4.17143], color: [1,0.7,0.7], radius: 0.171429, type: 1},
{pos: [-0.585714,-1.54286,4.15714], color: [1,0.7,0.7], radius: 0.157143, type: 1},
{pos: [-0.714286,-1.65714,4.14286], color: [1,0.7,0.7], radius: 0.142857, type: 1},
{pos: [-0.842857,-1.77143,4.12857], color: [1,0.7,0.7], radius: 0.128571, type: 1},
{pos: [-0.971429,-1.88571,4.11429], color: [1,0.7,0.7], radius: 0.114286, type: 1},
{pos: [-1.1,-2,4.1], color: [1,0.7,0.7], radius: 0.1, type: 1},
{pos: [0,0.7,5.1], color: [1,0.7,0.7], radius: 0.2, type: 1},
{pos: [-0.0333333,0.783333,4.95], color: [1,0.7,0.7], radius: 0.2, type: 1},
{pos: [-0.0666667,0.866667,4.8], color: [1,0.7,0.7], radius: 0.2, type: 1},
{pos: [-0.1,0.95,4.65], color: [1,0.7,0.7], radius: 0.2, type: 1},
{pos: [-0.133333,1.03333,4.5], color: [1,0.7,0.7], radius: 0.2, type: 1},
{pos: [-0.166667,1.11667,4.35], color: [1,0.7,0.7], radius: 0.2, type: 1},
{pos: [-0.2,1.2,4.2], color: [1,0.7,0.7], radius: 0.2, type: 1},
{pos: [-0.314286,1.3,4.28571], color: [1,0.7,0.7], radius: 0.185714, type: 1},
{pos: [-0.428571,1.4,4.37143], color: [1,0.7,0.7], radius: 0.171429, type: 1},
{pos: [-0.542857,1.5,4.45714], color: [1,0.7,0.7], radius: 0.157143, type: 1},
{pos: [-0.657143,1.6,4.54286], color: [1,0.7,0.7], radius: 0.142857, type: 1},
{pos: [-0.771429,1.7,4.62857], color: [1,0.7,0.7], radius: 0.128571, type: 1},
{pos: [-0.885714,1.8,4.71429], color: [1,0.7,0.7], radius: 0.114286, type: 1},
{pos: [-1,1.9,4.8], color: [1,0.7,0.7], radius: 0.1, type: 1}];

var rtlamps = [               // lights in the scene
   { pos: [ -100.0, 50.0, 150.0 ], radius: 15.0, color: [ 1, 1, 1 ]}];

var rtobserver = {
   obspos: [ -10, -4, 5.5 ],  // camera position
   nx: 640,                   // output image width, pixels
   ny: 480,                   // output image height, pixels
   hratio: 0.75,              // image height to width ratio (frame aspect)
   alt: -10.0,                // altitude of the viewing direction, degrees
   az: 20.0,                  // azimuth of the viewing direction, degrees
   fl: 35.0 };                // focal length, mm

var rttiles = [               // floor tile colors
   { pos: [ 0, 0, 0 ], normal: [ 0, 0, 1 ], color: [ 1.5, 1.5, 0 ]},
   { pos: [ 0, 0, 0 ], normal: [ 0, 0, 1 ], color: [ 0, 1.5, 0 ]}];

var rtambient = [ 0.25, 0.25, 0.25 ];  // ambient color
var rtskyzen = [ 0.1, 0.1, 1.0 ];      // sky zenith color
var rtskyhor = [ 0.7, 0.7, 1.0 ];      // sky horizon color


/*
======================================================================
setup()

Initialize the observer and the world.
====================================================================== */

function setup( o, w )
{
   var i, j, k, r, t, lampfac;
   var degtorad = Math.PI / 180.0;

   o.obspos = rtobserver.obspos;
   o.nx = rtobserver.nx;
   o.ny = rtobserver.ny;
   o.hratio = rtobserver.hratio;
   o.alt = rtobserver.alt * degtorad;
   o.az = rtobserver.az * degtorad;
   o.fl = rtobserver.fl * 0.028;

   o.px = 1.0 / o.nx;
   o.py = o.hratio / o.ny;
   o.viewdir = [ Math.cos( o.az ) * Math.cos( o.alt ),
                 Math.sin( o.az ) * Math.cos( o.alt ),
                 Math.sin( o.alt )];
   o.uhat = [ Math.sin( o.az ), -Math.cos( o.az ), 0.0 ];
   o.vhat = [ -Math.cos( o.az ) * Math.sin( o.alt ),
              -Math.sin( o.az ) * Math.sin( o.alt ),
               Math.cos( o.alt )];

   w.numsp = rtspheres.length;
   w.sp = rtspheres;

   w.numlmp = rtlamps.length;
   w.lmp = rtlamps;

   w.horizon = rttiles;
   w.illum = rtambient;
   w.skyzen = rtskyzen;
   w.skyhor = rtskyhor;

   /* modify the lamp brightness so as to */
   /* get the right exposure              */

   lampfac = BIG;
   var tp = [];
   for ( i = 0; i < w.numsp; i++ )
      for ( j = 0; j < w.numlmp; j++ ) {
         vecsub( tp, w.sp[ i ].pos, w.lmp[ j ].pos );
         r = Math.sqrt( dot( tp, tp )) - w.sp[ i ].radius;
         for ( k = 0; k < 3; k++ ) {
            t = w.sp[ i ].color[ k ] * w.lmp[ j ].color[ k ] / ( r * r );
            if ( t == 0.0 ) continue;
            t = ( 1.0 - w.sp[ i ].color[ k ] * w.illum[ k ]) / t;
            if ( t < lampfac ) lampfac = t;
         }
      }

   for ( j = 0; j < w.numlmp; j++ )
      for ( k = 0; k < 3; k++ )
         w.lmp[ j ].color[ k ] *= lampfac;
}
