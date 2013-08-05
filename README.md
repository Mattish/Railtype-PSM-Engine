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
~~~~~~~~~~~~
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

