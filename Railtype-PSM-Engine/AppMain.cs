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
		Railtype_PSM_Engine.Util.WaveFrontObject wfo;
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
			wfo = new Railtype_PSM_Engine.Util.WaveFrontObject("/Application/cube.obj");
			tm = new ThingManager(graphics);			
			Globals.gpuHard = new ShaderProgram("/Application/shaders/gpuHard.cgx");		
			texture = new Texture2D("/Application/railgun.png",false,PixelFormat.Rgba);
			
			
			//GPUSoft
			Globals.gpuHard.SetAttributeBinding(0, "a_Position");			
			Globals.gpuHard.SetAttributeBinding(1, "uv");
			Globals.gpuHard.SetAttributeBinding(2, "matrixNumber");
			int k = Globals.gpuHard.FindUniform("WorldViewProj");
			Globals.gpuHard.SetUniformBinding(k, "WorldViewProj");			
			k = Globals.gpuHard.FindUniform("scalexyzrot");
			Globals.gpuHard.SetUniformBinding(k, "scalexyzrot");
			
			sw = new Stopwatch();
			sw.Start();
			graphics.Enable (EnableMode.Blend);
			graphics.Enable (EnableMode.CullFace);
			graphics.Enable (EnableMode.DepthTest);
			graphics.SetBlendFunc(new BlendFunc(BlendFuncMode.Add,BlendFuncFactor.SrcAlpha,BlendFuncFactor.OneMinusSrcAlpha));
			graphics.SetTexture(0,texture);
		}
 
		public void Update(){
			
			GamePadData gpd = GamePad.GetData(0);
			if(gpd.ButtonsDown.HasFlag(GamePadButtons.Cross)){
				for(int i = 0; i < 1; i++){
					Globals.thingManager.AddThing(new Thing(wfo));
				}
			}
			if(gpd.ButtonsDown.HasFlag(GamePadButtons.Square)){
				for(int i = 0; i < 10; i++){
					Globals.thingManager.AddThing(new Thing(wfo));
				}
			}
			if(gpd.ButtonsDown.HasFlag(GamePadButtons.Triangle)){
				for(int i = 0; i < 100; i++){
					Globals.thingManager.AddThing(new Thing(wfo));
				}
			}			
			if (gpd.ButtonsDown.HasFlag(GamePadButtons.L)){
				for(int i = 0; i < 10; i++){
					Globals.thingManager.RemoveThing(Globals.thingManager.GetFirstThing());
				}
			}
			if (gpd.ButtonsDown.HasFlag(GamePadButtons.R)){
				int count = Globals.thingManager.ThingCount();
				for(int i = 0; i < count; i++){
					Globals.thingManager.RemoveThing(Globals.thingManager.GetFirstThing());
				}
			}
			
			if (gpd.ButtonsDown.HasFlag(GamePadButtons.Start))
				Environment.Exit(0);
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
