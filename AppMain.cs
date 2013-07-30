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
		ShaderProgram shaderProgram;
        const int indexSize = 6;
		int counter,fpsCounter;
        Matrix4 cameraToWorld;
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
            shaderProgram = new ShaderProgram("/Application/shaders/Basic.cgx");
			int k = shaderProgram.FindAttribute("a_Position");
			shaderProgram.SetAttributeBinding(k, "a_Position");
			
			k = shaderProgram.FindUniform("WorldViewProj");
            shaderProgram.SetUniformBinding(k, "WorldViewProj");
 			cameraToWorld = Matrix4.Identity;
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
            graphics.SetShaderProgram(shaderProgram);
							
			counter++;
			Matrix4 VP = buildProjectionMatrix(ref cameraToWorld); // Build camera to world projection
            shaderProgram.SetUniformValue(0, ref VP);			
			
            Globals.DoDrawing(ref graphics, ref shaderProgram);
            graphics.SwapBuffers();
        }
		
		public Matrix4 buildProjectionMatrix(ref Matrix4 cameraToWorld){
			Matrix4 worldToCamera = Matrix4.Identity;
			cameraToWorld.Inverse(out worldToCamera);
			Matrix4 cameraToProjection = Matrix4.Perspective(FMath.Radians(35.0f), graphics.Screen.AspectRatio, 1.0f, 1000.0f);
			Matrix4 tmp = worldToCamera * cameraToProjection;
			return tmp;
		}
		
    }
}
