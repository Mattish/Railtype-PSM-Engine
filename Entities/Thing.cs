using System;
using System.Collections;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core;
using Railtype_PSM_Engine;

namespace Railtype_PSM_Engine.Entities{
	public class Thing{
		
		public float[] modelVertex, scale, xyz, rot;
		public int number;
		public Matrix4 modelToWorld;
		public Primitive prim;
		public bool updated;
		

		Thing(){
			updated = false;
			modelToWorld = Matrix4.Identity;
			scale = new float[3]{1.0f,1.0f,1.0f};
			xyz = new float[3]{0.0f,0.0f,0.0f};
			rot = new float[3]{0.0f,0.0f,0.0f};
		}

		public Thing(float[] model) : this(){
			modelVertex = model;
			prim.Count = (ushort)(model.Length / 3);
			prim.Mode = DrawMode.Triangles;
		}

		public Thing(float[] model, int number) : this(){
			this.number = number;
			modelVertex = model;
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
			updated = false;
			
			xyz[2] = -5.0f;
			rot[0] = rot[1] += 0.005f;
			updated = true;
			
			//if (updated)
				UpdateModelToWorld();
		}
		
		Vector4 tmp;
		Matrix4 tmpMatrix;

		private Matrix4 UpdateModelToWorld(){
			Matrix4.RotationXyz(rot[0], rot[1], rot[2], out modelToWorld);	
			//modelToWorld *= Matrix4.Translation(xyz[0],xyz[1],xyz[2]);			
			//modelToWorld *= Matrix4.RotationXyz(rot[0],rot[1],rot[2]);
			Matrix4.Scale(scale[0], scale[1], scale[2], out tmpMatrix);
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
			Array.Copy(modelVertex, 0, input, position, modelVertex.Length); 
		}

	}
}