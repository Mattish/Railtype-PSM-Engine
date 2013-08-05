using System;
using System.Collections;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core;
using Railtype_PSM_Engine;

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
			z = -5.0f;
			getModelToWorld();
			rotx = roty += 0.005f;
		}
		
		public Matrix4 getModelToWorld(){
			modelToWorld = Matrix4.Identity;			
			modelToWorld *= Matrix4.RotationXyz(rotx,roty,rotz);
			modelToWorld *= Matrix4.Scale(scalex,scaley,scalez);
			modelToWorld.RowW = new Vector4(x,y,z,modelToWorld.RowW.W);		
			if(Globals.CPU_CALCULATION)
				modelToWorld = modelToWorld.Transpose();
			return modelToWorld;
		}

		public void PutModelVertexIntoArray(ref float[] input, int position){
			if (Globals.CPU_CALCULATION){
				Vector4 tmpv = new Vector4(0,0,0,1);
				Vector4 result = new Vector4(0,0,0,1);
				for(int i = 0; i < modelVertex.Length; i+=3 ){
					tmpv.X = modelVertex[i];
					tmpv.Y = modelVertex[i+1];
					tmpv.Z = modelVertex[i+2];
					result = getModelToWorld().Transform(tmpv);
					input[position+i] = result.X;
					input[position+i+1] = result.Y;
					input[position+i+2] = result.Z;
				}
			}
			else{
				Array.Copy(modelVertex,0,input,position,modelVertex.Length); 
			}
		}

	}
}