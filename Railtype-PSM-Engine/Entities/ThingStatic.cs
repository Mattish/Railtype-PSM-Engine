using System;
using Railtype_PSM_Engine.Util;

namespace Railtype_PSM_Engine.Entities{
	public class ThingStatic : Thing{
		public ThingStatic(string name) : base(name){
		}
		
		public override void Update(){
			scalexyzrot[3] = 2.0f;
			scalexyzrot[5] = 1f;
			//UpdateModelToWorld(false);
		}
	}
}

