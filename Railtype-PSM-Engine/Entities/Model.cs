using System;

namespace Railtype_PSM_Engine{
	public class Model{
		public float[] vertex, uv;
		public ushort[] indicies;
		public int size;
		public int vertexIndex;
		
		public Model(){
			size = vertexIndex = 0;
		}
		
		public void Set(float[] _vertex, float[] _uv, ushort[] _indicies){
			vertex = _vertex;
			uv = _uv;
			indicies = _indicies;
			size = _indicies.Length;
		}
	}
}

