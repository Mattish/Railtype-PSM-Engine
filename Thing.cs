using System;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core;

namespace Railtype_PSM_Engine{
	public class Thing{
		
		int primNumber;
		float scalex,scaley,scalez,x,y,z,rotx,roty,rotz;
		float[] modelVertex;
		Matrix4 modelToWorld;
			
		public Thing (){
			primNumber = Globals.getPrimitiveSlot();
			modelToWorld = Matrix4.Identity;
		}
		
		public void PutModelVertexIntoArray(ref float[] input, int position){
			Array.Copy(modelVertex,0,input,position,modelVertex.Length); 	
		}
		
		
	}
}

