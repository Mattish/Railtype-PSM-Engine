Railtype-PSM-Engine
===================

PSM 3D graphics engine aimed for Vita using the PSM SDK

-------------------

This is a side project and currently not anything serious. Mainly wanting to look into what kind of performance can be achieved. The SDK on vita currently has /MASSIVE/ timesink for gfx calls, so attempting to limit them as much as possible.


* Batch modelToWorld matrix calls for each entity to the shader in X amounts(10 is doable with some uniform room left).
* Vertex attribute for the matrix array position
* Aiming for something with complex models or lots of small simple ones

These are run on the device(vita) in release mode.
~~~~~~~~~~~~
A quad here being 18 vertex.

46 FPS on 500 quads with batch sending matrix as uniform
53 FPS on 500 quads with single call with CPU model positioning