using System;
using System.Collections;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core;
using Railtype_PSM_Engine;

namespace Railtype_PSM_Engine.Entities{
	public class Thing{
		
		public float[] scalexyzrot;
		public int number, textureNumber;
		public Matrix4 modelToWorld;
		public float rand1,rand2;
		public bool draw, disposable;
		public string modelName;
		public Primitive prim;
		public int vertexCount;
		public ushort vertexIndex;

		public Thing(){
			scalexyzrot = new float[7]{1.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f};
			modelToWorld = Matrix4.Identity;
			rand1 = (float)(Globals.random.NextDouble()*2)-1.0f;
			rand2 = (float)(Globals.random.NextDouble()*2)-1.0f;
			draw = true;
			vertexCount = 0;
			vertexIndex = 0;
		}
		
		public Thing(string modelName_) : this(){
			modelName = modelName_;
			prim.Count = (ushort)Globals.modelManager.GetModel(modelName_).models[0].indices.Length;
			prim.Mode = DrawMode.Triangles;
			vertexCount = (ushort)(Globals.modelManager.GetModel(modelName_).models[0].verticies.Length / 3);
		}
		
		public virtual void Update(){
			UpdateModelToWorld(true);
		}
		
		Vector4 tmp;
		Matrix4 tmpMatrix;
		protected void UpdateModelToWorld(bool cpu){
			if (cpu){
				Matrix4.RotationXyz(scalexyzrot[4], scalexyzrot[5], scalexyzrot[6], out modelToWorld);	
				Matrix4.Scale(scalexyzrot[0], scalexyzrot[0], scalexyzrot[0], out tmpMatrix);
				modelToWorld *= tmpMatrix;
				tmp.X = scalexyzrot[1];
				tmp.Y = scalexyzrot[2];
				tmp.Z = scalexyzrot[3];
				tmp.W = 1;
				modelToWorld.RowW = tmp;
			}
			else{
				modelToWorld.ColumnX = new Vector4(scalexyzrot[0],	scalexyzrot[0],scalexyzrot[0],0);
				modelToWorld.ColumnY = new Vector4(scalexyzrot[1],	scalexyzrot[2],scalexyzrot[3],0);
				modelToWorld.ColumnZ = new Vector4(scalexyzrot[4],	scalexyzrot[5],scalexyzrot[6],0);
				modelToWorld.ColumnW = new Vector4(0,0,0,0);	
			}
		}

	}
}