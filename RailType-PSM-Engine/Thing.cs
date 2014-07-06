using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using RailTypePSMEngine.Asset;
using RailTypePSMEngine.Graphics;
using RailTypePSMEngine.Entity;

namespace RailTypePSMEngine{
	public class Thing : IEquatable<Thing>{
		private Model model;
		
		public static ThingHandler parentThingHandler;
		public static GraphicsHandler parentGraphicsHandler;
		
		public float[] scalexyzrot;
		public Tuple<int,int> shaderTextureNo;
		public Matrix4 modelToWorld;
		public bool draw, disposable, dirtyMatrix;
		public Primitive prim;
		public int globalNumber;
		public static int thingNumberCounter = 0;

		public Thing(){
			scalexyzrot = new float[7]{1.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f};
			modelToWorld = Matrix4.Identity;
			draw = true;
			dirtyMatrix = true;
			if (shaderTextureNo == null)
				shaderTextureNo = new Tuple<int, int>(0,0);
			
			globalNumber = Thing.thingNumberCounter++;
			parentThingHandler.AddThing(this);	
		}
		
		~Thing(){
			parentGraphicsHandler.ReleasePrimitive(prim);
		}
		
		public Thing(Model model_) : this(){
			model = model_;
			prim = parentGraphicsHandler.RequestPrimitive(model,globalNumber);
		}
		
		public virtual void Update(){
			UpdateModelToWorld();
		}
		
		public void Destroy(){
			disposable = true;
		}
		
		public bool Equals(Thing inputThing){
			return inputThing.globalNumber == this.globalNumber;
		}
		
		Vector4 tmp;
		Matrix4 tmpMatrix;
		protected void UpdateModelToWorld(){
			if (dirtyMatrix){
				Matrix4.RotationXyz(scalexyzrot[4], scalexyzrot[5], scalexyzrot[6], out modelToWorld);	
				Matrix4.Scale(scalexyzrot[0], scalexyzrot[0], scalexyzrot[0], out tmpMatrix);
				modelToWorld *= tmpMatrix;
				tmp.X = scalexyzrot[1];
				tmp.Y = scalexyzrot[2];
				tmp.Z = -scalexyzrot[3];
				tmp.W = 1.0f;
				modelToWorld.RowW = tmp;
				modelToWorld = modelToWorld.Transpose();
				dirtyMatrix = false;
			}
		}

	}
}

