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

I've done proper optimizations on packing data into the VBO and the likes. Framerate is now far greater then it was. Currently with just positional data I can push the VBO to it's maximum capacity with system inplace for replacing disposed Things. This is looking to be improved even more.

~~Ok, so calculating on the CPU is pretty much not worth it unless you are doing very little in terms of vertex count. The previous results are lower down.
These are 2 GPU methods I chose:
GPU_SOFT: Same as before with pushing the matrix in batches to uniform in 10s. float attribute per vertex for matrix array number;
GPU_HARD: Attributes for rotation, scale, translation and position each have 3 floats per vertex.~~

Current state of the application @ 100 and 800 Things

```
GPU_SOFT - Amount of Things:100 - fps:61 Memory usage: 2120KB
CheckNewThings():0ms
foreach update:1ms
CheckVertexBuffer():0ms
Draw():1ms


GPU_SOFT - Amount of Things:800 - fps:35 Memory usage: 2485KB
CheckNewThings():0ms
foreach update:12ms
CheckVertexBuffer():0ms
Draw():14ms
```