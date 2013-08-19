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
		public Texture2D texture;
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
			Globals.gpuSoft = new ShaderProgram("/Application/shaders/gpuSoft.cgx");			
			//Globals.gpuSoft = new ShaderProgram("/Application/shaders/gpuHard.cgx");		
			texture = new Texture2D("/Application/test.png",false,PixelFormat.Rgba);
			
			
			//GPUSoft
			Globals.gpuSoft.SetAttributeBinding(0, "a_Position");			
			Globals.gpuSoft.SetAttributeBinding(1, "uv");
			Globals.gpuSoft.SetAttributeBinding(2, "matrixNumber");
			int k = Globals.gpuSoft.FindUniform("WorldViewProj");
			Globals.gpuSoft.SetUniformBinding(k, "WorldViewProj");			
			k = Globals.gpuSoft.FindUniform("modelToWorld");
			Globals.gpuSoft.SetUniformBinding(k, "modelToWorld");
			//k = Globals.gpuSoft.FindUniform("scalexyzrot");
			//Globals.gpuSoft.SetUniformBinding(k, "scalexyzrot");
			
			sw = new Stopwatch();
			sw.Start();
			graphics.Enable (EnableMode.Blend);
			graphics.Enable (EnableMode.CullFace);
			graphics.SetBlendFunc(new BlendFunc(BlendFuncMode.Add,BlendFuncFactor.SrcAlpha,BlendFuncFactor.OneMinusSrcAlpha));
			graphics.SetTexture(0,texture);
		}
 
		public void Update(){
			
			GamePadData gpd = GamePad.GetData(0);
			if(gpd.ButtonsDown.HasFlag(GamePadButtons.Cross)){
				for(int i = 0; i < 1; i++){
					Globals.thingManager.AddThing(new Thing(Globals.cubevertex,Globals.cubeuv,Globals.frameCount+i));
				}
			}
			if(gpd.ButtonsDown.HasFlag(GamePadButtons.Triangle)){
				for(int i = 0; i < 10; i++){
					Globals.thingManager.AddThing(new Thing(Globals.cubevertex,Globals.cubeuv,Globals.frameCount+i));
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
