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
            Globals.cpu = new ShaderProgram("/Application/shaders/cpu.cgx");
			Globals.gpu = new ShaderProgram("/Application/shaders/gpu.cgx");
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
			
            Globals.DoDrawing(ref graphics);
            graphics.SwapBuffers();
        }
	
		
    }
}
