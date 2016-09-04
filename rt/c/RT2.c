/*      RT2.C
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
*/
 
#define BIG 1.0e10
#define DULL    0
#define BRIGHT  1
#define MIRROR  2
 
double dot();   /* Vector dot product */
 
struct lamp {
    double pos[3];      /* position of lamp */
    double color[3];    /* color of lamp */
    double radius;      /* size of lamp */
};
 
struct sphere {
    double pos[3];      /* position of sphere */
    double color[3];    /* color of sphere */
    double radius;      /* size of sphere */
    int type;   /* type of surface, DULL, BRIGHT or MIRROR */
};
 
struct patch {          /* a small bit of something visible */
    double pos[3];      /* position */
    double normal[3];   /* direction 90 degrees to surface */
    double color[3];    /* color of patch */
};
 
struct world {  /* everything in the universe, except observer */
    int numsp;          /* number of spheres */
    struct sphere *sp;  /* array of spheres */
    int numlmp;         /* number of lamps */
    struct lamp *lmp;   /* array of lamps */
    struct patch horizon[2]; /* alternate squares on the ground */
    double illum[3];    /* background diffuse illumination */
    double skyhor[3];   /* sky color at horizon */
    double skyzen[3];   /* sky color overhead */
};
 
struct observer {       /* now the observer */
    double obspos[3];   /* his position */
    double viewdir[3];  /* direction he is looking */
    double uhat[3];     /* left to right in view plane */
    double vhat[3];     /* down to up in view plane */
    double fl,px,py;    /* focal length and pixel sizes */
    int nx,ny;          /* number of pixels */
};
 
 
setup(o,w,skip)         /*      This function is really a stub, you should */
struct observer *o;     /*      change it to suit your needs               */
struct world *w;
int *skip;
{
 double alt,az,degtorad;
 int i,j,k;
 double t,r,tp[3],lampfac,pmin[3],pmax[3];
 double sqrt(),sin(),cos();
 
 degtorad=0.0174533;
 *skip=2;
 for (k=0; k<3; ++k) {pmin[k]=BIG; pmax[k]=-BIG;}
 
 o->obspos[0]=-10.;
 o->obspos[1]=0.;
 o->obspos[2]=4.;
 
 o->nx=320; o->ny=200;
 o->px=1.0/o->nx; o->py=0.75/o->ny;
 
 alt=-10.0; az=0.0;
 
 (o->fl)=0.028*30.0;
 
 alt*=degtorad;
 az*=degtorad;
 
 o->viewdir[0]=cos(az)*cos(alt);
 o->viewdir[1]=sin(az)*cos(alt);
 o->viewdir[2]=sin(alt);
 
 o->uhat[0]=sin(az);
 o->uhat[1]=-cos(az);
 o->uhat[2]=0.0;
 
 o->vhat[0]=-cos(az)*sin(alt);
 o->vhat[1]=-sin(az)*sin(alt);
 o->vhat[2]=cos(alt);
 
 w->numsp=1;
 
 w->sp=(struct sphere *)malloc(sizeof(struct sphere)*w->numsp);
 if (!w->sp)
        {printf("\nUnable to allocate memory"); cleanup(0);}
 
 w->sp[0].pos[0]=0.0;
 w->sp[0].pos[1]=0.0;
 w->sp[0].pos[2]=2.0;
 w->sp[0].radius=3.0;
 w->sp[0].type=DULL;
 w->sp[0].color[0]=1.0;
 w->sp[0].color[1]=0.7;
 w->sp[0].color[2]=0.7;
 
 w->numlmp=1;
 
 w->lmp=(struct lamp *)malloc(sizeof(struct lamp)*w->numlmp);
 if (!w->lmp)
        {printf("\nUnable to allocate lamp memory"); cleanup("lamp alloc");}
 
 w->lmp[0].pos[0]=-100;
 w->lmp[0].pos[1]=-100;
 w->lmp[0].pos[2]=100;
 w->lmp[0].radius=10.0;
 
 w->lmp[0].color[0]=1.0;
 w->lmp[0].color[1]=1.0;
 w->lmp[0].color[2]=1.0;
 
 w->horizon[0].normal[0]=0.0;
 w->horizon[0].normal[1]=0.0;
 w->horizon[0].normal[2]=1.0;
 w->horizon[1].normal[0]=0.0;
 w->horizon[1].normal[1]=0.0;
 w->horizon[1].normal[2]=1.0;
 
 w->horizon[0].color[0]=1.0;
 w->horizon[0].color[1]=0.1;
 w->horizon[0].color[2]=0.2;
 
 w->horizon[1].color[0]=0.1;
 w->horizon[1].color[1]=1.;
 w->horizon[1].color[2]=0.2;
 
 w->illum[0]=0.3;
 w->illum[1]=0.3;
 w->illum[2]=0.3;
 
 w->skyzen[0]=0.1;
 w->skyzen[1]=0.1;
 w->skyzen[2]=1.0;
 
 w->skyhor[0]=0.2;
 w->skyhor[1]=0.2;
 w->skyhor[2]=0.4;
 
 
 lampfac=BIG;                   /* modify the lamp brightness so as to */
 for (i=0; i<w->numsp; ++i)     /* get the right exposure              */
        for (j=0; j<w->numlmp; ++j)
                {vecsub(tp,w->sp[i].pos,w->lmp[j].pos);
                 r=sqrt(dot(tp,tp));
                 r-=w->sp[i].radius;
                 for (k=0; k<3; ++k)
                        {t=w->sp[i].color[k]*w->lmp[j].color[k]/(r*r);
                         if (t == 0.0) continue;
                         t=(1.0-w->sp[i].color[k]*w->illum[k])/t;
                         if (t<lampfac) lampfac=t;
                        }
                }
 
 for (j=0; j<w->numlmp; ++j)
        for (k=0; k<3; ++k)
                w->lmp[j].color[k]*=lampfac;
 printf("\nlampfac=%f",lampfac);
}
 
 
ham(i,j,brite)
int i,j;
double brite[3];
{int k,pix[3];
 for (k=0; k<3; ++k)
        {pix[k]=16.0*brite[k]+0.5;
         if (pix[k] < 0) pix[k]=0;
         if (pix[k] > 15) pix[k]=15;
        }
 ham2(i,j,pix);
}