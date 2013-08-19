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
		public int counter, fpsCounter;
		Stopwatch sw;
		
		ThingManager tm;
 
		public static void Main(string[] args){
			AppMain appmain = new AppMain();
			appmain.Initialize();
			while(true){
				SystemEvents.CheckEvents();
				appmain.Update();
				appmain.Render();
			}
		}
 
		public void Initialize(){
			graphics = new GraphicsContext();
			Globals.Setup(graphics);
			tm = new ThingManager(graphics);
			//Globals.gpuSoft = new ShaderProgram("/Application/shaders/gpuSoft.cgx");			
			Globals.gpuSoft = new ShaderProgram("/Application/shaders/gpuHard.cgx");		
			
			
			//GPUSoft
			int k = Globals.gpuSoft.FindAttribute("a_Position");
			Globals.gpuSoft.SetAttributeBinding(k, "a_Position");			
			k = Globals.gpuSoft.FindAttribute("matrixNumber");
			Globals.gpuSoft.SetAttributeBinding(k, "matrixNumber");
			k = Globals.gpuSoft.FindUniform("WorldViewProj");
			Globals.gpuSoft.SetUniformBinding(k, "WorldViewProj");			
			//k = Globals.gpuSoft.FindUniform("modelToWorld");
			//Globals.gpuSoft.SetUniformBinding(k, "modelToWorld");
			k = Globals.gpuSoft.FindUniform("scalexyzrot");
			Globals.gpuSoft.SetUniformBinding(k, "scalexyzrot");
			
			sw = new Stopwatch();
			sw.Start();
		}
 
		public void Update(){
			
			GamePadData gpd = GamePad.GetData(0);
			if(gpd.ButtonsDown.HasFlag(GamePadButtons.Cross)){
				for(int i = 0; i < 10; i++){
					Globals.thingManager.AddThing(new Thing(333,Globals.frameCount));
				}
			}
			if(gpd.ButtonsDown.HasFlag(GamePadButtons.Triangle)){
				for(int i = 0; i < 1; i++){
					Globals.thingManager.RemoveThing(tm.GetFirstThing());
				}
				
			}
			if(gpd.ButtonsDown.HasFlag(GamePadButtons.Square)){
				for(int i = 0; i < 5; i++){
					Globals.thingManager.AddThing(new Thing(7,Globals.frameCount));
				}
				
			}
			
			Globals.thingManager.Update();
			
		}

		public void Render(){
			graphics.SetClearColor(1.0f, 1.0f, 1.0f, 1.0f);
			graphics.Clear();
							
			counter++;
			
			Globals.thingManager.Draw();
			graphics.SwapBuffers();

		}
		
		
	
		
	}
}
