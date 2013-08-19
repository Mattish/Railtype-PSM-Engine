Railtype-PSM-Engine
===================

PSM 3D graphics engine aimed for Vita using the PSM SDK

![](http://mattish.co.uk/github/railgun.jpg)

Can whack out a bunch of stuff while keeping 60fps. Hardware/VM limitations seems to be the major downfall. When many objects overlap, blending chugs the fps pretty hard.

-------------------

The SDK on vita currently has /MASSIVE/ timesink for gfx calls, so attempting to limit them as much as possible.

as per https://psm.playstation.net/static/general/dev/en/sdk_docs/overview_graphics_en.html ; "MaxVertexUniformVectors 128"
So cant push more then maybe 15 matrix per draw call even with a simple shader.

I've done proper optimizations on packing data into the VBO and the likes. Framerate is now far greater then it was. Currently with just positional data I can push the VBO to it's maximum capacity with system inplace for replacing disposed Things.

![](http://mattish.co.uk/github/example.jpg)

_The UVs in this example are identical to the cube vertex data_