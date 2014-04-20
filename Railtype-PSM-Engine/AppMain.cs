using System;
using System.Collections.Generic;
using System.Diagnostics;
using Railtype_PSM_Engine.Entities;
using Railtype_PSM_Engine.Util;
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
		CharMetrics[] charMetricsResult;
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
			int counter = 0;
			graphics = new GraphicsContext();
			Globals.Setup(graphics);
			wfo = new WaveFrontObject("/Application/objects/square.obj");
			Globals.modelManager.AddModel("square",wfo);
			wfo = new WaveFrontObject();
			wfo.MakeCircle();
			Globals.modelManager.AddModel("circle",wfo);
			tm = new ThingManager();			
			Globals.gpuHard = new ShaderProgram("/Application/shaders/gpuHardBase.cgx");	
			Font font = new Font(FontAlias.System, 18, FontStyle.Regular);
			//Texture2D tmpFontTexture = createTexture("1234567890abcdefghijABCDEFGHIJ",font,0xff0000ff);
			Globals.textureManager.TryAddTexture(Globals.fontManager._fontTexture);
			Globals.textureManager.TryAddTexture("railgun.png");
			Globals.textureManager.TryAddTexture("marisa.png");
			Globals.textureManager.TryAddTexture("uiharu.png");
			Globals.textureManager.TryAddTexture("squareborder.png");
			Globals.textureManager.TryAddTexture("skybox.png");
			Globals.textureManager.TryAddTexture("test.png");
			charMetricsResult = font.GetTextMetrics("1234567890abcdefghijABCDEFGHIJ");
			//GPUHard
			Globals.gpuHard.SetAttributeBinding(0, "a_Position");			
			Globals.gpuHard.SetAttributeBinding(1, "uv");
			int k = Globals.gpuHard.FindUniform("WorldViewProj");
			Globals.gpuHard.SetUniformBinding(k, "WorldViewProj");			
			k = Globals.gpuHard.FindUniform("modelToWorld");
			Globals.gpuHard.SetUniformBinding(k, "modelToWorld");
			k = Globals.gpuHard.FindUniform("textureNumber");
			Globals.gpuHard.SetUniformBinding(k, "textureNumber");
			
			sw = new Stopwatch();
			sw.Start();
			graphics.Enable(EnableMode.Blend);
			//graphics.Enable (EnableMode.CullFace);
			graphics.Enable(EnableMode.DepthTest);
			graphics.SetBlendFunc(new BlendFunc(BlendFuncMode.Add, BlendFuncFactor.SrcAlpha, BlendFuncFactor.OneMinusSrcAlpha));
			Globals.textureManager.SetActiveTexture(0, 0);
			Globals.textureManager.SetActiveTexture(1, 1);
			Globals.textureManager.SetActiveTexture(2, 2);
			Globals.textureManager.SetActiveTexture(3, 3);
			Globals.textureManager.SetActiveTexture(4, 4);
			Globals.textureManager.SetActiveTexture(5, 5);
		}
		public void Update(){
			Stopwatch sw = new Stopwatch();
			sw.Start();			
			GamePadData gpd = GamePad.GetData(0);
			
			if(gpd.ButtonsDown.HasFlag(GamePadButtons.Left)){
				//Globals.cameraToWorld.RowW = Globals.cameraToWorld.RowW.Add(new Vector4(-0.1f, 0.0f, 0.0f, 0.0f));
				while(counter > 0)
					Globals.thingManager.RemoveThing("" + counter--);
			}
			if(gpd.ButtonsDown.HasFlag(GamePadButtons.Right)){
				float distance = 5.0f;
			
				Thing tmpThing;
				for(float x = -20f; x < 20f; x+=2.8645f){
					for(float y = -20f; y < 20f; y+=2.8645f){
						//tmpThing = Globals.fontManager.NewThingText("test:" + x + y);
						tmpThing = new Thing("square");
						tmpThing.scalexyzrot[0] = 1.0f;
						tmpThing.scalexyzrot[1] = x;
						tmpThing.scalexyzrot[2] = y;
						tmpThing.scalexyzrot[3] = distance;
						tmpThing.textureNumber = 8;
						Globals.thingManager.AddThing("" + counter, tmpThing);
						counter++;
					}
				}	
			}
				//Globals.cameraToWorld.RowW = Globals.cameraToWorld.RowW.Add(new Vector4(0.1f, 0.0f, 0.0f, 0.0f));
			if(gpd.ButtonsDown.HasFlag(GamePadButtons.Up))
				Globals.cameraToWorld.RowW = Globals.cameraToWorld.RowW.Add(new Vector4(0.0f, 0.0f, 0.5f, 0.0f));		
			if(gpd.ButtonsDown.HasFlag(GamePadButtons.Down))
				Globals.cameraToWorld.RowW = Globals.cameraToWorld.RowW.Add(new Vector4(0.0f, 0.0f, -0.5f, 0.0f));	
			if(gpd.ButtonsDown.HasFlag(GamePadButtons.L))
				Globals.cameraToWorld *= Matrix4.RotationY(0.05f);
			if(gpd.ButtonsDown.HasFlag(GamePadButtons.R))
				Globals.cameraToWorld *= Matrix4.RotationY(-0.05f);
			
			if(gpd.ButtonsDown.HasFlag(GamePadButtons.Start))
				Environment.Exit(0);

			Globals.thingManager.Update();
			sw.Stop();
			if (Globals.frameCount % 60 == 0)
				Console.WriteLine("mainUpdateLoop:" + sw.ElapsedMilliseconds);
		}

		public void Render(){
			Stopwatch sw = new Stopwatch();
			sw.Start();	
			graphics.SetClearColor(1.0f, 1.0f, 1.0f, 1.0f);
			graphics.Clear();			
			Globals.thingManager.Draw();
			sw.Stop();
			if (Globals.frameCount % 60 == 0)
				Console.WriteLine("mainRenderLoop:" + sw.ElapsedMilliseconds);
			sw.Reset();
			sw.Start();
			graphics.SwapBuffers();
			sw.Stop();
			if (Globals.frameCount % 60 == 0)
				Console.WriteLine("graphics.SwapBuffers():" + sw.ElapsedMilliseconds);
		}
		
		
	
		
	}
}
