Railtype-PSM-Engine
===================

PSM 3D graphics engine aimed for Vita using the PSM SDK

![](http://mattish.co.uk/github/railgun.jpg)

Can whack out a bunch of stuff while keeping 60fps. Hardware/VM limitations seems to be the major downfall. When many objects overlap, blending chugs the fps pretty hard.

WaveFront obj loader is now in place. Example image below uses a simple box from 3ds max exported.

-------------------

The SDK on vita currently has **massive** timesink for gfx calls as well as **very** slow CPU processing compared to other PSM devices, so attempting to limit them as much as possible and shove stuff onto the GPU.

as per https://psm.playstation.net/static/general/dev/en/sdk_docs/overview_graphics_en.html ; "MaxVertexUniformVectors 128"
So cant push more then maybe 15 matrix per draw call even with a simple shader.

I've done proper optimizations on packing data into the VBO and the likes. Framerate is now far greater then it was. Currently with just positional data I can push the VBO to it's maximum capacity with system inplace for replacing disposed Things.

![](http://mattish.co.uk/github/example2.jpg)

_400 textured cubes flying out from the center while rotating. Holds at 60 fps on the device.
Can pumped up to 2000 cubes before hitting VBO limit, and as explained matrix per drawcall drops fps to about 25~ before it crashes_