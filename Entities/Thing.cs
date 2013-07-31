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
		float var1;
		int number;
			
		public Thing (){
			VertexCount = 0;
			modelToWorld = Matrix4.Identity;
			modelToWorld.RowW = modelToWorld.RowW.Add(new Vector4(0.0f,0.0f,-1.0f,0.0f));
		}
		
		public Thing(float[] model){
			modelVertex = model;
			VertexCount = model.Length/3;
			modelToWorld = Matrix4.Identity;
			modelToWorld.RowW = modelToWorld.RowW.Add(new Vector4(0.0f,0.0f,-1.0f,0.0f));
		}
		
		public Thing(float[] model, int input){
			number = input;
			modelVertex = model;
			VertexCount = model.Length/3;
			modelToWorld = Matrix4.Identity;
			//modelToWorld *= Matrix4.RotationX(0.75f);
			modelToWorld.ColumnW = modelToWorld.ColumnW.Add(new Vector4(0.2f*input,0.0f,-2.0f,0.0f));
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
			modelToWorld = Matrix4.Identity;
			modelToWorld *= Matrix4.RotationZ(var1);			
			modelToWorld.ColumnW = modelToWorld.ColumnW.Add(new Vector4(0.05f*number,0.0f,-5.0f,0.0f));
			var1+= 0.005f;
		}
		
		public void PutModelVertexIntoArray(ref float[] input, int position){
			Vector4 tmpv = new Vector4(0,0,0,1); // Working out positions on CPU rather then passing matrix
			Vector4 result = new Vector4(0,0,0,1);
			for(int i = 0; i < modelVertex.Length; i+=3 ){
				tmpv.X = modelVertex[i];
				tmpv.Y = modelVertex[i+1];
				tmpv.Z = modelVertex[i+2];
				result = modelToWorld.Transform(tmpv);
				input[position+i] = result.X;
				input[position+i+1] = result.Y;
				input[position+i+2] = result.Z;
			}
			
			//Array.Copy(modelVertex,0,input,position,modelVertex.Length); 	Old method
		}
		
	}
}

