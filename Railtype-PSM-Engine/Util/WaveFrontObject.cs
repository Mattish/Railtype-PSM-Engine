using System;
using System.Collections.Generic;
using System.IO;
using Sce.PlayStation.Core;

namespace Railtype_PSM_Engine.Util{
	public class WaveFrontObject{
		
		List<Vector3> vertex, uv, normals;
		List<string> lines;
		public List<Model> models;
		public WaveFrontObject(string path){
			vertex = new List<Vector3>();
			uv = new List<Vector3>();
			normals = new List<Vector3>();
			vertex.Add(Vector3.One);
			uv.Add(Vector3.One);
			normals.Add(Vector3.One);
			models = new List<Model>();
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
						for(int j = 1; j < tmpStrings.Length; j++){ // For each faceVertex
							faceVertex = tmpStrings[j].Split(splitter2);
							if (faceVertex.Length > 0 && faceVertex[0].Length > 0){
								currentModel._vertex.Add(vertex[int.Parse(faceVertex[0])].X);
								currentModel._vertex.Add(vertex[int.Parse(faceVertex[0])].Y);
								currentModel._vertex.Add(vertex[int.Parse(faceVertex[0])].Z);
							}
							if (faceVertex.Length == 2){
								currentModel._uv.Add(uv[int.Parse(faceVertex[1])].X);
								currentModel._uv.Add(uv[int.Parse(faceVertex[1])].Y);
								currentModel._uv.Add(uv[int.Parse(faceVertex[1])].Z);
							}
							if(faceVertex.Length == 3){
								currentModel._normal.Add(normals[int.Parse(faceVertex[2])].X);
								currentModel._normal.Add(normals[int.Parse(faceVertex[2])].Y);
								if (faceVertex[1].Length > 0){
									currentModel._uv.Add(uv[int.Parse(faceVertex[1])].X);
									currentModel._uv.Add(uv[int.Parse(faceVertex[1])].Y);
									currentModel._uv.Add(uv[int.Parse(faceVertex[1])].Z);
								}
							}
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
								normals.Add(tmpVector3);
								break;
							case ' ': // Vertex
								tmpVector3.X = float.Parse(tmpStrings[1]);
								tmpVector3.Y = float.Parse(tmpStrings[2]);
								tmpVector3.Z = float.Parse(tmpStrings[3]);
								vertex.Add(tmpVector3);
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
			public List<float> _vertex,_normal,_uv;
			public float[] vertex, normal, uv;
			
			public Model(){
				_vertex = new List<float>(3);
				_normal = new List<float>(3);
				_uv = new List<float>(2);
				name = "ididntchangethename";
			}
			
			public void finalize(){
				vertex = _vertex.ToArray();
				normal = _normal.ToArray();
				uv = _uv.ToArray();
			}
		}

	}
}

