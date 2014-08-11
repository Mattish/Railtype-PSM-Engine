using System;
using System.Collections.Generic;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Input;
using RailTypePSMEngine.Asset;

namespace RailTypePSMEngine{
	public class AppMain{
		private static GraphicsContext graphics;
		private static RailTypeEngine rte;
		
		private static int AMOUNT_OF_THINGS_IN_CIRCLE = 8;
		
		
		public static void Main(string[] args){
			Initialize();
			while(true){
				SystemEvents.CheckEvents();
				Update();
				Render();
			}
		}
		private static WaveFrontObject wfo = new WaveFrontObject("/Application/objects/cube.obj");
		
		public static void Initialize(){
			// Set up the graphics system
			graphics = new GraphicsContext();
			rte = new RailTypeEngine(graphics);
			
			WaveFrontObject wfo2 = new WaveFrontObject();
			wfo2.MakeCircle(20);
			
		}
		
		
		public static int someThingCounterDeleter = 4;
		
		public static void Update(){
			// Query gamepad for current state
			var gamePadData = GamePad.GetData(0);
			rte.Update();
			float doublePI = (float)Math.PI*1.7f;
			float floatit = doublePI/AMOUNT_OF_THINGS_IN_CIRCLE;
			if (rte.GetFrameNumber() % 10 == 0 && rte.GetFrameNumber() < 80){
				//_things[rte.GetFrameNumber()/10].Destroy();
				int j = rte.GetFrameNumber()/10;
				ThingRotating tmpThing = new ThingRotating(wfo.models[0]);
				tmpThing.scalexyzrot[3] = 3.0f-(0.01f*rte.GetFrameNumber());
				tmpThing.scalexyzrot[2] = (float)Math.Sin(floatit*(float)j)*(0.25f+(j*0.01f));
				tmpThing.scalexyzrot[1] = (float)Math.Cos(floatit*(float)j)*(0.25f+(j*0.01f));
				tmpThing.scalexyzrot[0] = 0.1f+(0.005f*j);	
			}
			
			if (gamePadData.ButtonsDown == GamePadButtons.Down){
				rte[someThingCounterDeleter++].Destroy();
			}
			
			//_gh.cameraToWorld.ColumnW += new Vector4(0.0f,0.0f,0.01f,0.0f);
		}

		public static void Render(){
			rte.Render();
		}
	}
}
