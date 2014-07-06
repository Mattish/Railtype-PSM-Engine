using System;
using System.Collections.Generic;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core;

namespace RailTypePSMEngineNew.Graphics{
	public class GraphicsHandler{
		private GraphicsContext _gc;
		//private Dictionary<int,ModelLocationData> _indexToModelLocationData;
		
		private VertexBuffer _vb;
		private ushort _indiciesIndex, _verticiesIndex;	
		private int fragmentationIndiciesCount, totalIndiciesCount;
		private int fragmentationVerticiesCount, totalVerticiesCount;
		//private int[] activeTextureBuffers;
		
		public Matrix4 modelToWorld, cameraToWorld;
		
		public GraphicsHandler(GraphicsContext gc){
			_gc = gc;
			_vb = new VertexBuffer(8192, 8192, new VertexFormat[]{VertexFormat.Float3, VertexFormat.UShort2N, VertexFormat.Float});
			_indiciesIndex = _verticiesIndex = 0;
			
//			cameraToProjection = Matrix4.Perspective(FMath.Radians(45.0f), gc.Screen.AspectRatio, 1f, 1000.0f);
//			cameraToProjection = Matrix4.Identity;
//			cameraToProjection *= Matrix4.Ortho(-_gc.Screen.AspectRatio, _gc.Screen.AspectRatio,
//			                                   -1.0f, 1.0f, 0.1f, 100.0f);
			
			modelToWorld = Matrix4.Identity; 
			cameraToWorld = Matrix4.Identity;
		}

		
		public Primitive RequestPrimitive(Model model, int thingNumber){
			Primitive outputPrimitive = new Primitive(DrawMode.Triangles,_indiciesIndex,(ushort)model.indices.Length,0);
			//SetIndicies
			ushort[] indiciestmp = new ushort[model.indices.Length];
			Array.Copy(model.indices,0,indiciestmp,0,model.indices.Length);
			for(int i = 0; i < indiciestmp.Length;i++){
				indiciestmp[i]+= _verticiesIndex;		
			}
			
			_vb.SetIndices(indiciestmp,outputPrimitive.First,0,indiciestmp.Length);
			
			_indiciesIndex += (ushort)model.indices.Length;
			
			//SetVerticies
			//Verticies
			_vb.SetVertices(0,model.verticies,_verticiesIndex,0,model.verticies.Length/3);
			//UVs
			_vb.SetVertices(1,model.uv,_verticiesIndex,0,model.uv.Length);
			//UniqueThingNumber
			float[] theseNumbers = new float[model.verticies.Length/3];
			float[] arrayFill = new float[1]{(float)thingNumber};
			ArrayFillComplex<float>(ref theseNumbers,0,ref arrayFill,theseNumbers.Length);
			//Fill Array with numbers
			_vb.SetVertices(2,theseNumbers,_verticiesIndex,0,theseNumbers.Length);
			
			_verticiesIndex += (ushort)(model.verticies.Length/3);
			return outputPrimitive;
		}
		
		public void ReleasePrimitive(Primitive inputLocation){
			
		}
		
		public void SetCameraToWorld(Matrix4 cameraToWorld_){
			cameraToWorld = cameraToWorld_;
		}
		
		public void Draw(Primitive[] primsToDraw){
			_gc.SetVertexBuffer(0,_vb);
			_gc.SetShaderProgram(AssetHandler.GetInstance().GetSPSimple());
			Matrix4 VP = buildProjectionMatrix(cameraToWorld);
			AssetHandler.GetInstance().GetSPSimple().SetUniformValue(0, ref VP);
			_gc.DrawArrays(primsToDraw);	
		}
		
		public void Draw(Primitive[] primsToDraw, int start, int count){
			_gc.SetVertexBuffer(0,_vb);
			_gc.SetShaderProgram(AssetHandler.GetInstance().GetSPSimple());
			Matrix4 VP = buildProjectionMatrix(cameraToWorld);
			AssetHandler.GetInstance().GetSPSimple().SetUniformValue(0, ref VP);
			_gc.DrawArrays(primsToDraw,start,count);	
		}
		
		public void Draw(Primitive[] primsToDraw, Matrix4[] batchMatricies, int[] batchThingNumbers, int start, int count,
                 			int shaderNo, int textureNo){
			_gc.SetVertexBuffer(0, _vb);
			_gc.SetShaderProgram(AssetHandler.GetInstance().GetSPSimple());
			Matrix4 VP = buildProjectionMatrix(cameraToWorld);
			//WorldToProjection
			AssetHandler.GetInstance().GetSPSimple().SetUniformValue(0, ref VP);
			//ModelMatricies
			AssetHandler.GetInstance().GetSPSimple().SetUniformValue(1, batchMatricies, 0, start, count);
			//ThingNumbers
			AssetHandler.GetInstance().GetSPSimple().SetUniformValue(2, batchThingNumbers, 0, start, count);
			_gc.DrawArrays(primsToDraw, start, count);	
		}
		
		private Matrix4 buildProjectionMatrix(Matrix4 cameraToWorld, float n = 0.1f, float f = 1000.0f){
			Matrix4 worldToCamera;
			cameraToWorld.Inverse(out worldToCamera);
			
			Matrix4 cameraToProjection = Matrix4.Identity;
//			cameraToProjection *= Matrix4.Ortho(-_gc.Screen.AspectRatio, _gc.Screen.AspectRatio, -1.0f, 1.0f, 0.1f, 100.0f);
			cameraToProjection *= Matrix4.Perspective(FMath.Radians(45.0f), _gc.Screen.AspectRatio, 0.1f, 1000.0f);
			
//		    cameraToProjection = Matrix4.Identity;
//		    cameraToProjection *= Matrix4.Frustum(-n, n, -n, n, n, f);
			
			Matrix4 tmp = cameraToProjection * worldToCamera;
			return tmp;
		}
		
		private void ArrayFillComplex<T>(ref T[] arrayToFill, int destinationIndex, ref T[] fillValue, int count){
			Array.Copy(fillValue, 0, arrayToFill, destinationIndex, fillValue.Length);
			int arrayToFillHalfLength = count / 2;
		 
			for(int i = fillValue.Length; i < count; i *= 2){
				int copyLength = i;
				if(i > arrayToFillHalfLength){
					copyLength = count - i;
				}
				Array.Copy(arrayToFill, destinationIndex, arrayToFill, destinationIndex + i, copyLength);
			}
		}
		
	}
}

