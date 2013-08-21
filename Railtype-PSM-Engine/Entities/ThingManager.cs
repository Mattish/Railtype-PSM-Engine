using System;
using System.Collections.Generic;
using Railtype_PSM_Engine.Entities;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core;
using System.Diagnostics;

namespace Railtype_PSM_Engine.Entities{
	public class ThingManager{
		float[] vertex, matrixNumber, uv;
		ushort[] indicies;
		GraphicsContext gc;
		int fragmentedFloats, lastIndex, lastVerticiesIndex, fpsCounter;
		long[] longestTook;
		List<Thing> disposed, things, toAdd;
		float[] thingShaderInfo;
		Matrix4 cameraToProjection;
		Primitive[] prims;
		Stopwatch sw;
					
		public ThingManager(GraphicsContext gc_){
			sw = new Stopwatch();
			gc = gc_;
			fragmentedFloats = 0;
			longestTook = new long[10];
			fpsCounter = 0;
			cameraToProjection = Matrix4.Perspective(FMath.Radians(30.0f), gc.Screen.AspectRatio, 1f, 1000.0f);
			//cameraToProjection = Matrix4.Identity;
			//cameraToProjection *= Matrix4.Ortho(-1f,1f,-1f,1f,1f,1000.0f);
			lastIndex = -1;
			lastVerticiesIndex = 0;
			vertex = new float[(65000 * 3)];
			uv = new float[(65000 * 2)];
			matrixNumber = new float[65000];
			indicies = new ushort[65000];
			thingShaderInfo = new float[7];
			
			disposed = new List<Thing>();
			things = new List<Thing>();
			toAdd = new List<Thing>();
			prims = new Primitive[1];
			sw.Start();
		}
		
		public int ThingCount(){
			return things.Count;	
		}
		
		public void AddThing(Thing input){
			toAdd.Add(input);
		}
		
		public Thing GetFirstThing(){
			if(things.Count > 0)
				return things[0];
			else
				return null;
		}
		
		public bool RemoveThing(Thing input){
			int index = things.IndexOf(input);
			if(index != -1){ // Found it
				disposed.Add(things[index]);
				fragmentedFloats += things[index].prim.Count;
				things.RemoveAt(index);
				return true;
			}
			return false;
		}
		
		int bufferLowIndex, bufferHighIndex, vertexCountDiff, fitIns, newOnes;

		public void CheckNewThings(){
			bufferLowIndex = int.MaxValue;
			bufferHighIndex = vertexCountDiff = fitIns = newOnes = 0;
			if(toAdd.Count > 0){ // Need to push things into main list
				while(toAdd.Count > 0){
					bool foundDisposable = false;
					
					if(disposed.Count > 0){
						for(int i = 0; i < disposed.Count; i++){
							//primCountDiff = disposed[i].prim.Count - toAdd[0].prim.Count;
							vertexCountDiff = disposed[i].vertexCount - toAdd[0].vertexCount;
							if(disposed[i].vertexIndex == lastIndex){ // if its the last prim in vertex array
								vertexCountDiff = 0;
								lastVerticiesIndex = (lastIndex + toAdd[0].vertexCount) * 3;
								if(vertex.Length < lastVerticiesIndex){
									Array.Resize<float>(ref vertex, lastVerticiesIndex);
									Array.Resize<float>(ref uv, (lastIndex + toAdd[0].vertexCount) * 2);
								}
							}	
							if(vertexCountDiff >= 0){ // Found a dispose to replace
								fragmentedFloats -= toAdd[0].vertexCount;
								toAdd[0].PutModelVertexIntoArray(ref vertex, disposed[i].vertexIndex * 3);
								toAdd[0].PutModelUVIntoArray(ref uv, disposed[i].vertexIndex * 2);
								bufferLowIndex = disposed[i].vertexIndex < bufferLowIndex ? disposed[i].vertexIndex : bufferLowIndex;
								bufferHighIndex = (disposed[i].vertexIndex + toAdd[0].vertexCount) > bufferHighIndex ? 
									(disposed[i].vertexIndex + toAdd[0].vertexCount) : bufferHighIndex;
								toAdd[0].vertexIndex = disposed[i].vertexIndex;
								if(vertexCountDiff > 0)
									disposed[i].vertexIndex += toAdd[0].vertexCount;
								else
									disposed.RemoveAt(i);
								foundDisposable = true;
								fitIns++;
								break;
							}
						}
					}
					
					if(!foundDisposable){ // Cant find something to replace, add to the end of array
						if(lastIndex < 0)
							lastIndex = 0;
						else
							lastIndex = (lastIndex + toAdd[0].vertexCount);
						toAdd[0].vertexIndex = (ushort)lastIndex;
						if(vertex.Length < (lastVerticiesIndex + toAdd[0].vertexCount * 3)){
							Array.Resize<float>(ref vertex, (lastVerticiesIndex + toAdd[0].vertexCount * 3));	
							Array.Resize<float>(ref uv, (lastVerticiesIndex + toAdd[0].vertexCount * 2));
						}
						toAdd[0].PutModelVertexIntoArray(ref vertex, toAdd[0].vertexIndex * 3);
						toAdd[0].PutModelUVIntoArray(ref uv, toAdd[0].vertexIndex * 2);
						bufferLowIndex = toAdd[0].vertexIndex < bufferLowIndex ? toAdd[0].vertexIndex : bufferLowIndex;
						bufferHighIndex = toAdd[0].vertexIndex + toAdd[0].vertexCount;
						lastVerticiesIndex = bufferHighIndex * 3;
						newOnes++;
					}
					things.Add(toAdd[0]);
					toAdd.RemoveAt(0);
					
				}
				
				Console.WriteLine("fitIns:" + fitIns + " - newOnes:" + newOnes);
			}
		}
		
		void CheckVertexBuffer(){
			int vertexCount = vertex.Length / 3;
			if(Globals.modelVertexBuffer == null)
				Globals.modelVertexBuffer = new VertexBuffer(vertexCount, VertexFormat.Float3, VertexFormat.Float2, VertexFormat.Float);
			else if(Globals.modelVertexBuffer.VertexCount < vertexCount){
					Globals.modelVertexBuffer.Dispose();
					Globals.modelVertexBuffer = new VertexBuffer(vertexCount, VertexFormat.Float3, VertexFormat.Float2, VertexFormat.Float);
				}
			if(bufferLowIndex != int.MaxValue){
				Globals.modelVertexBuffer.SetVertices(0, vertex, bufferLowIndex, bufferLowIndex, bufferHighIndex - bufferLowIndex);
				Globals.modelVertexBuffer.SetVertices(1, uv, bufferLowIndex, bufferLowIndex, bufferHighIndex - bufferLowIndex);
				int position = 0;
				
				if(prims.Length < things.Count){
					prims = new Primitive[things.Count];
					Array.Resize<float>(ref thingShaderInfo, things.Count * 7);
				}
				
				if(matrixNumber.Length < vertex.Length / 3)
					matrixNumber = new float[vertex.Length / 3];
				
				for(int i = 0; i < things.Count; i++){
					//MatrixNumber Array
					float[] number = new float[]{(i % Globals.AmountPerPush)};
					ArrayFillComplex(ref matrixNumber, position, ref number, things[i].prim.Count);
					position += things[i].prim.Count;		
				}
				Globals.modelVertexBuffer.SetVertices(2, matrixNumber, 0, 0, matrixNumber.Length);
			}
			prims = new Primitive[things.Count];
			for(int i = 0; i < things.Count; i++){
				//Prim Array
				prims[i] = things[i].prim;		
				//Matrix Array
				Array.Copy(things[i].scalexyzrot, 0, thingShaderInfo, i * 7, 7);
			}
			
			gc.SetVertexBuffer(0, Globals.modelVertexBuffer);
		}
		
		public void Update(){
			
			System.Diagnostics.Stopwatch sww = new System.Diagnostics.Stopwatch();
			sww.Start();
			CheckNewThings();
			sww.Stop();
			longestTook[0] = sww.ElapsedMilliseconds > longestTook[0] ? sww.ElapsedMilliseconds : longestTook[0];
			if(sw.ElapsedMilliseconds > 1000)
				Console.WriteLine("CheckNewThings():" + sww.ElapsedMilliseconds + "ms - longest:" + longestTook[0] + "ms");
			sww.Reset();
			sww.Start();
			foreach(Thing thing in things){
				thing.update();	
			}
			sww.Stop();
			longestTook[1] = sww.ElapsedMilliseconds > longestTook[1] ? sww.ElapsedMilliseconds : longestTook[1];
			if(sw.ElapsedMilliseconds > 1000)
				Console.WriteLine("foreach update:" + sww.ElapsedMilliseconds + "ms - longest:" + longestTook[1] + "ms");
			sww.Reset();
			sww.Start();			
			CheckVertexBuffer();		
			sww.Stop();
			longestTook[2] = sww.ElapsedMilliseconds > longestTook[2] ? sww.ElapsedMilliseconds : longestTook[2];
			if(sw.ElapsedMilliseconds > 1000)
				Console.WriteLine("CheckVertexBuffer():" + sww.ElapsedMilliseconds + "ms - longest:" + longestTook[2] + "ms");
		}
		
		public void Draw(){
			Stopwatch sww = new Stopwatch();
			sww.Start();
			Matrix4 VP = buildProjectionMatrix(ref gc); // Build camera to world projection			
			if(Globals.COMPUTE_BY == Globals.COMPUTATION_TYPE.GPU_SOFT){ // Pushing Matricies to shader as uniform
				gc.SetShaderProgram(Globals.gpuHard);
				int k = Globals.gpuHard.FindUniform("WorldViewProj");
				Globals.gpuHard.SetUniformValue(k, ref VP);
				int primCounter = 0, HowManyToPush = 0;
				while(primCounter < prims.Length){
					HowManyToPush = prims.Length - primCounter > 10 ? 10 : prims.Length - primCounter;
					//k = Globals.gpuSoft.FindUniform("modelToWorld");
					//Globals.gpuSoft.SetUniformValue(k, matricies, 0, primCounter, HowManyToPush);			
					k = Globals.gpuHard.FindUniform("scalexyzrot");
					Globals.gpuHard.SetUniformValue(k, thingShaderInfo, 0, primCounter * 7, HowManyToPush * 7);			
					gc.DrawArrays(prims, primCounter, HowManyToPush);
					primCounter += HowManyToPush;
				}
			}
			sww.Stop();
			longestTook[3] = sww.ElapsedMilliseconds > longestTook[3] ? sww.ElapsedMilliseconds : longestTook[3];
			if(sw.ElapsedMilliseconds > 1000){
				Console.WriteLine("Draw():" + sww.ElapsedMilliseconds + "ms - longest:" + longestTook[3] + "ms");
				Console.WriteLine(Globals.COMPUTE_BY.ToString() + " - Amount of Things:" + ThingCount() + " - fps:" + fpsCounter + " Memory usage: " + (GC.GetTotalMemory(true) / 1024) + "KB");
				fpsCounter = 0;
				sw.Reset();
				sw.Start();
			}
			fpsCounter++;
			Globals.frameCount++;
			longestTook = new long[10];
		}
		
		//Tweaked version of this to allow for positional inserts
		//http://coding.grax.com/2013/06/fast-array-fill-function-revisited.html}
		public void ArrayFillComplex<T>(ref T[] arrayToFill, int destinationIndex, ref T[] fillValue, int count){
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
		
		public Matrix4 buildProjectionMatrix(ref GraphicsContext gc){
			Matrix4 worldToCamera;
			Globals.cameraToWorld.Inverse(out worldToCamera);
			Matrix4 tmp = worldToCamera * cameraToProjection;
			return tmp;
		}	
	}
}

