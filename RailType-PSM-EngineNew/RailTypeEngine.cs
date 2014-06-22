using System;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core;
using System.Diagnostics;

namespace RailTypePSMEngineNew{
	public class RailTypeEngine{
		GraphicsContext _gc;
		public static Matrix4 cameraToWorld;
		int frameCount;
		int totalFrameCount;
		Stopwatch frameCounter;
		
		public RailTypeEngine(GraphicsContext gc){
			_gc = gc;
			GraphicsHandler.Init(gc);
			AssetHandler.Init();
			frameCount = 0;
			frameCounter = new Stopwatch();
			totalFrameCount = 0;
			cameraToWorld = Matrix4.Identity;
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
			ThingHandler.GetInstance().TryGetBatchesOfThings(out batchInfo, out batchPrims, out batchMatricies, out batchThingNumbers);
			foreach(BatchInfo bi in batchInfo){
				GraphicsHandler.GetGHInstance().Draw(batchPrims, batchMatricies, batchThingNumbers, bi.index, bi.count,
		                                     			bi.shaderNumber,bi.textureNumber);
			}
			_gc.SwapBuffers();
			frameCount++;
			if (frameCounter.ElapsedMilliseconds > 1000){	
				System.Console.WriteLine("fps:{0:D}",frameCount);
				System.Console.WriteLine("drawables:{0:D}",ThingHandler.GetInstance().GetTotalDrawableAmount());
				frameCount = 0;
				frameCounter.Reset();
				frameCounter.Start();
			}
			totalFrameCount++;
		}
		
		public void Update(){
			ThingHandler.GetInstance().Update();
		}

		public int GetFrameNumber(){
			return totalFrameCount;
		}
	}
}

