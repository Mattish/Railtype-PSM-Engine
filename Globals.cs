using System;
using System.Collections.Generic;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core;
using System.Threading;
using Railtype_PSM_Engine.Entities;

namespace Railtype_PSM_Engine{
	public static class Globals{
		
		public static int AmountPerPush = 10;
		
		public static List<Thing> things;
		public static int vertexSize;
		public static VertexBuffer modelVertexBuffer;
		static private GraphicsContext _graphic;
		public static Matrix4 ViewProjection;
		public static Random random;
		static public void Setup(GraphicsContext graphic){
			_graphic = graphic;
			random = new Random();
			things = new List<Thing>(100);
			
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
				things.Add(new Thing(cubevertex,i));
			}
		}
		
		static public void DoDrawing(ref GraphicsContext gc, ref ShaderProgram shaderProgram){
			System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
			Primitive[] primArray;
			Matrix4[] matriciesToPush = PackModelVertexBuffer(ref gc, ref shaderProgram, out primArray);
			sw.Start();
			
			gc.DrawArrays(primArray,0,primArray.Length);
			modelVertexBuffer.Dispose();
			
			sw.Stop();
			//Console.WriteLine("Draw/dispose loop took:" + sw.ElapsedMilliseconds + "ms");
		}
		
		static Matrix4[] PackModelVertexBuffer(ref GraphicsContext gc, ref ShaderProgram shaderProgram, out Primitive[] input){
			float[] vertex;
			Matrix4[] matrixUniform = new Matrix4[things.Count];
			input = new Primitive[things.Count];
			int amountOfVertex = 0, matrixCounter = 0, position = 0, HowManyWeGot = 0;
			for(int i = 0; i < things.Count;i++){ 
				amountOfVertex += things[i].VertexCount;
				matrixUniform[i] = things[i].modelToWorld;
				HowManyWeGot++;
			}
			
			vertex = new float[amountOfVertex*3];
			for(int i = 0; i < things.Count;i++){ // Now for each, put them into the arrays
				things[i].PutModelVertexIntoArray(ref vertex,(position*3)); // For verticies
				
				matrixCounter++;
				input[i] = new Primitive(DrawMode.Triangles,position,things[i].VertexCount,0);
				position += (things[i].VertexCount);
			}			
            modelVertexBuffer = new VertexBuffer(amountOfVertex, VertexFormat.Float3);
            modelVertexBuffer.SetVertices(0, vertex);
            gc.SetVertexBuffer(0, modelVertexBuffer);
			return matrixUniform;
		}
		
	}
}

