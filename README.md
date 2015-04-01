Railtype-PSM-Engine
===================

**PlayStation Mobile is being discontinued**
===========================================

PSM 3D graphics engine aimed for Vita using the PSM SDK

Do not expect this project to be actively updated, but I will complete it at some point.
I am in the process of wanting to redesign and currently working out domains

One project file is for Visual studio and one is for their PSM studio as they are not compatible.

-------------------

The SDK on vita currently has **massive** timesink for gfx calls as well as **very** slow CPU vector processing compared to other PSM devices. Probably due to the vm that is being used on the vita.

Previous attempts at optimizations by pushing more work to the GPU proved unsuccessful due to the worked shoved wasnt worth the GPU strain once multiple texture units had been implemented.

* Creation for text with kerneling exists.
* Looking to make for ease of use.
* Handling for too-many-to-handle texture counts just to make sure.
* Entity handling to avoid fragmentation

![](http://puu.sh/8cSZX.jpg)

_several textures and some text_
