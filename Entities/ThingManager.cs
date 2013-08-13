using System;
using System.Collections.Generic;
using Railtype_PSM_Engine.Entities;
using Sce.PlayStation.Core.Graphics;

namespace Railtype_PSM_Engine{
	public class ThingManager{
		float[] vertex, matrixNumber;
		List<Thing> disposed, things;
		Primitive[] prims;
					
		public ThingManager(){
			vertex = new float[32000];
			matrixNumber = new float[vertex.Length / 3];
		}
	}
}

