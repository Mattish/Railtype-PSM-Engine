using System;
using System.Collections;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core;
using Railtype_PSM_Engine;

namespace Railtype_PSM_Engine.Entities{
	public class Thing{
		
		public float[] modelVertex, uv, scalexyzrot;
		public int number;
		public Matrix4 modelToWorld;
		public Primitive prim;
		float rand1,rand2;
		public bool updated;
		

		Thing(){
			updated = false;
			modelToWorld = Matrix4.Identity;
			scalexyzrot = new float[7]{1.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f};
			rand1 = (float)(Globals.random.NextDouble()*2)-1.0f;
			rand2 = (float)(Globals.random.NextDouble()*2)-1.0f;
		}

		public Thing(float[] model, float[] _uv, int number) : this(){
			this.number = number;
			modelVertex = model;
			uv = _uv;
			prim.Count = (ushort)(model.Length / 3);
			prim.Mode = DrawMode.Triangles;
		}

		public Thing(int amountOfVertex, int number) : this(){
			modelVertex = new float[amountOfVertex * 3];
			this.number = number;
			for(int i = 0; i < modelVertex.Length; i+=3){
				modelVertex[i] = (float)Globals.random.NextDouble();
				modelVertex[i + 1] = (float)Globals.random.NextDouble();
				modelVertex[i + 2] = (float)Globals.random.NextDouble();
			}
			prim.Count = (ushort)(modelVertex.Length / 3);
			prim.Mode = DrawMode.Triangles;
		}

		public void update(){
			scalexyzrot[3] = -5.0f;
			scalexyzrot[1] += rand1*0.05f;
			scalexyzrot[2] += rand2*0.05f;
			
			scalexyzrot[4] = scalexyzrot[5] += rand1*0.05f;
			
			//if (updated)
			UpdateModelToWorld();
		}
		
		Vector4 tmp;
		Matrix4 tmpMatrix;

		private Matrix4 UpdateModelToWorld(){
			Matrix4.RotationXyz(scalexyzrot[4], scalexyzrot[5], scalexyzrot[6], out modelToWorld);	
			Matrix4.Scale(scalexyzrot[0], scalexyzrot[0], scalexyzrot[0], out tmpMatrix);
			modelToWorld *= tmpMatrix;
			tmp.X = scalexyzrot[1];
			tmp.Y = scalexyzrot[2];
			tmp.Z = scalexyzrot[3];
			tmp.W = 1;
			modelToWorld.RowW = tmp;
			modelToWorld = modelToWorld.Transpose();
			return modelToWorld;
		}

		public void PutModelVertexIntoArray(ref float[] input, int position){
			Array.Copy(modelVertex, 0, input, position, modelVertex.Length); 
		}
		
		public void PutModelUVIntoArray(ref float[] input, int position){
			Array.Copy(uv,0,input,position,uv.Length);	
		}

	}
}