using System;
using System.Collections.Generic;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;

namespace RailTypePSMEngineNew{
	public class ThingHandler{
		
		private int _totalAmount;
		private int _totalDrawableAmount;
		
		static private ThingHandler _th;
		
		// shaderNo, textureNo
		private Dictionary<Tuple<int,int>,List<Thing>> _batchedMap;
		private Dictionary<Tuple<int,int>,List<Thing>> _batchedMapPerUpdate;
		
		private ThingHandler(){
			_batchedMap = new Dictionary<Tuple<int, int>, List<Thing>>();
			_batchedMapPerUpdate = new Dictionary<Tuple<int, int>, List<Thing>>();
			_totalAmount = _totalDrawableAmount = 0;	
		}
		
		public static ThingHandler GetInstance(){
			if (_th == null){
				_th = new ThingHandler();
			}
			return _th;
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
				for(int i = 0; i < entry.Value.Count; i++){
					Thing someThing = entry.Value[i];
					if (!someThing.disposable){ // update the alive Thing
						someThing.Update();
						if (someThing.draw){
							_batchedMapPerUpdate[someThing.shaderTextureNo].Add(someThing);
							_totalDrawableAmount++;
						}
					}
					else{ // Remove the disposable Thing
						entry.Value.RemoveAt(i);
						i--;
					}
				}
			}
		}
		
		public int GetTotalDrawableAmount(){
			return _totalDrawableAmount;	
		}
	}
	
	public struct BatchInfo{
		public int shaderNumber;	
		public int textureNumber;
		public int count;
		public int index;
	}
}

