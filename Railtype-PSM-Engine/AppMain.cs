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
		
	    private static Texture2D createTexture(string text, Font font, uint argb){
	        int width = 400;
	        int height = 400;
	
	        var image = new Image(ImageMode.Rgba,
	                              new ImageSize(width, height),
	                              new ImageColor(0, 0, 0, 0));
	
	        image.DrawText(text,
	                       new ImageColor((int)((argb >> 16) & 0xff),
	                                      (int)((argb >> 8) & 0xff),
	                                      (int)((argb >> 0) & 0xff),
	                                      (int)((argb >> 24) & 0xff)),
	                       font, new ImagePosition(0, 0));
	
	        var texture = new Texture2D(width, height, false, PixelFormat.Rgba);
	        texture.SetPixels(0, image.ToBuffer());
	        image.Dispose();
	
	        return texture;
	    }
		
		public void Initialize(){
			graphics = new GraphicsContext();
			Globals.Setup(graphics);
			wfo = new Railtype_PSM_Engine.Util.WaveFrontObject("/Application/objects/square.obj");
			tm = new ThingManager(graphics);			
			Globals.gpuHard = new ShaderProgram("/Application/shaders/gpuHardBase.cgx");	
			Font font = new Font(FontAlias.System,18,FontStyle.Regular);
			Texture2D tmpFontTexture = createTexture("1234567890abcdefghijABCDEFGHIJ",font,0xff0000ff);
			Globals.textureManager.TryAddTexture(tmpFontTexture);
			Globals.textureManager.TryAddTexture("railgun.png");
			Globals.textureManager.TryAddTexture("marisa.png");
			Globals.textureManager.TryAddTexture("uiharu.png");
			Globals.textureManager.TryAddTexture("squareborder.png");
			Globals.textureManager.TryAddTexture("skybox.png");
			Globals.textureManager.TryAddTexture("test.png");
			
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
			Globals.textureManager.SetActiveTexture(0,1);
			Globals.textureManager.SetActiveTexture(1,2);
			Globals.textureManager.SetActiveTexture(2,3);
			Globals.textureManager.SetActiveTexture(3,4);
			Globals.textureManager.SetActiveTexture(4,5);
			Globals.textureManager.SetActiveTexture(5,6);
			
			float distance = 10.0f;
			int counter = 0;
			Thing tmpThing = new Thing(wfo);
			for(float x = -5f; x < 5f; x+=1.0f){
				for(float y = -5f; y < 5f;y+=1f){
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
				switch(gpd.ButtonsDown){
					case GamePadButtons.Left:	
						Globals.cameraToWorld.RowW = Globals.cameraToWorld.RowW.Add(new Vector4(-0.1f, 0.0f, 0.0f, 0.0f));		
						break;
					case GamePadButtons.Right:	
						Globals.cameraToWorld.RowW = Globals.cameraToWorld.RowW.Add(new Vector4(0.1f, 0.0f, 0.0f, 0.0f));
						break;
					case GamePadButtons.Up:	
						Globals.cameraToWorld.RowW = Globals.cameraToWorld.RowW.Add(new Vector4(0.0f, 0.0f, 0.5f, 0.0f));		
						break;
					case GamePadButtons.Down:	
						Globals.cameraToWorld.RowW = Globals.cameraToWorld.RowW.Add(new Vector4(0.0f, 0.0f, -0.5f, 0.0f));		
						break;
					case GamePadButtons.L:	
						Globals.cameraToWorld *= Matrix4.RotationY(0.05f);
						break;
					case GamePadButtons.R:	
						Globals.cameraToWorld *= Matrix4.RotationY(-0.05f);
						break;
				}
			
			
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
