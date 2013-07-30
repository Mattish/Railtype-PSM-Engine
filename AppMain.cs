using System;
using System.Collections.Generic;
using System.Diagnostics;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Input;
using Sce.PlayStation.Core.Imaging;

namespace Railtype_PSM_Engine{
	public class AppMain{
		private GraphicsContext graphics;
		ShaderProgram shaderProgram;
        float[] vertices=new float[12];
        const int indexSize = 6;
        Matrix4 cameraToWorld, modelToWorld;
		Stopwatch sw;
 
        public static void Main (string[] args){
			AppMain appmain = new AppMain();
            appmain. Initialize ();
 
            while (true) {
                SystemEvents.CheckEvents();
                appmain.Update();
                appmain.Render();
            }
        }
 
        public void Initialize (){
            graphics = new GraphicsContext();
			Globals.Setup(graphics);
            shaderProgram = new ShaderProgram("/Application/shaders/Basic.cgx");
			shaderProgram.SetAttributeBinding(0, "a_Position");
			shaderProgram.SetAttributeBinding(1, "idontgetit");
            shaderProgram.SetUniformBinding(0, "WorldViewProj");
			shaderProgram.SetUniformBinding(1, "inputColor");
			shaderProgram.SetUniformBinding(2, "TestMovement");
 			cameraToWorld = Matrix4.Identity;
			sw = new Stopwatch();
        }
 
        public void Update (){
			Console.WriteLine(sw.ElapsedMilliseconds);
			sw.Reset();
			sw.Start();
			
		}
		
		int counter;
        public void Render (){
			graphics.SetClearColor (1.0f, 1.0f, 1.0f, 1.0f);
            graphics.Clear();
            graphics.SetShaderProgram(shaderProgram);
			
			modelToWorld = Matrix4.Identity;		
			modelToWorld *= Matrix4.RotationY((float)(Math.PI/60.0f)*counter);
			modelToWorld.RowW = modelToWorld.RowW.Add(new Vector4(0.0f,0.0f,-3.0f,0.0f));				
			counter++;
			Matrix4 VP = buildProjectionMatrix(ref cameraToWorld);
			
            shaderProgram.SetUniformValue(0, ref VP);
			float[] inputColor = new float[]{1.0f,0.0f,1.0f,1.0f};
			shaderProgram.SetUniformValue(1, inputColor);			
			shaderProgram.SetUniformValue(2, ref modelToWorld);
			
            graphics.DrawArrays(Globals._prims);
            graphics.SwapBuffers();
        }
		
		public Matrix4 buildProjectionMatrix(ref Matrix4 cameraToWorld){
			Matrix4 worldToCamera = Matrix4.Identity;
			cameraToWorld.Inverse(out worldToCamera);
			Matrix4 cameraToProjection = Matrix4.Perspective(FMath.Radians(45.0f), graphics.Screen.AspectRatio, 1.0f, 1000.0f);
			Matrix4 tmp = worldToCamera * cameraToProjection;
			return tmp;
		}
		
    }
}
