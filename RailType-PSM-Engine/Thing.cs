using System;
using Sce.PlayStation.Core;
using RailTypePSMEngine.Asset;
using RailTypePSMEngine.Graphics;
using RailTypePSMEngine.Entity;

namespace RailTypePSMEngine {
    public abstract class GraphicThing : INumberedThing, IDrawable {
        public static GraphicsHandler GraphicsHandler;
        private ModelBufferLocation _modelBufferLocation;

        protected Tuple<int, int> _shaderTextureNo;

        abstract public int GlobalNumber { get; }
        abstract public bool Draw { get; }
        public Tuple<int, int> ShaderTextureNo { get { return _shaderTextureNo; } }

        private Model _model;

        public Model Model {
            get {
                return _model;
            }
        }

        public bool HasModel {
            get {
                return _model != null;
            }
        }

        public ModelBufferLocation ModelBufferLocation {
            get {
                return _modelBufferLocation;
            }
        }

        protected GraphicThing() {
            if (_shaderTextureNo == null)
                _shaderTextureNo = new Tuple<int, int>(0, 0);
        }

        protected GraphicThing(Model model) {
            _model = model;
        }

        protected void Register() {
            GraphicsHandler.Register(this);
        }

        protected void Unregister() {
            GraphicsHandler.Unregister(this);
        }

        public void UpdateModelBufferLocation(ModelBufferLocation modelBufferLocation) {
            _modelBufferLocation = modelBufferLocation;
        }
    }

    public interface INumberedThing {
        int GlobalNumber { get; }
    }

    public interface IDrawable {
        bool Draw { get; }
        Tuple<int, int> ShaderTextureNo { get; }
    }

    public interface IThing : IEquatable<IThing>, IDisposable, INumberedThing, IDrawable {
        float[] Scalexyzrot { get; }
        Matrix4 ModelToWorld { get; }
        bool Disposable { get; }
        bool DirtyMatrix { get; }
        void Update();
        void ForceDirty();
    }

    public class Thing : GraphicThing, IThing {
        public static ThingHandler ParentThingHandler;
        private float[] _scalexyzrot;
        private Matrix4 _modelToWorld;
        private bool _draw, _disposable, _dirtyMatrix;
        private int _globalNumber;

        protected static int ThingNumberCounter;

        public float[] Scalexyzrot { get { return _scalexyzrot; } }
        public Matrix4 ModelToWorld { get { return _modelToWorld; } }
        public override bool Draw { get { return _draw; } }
        public bool Disposable { get { return _disposable; } }
        public bool DirtyMatrix { get { return _dirtyMatrix; } }
        public override int GlobalNumber { get { return _globalNumber; } }

        public Thing()
            : base() {
            Setup();
        }

        protected Thing(Model model)
            : base(model) {
            Setup();
        }

        private void Setup() {
            _scalexyzrot = new[] { 1.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f };
            _modelToWorld = Matrix4.Identity;
            _draw = true;
            _dirtyMatrix = true;
            ParentThingHandler.Register(this);
            _globalNumber = ThingNumberCounter++;
            if (_shaderTextureNo == null)
                _shaderTextureNo = new Tuple<int, int>(0, 0);
            Register();
        }

        public virtual void Update() {
            UpdateModelToWorld();
        }

        public void ForceDirty() {
            _dirtyMatrix = true;
        }

        public void Dispose() {
            _disposable = true;
            Unregister();
        }

        public bool Equals(IThing inputThing) {
            return inputThing.GlobalNumber == GlobalNumber;
        }

        Vector4 _tmp;
        Matrix4 _tmpMatrix;

        protected void UpdateModelToWorld() {
            if (DirtyMatrix) {
                Matrix4.RotationXyz(_scalexyzrot[4], _scalexyzrot[5], _scalexyzrot[6], out _modelToWorld);
                Matrix4.Scale(_scalexyzrot[0], _scalexyzrot[0], _scalexyzrot[0], out _tmpMatrix);
                _modelToWorld *= _tmpMatrix;
                _tmp.X = _scalexyzrot[1];
                _tmp.Y = _scalexyzrot[2];
                _tmp.Z = -_scalexyzrot[3];
                _tmp.W = 1.0f;
                _modelToWorld.RowW = _tmp;
                _modelToWorld = _modelToWorld.Transpose();
                _dirtyMatrix = false;
            }
        }
    }
}

