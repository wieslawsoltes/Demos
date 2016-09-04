/*
======================================================================
rt3.js

Ernie Wright  23 Feb 2014

Initialize the raytracer's drawing surface (an HTML5 canvas).  This is
a Javascript version of Eric Graham's rt3.c raytracer source file.
====================================================================== */


/*
======================================================================
initsc()

Create a canvas, a 2D drawing context, and an ImageData to hold the
pixel values.
====================================================================== */

function initsc( o )
{
   var canvas = document.getElementById( "jglrcanvas" );
   o.ctx = canvas.getContext( "2d" );
   o.imgdata = o.ctx.createImageData( o.nx, o.ny );
   o.ctx.clearRect( 0, 0, canvas.width, canvas.height );
}


/*
======================================================================
cleanup()

Display the completed render.
====================================================================== */

function cleanup( o )
{
   o.ctx.putImageData( o.imgdata, 0, 0 );
}


/*
======================================================================
ham()

Scale the floating-point RGB value of a pixel into bytes and store the
result in the ImageData.
====================================================================== */

function ham( i, j, brite, o )
{
   var d, ch, level;
   
   d = 4 * ( j * o.nx + i );
   for ( ch = 0; ch < 3; ch++ ) {
      level = Math.round( Math.max( Math.min( brite[ ch ] * 255, 255 ), 0 ));
      o.imgdata.data[ d + ch ] = level;
   }
   o.imgdata.data[ d + 3 ] = 255;
}
