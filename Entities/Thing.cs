using System;
using System.Collections;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core;

namespace Railtype_PSM_Engine.Entities{
	public class Thing{
		
		float scalex,scaley,scalez,x,y,z,rotx,roty,rotz;
		float[] modelVertex;
		public int VertexCount;
		public Matrix4 modelToWorld;
		int number;
		float var1;
			
		public Thing (){
			VertexCount = 0;
			modelToWorld = Matrix4.Identity;
			modelToWorld.RowW = modelToWorld.RowW.Add(new Vector4(0.0f,0.0f,-1.0f,0.0f));
		}
		
		public Thing(float[] model){
			modelVertex = model;
			VertexCount = model.Length;
			modelToWorld = Matrix4.Identity;
			modelToWorld.RowW = modelToWorld.RowW.Add(new Vector4(0.0f,0.0f,-1.0f,0.0f));
		}
		
		public Thing(float[] model, int input){
			modelVertex = model;
			VertexCount = model.Length;
			modelToWorld = Matrix4.Identity;
			modelToWorld.RowW = modelToWorld.RowW.Add(new Vector4(0.2f*input,0.0f,-2.0f,0.0f));
			var1 = 0.0f;
		}
		
		public Thing(int amountOfVertex, int input){
			modelVertex = new float[amountOfVertex*3];
			VertexCount = amountOfVertex;
			number = input;
			for(int i = 0; i < modelVertex.Length; i+=3){
				modelVertex[i] = (float)Globals.random.NextDouble();
				modelVertex[i+1] = (float)Globals.random.NextDouble();
				modelVertex[i+2] = (float)Globals.random.NextDouble();
			}
		}
		
		public void update(){
			var1 += 0.005f;
			modelToWorld = Matrix4.Identity;
			modelToWorld.RowW = modelToWorld.RowW.Add(new Vector4(0.2f,0.0f,-2.0f,0.0f));
			modelToWorld *= Matrix4.RotationZ(var1);
		}
		
		public void PutModelVertexIntoArray(ref float[] input, int position){
			Array.Copy(modelVertex,0,input,position,modelVertex.Length); 	
		}
		
	}
}

