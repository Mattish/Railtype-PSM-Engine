Railtype-PSM-Engine
===================

PSM 3D graphics engine aimed for Vita using the PSM SDK

**Currently being re-written**

stuff done:
WaveFront obj loader
Font rendering in place
basic entity system

-------------------

The SDK on vita currently has **massive** timesink for gfx calls as well as **very** slow CPU processing compared to other PSM devices.

Previous attempts at optimizations by pushing more work to the GPU proved unsuccessful due to the worked shoved wasnt worth the GPU strain once multiple texture units had been implemented. 

![](http://puu.sh/8cSZX.jpg)

_ascii 256 chars rendered to a texture with custom ttf font able to render out any string_
_5 other textures rendering at 60+fps_
