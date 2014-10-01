using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using RailTypePSMEngine.Graphics;

namespace RailTypePSMEngine.Entity{
	public class ThingHandler{
	    private int _totalDrawableAmount;
		private GraphicsHandler _gh;
		
		// shaderNo, textureNo
		private Dictionary<Tuple<int,int>,List<IThing>> _batchedMap;
		private Dictionary<Tuple<int,int>,List<IThing>> _batchedMapPerUpdate;
		private Queue<IThing> _thingsToAdd;
		public Dictionary<int,IThing> thingMap;
		
		public ThingHandler(GraphicsHandler gh){
			_gh = gh;
			Thing.ParentThingHandler = this;
			_batchedMap = new Dictionary<Tuple<int, int>, List<IThing>>();
			_batchedMapPerUpdate = new Dictionary<Tuple<int, int>, List<IThing>>();
			thingMap = new Dictionary<int, IThing>();
			_thingsToAdd = new Queue<IThing>();
			TotalAmount = _totalDrawableAmount = 0;	
		}

	    public int TotalAmount { get; set; }

	    public void TryGetBatchesOfThings(out BatchInfo[] bi, out Primitive[] prims, out Matrix4[] matricies, out int[] thingNumbers){
			int amountOfBatches = 1;
			foreach(KeyValuePair<Tuple<int,int>,List<IThing>> entry in _batchedMapPerUpdate){
				amountOfBatches += (int)Math.Ceiling((double)(entry.Value.Count/16));
			}
			
			bi = new BatchInfo[amountOfBatches];
			prims = new Primitive[_totalDrawableAmount];
			matricies = new Matrix4[_totalDrawableAmount];
			thingNumbers = new int[_totalDrawableAmount];
			
			int thingIndex = 0;
			int batchInfoIndex = 0;
			foreach(KeyValuePair<Tuple<int,int>,List<IThing>> entry in _batchedMapPerUpdate){
				int thingCounter = 0;
				foreach(Thing thg in entry.Value){
					prims[thingIndex+thingCounter] = thg.ModelBufferLocation.prim;
					matricies[thingIndex+thingCounter] = thg.ModelToWorld;
					thingNumbers[thingIndex+thingCounter] = thg.GlobalNumber;
					thingCounter++;
				}
				int AmountOfThings = 0;
				int totalAmountOfThings = 0;
				while(totalAmountOfThings < thingCounter){
					AmountOfThings = (thingCounter-totalAmountOfThings) < 16 ? (thingCounter-totalAmountOfThings) : 16;
					bi[batchInfoIndex] = new BatchInfo(){ShaderNumber = entry.Key.Item1,
														TextureNumber = entry.Key.Item2,
														Count = AmountOfThings,
														Index = thingIndex+totalAmountOfThings};
					totalAmountOfThings += AmountOfThings;
					batchInfoIndex++;
				}
				thingIndex += thingCounter;
			}
		}
		
		public void Register(IThing thg){
			_thingsToAdd.Enqueue(thg);	
		}
		
		public void _AddThing(IThing thg){
			if (!_batchedMap.ContainsKey(thg.ShaderTextureNo)){
				_batchedMap[thg.ShaderTextureNo] = new List<IThing>(1);
				_batchedMap[thg.ShaderTextureNo].Add(thg);
			}
			else{
				_batchedMap[thg.ShaderTextureNo].Add(thg);
			}
			thingMap.Add(thg.GlobalNumber,thg);
			TotalAmount++;
		}
		
		public void PrintInfo(){
			Console.WriteLine("drawables:{0:D}",_totalDrawableAmount);	
		}
		
		public void Update(){
			if (_thingsToAdd.Count > 0)
				_AddThing(_thingsToAdd.Dequeue());
			_batchedMapPerUpdate.Clear();
			_totalDrawableAmount = 0;
			foreach(KeyValuePair<Tuple<int,int>,List<IThing>> entry in _batchedMap){
				_batchedMapPerUpdate.Add(entry.Key,new List<IThing>(1));
				for(int i = 0; i < entry.Value.Count; i++){
					IThing someThing = entry.Value[i];
					if (!someThing.Disposable){ // update the alive Thing
						someThing.Update();
						if (someThing.Draw){
							_batchedMapPerUpdate[someThing.ShaderTextureNo].Add(someThing);
							_totalDrawableAmount++;
						}
					}
					else{ // Remove the disposable Thing
						entry.Value.RemoveAt(i);
						thingMap.Remove(someThing.GlobalNumber);
						i--;
					}
				}
			}
		}
	}
	
	public struct BatchInfo{
		public int ShaderNumber;	
		public int TextureNumber;
		public int Count;
		public int Index;
	}
}

