using System;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core;
using System.Diagnostics;
using RailTypePSMEngine.Entity;
using RailTypePSMEngine.Graphics;
using RailTypePSMEngine.Asset;

namespace RailTypePSMEngine{
	public class RailTypeEngine{
		GraphicsContext _gc;
		public static Matrix4 cameraToWorld;
		int frameCount;
		int totalFrameCount;
		Stopwatch frameCounter;
		private ThingHandler _th;
		private GraphicsHandler _gh;
		
		public RailTypeEngine(GraphicsContext gc){
			_gc = gc;
			_gh = new GraphicsHandler(gc);
			_th = new ThingHandler(_gh);			
			AssetHandler.Init();
			frameCount = 0;
			frameCounter = new Stopwatch();
			totalFrameCount = 0;
			cameraToWorld = Matrix4.Identity;
			frameCounter.Start();
		}
		
		public Thing this[int i]{
			get{
				Thing someThing;
				_th.thingMap.TryGetValue(i,out someThing);
				return someThing;
			}
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
				_gh.Draw(batchPrims, batchMatricies, batchThingNumbers, bi.index, bi.count,
		                                     			bi.shaderNumber,bi.textureNumber);
			}
			_gc.SwapBuffers();
			frameCount++;
			if (frameCounter.ElapsedMilliseconds > 1000){	
				System.Console.WriteLine("fps:{0:D}",frameCount);
				_th.PrintInfo();
				_gh.PrintInfo();
				frameCount = 0;
				frameCounter.Reset();
				frameCounter.Start();
			}
			totalFrameCount++;
		}
		
		public void Update(){
			_th.Update();
		}

		public int GetFrameNumber(){
			return totalFrameCount;
		}
	}
}

