using System;
using RailTypePSMEngine.Asset;

namespace RailTypePSMEngine{
	public class ThingRotating : Thing{
		
		private float randomFloat;
		private static Random _r;
		public ThingRotating(Model model_) : base(model_){
			if (_r == null)
				_r = new Random();
			randomFloat = (float)_r.NextDouble();
			randomFloat /= 10;
		}
		
		public override void Update(){
			scalexyzrot[4] += randomFloat;
			scalexyzrot[5] += randomFloat;
			dirtyMatrix = true;
			base.Update();
		}
	}
}

