using System;
using System.Collections.Generic;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core;
using System.Threading;
using Railtype_PSM_Engine.Entities;

namespace Railtype_PSM_Engine{
	public static class Globals{
		public enum COMPUTATION_TYPE{
			CPU = 0,
			GPU_SOFT = 1,
			GPU_HARD = 2
		};
		
		public static int AmountPerPush = 10;
		public static COMPUTATION_TYPE COMPUTE_BY = COMPUTATION_TYPE.GPU_SOFT;
		public static int frameCount;
		public static ShaderProgram cpu, gpuSoft, gpuHard;
        public static Matrix4 cameraToWorld;
		public static List<Thing> things;
		public static int vertexSize;
		public static VertexBuffer modelVertexBuffer;
		static private GraphicsContext _graphic;
		public static Matrix4 ViewProjection;
		public static Random random;
		static public void Setup(GraphicsContext graphic){
			frameCount = 0;
			_graphic = graphic;
			random = new Random();
			things = new List<Thing>(100);
 			cameraToWorld = Matrix4.Identity;

			Vector3[] points = new Vector3[]{new Vector3(1,1,1),
					new Vector3(1,1,-1),
					new Vector3(1,-1,1),
					new Vector3(1,-1,-1),
					new Vector3(-1,1,1),
					new Vector3(-1,1,-1),
					new Vector3(-1,-1,1),
					new Vector3(-1,-1,-1)};

			int[] verticies = new int[]{0,1,5,0,5,4, 
										0,1,3,0,3,2,
										1,5,7,1,7,3,
										2,3,7,2,7,6,
										0,4,6,0,6,2,
										4,5,7,4,7,6};
			float[] cubevertex = new float[108];
			for(int i = 0; i < verticies.Length;i++){
				cubevertex[(i*3)] = points[verticies[i]].X;
				cubevertex[(i*3)+1] = points[verticies[i]].Y;
				cubevertex[(i*3)+2] = points[verticies[i]].Z;
			}
			for(int i = 0; i < 1;i++){
				things.Add(new Thing((3*36*250),i));
			}
		}

	}
}