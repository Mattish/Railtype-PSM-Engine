using System;
using System.Collections.Generic;

namespace Railtype_PSM_Engine{
	public class ModelManager{
		
		private Dictionary<string,Model> modelByName;
		private List<Model> models;
		public int amountOfVertex;
		public bool changed;
		public float[] vertex;
		
		public ModelManager(){
			modelByName = new Dictionary<string, Model>();
			models = new List<Model>();
			vertex = new float[65000];
			amountOfVertex = 0;
			changed = false;
		}
		
		public Model AddModel(Model inputModel, string name){
			Model tmp;
			if (modelByName.TryGetValue(name,out tmp))
				return tmp;	
			else{
				modelByName.Add(name,inputModel);
				changed = true;	
			}
			amountOfVertex += inputModel.vertex.Length/3;
			return inputModel;
		}
		
		public Model GetModelByName(string name){
			Model tmp;
			if (modelByName.TryGetValue(name,out tmp))
				return tmp;	
			else
				return null;
		}
		
		public Model GetModelByIndex(int i){
			if (i < models.Count)
				return models[i];
			else
				return null;
		}
		
		public List<Model> GetModelList(){
			return models;
		}
		
		public void Update(){
			if (changed){ // if new model was added
				if (amountOfVertex*3 > vertex.Length){ // double checking that model size > 0 was added
					Array.Resize<float>(ref vertex,amountOfVertex*3);
				}
			}
		}
	}
}

