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
		int counter,fpsCounter;
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
			Globals.gpu = new ShaderProgram("/Application/shaders/gpu.cgx");			
            Globals.cpu = new ShaderProgram("/Application/shaders/cpu.cgx");
			int k = Globals.cpu.FindAttribute("a_Position");
			Globals.cpu.SetAttributeBinding(k, "a_Position");
			k = Globals.gpu.FindAttribute("a_Position");
			Globals.gpu.SetAttributeBinding(k, "a_Position");			
			k = Globals.gpu.FindAttribute("matrixNumber");
			Globals.gpu.SetAttributeBinding(k, "matrixNumber");
			k = Globals.gpu.FindUniform("WorldViewProj");
            Globals.gpu.SetUniformBinding(k, "WorldViewProj");
			k = Globals.cpu.FindUniform("WorldViewProj");
            Globals.cpu.SetUniformBinding(k, "WorldViewProj");
			k = Globals.gpu.FindUniform("modelToWorld");
			Globals.gpu.SetUniformBinding(k, "modelToWorld");
			sw = new Stopwatch();
			sw.Start();
        }
 
        public void Update (){
			if (sw.ElapsedMilliseconds > 1000){
				Console.WriteLine("fps:" + fpsCounter);
				fpsCounter = 0;
				sw.Reset();
				sw.Start();
			}
			fpsCounter++;
			
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
        }
		
		static public void DoDrawing(ref GraphicsContext gc){
			Matrix4 VP = buildProjectionMatrix(ref gc); // Build camera to world projection			
			
			System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
			Primitive[] primArray;
			sw.Start();
			Matrix4[] matriciesToPush = PackModelVertexBuffer(ref gc, out primArray);
			if (Globals.COMPUTE_BY == Globals.COMPUTATION_TYPE.CPU){
	            gc.SetShaderProgram(Globals.cpu);						
	            Globals.cpu.SetUniformValue(0, ref VP);				
				gc.DrawArrays(primArray,0,primArray.Length);
					
			}
			else if (Globals.COMPUTE_BY == Globals.COMPUTATION_TYPE.GPU_SOFT){ // Pushing Matricies to shader as uniform
	            gc.SetShaderProgram(Globals.gpu);
				//WorldViewProj
				int k = Globals.gpu.FindUniform("WorldViewProj");
	            Globals.gpu.SetUniformValue(k, ref VP);
				
				int primCounter = 0, HowManyToPush = 0;
				while (primCounter < primArray.Length){
					HowManyToPush = primArray.Length - primCounter > 10 ? 10 : primArray.Length - primCounter;
					k = Globals.gpu.FindUniform("modelToWorld");
					Globals.gpu.SetUniformValue(k,matriciesToPush,0,primCounter,HowManyToPush);			
					gc.DrawArrays(primArray,primCounter,HowManyToPush);
					primCounter += HowManyToPush;
				}
			}
			Globals.modelVertexBuffer.Dispose();
			sw.Stop();
			Console.WriteLine("Draw/dispose loop took:" + sw.ElapsedMilliseconds + "ms");
			Globals.frameCount++;
			if (Globals.frameCount % 60 == 0)
				Globals.COMPUTE_BY = ((int)Globals.COMPUTE_BY+1) < Enum.GetValues(typeof(Globals.COMPUTATION_TYPE)).Length ? Globals.COMPUTE_BY+1 : 0;
		}

		static Matrix4[] PackModelVertexBuffer(ref GraphicsContext gc, out Primitive[] input){
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
					position += (Globals.things[i].VertexCount);
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
	
		
    }
}
