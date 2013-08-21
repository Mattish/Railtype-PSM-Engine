using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using Sce.PlayStation.Core;

namespace Railtype_PSM_Engine.Util{
	public class WaveFrontObject{
		public struct Vertex : IEquatable<Vertex>{
			public Vector3 pos, uv, normal;
		
		    public bool Equals(Vertex v2){
		        return pos.Equals(v2.pos) &&
				    uv.Equals(v2.uv) &&
				    normal.Equals(v2.normal);
		    }
		}
		
		List<Vector3> pos, uv, normal;
		List<string> lines;
		public List<Model> models;
		List<Vertex> vertexList;
		public WaveFrontObject(string path){
			pos = new List<Vector3>();
			uv = new List<Vector3>();
			normal = new List<Vector3>();
			pos.Add(Vector3.One);
			uv.Add(Vector3.One);
			normal.Add(Vector3.One);
			models = new List<Model>();
			vertexList = new List<Vertex>();
			lines = new List<string>(File.ReadAllLines(path));
			Model currentModel = new Model();
			char[] splitter = new char[]{' '}, splitter2 = new char[]{'/'};
			Vector3 tmpVector3 = new Vector3(0.0f,0.0f,0.0f);
			string[] tmpStrings, faceVertex;
			for(int i = 0; i < lines.Count; i++){
				if (lines[i].Length < 2)
					continue;
				lines[i] = lines[i].Replace("  ", " ");
				tmpStrings = lines[i].Split(splitter);
				switch(lines[i][0]){
					case 'g':	// Group
						currentModel = new Model();
						models.Add(currentModel);
						currentModel.name = tmpStrings[1];
						break;
					case 'f': // Face
						Vertex tmpVertex = new Vertex();
						for(int j = 1; j < 4; j++){ // For each faceVertex
							faceVertex = tmpStrings[j].Split(splitter2);
							if (faceVertex.Length > 0 && faceVertex[0].Length > 0){
								tmpVertex.pos.X = pos[int.Parse(faceVertex[0])].X;
								tmpVertex.pos.Y = pos[int.Parse(faceVertex[0])].Y;
								tmpVertex.pos.Z = pos[int.Parse(faceVertex[0])].Z;
							}
							if (faceVertex.Length == 2){
								tmpVertex.uv.X = uv[int.Parse(faceVertex[1])].X;
								tmpVertex.uv.Y = uv[int.Parse(faceVertex[1])].Y;
							}
							if(faceVertex.Length == 3){
								tmpVertex.normal.X = normal[int.Parse(faceVertex[2])].X;
								tmpVertex.normal.Y = normal[int.Parse(faceVertex[2])].Y;
								tmpVertex.normal.Z = normal[int.Parse(faceVertex[2])].Z;
								if (faceVertex[1].Length > 0){
									tmpVertex.uv.X = uv[int.Parse(faceVertex[1])].X;
									tmpVertex.uv.Y = uv[int.Parse(faceVertex[1])].Y;
								}
							}
							//Does this vertex exist already?
							int vertexLocation = -1;
							if ((vertexLocation = vertexList.IndexOf(tmpVertex)) == -1){
								vertexLocation = vertexList.Count;
								vertexList.Add(tmpVertex);
								currentModel._vertex.Add(vertexList[vertexLocation]);
							}
							currentModel._indices.Add((ushort)vertexLocation);
							
						}
						break;
					case 'v': // Vertex
						switch(lines[i][1]){
							case 't': // UV
								tmpVector3.X = float.Parse(tmpStrings[1]);
								tmpVector3.Y = float.Parse(tmpStrings[2]);
								if (tmpStrings.Length > 3)
									tmpVector3.Z = float.Parse(tmpStrings[3]);
								uv.Add(tmpVector3);
								break;
							case 'n': // Normal
								tmpVector3.X = float.Parse(tmpStrings[1]);
								tmpVector3.Y = float.Parse(tmpStrings[2]);
								tmpVector3.Z = float.Parse(tmpStrings[3]);
								normal.Add(tmpVector3);
								break;
							case ' ': // Vertex
								tmpVector3.X = float.Parse(tmpStrings[1]);
								tmpVector3.Y = float.Parse(tmpStrings[2]);
								tmpVector3.Z = float.Parse(tmpStrings[3]);
								pos.Add(tmpVector3);
								break;
						}
						break;
				}
			}
			foreach(Model m in models)
				m.finalize();
		}
		
		public class Model{
			public string name;
			public List<WaveFrontObject.Vertex> _vertex;
			public float[] pos, normal, uv;
			public ushort[] indices;
			public List<ushort> _indices;
			
			public Model(){
				_vertex = new List<Vertex>();
				_indices = new List<ushort>();
				name = "ididntchangethename";
			}
			
			public void finalize(){
				pos = new float[_vertex.Count*3];
				normal = new float[_vertex.Count*3];
				uv = new float[_vertex.Count*2];
				for(int i = 0; i < _vertex.Count; i++){
					pos[(i*3)] = _vertex[i].pos.X;
					pos[(i*3)+1] = _vertex[i].pos.Y;
					pos[(i*3)+2] = _vertex[i].pos.Z;
					normal[(i*3)] = _vertex[i].normal.X;
					normal[(i*3)+1] = _vertex[i].normal.Y;
					normal[(i*3)+2] = _vertex[i].normal.Z;
					uv[(i*2)] = _vertex[i].uv.X;
					uv[(i*2)+1] = _vertex[i].uv.Y;
				}
				indices = _indices.ToArray();
				_vertex.Clear();
				_indices.Clear();
			}
		}

	}
}

