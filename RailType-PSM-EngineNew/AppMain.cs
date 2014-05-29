using System;
using System.Collections.Generic;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Input;

namespace RailTypePSMEngineNew{
	public class AppMain{
		private static GraphicsContext graphics;
		private static RailTypeEngine rte;
		public static void Main(string[] args){
			Initialize();

			while(true){
				SystemEvents.CheckEvents();
				Update();
				Render();
			}
		}

		public static void Initialize(){
			// Set up the graphics system
			graphics = new GraphicsContext();
			rte = new RailTypeEngine(graphics);
		}

		public static void Update(){
			// Query gamepad for current state
			var gamePadData = GamePad.GetData(0);
			rte.Update();
		}

		public static void Render(){
			rte.Render();
		}
	}
}
