using System;
using System.Collections.Generic;
using System.Diagnostics;
using Railtype_PSM_Engine.Entities;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Input;
using Sce.PlayStation.Core.Imaging;

namespace Railtype_PSM_Engine{
	public class AppMain{
		private GraphicsContext graphics;
		float[] vertex;
		Matrix4[] matrixUniform;
		Matrix4 cameraToProjection;
		float[] matrixNumber;
		Primitive[] primArray;
		public int counter,fpsCounter;
		Stopwatch sw;
 
        public static void Main (string[] args){
			AppMain appmain = new AppMain();
            appmain.Initialize ();
 
            while (true) {
                SystemEvents.CheckEvents();
                appmain.Update();
                appmain.Render();
            }
        }
 
        public void Initialize (){
            graphics = new GraphicsContext();
			Globals.Setup(graphics);
			Globals.gpuSoft = new ShaderProgram("/Application/shaders/gpuSoft.cgx");			
			
			//GPUSoft
			int k = Globals.gpuSoft.FindAttribute("a_Position");
			Globals.gpuSoft.SetAttributeBinding(k, "a_Position");			
			k = Globals.gpuSoft.FindAttribute("matrixNumber");
			Globals.gpuSoft.SetAttributeBinding(k, "matrixNumber");
			k = Globals.gpuSoft.FindUniform("WorldViewProj");
            Globals.gpuSoft.SetUniformBinding(k, "WorldViewProj");			
			k = Globals.gpuSoft.FindUniform("modelToWorld");
			Globals.gpuSoft.SetUniformBinding(k, "modelToWorld");
			
			sw = new Stopwatch();
			sw.Start();
			
			cameraToProjection = Matrix4.Perspective(FMath.Radians(45.0f), graphics.Screen.AspectRatio, 1.0f, 1000.0f);
        }
 
        public void Update (){
			foreach(Thing thing in Globals.things){
				thing.update();		
			}
			
			GamePadData gpd = GamePad.GetData(0);
			if (gpd.ButtonsDown.HasFlag(GamePadButtons.Cross)){
				Globals.things.Add(new Thing(208,Globals.frameCount));
			}
		}

        public void Render (){
			graphics.SetClearColor (1.0f, 1.0f, 1.0f, 1.0f);
            graphics.Clear();
							
			counter++;
			
            DoDrawing(ref graphics);
            graphics.SwapBuffers();
			if (sw.ElapsedMilliseconds > 1000){
				Console.WriteLine(Globals.COMPUTE_BY.ToString() + " - Amount of Things:" + Globals.things.Count + " - fps:" + fpsCounter + " Memory usage: " + (GC.GetTotalMemory(true)/1024) + "KB");
				fpsCounter = 0;
				sw.Reset();
				sw.Start();
			}
			fpsCounter++;
        }
		
		public void DoDrawing(ref GraphicsContext gc){
			Matrix4 VP = buildProjectionMatrix(ref gc); // Build camera to world projection			
			System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
			sw.Start();
			PackModelVertexBuffer(ref gc);
			sw.Stop();
			if (Globals.frameCount % 60 == 0)
				Console.WriteLine("Packing " + Globals.COMPUTE_BY.ToString() + " took :" + sw.ElapsedMilliseconds + "ms");
			sw.Reset();
			sw.Start();
			if (Globals.COMPUTE_BY == Globals.COMPUTATION_TYPE.GPU_SOFT){ // Pushing Matricies to shader as uniform
	            gc.SetShaderProgram(Globals.gpuSoft);
				
				int k = Globals.gpuSoft.FindUniform("WorldViewProj");
	            Globals.gpuSoft.SetUniformValue(k, ref VP);
				
				int primCounter = 0, HowManyToPush = 0;
				while (primCounter < primArray.Length){
					HowManyToPush = primArray.Length - primCounter > 10 ? 10 : primArray.Length - primCounter;
					k = Globals.gpuSoft.FindUniform("modelToWorld");
					Globals.gpuSoft.SetUniformValue(k,matrixUniform,0,primCounter,HowManyToPush);			
					gc.DrawArrays(primArray,primCounter,HowManyToPush);
					primCounter += HowManyToPush;
				}
			}
			//Globals.modelVertexBuffer.Dispose();
			sw.Stop();
			if (Globals.frameCount % 60 == 0)
				Console.WriteLine("Draw & Dispose " + Globals.COMPUTE_BY.ToString() + " took :" + sw.ElapsedMilliseconds + "ms");
			
			Globals.frameCount++;
		}
		
		void PackModelVertexBuffer(ref GraphicsContext gc){
			Stopwatch sww = new Stopwatch();
			sww.Start();
			bool matrixChanged = true, primChanged = false;
			
			if (matrixUniform == null)
				matrixUniform = new Matrix4[Globals.things.Count];
			else if (matrixUniform.Length < Globals.things.Count)
				Array.Resize<Matrix4>(ref matrixUniform,Globals.things.Count*2);
			
			if (primArray == null)
				primArray = new Primitive[Globals.things.Count];
			else if (primArray.Length < Globals.things.Count)
				Array.Resize<Primitive>(ref primArray,Globals.things.Count*2);
			
			
			int amountOfVertex = 0, matrixCounter = 0, position = 0, HowManyWeGot = 0;
			
			for(int i = 0; i < Globals.things.Count;i++){ 
				amountOfVertex += Globals.things[i].VertexCount;
				if (matrixChanged)
					matrixUniform[i] = Globals.things[i].getModelToWorld();
				HowManyWeGot++;
			}
			
			if (vertex == null)
				vertex = new float[amountOfVertex*3];
			else if (vertex.Length < amountOfVertex*3)
				Array.Resize<float>(ref vertex,(amountOfVertex*3)*2);
			
			if (matrixNumber == null)
				matrixNumber = new float[amountOfVertex];
			else if (matrixNumber.Length < amountOfVertex)
				Array.Resize<float>(ref matrixNumber,amountOfVertex*2);
				
			for(int i = 0; i < Globals.things.Count;i++){ // Now for each, put them into the arrays
				Globals.things[i].PutModelVertexIntoArray(ref vertex,(position*3)); // For verticies
				sww.Reset();
				sww.Start();
				float[] number = new float[]{(i % Globals.AmountPerPush)};
				ArrayFillComplex(ref matrixNumber,position,ref number,Globals.things[i].VertexCount);
				sww.Stop();
				//Console.WriteLine("doing inner packing took: " + sww.ElapsedMilliseconds + "ms");
				matrixCounter++;
				primArray[i].Set(DrawMode.Triangles,position,Globals.things[i].VertexCount,0);
				position += (Globals.things[i].VertexCount);
			}					
			sww.Stop();
			//Console.WriteLine("took: " + sww.ElapsedMilliseconds + "ms");	
			
			if (Globals.modelVertexBuffer == null)
				Globals.modelVertexBuffer = new VertexBuffer(amountOfVertex, VertexFormat.Float3,VertexFormat.Float);
			else if(Globals.modelVertexBuffer.VertexCount != amountOfVertex){
				Globals.modelVertexBuffer.Dispose();
				Globals.modelVertexBuffer = new VertexBuffer(amountOfVertex, VertexFormat.Float3,VertexFormat.Float);				
			}
			
			sww.Reset();
			sww.Start();
            Globals.modelVertexBuffer.SetVertices(0,vertex,0,0,amountOfVertex);
			Globals.modelVertexBuffer.SetVertices(1, matrixNumber,0,0,amountOfVertex);			
			sww.Stop();
			//Console.WriteLine("took: " + sww.ElapsedMilliseconds + "ms");	
            gc.SetVertexBuffer(0, Globals.modelVertexBuffer);
		}
		
		public Matrix4 buildProjectionMatrix(ref GraphicsContext gc){
			Matrix4 worldToCamera;
			Globals.cameraToWorld.Inverse(out worldToCamera);
			Matrix4 tmp = worldToCamera * cameraToProjection;
			return tmp;
		}	
		
		//http://coding.grax.com/2013/06/fast-array-fill-function-revisited.html
		public static void ArrayFill<T>(T[] arrayToFill, T[] fillValue)
		{
		    /*if (fillValue.Length >= arrayToFill.Length){
		        throw new ArgumentException("fillValue array length must be smaller than length of arrayToFill");
		    }*/
		 
		    // set the initial array value
		    Array.Copy(fillValue, arrayToFill, fillValue.Length);
		 
		    int arrayToFillHalfLength = arrayToFill.Length / 2;
		 
		    for (int i = fillValue.Length; i < arrayToFill.Length; i *= 2){
		        int copyLength = i;
		        if (i > arrayToFillHalfLength){
		            copyLength = arrayToFill.Length - i;
		        }
		        Array.Copy(arrayToFill, 0, arrayToFill, i, copyLength);
		    }
		}
		
		
		//Need to fill arrays at certain places, altered for that.
		public static void ArrayFillComplex<T>(ref T[] arrayToFill, int destinationIndex, ref T[] fillValue, int count)
		{
		    /*if (fillValue.Length >= arrayToFill.Length){
		        throw new ArgumentException("fillValue array length must be smaller than length of arrayToFill");
		    }*/
		 
		    // set the initial array value
		    Array.Copy(fillValue,0,arrayToFill,destinationIndex,fillValue.Length);
		    int arrayToFillHalfLength = count / 2;
		 
		    for (int i = fillValue.Length; i < count; i *= 2){
		        int copyLength = i;
		        if (i > arrayToFillHalfLength){
		            copyLength = count - i;
		        }
		        Array.Copy(arrayToFill, destinationIndex, arrayToFill, destinationIndex+i,copyLength);
		    }
		}
	
		
    }
}
