using System;
using RailTypePSMEngine.Asset;

namespace RailTypePSMEngine.Entity{
    public class ThingRotating : Thing{
        private readonly float _randomFloat;
        private static Random _r;

        public ThingRotating(Model model) : base(model){
            if (_r == null) _r = new Random();
            _randomFloat = (float) _r.NextDouble();
            _randomFloat /= 10;
        }

        public override void Update(){
            Scalexyzrot[4] += _randomFloat;
            Scalexyzrot[5] += _randomFloat;
            ForceDirty();
            base.Update();
        }
    }
}