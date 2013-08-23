using System;
using System.Collections.Generic;
using Railtype_PSM_Engine.Entities;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core;
using System.Diagnostics;

namespace Railtype_PSM_Engine.Entities{
	public class ThingManager{
		float[] vertex, matrixNumber, uv;
		ushort[] indices;
		GraphicsContext gc;
		int fragmentedFloats, fpsCounter;
		long[] longestTook;
		List<Thing> disposed, things, toAdd;
		//float[] thingShaderInfo;
		Matrix4[] matrixForShader;
		Matrix4 cameraToProjection;
		Primitive[] prims;
		Stopwatch sw;
					
		public ThingManager(GraphicsContext gc_){
			sw = new Stopwatch();
			gc = gc_;
			fragmentedFloats = 0;
			longestTook = new long[10];
			fpsCounter = 0;
			//cameraToProjection = Matrix4.Perspective(FMath.Radians(30.0f), gc.Screen.AspectRatio, 1f, 1000.0f);
			cameraToProjection = Matrix4.Identity;
			cameraToProjection *= Matrix4.Ortho(-gc.Screen.AspectRatio,gc.Screen.AspectRatio
			                                    ,-1.0f,1.0f,1.0f,1000.0f);
			lastVertexIndex = lastIndexIndex = -1;
			lastVerticiesIndex = lastIndiciesIndex = 0;
			vertex = new float[(ushort.MaxValue * 3)];
			uv = new float[(ushort.MaxValue * 2)];
			matrixNumber = new float[ushort.MaxValue];
			indices = new ushort[ushort.MaxValue];
			//thingShaderInfo = new float[7];
			matrixForShader = new Matrix4[10];
			
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
		
		int vertexBufferLowIndex, vertexBufferHighIndex, vertexCountDiff, lastVertexIndex, lastVerticiesIndex;
		int indexBufferLowIndex, indexBufferHighIndex, indexCountDiff, lastIndexIndex, lastIndiciesIndex;
		

		public void CheckNewThings(){
			vertexBufferLowIndex = indexBufferLowIndex = int.MaxValue;
			vertexBufferHighIndex = vertexCountDiff = indexBufferHighIndex = indexCountDiff = 0;
			if(toAdd.Count > 0){ // Need to push things into main list
				while(toAdd.Count > 0){
					bool foundDisposable = false;
					
					if(disposed.Count > 0){
						for(int i = 0; i < disposed.Count; i++){
							indexCountDiff = disposed[i].prim.Count - toAdd[0].prim.Count;
							vertexCountDiff = disposed[i].vertexCount - toAdd[0].vertexCount;
							if(disposed[i].vertexIndex == lastVertexIndex){ // if its the last prim in vertex array
								vertexCountDiff = indexCountDiff = 0;
								
								lastVerticiesIndex = (lastVertexIndex + toAdd[0].vertexCount) * 3;
								lastIndiciesIndex = (lastIndexIndex + toAdd[0].prim.Count);
								
								if(vertex.Length < lastVerticiesIndex){
									Array.Resize<ushort>(ref indices, lastIndiciesIndex);
									Array.Resize<float>(ref vertex, lastVerticiesIndex);
									Array.Resize<float>(ref uv, (lastVertexIndex + toAdd[0].vertexCount) * 2);
								}
							}	
							if(vertexCountDiff >= 0){ // Found a dispose to replace
								fragmentedFloats -= toAdd[0].vertexCount;
								toAdd[0].vertexIndex = disposed[i].vertexIndex;
								toAdd[0].prim.First = disposed[i].prim.First;
								
								
								toAdd[0].PutModelVertexIntoArray(ref vertex, disposed[i].vertexIndex * 3);
								toAdd[0].PutModelUVIntoArray(ref uv, disposed[i].vertexIndex * 2);
								for(int ii = 0; ii < toAdd[0].indicies.Length; ii++)
									toAdd[0].indicies[ii] += toAdd[0].vertexIndex;								
								toAdd[0].PutIndiciesIntoArray(ref indices,disposed[i].prim.First);
								
								// Set BufferLowIndex to index of where Disposed Index is
								vertexBufferLowIndex = disposed[i].vertexIndex < vertexBufferLowIndex ? disposed[i].vertexIndex : vertexBufferLowIndex;
								indexBufferLowIndex = disposed[i].prim.First < indexBufferLowIndex ? disposed[i].prim.First : indexBufferLowIndex;
								// Set BufferHighIndex to index of where Disposed Index is + toAdd vertex/index count
								vertexBufferHighIndex = (disposed[i].vertexIndex + toAdd[0].vertexCount) > vertexBufferHighIndex ? 
									(disposed[i].vertexIndex + toAdd[0].vertexCount) : vertexBufferHighIndex;
								indexBufferHighIndex = (disposed[i].prim.First + toAdd[0].prim.Count) > indexBufferHighIndex ?
									(disposed[i].prim.First + toAdd[0].prim.Count) : indexBufferHighIndex;
								
								
								if(vertexCountDiff > 0){
									disposed[i].vertexIndex += (ushort)toAdd[0].vertexCount;
									disposed[i].prim.First += toAdd[0].prim.Count;
								}
								else
									disposed.RemoveAt(i);
								foundDisposable = true;
								break;
							}
						}
					}
					
					if(!foundDisposable){ // Cant find something to replace, add to the end of array
						if(lastVertexIndex < 0)
							lastVertexIndex = lastIndexIndex = 0;
						else{
							lastVertexIndex = (lastVertexIndex + toAdd[0].vertexCount);
							lastIndexIndex = (lastIndexIndex + toAdd[0].prim.Count);	
						}
						
						toAdd[0].vertexIndex = (ushort)lastVertexIndex;
						toAdd[0].prim.First = (ushort)lastIndexIndex;
						
						if(vertex.Length < (lastVerticiesIndex + toAdd[0].vertexCount * 3)){
							Array.Resize<float>(ref vertex, (lastVerticiesIndex + toAdd[0].vertexCount * 3));	
							Array.Resize<float>(ref uv, (lastVerticiesIndex + toAdd[0].vertexCount * 2));
							Array.Resize<ushort>(ref indices, (lastIndiciesIndex + toAdd[0].prim.Count));
						}
						
						toAdd[0].PutModelVertexIntoArray(ref vertex, toAdd[0].vertexIndex * 3);
						toAdd[0].PutModelUVIntoArray(ref uv, toAdd[0].vertexIndex * 2);
						
						for(int i = 0; i < toAdd[0].indicies.Length; i++)
							toAdd[0].indicies[i] += toAdd[0].vertexIndex;
						toAdd[0].PutIndiciesIntoArray(ref indices, toAdd[0].prim.First);
						
						vertexBufferLowIndex = toAdd[0].vertexIndex < vertexBufferLowIndex ? toAdd[0].vertexIndex : vertexBufferLowIndex;
						indexBufferLowIndex = toAdd[0].prim.First < indexBufferLowIndex ? toAdd[0].prim.First : indexBufferLowIndex;
						
						vertexBufferHighIndex = toAdd[0].vertexIndex + toAdd[0].vertexCount;
						indexBufferHighIndex = toAdd[0].prim.First + toAdd[0].prim.Count;
						
						lastVerticiesIndex = vertexBufferHighIndex * 3;
						lastIndiciesIndex = indexBufferHighIndex;
					}
					things.Add(toAdd[0]);
					toAdd.RemoveAt(0);	
				}
			}
		}
		
		void CheckVertexBuffer(){
			int vertexCount = vertex.Length / 3;
			int indexCount = indices.Length;
			
			if(Globals.modelVertexBuffer == null)
				Globals.modelVertexBuffer = new VertexBuffer(vertexCount, indexCount, VertexFormat.Float3, VertexFormat.Float2, VertexFormat.Float);
			else if(Globals.modelVertexBuffer.VertexCount < vertexCount){
					Globals.modelVertexBuffer.Dispose();
					Globals.modelVertexBuffer = new VertexBuffer(vertexCount, indexCount, VertexFormat.Float3, VertexFormat.Float2, VertexFormat.Float);
				}
			if(vertexBufferLowIndex != int.MaxValue){
				Globals.modelVertexBuffer.SetVertices(0, vertex, vertexBufferLowIndex, vertexBufferLowIndex, vertexBufferHighIndex - vertexBufferLowIndex);
				Globals.modelVertexBuffer.SetVertices(1, uv, vertexBufferLowIndex, vertexBufferLowIndex, vertexBufferHighIndex - vertexBufferLowIndex);
				Globals.modelVertexBuffer.SetIndices(indices,indexBufferLowIndex,indexBufferLowIndex,indexBufferHighIndex - indexBufferLowIndex);
				
				
				if(prims.Length < things.Count){
					prims = new Primitive[things.Count];
					//Array.Resize<float>(ref thingShaderInfo, things.Count * 7);
					Array.Resize<Matrix4>(ref matrixForShader, things.Count);
				}
				
				if(matrixNumber.Length < vertex.Length / 3)
					matrixNumber = new float[vertex.Length / 3];
				
				for(int i = 0; i < things.Count; i++){
					//MatrixNumber Array
					float[] number = new float[]{(i % Globals.AmountPerPush)};
					ArrayFillComplex(ref matrixNumber, things[i].vertexIndex, ref number, things[i].vertexCount);		
				}
				Globals.modelVertexBuffer.SetVertices(2, matrixNumber, 0, 0, matrixNumber.Length);
			}
			prims = new Primitive[things.Count];
			for(int i = 0; i < things.Count; i++){
				//Prim Array
				prims[i] = things[i].prim;		
				//Matrix Array
				//Array.Copy(things[i].scalexyzrot, 0, thingShaderInfo, i * 7, 7);
				matrixForShader[i] = things[i].modelToWorld;
			}
			/*Globals.modelVertexBuffer.SetVertices(0, vertex);
			Globals.modelVertexBuffer.SetVertices(1, uv);
			Globals.modelVertexBuffer.SetVertices(2, matrixNumber);
			Globals.modelVertexBuffer.SetIndices(indicies);*/
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
				thing.Update();	
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
					HowManyToPush = prims.Length - primCounter > Globals.AmountPerPush ? Globals.AmountPerPush : prims.Length - primCounter;
					k = Globals.gpuHard.FindUniform("modelToWorld");
					Globals.gpuHard.SetUniformValue(k, matrixForShader, 0, primCounter, HowManyToPush);			
					//k = Globals.gpuHard.FindUniform("scalexyzrot");
					//Globals.gpuHard.SetUniformValue(k, thingShaderInfo, 0, primCounter * 7, HowManyToPush * 7);			
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

