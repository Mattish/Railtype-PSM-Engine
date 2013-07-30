using System;
using Sce.PlayStation.Core.Graphics;

namespace Railtype_PSM_Engine{
	public static class Globals{
		public static Primitive[] _prims;
		public static Thing[] things;
		public static int vertexSize;
		public static VertexBuffer modelVertexBuffer;
		static private GraphicsContext _graphic;
		static int counter;
		static public void Setup(GraphicsContext graphic){
			_graphic = graphic;
			float[] vertices = new float[120];
            vertices[0]=0.0f;   // x0
            vertices[1]=0.0f;   // y0
            vertices[2]=0.0f;   // z0
 
            vertices[3]=0.0f;   // x1
            vertices[4]=1.0f;   // y1
            vertices[5]=0.0f;   // z1
 
            vertices[6]=1.0f;   // x2
            vertices[7]=0.0f;   // y2
            vertices[8]=0.0f;   // z2
			
			float[] colors = new float[120];
            colors[0]=1.0f;   // x0
            colors[1]=0.0f;   // y0
            colors[2]=0.0f;   // z0
 
            colors[3]=0.0f;   // x1
            colors[4]=1.0f;   // y1
            colors[5]=0.0f;   // z1
 
            colors[6]=0.0f;   // x2
            colors[7]=0.0f;   // y2
            colors[8]=1.0f;   // z2
 
            modelVertexBuffer = new VertexBuffer(120/3, VertexFormat.Float3,VertexFormat.Float3);
            modelVertexBuffer.SetVertices(0, vertices);
			modelVertexBuffer.SetVertices(1, colors);
            _graphic.SetVertexBuffer(0, modelVertexBuffer);
			_prims = new Primitive[1000];	
			counter = 0;
			
			things = new Thing[10];
			things[0] = new Thing();
		}
		static public int getPrimitiveSlot(){
			return counter++;
		}
		
	}
}

