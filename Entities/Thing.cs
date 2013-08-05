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

		public Thing (){
			modelToWorld = Matrix4.Identity;
			scalex = scaley = scalez = 1.0f;
			x = y = z = rotx = roty = rotz = 0.0f;
		}

		public Thing(float[] model) : this(){
			modelVertex = model;
			VertexCount = model.Length/3;
		}

		public Thing(float[] model, int input) : this(){
			number = input;
			modelVertex = model;
			VertexCount = model.Length/3;
			modelToWorld = Matrix4.Identity;
		}

		public Thing(int amountOfVertex, int input) : this(){
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
			modelToWorld *= Matrix4.RotationXyz(rotx,roty,rotz);
			//modelToWorld.ColumnW = modelToWorld.ColumnW.Add(new Vector4(0.05f*number,0.0f,-5.0f,0.0f));
			modelToWorld.ColumnW = new Vector4(x,y,z,modelToWorld.ColumnW.W);
			modelToWorld *= Matrix4.Scale(scalex,scaley,scalez);
			roty += 0.005f;
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