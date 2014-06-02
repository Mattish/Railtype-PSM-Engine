using System;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core;
using System.Diagnostics;

namespace RailTypePSMEngineNew{
	public class RailTypeEngine{
		ThingHandler _th;
		GraphicsContext _gc;
		public static Matrix4 cameraToWorld;
		int frameCount;
		Stopwatch frameCounter;
		
		public RailTypeEngine(GraphicsContext gc){
			_th = new ThingHandler();	
			_gc = gc;
			GraphicsHandler.Init(gc);
			AssetHandler.Init();
			frameCount = 0;
			frameCounter = new Stopwatch();
			
			cameraToWorld = Matrix4.Identity;
			WaveFrontObject wfo = new WaveFrontObject("/Application/objects/cube.obj");

			float doublePI = (float)Math.PI*2;
			float floatit = doublePI/35;
			for(float j = 0.0f; j < doublePI;j+=floatit){
				ThingRotating tmpThing = new ThingRotating(wfo.models[0]);
				tmpThing.scalexyzrot[3] = 3.0f;
				tmpThing.scalexyzrot[2] = (float)Math.Sin(j);
				tmpThing.scalexyzrot[1] = (float)Math.Cos(j);
				tmpThing.scalexyzrot[0] = 0.1f;
				_th.AddThing(tmpThing);				
			}
			frameCounter.Start();
		}
		
		public void Render(){
			_gc.SetViewport(0, 0, _gc.Screen.Width, _gc.Screen.Height);
			_gc.SetClearColor(1.0f, 1.0f, 1.0f, 1.0f);
			_gc.Clear();			
			_gc.Enable(EnableMode.Blend);
			//_gc.Enable(EnableMode.CullFace);
			_gc.Enable(EnableMode.DepthTest);
			_gc.SetBlendFunc(BlendFuncMode.Add, BlendFuncFactor.SrcAlpha, BlendFuncFactor.OneMinusSrcAlpha);
			
			BatchInfo[] batchInfo;
			Primitive[] batchPrims;
			Matrix4[] batchMatricies;
			int[] batchThingNumbers;
			_th.TryGetBatchesOfThings(out batchInfo, out batchPrims, out batchMatricies, out batchThingNumbers);
			foreach(BatchInfo bi in batchInfo){
				GraphicsHandler.GetGHInstance().Draw(ref batchPrims, ref batchMatricies, ref batchThingNumbers, bi.index, bi.count);
			}
			_gc.SwapBuffers();
			frameCount++;
			if (frameCounter.ElapsedMilliseconds > 1000){	
				System.Console.WriteLine("fps:{0:D}",frameCount);
				frameCount = 0;
				frameCounter.Reset();
				frameCounter.Start();
			}
		}
		
		public void Update(){
			_th.Update();
		}

	}
}

