using System;
using Railtype_PSM_Engine.Util;

namespace Railtype_PSM_Engine.Entities{
	public class ThingStatic : Thing{
		public ThingStatic(WaveFrontObject wfo) : base(wfo){
		}
		
		public override void Update(){
			scalexyzrot[3] = 2.0f;
			UpdateModelToWorld(false);
		}
	}
}

