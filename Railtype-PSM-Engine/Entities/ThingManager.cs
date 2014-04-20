using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Railtype_PSM_Engine.Entities;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core;
using System.Diagnostics;

namespace Railtype_PSM_Engine.Entities{
	public class ThingManager{
		int fpsCounter;
		Matrix4 cameraToProjection;
		Stopwatch sw;
		Dictionary<string,Thing> _thingMap;
					
		public ThingManager(){
			sw = new Stopwatch();
			fpsCounter = 0;
			//cameraToProjection = Matrix4.Perspective(FMath.Radians(1.0f), gc.Screen.AspectRatio, 1f, 1000.0f);
			cameraToProjection = Matrix4.Identity;
			cameraToProjection *= Matrix4.Ortho(-Globals.gc.Screen.AspectRatio, Globals.gc.Screen.AspectRatio,
			                                   -1.0f, 1.0f, 1.0f, 1000.0f);
			
			_thingMap = new Dictionary<string, Thing>();
			sw.Start();
		}
		
		public int ThingCount(){
			return _thingMap.Count;	
		}
		
		public bool AddThing(string name, Thing input){
			if (!_thingMap.ContainsKey(name)){
				_thingMap.Add(name,input);
				Globals.modelManager.AddThing(input);
				return true;
			}
			return false;
		}
		
		public bool TryGetThingByName(string name, out Thing thing){
			return _thingMap.TryGetValue(name,out thing);
		}
		
		public bool RemoveThing(string name){
			if (_thingMap.ContainsKey(name)){
				Globals.modelManager.RemoveThing(_thingMap[name]);
			}
			return _thingMap.Remove(name);
		}
		
		public void Update(){
			
			System.Diagnostics.Stopwatch sww = new System.Diagnostics.Stopwatch();
			sww.Start();
			Globals.modelManager.Update();
			sww.Stop();
			if(Globals.DEBUG_MODE && sw.ElapsedMilliseconds > 1000)
				Console.WriteLine("modelManager.Update():" + sww.ElapsedMilliseconds + "ms");
			sww.Reset();
			sww.Start();
			
			foreach(KeyValuePair<string,Thing> thing in _thingMap){
				thing.Value.Update();	
			}
			sww.Stop();
			if(Globals.DEBUG_MODE && sw.ElapsedMilliseconds > 1000)
				Console.WriteLine("foreach update:" + sww.ElapsedMilliseconds + "ms");
		}
		
		public void Draw(){
			Stopwatch sww = new Stopwatch();
			Stopwatch setUniformWatch = new Stopwatch();
			Matrix4 VP = buildProjectionMatrix(); // Build camera to world projection			
			Globals.gc.SetShaderProgram(Globals.gpuHard);
			int k = Globals.gpuHard.FindUniform("WorldViewProj");
			Globals.gpuHard.SetUniformValue(k, ref VP);
			Primitive[] tempPrim = new Primitive[1];
			foreach(KeyValuePair<string,Thing> thing in _thingMap){
				if (thing.Value.draw){
					setUniformWatch.Start();					
					k = Globals.gpuHard.FindUniform("textureNumber");
					Globals.gpuHard.SetUniformValue(k, Globals.textureManager.GetBufferForTextureNumber(thing.Value.textureNumber));
					k = Globals.gpuHard.FindUniform("modelToWorld");
					Globals.gpuHard.SetUniformValue(k, ref thing.Value.modelToWorld);
					setUniformWatch.Stop();
					sww.Start();
					tempPrim[0] = thing.Value.prim;
					Globals.gc.DrawArrays(tempPrim, 0, 1);
					sww.Stop();
				}
			}
			
			if(Globals.DEBUG_MODE && sw.ElapsedMilliseconds > 1000){
				Console.WriteLine("Draw:" + sww.ElapsedMilliseconds + "ms");
				Console.WriteLine("settingUniform:" + setUniformWatch.ElapsedMilliseconds + "ms");
				Console.WriteLine("Amount of Things:" + ThingCount() + " - fps:" + fpsCounter + " Memory usage: " + (GC.GetTotalMemory(true) / 1024) + "KB");
				fpsCounter = 0;
				sw.Reset();
				sw.Start();
			}
			fpsCounter++;
			Globals.frameCount++;
		}
		
		//Tweaked version of this to allow for positional inserts
		//http://coding.grax.com/2013/06/fast-array-fill-function-revisited.html}
		private void ArrayFillComplex<T>(ref T[] arrayToFill, int destinationIndex, ref T[] fillValue, int count){
			Array.Copy(fillValue, 0, arrayToFill, destinationIndex, fillValue.Length);
			int arrayToFillHalfLength = count / 2;
		 
			for(int i = fillValue.Length; i < count; i *= 2){
				int copyLength = i;
				if(i > arrayToFillHalfLength){
					copyLength = count - i;
				}
				Array.Copy(arrayToFill, destinationIndex, arrayToFill, destinationIndex + i, copyLength);
			}
		}
		
		private Matrix4 buildProjectionMatrix(){
			Matrix4 worldToCamera;
			Globals.cameraToWorld.Inverse(out worldToCamera);
			Matrix4 tmp = worldToCamera * cameraToProjection;
			return tmp;
		}	
	}
}

