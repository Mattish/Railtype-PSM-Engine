using System;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core;

namespace Railtype_PSM_Engine.Entities{
	public class Thing{
		
		float scalex,scaley,scalez,x,y,z,rotx,roty,rotz;
		float[] modelVertex;
		public int VertexCount;
		public Matrix4 modelToWorld;
			
		public Thing (){
			VertexCount = 0;
			modelToWorld = Matrix4.Identity;
			modelToWorld.RowW = modelToWorld.RowW.Add(new Vector4(0.0f,0.0f,-1.0f,0.0f));
		}
		
		public Thing(float[] model){
			modelVertex = model;
			VertexCount = 3;
			modelToWorld = Matrix4.Identity;
			modelToWorld.RowW = modelToWorld.RowW.Add(new Vector4(0.0f,0.0f,-1.0f,0.0f));
		}
		
		public Thing(float[] model, int input){
			modelVertex = model;
			VertexCount = 3;
			modelToWorld = Matrix4.Identity;
			modelToWorld.RowW = modelToWorld.RowW.Add(new Vector4(0.2f*input,0.0f,-2.0f,0.0f));
		}
		
		public void PutModelVertexIntoArray(ref float[] input, int position){
			Array.Copy(modelVertex,0,input,position,modelVertex.Length); 	
		}
		
	}
}

