using System;
using System.Collections.Generic;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core;
using Railtype_PSM_Engine.Entities;

namespace Railtype_PSM_Engine{
	public static class Globals{
		public static List<Thing> things;
		public static int vertexSize;
		public static VertexBuffer modelVertexBuffer;
		static private GraphicsContext _graphic;
		static int counter;
		static public void Setup(GraphicsContext graphic){
			_graphic = graphic;
			float[] vertices = new float[9];
            vertices[0]=0.0f;   // x0
            vertices[1]=0.0f;   // y0
            vertices[2]=0.0f;   // z0
 
            vertices[3]=0.0f;   // x1
            vertices[4]=1.0f;   // y1
            vertices[5]=0.0f;   // z1
 
            vertices[6]=1.0f;   // x2
            vertices[7]=0.0f;   // y2
            vertices[8]=0.0f;   // z2
			
			things = new List<Thing>(100);
			for(int i = 0; i < 20;i++){
				things.Add(new Thing(vertices,i));
			}
		}
		
		static public void DoDrawing(ref GraphicsContext gc, ref ShaderProgram shaderProgram){
			Primitive[] tmp = PackModelVertexBuffer(ref gc, ref shaderProgram); // THE FIRST 10 TEST
			gc.DrawArrays(tmp,0,10); // THE FIRST 10 TEST
		}
		
		static Primitive[] PackModelVertexBuffer(ref GraphicsContext gc, ref ShaderProgram shaderProgram){
			Primitive[] prims = new Primitive[10];
			float[] vertex;
			Matrix4[] matrixUniform = new Matrix4[10];
			float[] matrixNumber;
			int amountOfVertex = 0, matrixCounter = 0, position = 0;
			
			for(int i = 0; i < 10;i++){ // How many vertex THE FIRST 10
				amountOfVertex += things[i].VertexCount;
				matrixUniform[i] = things[i].modelToWorld;
			}
			int k = shaderProgram.FindUniform("modelToWorld");
			shaderProgram.SetUniformValue(k,matrixUniform,0,0,10);
			
			vertex = new float[amountOfVertex*3];
			matrixNumber = new float[amountOfVertex];
			for(int i = 0; i < 10;i++){ // Now for each, put them into the arrays THE FIRST 10
				things[i].PutModelVertexIntoArray(ref vertex,(position*3)); // For verticies
				
				for(int j = position; j < (position+things[i].VertexCount);j++){ // for matrixnumber
					matrixNumber[j] = matrixCounter;
				}
				
				matrixCounter++;
				prims[i] = new Primitive(DrawMode.Triangles,position,things[i].VertexCount,0);
				position += (things[i].VertexCount);
			}			
			
            modelVertexBuffer = new VertexBuffer(amountOfVertex, VertexFormat.Float3,VertexFormat.Float);
            modelVertexBuffer.SetVertices(0, vertex);
			modelVertexBuffer.SetVertices(1, matrixNumber);
            gc.SetVertexBuffer(0, modelVertexBuffer);
			return prims;
		}
		
	}
}

