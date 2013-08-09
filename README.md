Railtype-PSM-Engine
===================

PSM 3D graphics engine aimed for Vita using the PSM SDK

-------------------

This is a side project and currently not anything serious. Mainly wanting to look into what kind of performance can be achieved. The SDK on vita currently has /MASSIVE/ timesink for gfx calls, so attempting to limit them as much as possible.


* Batch modelToWorld matrix calls for each entity to the shader in X amounts(10 is doable with some uniform room left).
* Vertex attribute for the matrix array position
* Aiming for something with complex models or lots of small simple ones

as per https://psm.playstation.net/static/general/dev/en/sdk_docs/overview_graphics_en.html ; "MaxVertexUniformVectors 128"
So cant push more then maybe 10 matrix per draw call even with a simple shader.

These are run on the device(vita) in release mode.

Ok, so calculating on the CPU is pretty much not worth it unless you are doing very little in terms of vertex count. The previous results are lower down.

These are 2 GPU methods I chose:
GPU_SOFT: Same as before with pushing the matrix in batches to uniform in 10s. float attribute per vertex for matrix array number;
GPU_HARD: Attributes for rotation, scale, translation and position each have 3 floats per vertex.

# Disclaimer: The methods I use to pack up all the data for this may not be the best, who knows until someone else checks #

~~~
100 Cubes
GPU_SOFT(green): 60fps
GPU_HARD(red): 60fps
CPU(blue): 12fps

250 Cubes
GPU_SOFT(green): 37fps
GPU_HARD(red): 27fps
CPU: Takes too long.

1 model with same verticies count as 250 cubes (3*36*250)
GPU_SOFT(green): 60fps
GPU_HARD(red): 52fps
CPU: Takes too long.

(36*250) models with 3 vertex
GPU_SOFT(green): 18fps
GPU_HARD(red): 2fps
CPU: Takes too long.

~~~


Older results between GPU_SOFT & CPU

~~~
A quad here being 18 vertex.

CPU-based(single call with CPU model positioning)
53 FPS 500 quads 
43 FPS 500 2quads(36 vertex)

GPU-based(batch sending matrix as uniform in 10s)
46 FPS on 500 quads 
27 FPS on 500 2quads


With random models

CPU-based:
19fps 500 random model of 50 vertex each
22fps 250 random model of 100 vertex each
12fps 100 random model of 500 vertex each
24fps 100 random model of 250 vertex each

GPU-based:
20fps 500 random model of 50 vertex each
24fps 250 random model of 100 vertex each
15fps 100 random model of 500 vertex each
29fps 100 random model of 250 vertex each
~~~
