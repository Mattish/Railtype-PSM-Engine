using System;
using System.Collections.Generic;
using Railtype_PSM_Engine.Util;
using Railtype_PSM_Engine.Entities;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core;

namespace Railtype_PSM_Engine{
	public class ModelManager{
		
		Dictionary<string,WaveFrontObject> _modelMap;
		List<Thing> _toAdd;
		List<Thing> _disposed;
		int vertexBufferLowIndex, vertexBufferHighIndex, vertexCountDiff, lastVertexIndex, lastVerticiesIndex;
		int indexBufferLowIndex, indexBufferHighIndex, indexCountDiff, lastIndexIndex, lastIndiciesIndex;
		int fragmentedFloats;
		
		float[] vertex, uv;
		ushort[] indices;
		
		public ModelManager(){
			_modelMap = new Dictionary<string, WaveFrontObject>();
			_toAdd = new List<Thing>();
			_disposed = new List<Thing>();
			lastVertexIndex = lastIndexIndex = -1;
			lastVerticiesIndex = lastIndiciesIndex = 0;
			fragmentedFloats = 0;
			
			vertex = new float[(ushort.MaxValue * 3)];
			uv = new float[(ushort.MaxValue * 2)];
			indices = new ushort[ushort.MaxValue];
		}
		
		public void AddModel(string name, WaveFrontObject wfo){
			if (!ModelExists(name))
				_modelMap.Add(name,wfo);
		}
		public WaveFrontObject GetModel(string name){
			return _modelMap[name];
		}
		public void RemoveModel(string name){
			if (!ModelExists(name))
				_modelMap.Remove(name);
		}
		
		public void AddThing(Thing thing){
			_toAdd.Add(thing);
		}
		
		public void RemoveThing(Thing thing){
			_disposed.Add(thing);
		}
	
		
		private bool ModelExists(string name){
			return _modelMap.ContainsKey(name);	
		}
		
		public void Update(){
			vertexBufferLowIndex = indexBufferLowIndex = int.MaxValue;
			vertexBufferHighIndex = vertexCountDiff = indexBufferHighIndex = indexCountDiff = 0;
			if(_toAdd.Count > 0){ // Need to push things into main list
				while(_toAdd.Count > 0){
					bool foundDisposable = false;
					if(_disposed.Count > 0){
						for(int i = 0; i < _disposed.Count; i++){
							indexCountDiff = _disposed[i].prim.Count - _toAdd[0].prim.Count;
							vertexCountDiff = _disposed[i].vertexCount - _toAdd[0].vertexCount;
							if(_disposed[i].vertexIndex == lastVertexIndex){ // if its the last prim in vertex array
								vertexCountDiff = indexCountDiff = 0;
								
								lastVerticiesIndex = (lastVertexIndex + _toAdd[0].vertexCount) * 3;
								lastIndiciesIndex = (lastIndexIndex + _toAdd[0].prim.Count);
								
								if(vertex.Length < lastVerticiesIndex){
									Array.Resize<ushort>(ref indices, lastIndiciesIndex);
									Array.Resize<float>(ref vertex, lastVerticiesIndex);
									Array.Resize<float>(ref uv, (lastVertexIndex + _toAdd[0].vertexCount) * 2);
								}
							}	
							if(vertexCountDiff >= 0){ // Found a dispose to replace
								fragmentedFloats -= _toAdd[0].vertexCount;
								_toAdd[0].vertexIndex = _disposed[i].vertexIndex;
								_toAdd[0].prim.First = _disposed[i].prim.First;
								
								_modelMap[_toAdd[0].modelName].PutModelVertexIntoArray(ref vertex, _disposed[i].vertexIndex * 3);
								_modelMap[_toAdd[0].modelName].PutModelUVIntoArray(ref uv, _disposed[i].vertexIndex * 2);
								
								_modelMap[_toAdd[0].modelName].PutIndiciesIntoArray(ref indices, _disposed[i].prim.First);
								for(int ii = 0; ii < _modelMap[_toAdd[0].modelName].models[0].indices.Length; ii++)
									indices[_disposed[i].prim.First+i] += _toAdd[0].prim.Count;							
								
								
								// Set BufferLowIndex to index of where _disposed Index is
								vertexBufferLowIndex = _disposed[i].vertexIndex < vertexBufferLowIndex ? _disposed[i].vertexIndex : vertexBufferLowIndex;
								indexBufferLowIndex = _disposed[i].prim.First < indexBufferLowIndex ? _disposed[i].prim.First : indexBufferLowIndex;
								// Set BufferHighIndex to index of where _disposed Index is + toAdd vertex/index count
								vertexBufferHighIndex = (_disposed[i].vertexIndex + _toAdd[0].vertexCount) > vertexBufferHighIndex ? 
									(_disposed[i].vertexIndex + _toAdd[0].vertexCount) : vertexBufferHighIndex;
								indexBufferHighIndex = (_disposed[i].prim.First + _toAdd[0].prim.Count) > indexBufferHighIndex ?
									(_disposed[i].prim.First + _toAdd[0].prim.Count) : indexBufferHighIndex;
								
								
								if(vertexCountDiff > 0){
									_disposed[i].vertexIndex += (ushort)_toAdd[0].vertexCount;
									_disposed[i].prim.First += _toAdd[0].prim.Count;
								} else
									_disposed.RemoveAt(i);
								foundDisposable = true;
								break;
							}
						}
					}
					
					if(!foundDisposable){ // Cant find something to replace, add to the end of array
						if(lastVertexIndex < 0)
							lastVertexIndex = lastIndexIndex = 0;
						else{
							lastVertexIndex = (lastVertexIndex + _toAdd[0].vertexCount);
							lastIndexIndex = (lastIndexIndex + _toAdd[0].prim.Count);	
						}
						
						_toAdd[0].vertexIndex = (ushort)lastVertexIndex;
						_toAdd[0].prim.First = (ushort)lastIndexIndex;
						
						if(vertex.Length < (lastVerticiesIndex + _toAdd[0].vertexCount * 3)){
							Array.Resize<float>(ref vertex, (lastVerticiesIndex + _toAdd[0].vertexCount * 3));	
							Array.Resize<float>(ref uv, (lastVerticiesIndex + _toAdd[0].vertexCount * 2));
							Array.Resize<ushort>(ref indices, (lastIndiciesIndex + _toAdd[0].prim.Count));
						}
						
						_modelMap[_toAdd[0].modelName].PutModelVertexIntoArray(ref vertex, _toAdd[0].vertexIndex * 3);
						_modelMap[_toAdd[0].modelName].PutModelUVIntoArray(ref uv, _toAdd[0].vertexIndex * 2);
						
						_modelMap[_toAdd[0].modelName].PutIndiciesIntoArray(ref indices, _toAdd[0].prim.First);
						for(int i = 0; i < _modelMap[_toAdd[0].modelName].models[0].indices.Length; i++)
							indices[_toAdd[0].prim.First+i] += _toAdd[0].prim.Count;
						
						vertexBufferLowIndex = _toAdd[0].vertexIndex < vertexBufferLowIndex ? _toAdd[0].vertexIndex : vertexBufferLowIndex;
						indexBufferLowIndex = _toAdd[0].prim.First < indexBufferLowIndex ? _toAdd[0].prim.First : indexBufferLowIndex;
						
						vertexBufferHighIndex = _toAdd[0].vertexIndex + _toAdd[0].vertexCount;
						indexBufferHighIndex = _toAdd[0].prim.First + _toAdd[0].prim.Count;
						
						lastVerticiesIndex = vertexBufferHighIndex * 3;
						lastIndiciesIndex = indexBufferHighIndex;
					}
					//_modelMap[_toAdd[0].modelName] = _toAdd[0];
					_toAdd.RemoveAt(0);	
				}
			}
			CheckVertexBuffer();
		}
		
		private void CheckVertexBuffer(){
			int vertexCount = vertex.Length / 3;
			int indexCount = indices.Length;
			
			if(Globals.modelVertexBuffer == null)
				Globals.modelVertexBuffer = new VertexBuffer(vertexCount, indexCount, VertexFormat.Float3, VertexFormat.Float2, VertexFormat.UShort);
			else if(Globals.modelVertexBuffer.VertexCount < vertexCount){
					Globals.modelVertexBuffer.Dispose();
					Globals.modelVertexBuffer = new VertexBuffer(vertexCount, indexCount, VertexFormat.Float3, VertexFormat.Float2, VertexFormat.UShort);
				}
			if(vertexBufferLowIndex != int.MaxValue){
				Globals.modelVertexBuffer.SetVertices(0, vertex, vertexBufferLowIndex, vertexBufferLowIndex, vertexBufferHighIndex - vertexBufferLowIndex);
				Globals.modelVertexBuffer.SetVertices(1, uv, vertexBufferLowIndex, vertexBufferLowIndex, vertexBufferHighIndex - vertexBufferLowIndex);
				Globals.modelVertexBuffer.SetIndices(indices, indexBufferLowIndex, indexBufferLowIndex, indexBufferHighIndex - indexBufferLowIndex);
				
			}
					
			Globals.gc.SetVertexBuffer(0, Globals.modelVertexBuffer);
		}
		
		
	}
}

