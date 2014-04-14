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
			wfo = new Railtype_PSM_Engine.Util.WaveFrontObject("/Application/objects/square.obj");
			tm = new ThingManager(graphics);			
			Globals.gpuHard = new ShaderProgram("/Application/shaders/gpuHardBase.cgx");		
			Globals.textureManager.TryAddTexture("railgun.png");
			Globals.textureManager.TryAddTexture("marisa.png");
			Globals.textureManager.TryAddTexture("uiharu.png");
			Globals.textureManager.TryAddTexture("squareborder.png");
			Globals.textureManager.TryAddTexture("skybox.png");
			Globals.textureManager.TryAddTexture("test.png");
			
			//GPUHard
			Globals.gpuHard.SetAttributeBinding(0, "a_Position");			
			Globals.gpuHard.SetAttributeBinding(1, "uv");
			Globals.gpuHard.SetAttributeBinding(2, "matrixNumber");
			int k = Globals.gpuHard.FindUniform("WorldViewProj");
			Globals.gpuHard.SetUniformBinding(k, "WorldViewProj");			
			k = Globals.gpuHard.FindUniform("modelToWorld");
			Globals.gpuHard.SetUniformBinding(k, "modelToWorld");
			k = Globals.gpuHard.FindUniform("modelToWorld");
			Globals.gpuHard.SetUniformBinding(k, "modelToWorld");
			
			sw = new Stopwatch();
			sw.Start();
			graphics.Enable(EnableMode.Blend);
			//graphics.Enable (EnableMode.CullFace);
			graphics.Enable(EnableMode.DepthTest);
			graphics.SetBlendFunc(new BlendFunc(BlendFuncMode.Add, BlendFuncFactor.SrcAlpha, BlendFuncFactor.OneMinusSrcAlpha));
			Globals.textureManager.SetActiveTexture(0,1);
			Globals.textureManager.SetActiveTexture(1,2);
			Globals.textureManager.SetActiveTexture(2,3);
			Globals.textureManager.SetActiveTexture(3,4);
			Globals.textureManager.SetActiveTexture(4,5);
			Globals.textureManager.SetActiveTexture(5,6);
			
			float distance = 2.0f;
			int counter = 0;
			Thing tmpThing = new Thing(wfo);
			for(float x = -1.764706f; x < 1.764706f; x+=1.0f){
				for(float y = -2f; y < 2f;y+=1f){
					tmpThing = new Thing(wfo);
					tmpThing.scalexyzrot[0] = 1.0f;
					tmpThing.scalexyzrot[1] = x;
					tmpThing.scalexyzrot[2] = y;
					tmpThing.scalexyzrot[3] = distance;
					tmpThing.textureNumber = counter % 6;
					Globals.thingManager.AddThing(tmpThing);
					counter++;
				}
			}
			
		}
 
		public void Update(){
			Thing tmpThing;
			GamePadData gpd = GamePad.GetData(0);
			if(gpd.ButtonsDown.HasFlag(GamePadButtons.Cross)){
				for(int i = 0; i < 1; i++){
					tmpThing = new ThingStatic(wfo);
					Globals.thingManager.AddThing(tmpThing);
				}
			}
			if(gpd.ButtonsDown.HasFlag(GamePadButtons.Square)){
				for(int i = 0; i < 1; i++){
					Globals.thingManager.AddThing(new Thing(wfo));
				}
			}
			if(gpd.ButtonsDown.HasFlag(GamePadButtons.Triangle)){
				for(int i = 0; i < 100; i++){
					Globals.thingManager.AddThing(new Thing(wfo));
				}
			}			
			if(gpd.ButtonsDown.HasFlag(GamePadButtons.L)){
				for(int i = 0; i < 10; i++){
					//Globals.thingManager.RemoveThing(Globals.thingManager.GetFirstThing());
				}
			}
			if(gpd.ButtonsDown.HasFlag(GamePadButtons.R)){
				int count = Globals.thingManager.ThingCount();
				for(int i = 0; i < count; i++){
					//Globals.thingManager.RemoveThing(Globals.thingManager.GetFirstThing());
				}
			}
			
			if(gpd.ButtonsDown.HasFlag(GamePadButtons.Start))
				Environment.Exit(0);
			Globals.thingManager.Update();
			/*if(tmpThing is ThingStatic){
				switch(gpd.ButtonsDown){
					case GamePadButtons.Left:	
						Globals.cameraToWorld.RowW = Globals.cameraToWorld.RowW.Add(new Vector4(-0.1f, 0.0f, 0.0f, 0.0f));		
						break;
					case GamePadButtons.Right:	
						Globals.cameraToWorld.RowW = Globals.cameraToWorld.RowW.Add(new Vector4(0.1f, 0.0f, 0.0f, 0.0f));
						break;
					case GamePadButtons.Up:	
						tmpThing.scalexyzrot[2] += 0.1f;		
						break;
					case GamePadButtons.Down:	
						tmpThing.scalexyzrot[2] -= 0.1f;		
						break;
					case GamePadButtons.L:	
						Globals.cameraToWorld *= Matrix4.RotationY(0.05f);
						break;
					case GamePadButtons.R:	
						Globals.cameraToWorld *= Matrix4.RotationY(-0.05f);
						break;
				}
				tmpThing.scalexyzrot[5] += FMath.Radians(gpd.AnalogLeftX);
				tmpThing.scalexyzrot[4] -= FMath.Radians(gpd.AnalogLeftY);
			}	*/
			
			
		}

		public void Render(){
			graphics.SetClearColor(1.0f, 1.0f, 1.0f, 1.0f);
			//graphics.SetClearDepth
			graphics.Clear();			
			counter++;
			Globals.thingManager.Draw();
			graphics.SwapBuffers();

		}
		
		
	
		
	}
}
