using System;
using RailTypePSMEngine.Asset;
using RailTypePSMEngine.Graphics;

namespace RailTypePSMEngine.Entity{
    public abstract class GraphicThing : INumberedThing, IDrawable{
        public static GraphicsHandler GraphicsHandler;
        private ModelBufferLocation _modelBufferLocation;

        protected Tuple<int, int> _shaderTextureNo;

        public abstract int GlobalNumber { get; }
        public abstract bool Draw { get; }

        public Tuple<int, int> ShaderTextureNo { get { return _shaderTextureNo; } }

        private Model _model;

        public Model Model { get { return _model; } }

        public bool HasModel { get { return _model != null; } }

        public ModelBufferLocation ModelBufferLocation { get { return _modelBufferLocation; } }

        protected GraphicThing(){
            if (_shaderTextureNo == null) _shaderTextureNo = new Tuple<int, int>(0, 0);
        }

        protected GraphicThing(Model model){
            _model = model;
        }

        protected void Register(){
            GraphicsHandler.Register(this);
        }

        protected void Unregister(){
            GraphicsHandler.Unregister(this);
        }

        public void UpdateModelBufferLocation(ModelBufferLocation modelBufferLocation){
            _modelBufferLocation = modelBufferLocation;
        }
    }
}