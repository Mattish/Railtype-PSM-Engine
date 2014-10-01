using System;
using RailTypePSMEngine.Entity;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Input;
using RailTypePSMEngine.Asset;

namespace RailTypePSMEngine {
    public class AppMain {
        private static RailTypeEngine _rte;

        private const int AmountOfThingsInCircle = 8;
        private static int _someThingCounterDeleter = 4;

        public static void Main(string[] args) {
            Initialize();
            while (true) {
                SystemEvents.CheckEvents();
                Update();
                Render();
            }
        }
        private static WaveFrontObject wfo = new WaveFrontObject("/Application/objects/cube.obj");

        public static void Initialize() {
            _rte = new RailTypeEngine(new GraphicsContext());
            var wfo2 = new WaveFrontObject();
            wfo2.MakeCircle(20);
        }

        public static void Update() {
            // Query gamepad for current state
            var gamePadData = GamePad.GetData(0);
            _rte.Update();
            const float doublePi = (float)Math.PI * 1.7f;
            const float floatit = doublePi / AmountOfThingsInCircle;
            if (_rte.GetFrameNumber() % 10 == 0 && _rte.GetFrameNumber() < 80) {
                //_things[rte.GetFrameNumber()/10].Dispose();
                int j = _rte.GetFrameNumber() / 10;
                var tmpThing = new ThingRotating(wfo.models[0]);
                tmpThing.Scalexyzrot[3] = 3.0f - (0.01f * _rte.GetFrameNumber());
                tmpThing.Scalexyzrot[2] = (float)Math.Sin(floatit * j) * (0.25f + (j * 0.01f));
                tmpThing.Scalexyzrot[1] = (float)Math.Cos(floatit * j) * (0.25f + (j * 0.01f));
                tmpThing.Scalexyzrot[0] = 0.1f + (0.005f * j);
            }

            if (gamePadData.ButtonsDown == GamePadButtons.Down) {
                _rte[_someThingCounterDeleter++].Dispose();
            }

            //_gh.cameraToWorld.ColumnW += new Vector4(0.0f,0.0f,0.01f,0.0f);
        }

        public static void Render() {
            _rte.Render();
        }
    }
}
