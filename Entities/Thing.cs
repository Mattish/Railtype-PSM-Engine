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
			xyz[2] = -5.0f;
			getModelToWorld();
			rot[0] = rot[1] += 0.005f;
		}
		
		public Matrix4 getModelToWorld(){
			modelToWorld = Matrix4.Identity;		
			modelToWorld *= Matrix4.RotationXyz(rot[0],rot[1],rot[2]);
			modelToWorld *= Matrix4.Scale(scale[0],scale[1],scale[2]);
			modelToWorld.RowW = new Vector4(xyz[0],xyz[1],xyz[2],modelToWorld.RowW.W);		
			if(Globals.COMPUTE_BY == Globals.COMPUTATION_TYPE.CPU)
				modelToWorld = modelToWorld.Transpose();
			return modelToWorld;
		}

		public void PutModelVertexIntoArray(ref float[] input, int position){
			if (Globals.COMPUTE_BY == Globals.COMPUTATION_TYPE.CPU){
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
			else if(Globals.COMPUTE_BY == Globals.COMPUTATION_TYPE.GPU_SOFT ||
			        Globals.COMPUTE_BY == Globals.COMPUTATION_TYPE.GPU_HARD){
				Array.Copy(modelVertex,0,input,position,modelVertex.Length); 
			}
		}

	}
}