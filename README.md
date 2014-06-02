Railtype-PSM-Engine
===================

PSM 3D graphics engine aimed for Vita using the PSM SDK

**>> Currently being re-written <<**


-------------------

The SDK on vita currently has **massive** timesink for gfx calls as well as **very** slow CPU vector processing compared to other PSM devices. Probably due to the vm that is being used on the vita.

Previous attempts at optimizations by pushing more work to the GPU proved unsuccessful due to the worked shoved wasnt worth the GPU strain once multiple texture units had been implemented. 

![](http://puu.sh/8cSZX.jpg)

_several textures and some text_

%[](http://mattish.co.uk/github/output.webm)
