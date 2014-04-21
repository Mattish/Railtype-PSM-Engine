using System;
using System.Collections.Generic;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core;
using System.Threading;
using Railtype_PSM_Engine.Entities;

namespace Railtype_PSM_Engine{
	public static class Globals{
		
		public static int AmountPerPush = 30;
		public static int frameCount;
		public static bool DEBUG_MODE = true;
		public static ShaderProgram gpuHard;
        public static Matrix4 cameraToWorld;
		public static ThingManager thingManager;
		public static TextureManager textureManager;
		public static ModelManager modelManager;
		public static FontManager fontManager;
		public static int vertexSize;
		public static VertexBuffer modelVertexBuffer;
		public static GraphicsContext gc;
		public static Matrix4 ViewProjection;
		public static Random random;
		static public void Setup(GraphicsContext graphic){
			frameCount = 0;
			gc = graphic;
			thingManager = new ThingManager();
			textureManager = new TextureManager();
			fontManager = new FontManager();
			modelManager = new ModelManager();
			random = new Random();
 			cameraToWorld = Matrix4.Identity;
			cameraToWorld *= Matrix4.RotationY((float)Math.PI);
			Vector4 tmp = Vector4.One;
			tmp.X = 0.0f;
			tmp.Y = 0.0f;
			tmp.Z = 0.0f;
			cameraToWorld.RowW = tmp;
			
		}

	}
}