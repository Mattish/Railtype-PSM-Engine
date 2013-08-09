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
        float[] vertices=new float[12];
        const int indexSize = 6;
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
			Globals.gpuHard = new ShaderProgram("/Application/shaders/gpuHard.cgx");			
            Globals.cpu = new ShaderProgram("/Application/shaders/cpu.cgx");
			
			//CPU
			int k = Globals.cpu.FindAttribute("a_Position");
			Globals.cpu.SetAttributeBinding(k, "a_Position");			
			k = Globals.cpu.FindUniform("WorldViewProj");
            Globals.cpu.SetUniformBinding(k, "WorldViewProj");
			
			//GPUSoft
			k = Globals.gpuSoft.FindAttribute("a_Position");
			Globals.gpuSoft.SetAttributeBinding(k, "a_Position");			
			k = Globals.gpuSoft.FindAttribute("matrixNumber");
			Globals.gpuSoft.SetAttributeBinding(k, "matrixNumber");
			k = Globals.gpuSoft.FindUniform("WorldViewProj");
            Globals.gpuSoft.SetUniformBinding(k, "WorldViewProj");			
			k = Globals.gpuSoft.FindUniform("modelToWorld");
			Globals.gpuSoft.SetUniformBinding(k, "modelToWorld");
			
			//GPUHard
			k = Globals.gpuHard.FindAttribute("a_Position");
			Globals.gpuHard.SetAttributeBinding(k, "a_Position");
			k = Globals.gpuHard.FindAttribute("rot");
            Globals.gpuHard.SetAttributeBinding(k, "rot");
			k = Globals.gpuHard.FindAttribute("scale");
            Globals.gpuHard.SetAttributeBinding(k, "scale");
			k = Globals.gpuHard.FindAttribute("trans");
            Globals.gpuHard.SetAttributeBinding(k, "trans");
			k = Globals.gpuHard.FindUniform("WorldViewProj");
            Globals.gpuHard.SetUniformBinding(k, "WorldViewProj");
			
			sw = new Stopwatch();
			sw.Start();
        }
 
        public void Update (){
			foreach(Thing thing in Globals.things){
				thing.update();		
			}
		}

        public void Render (){
			graphics.SetClearColor (1.0f, 1.0f, 1.0f, 1.0f);
            graphics.Clear();
							
			counter++;
			
            DoDrawing(ref graphics);
            graphics.SwapBuffers();
			if (sw.ElapsedMilliseconds > 1000){
				Console.WriteLine(Globals.COMPUTE_BY.ToString() + " fps:" + fpsCounter);
				fpsCounter = 0;
				sw.Reset();
				sw.Start();
			}
			fpsCounter++;
        }
		
		public void DoDrawing(ref GraphicsContext gc){
			Matrix4 VP = buildProjectionMatrix(ref gc); // Build camera to world projection			
			System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
			Primitive[] primArray;
			sw.Start();
			Matrix4[] matriciesToPush = PackModelVertexBuffer(ref gc, out primArray);
			sw.Stop();
			Console.WriteLine("Packing " + Globals.COMPUTE_BY.ToString() + " took :" + sw.ElapsedMilliseconds + "ms");
			sw.Reset();
			sw.Start();
			if (Globals.COMPUTE_BY == Globals.COMPUTATION_TYPE.CPU){
	            gc.SetShaderProgram(Globals.cpu);						
	            Globals.cpu.SetUniformValue(0, ref VP);				
				gc.DrawArrays(primArray,0,primArray.Length);
					
			}
			else if (Globals.COMPUTE_BY == Globals.COMPUTATION_TYPE.GPU_SOFT){ // Pushing Matricies to shader as uniform
	            gc.SetShaderProgram(Globals.gpuSoft);
				//WorldViewProj
				int k = Globals.gpuSoft.FindUniform("WorldViewProj");
	            Globals.gpuSoft.SetUniformValue(k, ref VP);
				
				int primCounter = 0, HowManyToPush = 0;
				while (primCounter < primArray.Length){
					HowManyToPush = primArray.Length - primCounter > 10 ? 10 : primArray.Length - primCounter;
					k = Globals.gpuSoft.FindUniform("modelToWorld");
					Globals.gpuSoft.SetUniformValue(k,matriciesToPush,0,primCounter,HowManyToPush);			
					gc.DrawArrays(primArray,primCounter,HowManyToPush);
					primCounter += HowManyToPush;
				}
			}
			else if(Globals.COMPUTE_BY == Globals.COMPUTATION_TYPE.GPU_HARD){
				gc.SetShaderProgram(Globals.gpuHard);						
	            Globals.gpuHard.SetUniformValue(0, ref VP);				
				gc.DrawArrays(primArray,0,primArray.Length);					
			}
			Globals.modelVertexBuffer.Dispose();
			sw.Stop();
			Console.WriteLine("Draw & Dispose " + Globals.COMPUTE_BY.ToString() + " took :" + sw.ElapsedMilliseconds + "ms");
			
			Globals.frameCount++;
			if (Globals.frameCount % 360 == 0){
				//Globals.COMPUTE_BY = ((int)Globals.COMPUTE_BY+1) < Enum.GetValues(typeof(Globals.COMPUTATION_TYPE)).Length ? Globals.COMPUTE_BY+1 : 0;
				if (Globals.COMPUTE_BY == Globals.COMPUTATION_TYPE.GPU_HARD)
					Globals.COMPUTE_BY = Globals.COMPUTATION_TYPE.GPU_SOFT;
				else
					Globals.COMPUTE_BY = Globals.COMPUTATION_TYPE.GPU_HARD;
				sw.Reset();
				sw.Start();
				fpsCounter = 0;
			}
		}

		Matrix4[] PackModelVertexBuffer(ref GraphicsContext gc, out Primitive[] input){
			float[] vertex;
			Matrix4[] matrixUniform = new Matrix4[Globals.things.Count];
			input = new Primitive[Globals.things.Count];
			int amountOfVertex = 0, matrixCounter = 0, position = 0, HowManyWeGot = 0;
			for(int i = 0; i < Globals.things.Count;i++){ 
				amountOfVertex += Globals.things[i].VertexCount;
				matrixUniform[i] = Globals.things[i].getModelToWorld();
				HowManyWeGot++;
			}
			vertex = new float[amountOfVertex*3];
			
			if(Globals.COMPUTE_BY == Globals.COMPUTATION_TYPE.CPU){
				for(int i = 0; i < Globals.things.Count;i++){ // put them into the arrays
					Globals.things[i].PutModelVertexIntoArray(ref vertex,(position*3)); // For verticies					
					matrixCounter++;
					input[i] = new Primitive(DrawMode.Triangles,position,Globals.things[i].VertexCount,0);
					position += Globals.things[i].VertexCount;				
				}					
	            Globals.modelVertexBuffer = new VertexBuffer(amountOfVertex, VertexFormat.Float3);
	            Globals.modelVertexBuffer.SetVertices(0, vertex);
			}
			else if (Globals.COMPUTE_BY == Globals.COMPUTATION_TYPE.GPU_SOFT){ // Pushing Matricies to shader as uniform
				float[] matrixNumber = new float[amountOfVertex];
				for(int i = 0; i < Globals.things.Count;i++){ // Now for each, put them into the arrays
					Globals.things[i].PutModelVertexIntoArray(ref vertex,(position*3)); // For verticies
	
					for(int j = position; j < (position+Globals.things[i].VertexCount);j++){ // for matrixnumber
						matrixNumber[j] = matrixCounter % Globals.AmountPerPush;
					}
	
					matrixCounter++;
					input[i] = new Primitive(DrawMode.Triangles,position,Globals.things[i].VertexCount,0);
					position += (Globals.things[i].VertexCount);
				}		
				Globals.modelVertexBuffer = new VertexBuffer(amountOfVertex, VertexFormat.Float3,VertexFormat.Float);
	            Globals.modelVertexBuffer.SetVertices(0, vertex);
				Globals.modelVertexBuffer.SetVertices(1, matrixNumber);
			}
			else if (Globals.COMPUTE_BY == Globals.COMPUTATION_TYPE.GPU_HARD){ // Pushing everything and working out on shader	
				float[] rotArray = new float[vertex.Length];
				float[] scaleArray = new float[vertex.Length];
				float[] transArray = new float[vertex.Length];
				for(int i = 0; i < Globals.things.Count;i++){ // Now for each, put them into the arrays
					Globals.things[i].PutModelVertexIntoArray(ref vertex,(position*3)); // For verticies
					int nextPosition = (position+Globals.things[i].VertexCount);
					input[i] = new Primitive(DrawMode.Triangles,position,Globals.things[i].VertexCount,0);					
					
					float[] tmpArray = new float[Globals.things[i].VertexCount*3];
					ArrayFill<float>(tmpArray,Globals.things[i].rot);
					Array.Copy(tmpArray,0,rotArray,(position*3),tmpArray.Length); // For rot
					ArrayFill<float>(tmpArray,Globals.things[i].scale);
					Array.Copy(tmpArray,0,scaleArray,(position*3),tmpArray.Length); // For scale
					ArrayFill<float>(tmpArray,Globals.things[i].xyz);
					Array.Copy(tmpArray,0,transArray,(position*3),tmpArray.Length); // For trans
					position++;
				}
				Globals.modelVertexBuffer = new VertexBuffer(amountOfVertex,VertexFormat.Float3,VertexFormat.Float3,VertexFormat.Float3,VertexFormat.Float3);
				int k = Globals.gpuHard.FindAttribute("a_Position");
	            Globals.modelVertexBuffer.SetVertices(k, vertex);				
				k = Globals.gpuHard.FindAttribute("rot");
				Globals.modelVertexBuffer.SetVertices(k, rotArray);				
				k = Globals.gpuHard.FindAttribute("scale");
				Globals.modelVertexBuffer.SetVertices(k, scaleArray);				
				k = Globals.gpuHard.FindAttribute("trans");				
				Globals.modelVertexBuffer.SetVertices(k, transArray);				
			}
            gc.SetVertexBuffer(0, Globals.modelVertexBuffer);
			return matrixUniform;
		}
		
		static public Matrix4 buildProjectionMatrix(ref GraphicsContext gc){
			Matrix4 worldToCamera = Matrix4.Identity;
			Globals.cameraToWorld.Inverse(out worldToCamera);
			Matrix4 cameraToProjection = Matrix4.Perspective(FMath.Radians(45.0f), gc.Screen.AspectRatio, 1.0f, 1000.0f);
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
	
		
    }
}
