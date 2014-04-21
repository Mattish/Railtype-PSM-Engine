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
		float[] matrixNumbersArray;		
					
		public ThingManager(){
			matrixNumbersArray = new float[ushort.MaxValue];
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
			Stopwatch matrixFillWatch = new Stopwatch();
			Matrix4 VP = buildProjectionMatrix(); // Build camera to world projection			
			Globals.gc.SetShaderProgram(Globals.gpuHard);
			int k = Globals.gpuHard.FindUniform("WorldViewProj");
			Globals.gpuHard.SetUniformValue(k, ref VP);
			
			matrixFillWatch.Start();			
			List<Primitive> tempPrim = new List<Primitive>(500);
			List<Matrix4> tmpMatrixList = new List<Matrix4>(500);
			int thingCounter = 0;
			float[] inputArray = new float[1];
			foreach(KeyValuePair<string,Thing> thing in _thingMap){
				if (thing.Value.draw){
					tempPrim.Add(thing.Value.prim);
					inputArray[0] = (float)(thingCounter % Globals.AmountPerPush);
					ArrayFillComplex<float>(ref matrixNumbersArray,thing.Value.vertexIndex,ref inputArray,thing.Value.vertexCount);
					tmpMatrixList.Add(thing.Value.modelToWorld);
					thingCounter++;
				}
			}
			matrixFillWatch.Stop();				
			Globals.modelVertexBuffer.SetVertices(2,matrixNumbersArray,0,0,ushort.MaxValue);
			Globals.gc.SetVertexBuffer(0, Globals.modelVertexBuffer);		
			Primitive[] primArray = tempPrim.ToArray();
			Matrix4[] matrixArray = tmpMatrixList.ToArray();
				
			
			int primPosition = 0;
			while(primPosition < thingCounter){
				int amountToPush = primPosition + Globals.AmountPerPush > thingCounter ? thingCounter-primPosition : Globals.AmountPerPush;
				setUniformWatch.Start();
				k = Globals.gpuHard.FindUniform("modelToWorld");
				Globals.gpuHard.SetUniformValue(k, matrixArray,0,primPosition,amountToPush);
				setUniformWatch.Stop();
				sww.Start();
				Globals.gc.DrawArrays(primArray,primPosition,amountToPush);
				sww.Stop();
				primPosition += amountToPush;
			}
			
			if(Globals.DEBUG_MODE && sw.ElapsedMilliseconds > 1000){
				Console.WriteLine("Draw:" + sww.ElapsedMilliseconds + "ms");
				Console.WriteLine("settingUniform:" + setUniformWatch.ElapsedMilliseconds + "ms");
				Console.WriteLine("matrixFillWatch:" + matrixFillWatch.ElapsedMilliseconds + "ms");
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

