using System;
using System.Collections.Generic;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core;
using Railtype_PSM_Engine.Entities;

namespace Railtype_PSM_Engine{
	public static class Globals{
		
		public static int AmountToPush = 10;
		public static List<Thing> things;
		public static int vertexSize;
		public static VertexBuffer modelVertexBuffer;
		static private GraphicsContext _graphic;
		static public void Setup(GraphicsContext graphic){
			_graphic = graphic;
			float[] vertices = new float[36];
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
			
			vertices[18]=-1.0f;   // 4
            vertices[19]=-1.0f;   
            vertices[20]=-1.0f;   
 
            vertices[21]=-1.0f;   // 5
            vertices[22]=0.0f;   
            vertices[23]=-1.0f;   
 
            vertices[24]=0.0f;   // 6
            vertices[25]=-1.0f;   
            vertices[26]=-1.0f;   
			// -----------------
			vertices[27]=-1.0f;   // 5
            vertices[28]=0.0f;   
            vertices[29]=-1.0f;   
 
            vertices[30]=0.0f;   // 6
            vertices[31]=-1.0f;   
            vertices[32]=-1.0f;   

			vertices[33]=0.0f;   // 7
            vertices[34]=0.0f;   
            vertices[35]=-1.0f;
			
			things = new List<Thing>(100);
			for(int i = 0; i < 500;i++){
				things.Add(new Thing(vertices,i));
			}
		}
		
		static public void DoDrawing(ref GraphicsContext gc, ref ShaderProgram shaderProgram){
			Primitive[] primArray;
			int primCounter = 0, HowManyToPush = 0;
			Matrix4[] matrixArray = PackModelVertexBuffer(ref gc, ref shaderProgram, out primArray);
			         

			while (primCounter < primArray.Length){
				HowManyToPush = primArray.Length - primCounter > 10 ? 10 : primArray.Length - primCounter;
				int k = shaderProgram.FindUniform("modelToWorld");
				shaderProgram.SetUniformValue(k,matrixArray,0,primCounter,HowManyToPush);			
				gc.DrawArrays(primArray,primCounter,HowManyToPush);
				primCounter += HowManyToPush;
			}
			modelVertexBuffer.Dispose();
			
		}
		
		static Matrix4[] PackModelVertexBuffer(ref GraphicsContext gc, ref ShaderProgram shaderProgram, out Primitive[] input){
			input = new Primitive[things.Count];
			float[] vertex;
			Matrix4[] matrixUniform = new Matrix4[things.Count];
			float[] matrixNumber;
			int amountOfVertex = 0, matrixCounter = 0, position = 0;
			for(int i = 0; i < things.Count;i++){ 
				amountOfVertex += things[i].VertexCount;
				matrixUniform[i] = things[i].modelToWorld;
			}
			
			vertex = new float[amountOfVertex*3];
			matrixNumber = new float[amountOfVertex];
			for(int i = 0; i < things.Count;i++){ // Now for each, put them into the arrays
				things[i].PutModelVertexIntoArray(ref vertex,(position*3)); // For verticies
				
				for(int j = position; j < (position+things[i].VertexCount);j++){ // for matrixnumber
					matrixNumber[j] = matrixCounter % AmountToPush;
				}
				
				matrixCounter++;
				input[i] = new Primitive(DrawMode.Triangles,position,things[i].VertexCount,0);
				position += (things[i].VertexCount);
			}			
			
            modelVertexBuffer = new VertexBuffer(amountOfVertex, VertexFormat.Float3,VertexFormat.Float);
            modelVertexBuffer.SetVertices(0, vertex);
			modelVertexBuffer.SetVertices(1, matrixNumber);
            gc.SetVertexBuffer(0, modelVertexBuffer);
			return matrixUniform;
		}
		
	}
}

