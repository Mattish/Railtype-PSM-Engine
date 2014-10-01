using System;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core;
using System.Diagnostics;
using RailTypePSMEngine.Entity;
using RailTypePSMEngine.Graphics;
using RailTypePSMEngine.Asset;

namespace RailTypePSMEngine{
    public class RailTypeEngine{
        private GraphicsContext _graphicsContext;
        public static Matrix4 cameraToWorld;
        private int _frameCount;
        private int _totalFrameCount;
        private Stopwatch _stopwatch;
        private ThingHandler _thingHandler;
        private GraphicsHandler _graphicsHandler;

        public RailTypeEngine(GraphicsContext graphicsContext){
            _graphicsContext = graphicsContext;
            _graphicsHandler = new GraphicsHandler(graphicsContext);
            _thingHandler = new ThingHandler(_graphicsHandler);
            AssetHandler.Init();
            _frameCount = 0;
            _stopwatch = new Stopwatch();
            _totalFrameCount = 0;
            cameraToWorld = Matrix4.Identity;
            _stopwatch.Start();
        }

        public IThing this[int i]{
            get{
                IThing someThing;
                _thingHandler.thingMap.TryGetValue(i, out someThing);
                return someThing;
            }
        }

        public void Render(){
            _graphicsContext.SetViewport(0, 0, _graphicsContext.Screen.Width, _graphicsContext.Screen.Height);
            _graphicsContext.SetClearColor(1.0f, 1.0f, 1.0f, 1.0f);
            _graphicsContext.Clear();
            _graphicsContext.Enable(EnableMode.Blend);
            //_graphicsContext.Enable(EnableMode.CullFace);
            _graphicsContext.Enable(EnableMode.DepthTest);
            _graphicsContext.SetBlendFunc(BlendFuncMode.Add, BlendFuncFactor.SrcAlpha, BlendFuncFactor.OneMinusSrcAlpha);

            BatchInfo[] batchInfo;
            Primitive[] batchPrims;
            Matrix4[] batchMatricies;
            int[] batchThingNumbers;
            _thingHandler.TryGetBatchesOfThings(out batchInfo, out batchPrims, out batchMatricies, out batchThingNumbers);
            foreach (BatchInfo bi in batchInfo){
                _graphicsHandler.Draw(batchPrims, batchMatricies, batchThingNumbers, bi.Index, bi.Count,
                    bi.ShaderNumber, bi.TextureNumber);
            }
            _graphicsContext.SwapBuffers();
            _frameCount++;
            if (_stopwatch.ElapsedMilliseconds > 1000){
                Console.WriteLine("fps:{0:D}", _frameCount);
                _thingHandler.PrintInfo();
                _graphicsHandler.PrintInfo();
                _frameCount = 0;
                _stopwatch.Reset();
                _stopwatch.Start();
            }
            _totalFrameCount++;
        }

        public void Update(){
            _thingHandler.Update();
        }

        public int GetFrameNumber(){
            return _totalFrameCount;
        }
    }
}