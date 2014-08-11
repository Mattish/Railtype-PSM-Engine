using System;
using System.Collections.Generic;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core;
using RailTypePSMEngine.Asset;

namespace RailTypePSMEngine.Graphics{
	public class GraphicsHandler{
		private GraphicsContext _gc;
		private List<Thing> _thingsActive;
		private List<ModelBufferLocation> _mblDisposed;
		
		private VertexBuffer _vb;
		private ushort _indicesIndex, _verticesIndex;	
		private int fragmentationIndicesCount, totalIndicesCount;
		private int fragmentationVerticesCount, totalVerticesCount;
		//private int[] activeTextureBuffers;
		
		public Matrix4 modelToWorld, cameraToWorld;
		
		public GraphicsHandler(GraphicsContext gc){
			_gc = gc;
			_vb = new VertexBuffer(8192, 8192, new VertexFormat[]{VertexFormat.Float3, VertexFormat.UShort2N, VertexFormat.Float});
			_indicesIndex = _verticesIndex = 0;
			_mblDisposed = new List<ModelBufferLocation>();
			_thingsActive = new List<Thing>();
//			cameraToProjection = Matrix4.Perspective(FMath.Radians(45.0f), gc.Screen.AspectRatio, 1f, 1000.0f);
//			cameraToProjection = Matrix4.Identity;
//			cameraToProjection *= Matrix4.Ortho(-_gc.Screen.AspectRatio, _gc.Screen.AspectRatio,
//			                                   -1.0f, 1.0f, 0.1f, 100.0f);
			
			modelToWorld = Matrix4.Identity; 
			cameraToWorld = Matrix4.Identity;
		}

		
		public void Register(Thing inputThing){
			Model model = inputThing.GetModel();
			ushort[] indicestmp = new ushort[model.indices.Length];
			
			Primitive outputPrimitive = new Primitive(DrawMode.Triangles,_indicesIndex,(ushort)model.indices.Length,0);
			
			//SetIndicies
			Array.Copy(model.indices,0,indicestmp,0,model.indices.Length);
			for(int i = 0; i < indicestmp.Length;i++){
				indicestmp[i]+= _verticesIndex;		
			}
			
			//Gotta do these regardless
			float[] theseNumbers = new float[model.vertices.Length/3];
			float[] arrayFill = new float[1]{(float)inputThing.globalNumber};
			ArrayFillComplex<float>(ref theseNumbers,0,ref arrayFill,theseNumbers.Length);
			
			_vb.SetIndices(indicestmp,outputPrimitive.First,0,indicestmp.Length);			
			_vb.SetVertices(0,model.vertices,_verticesIndex,0,model.vertices.Length/3);			
			_vb.SetVertices(1,model.uv,_verticesIndex,0,model.uv.Length);			
			_vb.SetVertices(2,theseNumbers,_verticesIndex,0,theseNumbers.Length);
		
			inputThing.modelBufferLocation.prim = outputPrimitive;
			inputThing.modelBufferLocation.verticesCount = model.vertices.Length/3;
			inputThing.modelBufferLocation.verticesIndex = _verticesIndex;
			
			//only if on the end of buffer - section						
			_indicesIndex += (ushort)model.indices.Length;
			totalIndicesCount += model.indices.Length;
			_verticesIndex += (ushort)(model.vertices.Length/3);
			totalVerticesCount += model.vertices.Length/3;
			
			Console.WriteLine("Assigning new Thing:{0} - vertCount:{1:D}, vertIndex:{2:D}, IndIndex:{3:D}, IndCount:{4:D}",
			                  inputThing.globalNumber,
			                  inputThing.modelBufferLocation.verticesCount,
			                  inputThing.modelBufferLocation.verticesIndex,
			                  inputThing.modelBufferLocation.prim.First,
			                  inputThing.modelBufferLocation.prim.Count
			                  		);
		}
		
		public void PrintInfo(){
			Console.WriteLine("TotalAdded-Vertices:{0:D},Indicies:{1:D}",totalVerticesCount,totalIndicesCount);                              
			Console.WriteLine("Fragmented-Vertices:{0:D},Indicies:{1:D}",fragmentationVerticesCount,fragmentationIndicesCount); 
       	}
		
		public void Release(Thing inputThing){
			_thingsActive.Remove(inputThing);
			_mblDisposed.Add(inputThing.modelBufferLocation);
			fragmentationIndicesCount += inputThing.modelBufferLocation.prim.Count;
			fragmentationVerticesCount += inputThing.modelBufferLocation.verticesCount;
			totalIndicesCount -= inputThing.modelBufferLocation.prim.Count;
			totalVerticesCount -= inputThing.modelBufferLocation.verticesCount;
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

