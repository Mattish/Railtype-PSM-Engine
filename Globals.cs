using System;
using System.Collections.Generic;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core;
using Railtype_PSM_Engine.Entities;

namespace Railtype_PSM_Engine{
	public static class Globals{
		
		public static int AmountPerPush = 10;
		
		public static List<Thing> things;
		public static int vertexSize;
		public static VertexBuffer modelVertexBuffer;
		static private GraphicsContext _graphic;
		public static Matrix4 ViewProjection;
		static public void Setup(GraphicsContext graphic){
			_graphic = graphic;
			float[] vertices = new float[18];
            vertices[0]=0.0f;   // 0
            vertices[1]=0.0f;   
            vertices[2]=0.0f;   
 
            vertices[3]=0.0f;   // 1
            vertices[4]=1.0f;   
            vertices[5]=0.0f;   
 
            vertices[6]=1.0f;   // 2
            vertices[7]=0.0f;   
            vertices[8]=0.0f;   
			// -----------------
			vertices[9]=0.0f;   // 1
            vertices[10]=1.0f;   
            vertices[11]=0.0f;   
 
            vertices[12]=1.0f;   // 2
            vertices[13]=0.0f;   
            vertices[14]=0.0f;   
			
			vertices[15]=1.0f;   // 3
            vertices[16]=1.0f;   
            vertices[17]=0.0f;   
			
			things = new List<Thing>(23);
			for(int i = 0; i < 500;i++){
				things.Add(new Thing(vertices,i));
			}
		}
		
		static public void DoDrawing(ref GraphicsContext gc, ref ShaderProgram shaderProgram){
			System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
			Primitive[] primArray;
			Matrix4[] matriciesToPush = PackModelVertexBuffer(ref gc, ref shaderProgram, out primArray);
			sw.Start();
			
			//int k = shaderProgram.FindUniform("modelToWorld");
			//shaderProgram.SetUniformValue(k,matriciesToPush,0,matriciesToPush.Length,matriciesToPush.Length);
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

