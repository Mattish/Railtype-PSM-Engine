using System;
using System.Collections.Generic;
using Railtype_PSM_Engine.Entities;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core;

namespace Railtype_PSM_Engine{
	public class ThingManager{
		float[] vertex, matrixNumber;
		GraphicsContext gc;
		int fragmentedFloats, lastIndex, lastVerticiesIndex;
		List<Thing> disposed, things, toAdd;
		Matrix4[] matricies;
		Matrix4 cameraToProjection;
		Primitive[] prims;
					
		public ThingManager(GraphicsContext gc_){
			gc = gc_;
			fragmentedFloats = 0;
			cameraToProjection = Matrix4.Perspective(FMath.Radians(45.0f), gc.Screen.AspectRatio, 1.0f, 1000.0f);
			lastIndex = -1;
			lastVerticiesIndex = 0;
			vertex = new float[65000*3];
			matrixNumber = new float[vertex.Length/3];
			matricies = new Matrix4[1];
			disposed = new List<Thing>();
			things = new List<Thing>();
			toAdd = new List<Thing>();
			prims = new Primitive[1];
		}
		
		public int ThingCount(){
			return things.Count;	
		}
		
		public void AddThing(Thing input){
			toAdd.Add(input);
		}
		
		public Thing GetFirstThing(){
			if (things.Count > 0)
				return things[0];	
			else
				return null;
		}
		
		public bool RemoveThing(Thing input){
			int index = things.IndexOf(input);
			if (index != -1){ // Found it
				disposed.Add(things[index]);
				fragmentedFloats += things[index].prim.Count;
				things.RemoveAt(index);
				return true;
			}
			return false;
		}
		
		public void Update(){
			int bufferLowIndex = int.MaxValue, bufferHighIndex = 0, primCountDiff = 0;			
			if (toAdd.Count > 0){ // Need to push things into main list
				while(toAdd.Count > 0){
					bool foundDisposable = false;
					if (disposed.Count > 0){
						for(int i = 0; i < disposed.Count; i++){
							primCountDiff = disposed[i].prim.Count - toAdd[0].prim.Count;
							if(disposed[i].prim.First == lastIndex){ // if its the last prim in vertex array
								primCountDiff = 0;
								lastVerticiesIndex = (lastIndex + toAdd[0].prim.Count)*3;
								if (vertex.Length < lastVerticiesIndex)
									Array.Resize<float>(ref vertex,lastVerticiesIndex);
							}	
							if (primCountDiff >= 0){ // Found a dispose to replace
								fragmentedFloats -= toAdd[0].prim.Count;
								toAdd[0].PutModelVertexIntoArray(ref vertex, disposed[i].prim.First*3);
								bufferLowIndex = disposed[i].prim.First < bufferLowIndex ? disposed[i].prim.First : bufferLowIndex;
								bufferHighIndex = (disposed[i].prim.First+toAdd[0].prim.Count) > bufferHighIndex ? 
									(disposed[i].prim.First+toAdd[0].prim.Count) : bufferHighIndex;
								toAdd[0].prim.First = disposed[i].prim.First;
								if (primCountDiff > 0)
									disposed[i].prim.First += toAdd[0].prim.Count;
								else
									disposed.RemoveAt(i);
								foundDisposable = true;
								break;
							}
						}
					}
					if (!foundDisposable){ // Cant find something to replace, add to the end of array
						if (lastIndex < 0)
							lastIndex = 0;
						else
							lastIndex = (lastIndex + toAdd[0].prim.Count);
						toAdd[0].prim.First = (ushort)lastIndex;
						if (vertex.Length < (lastVerticiesIndex + toAdd[0].prim.Count)*3){
							Array.Resize<float>(ref vertex,(lastVerticiesIndex + toAdd[0].prim.Count*3));	
						}
						toAdd[0].PutModelVertexIntoArray(ref vertex, toAdd[0].prim.First*3);
						bufferLowIndex = toAdd[0].prim.First < bufferLowIndex ? toAdd[0].prim.First : bufferLowIndex;
						bufferHighIndex = toAdd[0].prim.First + toAdd[0].prim.Count;
						lastVerticiesIndex = bufferHighIndex*3;
					}
					things.Add(toAdd[0]);
					toAdd.RemoveAt(0);
				}
			}
			
			foreach(Thing thing in things){
				thing.update();	
			}
			
			int vertexCount = vertex.Length/3;
			if(Globals.modelVertexBuffer == null)
				Globals.modelVertexBuffer = new VertexBuffer(vertexCount, VertexFormat.Float3, VertexFormat.Float);
			else if(Globals.modelVertexBuffer.VertexCount < vertexCount){
				Globals.modelVertexBuffer.Dispose();
				Globals.modelVertexBuffer = new VertexBuffer(vertexCount, VertexFormat.Float3, VertexFormat.Float);				
			}
			
			if(bufferLowIndex != int.MaxValue){
				Globals.modelVertexBuffer.SetVertices(0, vertex,bufferLowIndex,bufferLowIndex,bufferHighIndex-bufferLowIndex);
				//Globals.modelVertexBuffer.SetVertices(0, vertex,0,0,vertexCount);
				int position = 0;
				
				if (prims.Length < things.Count){
					prims = new Primitive[things.Count];
					Array.Resize<Matrix4>(ref matricies,things.Count);
				}
				
				if (matrixNumber.Length < vertex.Length/3)
					matrixNumber = new float[vertex.Length/3];
				
				for(int i = 0; i < things.Count; i++){
					//MatrixNumber Array
					float[] number = new float[]{(i % Globals.AmountPerPush)};
					ArrayFillComplex(ref matrixNumber, position, ref number, things[i].prim.Count);
					position += things[i].prim.Count;		
				}
				Globals.modelVertexBuffer.SetVertices(1, matrixNumber,0,0,matrixNumber.Length);
			}
			prims = new Primitive[things.Count];
			for(int i = 0; i < things.Count; i++){
				//Prim Array
				prims[i] = things[i].prim;		
				//Matrix Array
				matricies[i] = things[i].modelToWorld;
			}
			
			gc.SetVertexBuffer(0, Globals.modelVertexBuffer);
		}
		
		public void Draw(){
			Matrix4 VP = buildProjectionMatrix(ref gc); // Build camera to world projection			
			if(Globals.COMPUTE_BY == Globals.COMPUTATION_TYPE.GPU_SOFT){ // Pushing Matricies to shader as uniform
				gc.SetShaderProgram(Globals.gpuSoft);
				int k = Globals.gpuSoft.FindUniform("WorldViewProj");
				Globals.gpuSoft.SetUniformValue(k, ref VP);
				int primCounter = 0, HowManyToPush = 0;
				while(primCounter < prims.Length){
					HowManyToPush = prims.Length - primCounter > 10 ? 10 : prims.Length - primCounter;
					k = Globals.gpuSoft.FindUniform("modelToWorld");
					Globals.gpuSoft.SetUniformValue(k, matricies, 0, primCounter, HowManyToPush);			
					gc.DrawArrays(prims, primCounter, HowManyToPush);
					primCounter += HowManyToPush;
				}
			}
			Globals.frameCount++;
		}
		
		public void ArrayFillComplex<T>(ref T[] arrayToFill, int destinationIndex, ref T[] fillValue, int count){
			/*if (fillValue.Length >= arrayToFill.Length){
		        throw new ArgumentException("fillValue array length must be smaller than length of arrayToFill");
		    }*/
		 
			// set the initial array value
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

