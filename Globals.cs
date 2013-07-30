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
		static int drawCounter;
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
			for(int i = 0; i < 1000;i++){
				things.Add(new Thing(vertices,i));
			}
		}
		
		static public void DoDrawing(ref GraphicsContext gc, ref ShaderProgram shaderProgram){
			drawCounter = 0;
			Primitive[] primArray;
			int primAmount = 0;
			while ((primAmount = PackModelVertexBuffer(ref gc, ref shaderProgram, out primArray)) > 0){
				gc.DrawArrays(primArray,0,primAmount);
			}
			
		}
		
		static int PackModelVertexBuffer(ref GraphicsContext gc, ref ShaderProgram shaderProgram, out Primitive[] input){
			input = new Primitive[11];
			
			if (drawCounter == things.Count)
				return 0;
			float[] vertex;
			Matrix4[] matrixUniform = new Matrix4[11];
			float[] matrixNumber;
			int amountOfVertex = 0, matrixCounter = 0, position = 0, HowManyWeGot = 0;
			for(int i = drawCounter; i < things.Count;i++){ 
				amountOfVertex += things[i].VertexCount;
				matrixUniform[i-drawCounter] = things[i].modelToWorld;
				HowManyWeGot++;
				if (HowManyWeGot == 10)
					break;
			}
			int k = shaderProgram.FindUniform("modelToWorld");
			shaderProgram.SetUniformValue(k,matrixUniform,0,0,HowManyWeGot);
			
			vertex = new float[amountOfVertex*3];
			matrixNumber = new float[amountOfVertex];
			for(int i = drawCounter; i < (drawCounter+HowManyWeGot);i++){ // Now for each, put them into the arrays THE FIRST 10
				things[i].PutModelVertexIntoArray(ref vertex,(position*3)); // For verticies
				
				for(int j = position; j < (position+things[i].VertexCount);j++){ // for matrixnumber
					matrixNumber[j] = matrixCounter;
				}
				
				matrixCounter++;
				input[i-drawCounter] = new Primitive(DrawMode.Triangles,position,things[i].VertexCount,0);
				position += (things[i].VertexCount);
			}			
			
            modelVertexBuffer = new VertexBuffer(amountOfVertex, VertexFormat.Float3,VertexFormat.Float);
            modelVertexBuffer.SetVertices(0, vertex);
			modelVertexBuffer.SetVertices(1, matrixNumber);
            gc.SetVertexBuffer(0, modelVertexBuffer);
			drawCounter += HowManyWeGot;
			return HowManyWeGot;
		}
		
	}
}

