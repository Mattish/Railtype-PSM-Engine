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
		public static Random random;
		public static VertexBuffer modelVertexBuffer;
		static private GraphicsContext _graphic;
		static public void Setup(GraphicsContext graphic){
			_graphic = graphic;
			random = new Random();
			
			things = new List<Thing>(100);
			for(int i = 0; i < 100;i++){
				things.Add(new Thing(250,i));
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

