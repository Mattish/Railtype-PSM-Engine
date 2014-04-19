using System;
using System.Collections.Generic;
using Railtype_PSM_Engine.Util;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core;

namespace Railtype_PSM_Engine{
	public class ModelManager{
		
		Dictionary<string,WaveFrontObject> _modelMap;
		Dictionary<string,Primitive> _primMap;
		List<KeyValuePair<string,WaveFrontObject>> _toAdd;
		List<WaveFrontObject> _disposed;
		int vertexBufferLowIndex, vertexBufferHighIndex, vertexCountDiff, lastVertexIndex, lastVerticiesIndex;
		int indexBufferLowIndex, indexBufferHighIndex, indexCountDiff, lastIndexIndex, lastIndiciesIndex;
		int fragmentedFloats;
		
		float[] vertex, uv;
		ushort[] indices;
		
		public ModelManager(){
			_modelMap = new Dictionary<string, WaveFrontObject>();
			_primMap = new Dictionary<string, Primitive>();
			_toAdd = new List<KeyValuePair<string, WaveFrontObject>>();
			_disposed = new List<WaveFrontObject>();
			lastVertexIndex = lastIndexIndex = -1;
			lastVerticiesIndex = lastIndiciesIndex = 0;
			fragmentedFloats = 0;
			
			vertex = new float[(ushort.MaxValue * 3)];
			uv = new float[(ushort.MaxValue * 2)];
			indices = new ushort[ushort.MaxValue];
		}
		
		public void Add(string name, WaveFrontObject wfo){
			if (!Exists(name))
				_toAdd.Add(new KeyValuePair<string,WaveFrontObject>(name,wfo));
		}
		
		public bool Exists(string name){
			return _modelMap.ContainsKey(name);	
		}
		
		public Primitive GetModelPrimitiveByName(string name){
			return _primMap[name];	
		}
		
		public void Update(){
			vertexBufferLowIndex = indexBufferLowIndex = int.MaxValue;
			vertexBufferHighIndex = vertexCountDiff = indexBufferHighIndex = indexCountDiff = 0;
			if(_toAdd.Count > 0){ // Need to push things into main list
				while(_toAdd.Count > 0){
					bool foundDisposable = false;
					if(_disposed.Count > 0){
						for(int i = 0; i < _disposed.Count; i++){
							indexCountDiff = _disposed[i].prim.Count - _toAdd[0].Value.prim.Count;
							vertexCountDiff = _disposed[i].vertexCount - _toAdd[0].Value.vertexCount;
							if(_disposed[i].vertexIndex == lastVertexIndex){ // if its the last prim in vertex array
								vertexCountDiff = indexCountDiff = 0;
								
								lastVerticiesIndex = (lastVertexIndex + _toAdd[0].Value.vertexCount) * 3;
								lastIndiciesIndex = (lastIndexIndex + _toAdd[0].Value.prim.Count);
								
								if(vertex.Length < lastVerticiesIndex){
									Array.Resize<ushort>(ref indices, lastIndiciesIndex);
									Array.Resize<float>(ref vertex, lastVerticiesIndex);
									Array.Resize<float>(ref uv, (lastVertexIndex + _toAdd[0].Value.vertexCount) * 2);
								}
							}	
							if(vertexCountDiff >= 0){ // Found a dispose to replace
								fragmentedFloats -= _toAdd[0].Value.vertexCount;
								_toAdd[0].Value.vertexIndex = _disposed[i].vertexIndex;
								_toAdd[0].Value.prim.First = _disposed[i].prim.First;
								
								
								_toAdd[0].Value.PutModelVertexIntoArray(ref vertex, _disposed[i].vertexIndex * 3);
								_toAdd[0].Value.PutModelUVIntoArray(ref uv, _disposed[i].vertexIndex * 2);
								for(int ii = 0; ii < _toAdd[0].Value.models[0].indices.Length; ii++)
									_toAdd[0].Value.models[0].indices[ii] += _toAdd[0].Value.vertexIndex;								
								_toAdd[0].Value.PutIndiciesIntoArray(ref indices, _disposed[i].prim.First);
								
								// Set BufferLowIndex to index of where _disposed Index is
								vertexBufferLowIndex = _disposed[i].vertexIndex < vertexBufferLowIndex ? _disposed[i].vertexIndex : vertexBufferLowIndex;
								indexBufferLowIndex = _disposed[i].prim.First < indexBufferLowIndex ? _disposed[i].prim.First : indexBufferLowIndex;
								// Set BufferHighIndex to index of where _disposed Index is + toAdd vertex/index count
								vertexBufferHighIndex = (_disposed[i].vertexIndex + _toAdd[0].Value.vertexCount) > vertexBufferHighIndex ? 
									(_disposed[i].vertexIndex + _toAdd[0].Value.vertexCount) : vertexBufferHighIndex;
								indexBufferHighIndex = (_disposed[i].prim.First + _toAdd[0].Value.prim.Count) > indexBufferHighIndex ?
									(_disposed[i].prim.First + _toAdd[0].Value.prim.Count) : indexBufferHighIndex;
								
								
								if(vertexCountDiff > 0){
									_disposed[i].vertexIndex += (ushort)_toAdd[0].Value.vertexCount;
									_disposed[i].prim.First += _toAdd[0].Value.prim.Count;
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
							lastVertexIndex = (lastVertexIndex + _toAdd[0].Value.vertexCount);
							lastIndexIndex = (lastIndexIndex + _toAdd[0].Value.prim.Count);	
						}
						
						_toAdd[0].Value.vertexIndex = (ushort)lastVertexIndex;
						_toAdd[0].Value.prim.First = (ushort)lastIndexIndex;
						
						if(vertex.Length < (lastVerticiesIndex + _toAdd[0].Value.vertexCount * 3)){
							Array.Resize<float>(ref vertex, (lastVerticiesIndex + _toAdd[0].Value.vertexCount * 3));	
							Array.Resize<float>(ref uv, (lastVerticiesIndex + _toAdd[0].Value.vertexCount * 2));
							Array.Resize<ushort>(ref indices, (lastIndiciesIndex + _toAdd[0].Value.prim.Count));
						}
						
						_toAdd[0].Value.PutModelVertexIntoArray(ref vertex, _toAdd[0].Value.vertexIndex * 3);
						_toAdd[0].Value.PutModelUVIntoArray(ref uv, _toAdd[0].Value.vertexIndex * 2);
						
						for(int i = 0; i < _toAdd[0].Value.models[0].indices.Length; i++)
							_toAdd[0].Value.models[0].indices[i] += _toAdd[0].Value.vertexIndex;
						_toAdd[0].Value.PutIndiciesIntoArray(ref indices, _toAdd[0].Value.prim.First);
						
						vertexBufferLowIndex = _toAdd[0].Value.vertexIndex < vertexBufferLowIndex ? _toAdd[0].Value.vertexIndex : vertexBufferLowIndex;
						indexBufferLowIndex = _toAdd[0].Value.prim.First < indexBufferLowIndex ? _toAdd[0].Value.prim.First : indexBufferLowIndex;
						
						vertexBufferHighIndex = _toAdd[0].Value.vertexIndex + _toAdd[0].Value.vertexCount;
						indexBufferHighIndex = _toAdd[0].Value.prim.First + _toAdd[0].Value.prim.Count;
						
						lastVerticiesIndex = vertexBufferHighIndex * 3;
						lastIndiciesIndex = indexBufferHighIndex;
					}
					_modelMap[_toAdd[0].Key] = _toAdd[0].Value;
					_primMap[_toAdd[0].Key] = _toAdd[0].Value.prim;
					_toAdd.RemoveAt(0);	
				}
			}
			CheckVertexBuffer();
		}
		
		private void CheckVertexBuffer(){
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
				Globals.modelVertexBuffer.SetIndices(indices, indexBufferLowIndex, indexBufferLowIndex, indexBufferHighIndex - indexBufferLowIndex);
			}
					
			Globals.gc.SetVertexBuffer(0, Globals.modelVertexBuffer);
		}
		
		
	}
}

