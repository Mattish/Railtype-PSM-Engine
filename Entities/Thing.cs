using System;
using System.Collections;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core;
using Railtype_PSM_Engine;

namespace Railtype_PSM_Engine.Entities{
	public class Thing{
		
		public float[] modelVertex, scale, xyz, rot;
		public int VertexCount;
		public Matrix4 modelToWorld;
		int number;

		public Thing (){
			modelToWorld = Matrix4.Identity;
			scale = new float[3]{1.0f,1.0f,1.0f};
			xyz = new float[3]{0.0f,0.0f,0.0f};
			rot = new float[3]{0.0f,0.0f,0.0f};
		}

		public Thing(float[] model) : this(){
			modelVertex = model;
			VertexCount = model.Length/3;
		}

		public Thing(float[] model, int input) : this(){
			number = input;
			modelVertex = model;
			VertexCount = model.Length/3;
		}

		public Thing(int amountOfVertex, int input) : this(){
			modelVertex = new float[amountOfVertex*3];
			number = input;
			for(int i = 0; i < modelVertex.Length; i+=3){
				modelVertex[i] = (float)Globals.random.NextDouble();
				modelVertex[i+1] = (float)Globals.random.NextDouble();
				modelVertex[i+2] = (float)Globals.random.NextDouble();
			}
			VertexCount = modelVertex.Length/3;
		}

		public void update(){
			xyz[2] = -5.0f;
			getModelToWorld();
			rot[0] = rot[1] += 0.005f;
		}
		
		Vector4 tmp;
		Matrix4 tmpMatrix;
		public Matrix4 getModelToWorld(){
			Matrix4.RotationXyz(rot[0],rot[1],rot[2], out modelToWorld);	
			//modelToWorld *= Matrix4.Translation(xyz[0],xyz[1],xyz[2]);			
			//modelToWorld *= Matrix4.RotationXyz(rot[0],rot[1],rot[2]);
			Matrix4.Scale(scale[0],scale[1],scale[2], out tmpMatrix);
			modelToWorld *= tmpMatrix;
			//modelToWorld.RowW = new Vector4(xyz[0],xyz[1],xyz[2],1);
			tmp.X = xyz[0];
			tmp.Y = xyz[1];
			tmp.Z = xyz[2];
			tmp.W = 1;
			modelToWorld.RowW = tmp;
			return modelToWorld;
		}

		public void PutModelVertexIntoArray(ref float[] input, int position){
			Array.Copy(modelVertex,0,input,position,modelVertex.Length); 
		}

	}
}