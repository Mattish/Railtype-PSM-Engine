using System;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core;

namespace RailTypePSMEngineNew{
	public class RailTypeEngine{
		ThingHandler _th;
		GraphicsContext _gc;
		public static Matrix4 cameraToWorld;
		
		public RailTypeEngine(GraphicsContext gc){
			_th = new ThingHandler();	
			_gc = gc;
			GraphicsHandler.Init(gc);
			AssetHandler.Init();
			cameraToWorld = Matrix4.Identity;
			WaveFrontObject wfo = new WaveFrontObject("/Application/objects/square.obj");
			Thing tmpThing = new Thing(wfo.models[0]);
			tmpThing.scalexyzrot[3] = 10.0f;
			tmpThing.scalexyzrot[2] = 1.25f;
			tmpThing.scalexyzrot[1] = 1.25f;
			tmpThing.scalexyzrot[0] = 1.0f;
			
			
			Thing tmpThingg = new Thing(wfo.models[0]);
			tmpThingg.scalexyzrot[3] = 10.0f;
			tmpThingg.scalexyzrot[2] = -1.25f;
			tmpThingg.scalexyzrot[1] = -1.25f;
			tmpThingg.scalexyzrot[0] = 1.0f;
			
			_th.AddThing(tmpThingg);
			_th.AddThing(tmpThing);
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
		}
		
		public void Update(){
			_th.Update();
		}

	}
}

