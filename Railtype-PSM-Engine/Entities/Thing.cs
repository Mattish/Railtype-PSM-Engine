using System;
using System.Collections;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core;
using Railtype_PSM_Engine;

namespace Railtype_PSM_Engine.Entities{
	public class Thing{
		
		public float[] modelVertex, uv, scalexyzrot;
		public ushort[] indicies;
		public int number, vertexCount, textureNumber;
		public Matrix4 modelToWorld;
		public ushort vertexIndex;
		public Primitive prim;
		public float rand1,rand2;
		public bool draw;
		

		Thing(){
			scalexyzrot = new float[7]{1.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f};
			modelToWorld = Matrix4.Identity;
			rand1 = (float)(Globals.random.NextDouble()*2)-1.0f;
			rand2 = (float)(Globals.random.NextDouble()*2)-1.0f;
			vertexIndex = 0;
		}
		
		public Thing(ref float[] verticies, ref float[] uvs, ref ushort[] _indicies) : this(){
			modelVertex = new float[verticies.Length];
			Array.Copy(verticies,modelVertex,modelVertex.Length);
			uv = new float[uvs.Length];
			Array.Copy(uvs,uv,uv.Length);
			indicies = new ushort[_indicies.Length];
			Array.Copy(_indicies,indicies,indicies.Length);
			prim.Count = (ushort)indicies.Length;
			prim.Mode = DrawMode.Triangles;
			vertexCount = (ushort)(modelVertex.Length/3);
		}
		
		public Thing(Railtype_PSM_Engine.Util.WaveFrontObject wfo) : this(){
			modelVertex = new float[wfo.models[0].pos.Length];
			Array.Copy(wfo.models[0].pos,modelVertex,modelVertex.Length);
			uv = new float[wfo.models[0].uv.Length];
			Array.Copy(wfo.models[0].uv,uv,uv.Length);
			indicies = new ushort[wfo.models[0].indices.Length];
			Array.Copy(wfo.models[0].indices,indicies,indicies.Length);
			prim.Count = (ushort)indicies.Length;
			prim.Mode = DrawMode.Triangles;
			vertexCount = (ushort)(modelVertex.Length/3);
		}

		public virtual void Update(){
			//scalexyzrot[3] = 150.0f * rand1;
			/*scalexyzrot[3] = 20.0f;
			scalexyzrot[1] += rand1*0.05f;
			scalexyzrot[2] += rand2*0.05f;
			scalexyzrot[4] = scalexyzrot[5] += rand1*0.05f;*/
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
				//modelToWorld = modelToWorld.Transpose(); ??????
			}
			else{
				modelToWorld.ColumnX = new Vector4(scalexyzrot[0],	scalexyzrot[0],scalexyzrot[0],0);
				modelToWorld.ColumnY = new Vector4(scalexyzrot[1],	scalexyzrot[2],scalexyzrot[3],0);
				modelToWorld.ColumnZ = new Vector4(scalexyzrot[4],	scalexyzrot[5],scalexyzrot[6],0);
				modelToWorld.ColumnW = new Vector4(0,0,0,0);	
			}
		}

		public void PutModelVertexIntoArray(ref float[] input, int position){
			Array.Copy(modelVertex, 0, input, position, modelVertex.Length); 
		}
		
		public void PutModelUVIntoArray(ref float[] input, int position){
			Array.Copy(uv,0,input,position,uv.Length);	
		}
		
		public void PutIndiciesIntoArray(ref ushort[] input, int position){
			Array.Copy(indicies,0,input,position,indicies.Length);	
		}

	}
}