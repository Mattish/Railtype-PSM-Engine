using System;
using System.Collections.Generic;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;

namespace RailTypePSMEngineNew{
	public class ThingHandler{
		
		private int _totalAmount;
		private int _totalDrawableAmount;
		
		
		// shaderNo, textureNo
		private Dictionary<Tuple<int,int>,List<Thing>> _batchedMap;
		private Dictionary<Tuple<int,int>,List<Thing>> _batchedMapPerUpdate;
		
		
		public ThingHandler(){
			_batchedMap = new Dictionary<Tuple<int, int>, List<Thing>>();
			_batchedMapPerUpdate = new Dictionary<Tuple<int, int>, List<Thing>>();
			_totalAmount = _totalDrawableAmount = 0;
		}
		
		
		public void TryGetBatchesOfThings(out BatchInfo[] bi, out Primitive[] prims, out Matrix4[] matricies, out int[] thingNumbers){
			int amountOfBatches = 1;
			foreach(KeyValuePair<Tuple<int,int>,List<Thing>> entry in _batchedMapPerUpdate){
				amountOfBatches += (int)Math.Ceiling((double)(entry.Value.Count/16));
			}
			
			bi = new BatchInfo[amountOfBatches];
			prims = new Primitive[_totalDrawableAmount];
			matricies = new Matrix4[_totalDrawableAmount];
			thingNumbers = new int[_totalDrawableAmount];
			
			int thingIndex = 0;
			int batchInfoIndex = 0;
			foreach(KeyValuePair<Tuple<int,int>,List<Thing>> entry in _batchedMapPerUpdate){
				int thingCounter = 0;
				foreach(Thing thg in entry.Value){
					prims[thingIndex+thingCounter] = thg.prim;
					matricies[thingIndex+thingCounter] = thg.modelToWorld;
					thingNumbers[thingIndex+thingCounter] = thg.globalNumber;
					thingCounter++;
				}
				int AmountOfThings = 0;
				int totalAmountOfThings = 0;
				while(totalAmountOfThings < thingCounter){
					AmountOfThings = (thingCounter-totalAmountOfThings) < 16 ? (thingCounter-totalAmountOfThings) : 16;
					bi[batchInfoIndex] = new BatchInfo(){shaderNumber = entry.Key.Item1,
														textureNumber = entry.Key.Item2,
														count = AmountOfThings,
														index = thingIndex+totalAmountOfThings};
					totalAmountOfThings += AmountOfThings;
					batchInfoIndex++;
				}
				thingIndex += thingCounter;
			}
		}
		
		public void AddThing(Thing thg){
			if (!_batchedMap.ContainsKey(thg.shaderTextureNo)){
				_batchedMap[thg.shaderTextureNo] = new List<Thing>(1);
				_batchedMap[thg.shaderTextureNo].Add(thg);
			}
			else{
				_batchedMap[thg.shaderTextureNo].Add(thg);
			}
			_totalAmount++;
		}
		
		public void Update(){
			_batchedMapPerUpdate.Clear();
			_totalDrawableAmount = 0;
			foreach(KeyValuePair<Tuple<int,int>,List<Thing>> entry in _batchedMap){
				_batchedMapPerUpdate.Add(entry.Key,new List<Thing>(1));
				foreach(Thing thg in entry.Value){
					thg.Update();
					if (thg.draw){
						_batchedMapPerUpdate[thg.shaderTextureNo].Add(thg);
						_totalDrawableAmount++;
					}
				}
			}
		}
	}
	
	public struct BatchInfo{
		public int shaderNumber;	
		public int textureNumber;
		public int count;
		public int index;
	}
}

