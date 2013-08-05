using System;
using System.Collections.Generic;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core;
using System.Threading;
using Railtype_PSM_Engine.Entities;

namespace Railtype_PSM_Engine{
	public static class Globals{

		public static int AmountPerPush = 10;
		public static readonly bool CPU_CALCULATION = false;
		public static ShaderProgram cpu, gpu;
        public static Matrix4 cameraToWorld;
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
				things.Add(new Thing(cubevertex,i));
			}
		}

		static public void DoDrawing(ref GraphicsContext gc){
			Matrix4 VP = buildProjectionMatrix(ref gc, ref cameraToWorld); // Build camera to world projection			
			
			System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
			Primitive[] primArray;
			sw.Start();
			Matrix4[] matriciesToPush;
			if (Globals.CPU_CALCULATION){
				matriciesToPush = PackModelVertexBuffer(ref gc, ref cpu, out primArray);
	            gc.SetShaderProgram(cpu);						
	            cpu.SetUniformValue(0, ref VP);				
				gc.DrawArrays(primArray,0,primArray.Length);
					
			}
			else if (!Globals.CPU_CALCULATION){ // Pushing Matricies to shader as uniform
				matriciesToPush = PackModelVertexBuffer(ref gc, ref gpu, out primArray);
	            gc.SetShaderProgram(gpu);						
	            gpu.SetUniformValue(0, ref VP);
				
				int primCounter = 0, HowManyToPush = 0;
				while (primCounter < primArray.Length){
					HowManyToPush = primArray.Length - primCounter > 10 ? 10 : primArray.Length - primCounter;
					int k = gpu.FindUniform("modelToWorld");
					gpu.SetUniformValue(k,matriciesToPush,0,primCounter,HowManyToPush);			
					gc.DrawArrays(primArray,primCounter,HowManyToPush);
					primCounter += HowManyToPush;
				}
			}
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
				matrixUniform[i] = things[i].getModelToWorld();
				HowManyWeGot++;
			}
			vertex = new float[amountOfVertex*3];
			
			if(Globals.CPU_CALCULATION){
				for(int i = 0; i < things.Count;i++){ // put them into the arrays
					things[i].PutModelVertexIntoArray(ref vertex,(position*3)); // For verticies
	
					matrixCounter++;
					input[i] = new Primitive(DrawMode.Triangles,position,things[i].VertexCount,0);
					position += (things[i].VertexCount);
				}			
	            modelVertexBuffer = new VertexBuffer(amountOfVertex, VertexFormat.Float3);
	            modelVertexBuffer.SetVertices(0, vertex);
			}
			else if (!Globals.CPU_CALCULATION){ // Pushing Matricies to shader as uniform
				float[] matrixNumber = new float[amountOfVertex];
				for(int i = 0; i < things.Count;i++){ // Now for each, put them into the arrays
					things[i].PutModelVertexIntoArray(ref vertex,(position*3)); // For verticies
	
					for(int j = position; j < (position+things[i].VertexCount);j++){ // for matrixnumber
						matrixNumber[j] = matrixCounter % AmountPerPush;
					}
	
					matrixCounter++;
					input[i] = new Primitive(DrawMode.Triangles,position,things[i].VertexCount,0);
					position += (things[i].VertexCount);
				}		
				modelVertexBuffer = new VertexBuffer(amountOfVertex, VertexFormat.Float3,VertexFormat.Float);
	            modelVertexBuffer.SetVertices(0, vertex);
				modelVertexBuffer.SetVertices(1, matrixNumber);
			}
            gc.SetVertexBuffer(0, modelVertexBuffer);
			return matrixUniform;
		}
		
		static public Matrix4 buildProjectionMatrix(ref GraphicsContext gc, ref Matrix4 cameraToWorld){
			Matrix4 worldToCamera = Matrix4.Identity;
			cameraToWorld.Inverse(out worldToCamera);
			Matrix4 cameraToProjection = Matrix4.Perspective(FMath.Radians(45.0f), gc.Screen.AspectRatio, 1.0f, 1000.0f);
			Matrix4 tmp = worldToCamera * cameraToProjection;
			return tmp;
		}

	}
}