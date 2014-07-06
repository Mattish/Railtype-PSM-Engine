using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;

namespace RailTypePSMEngine.Asset{
	public struct Vertex : IEquatable<Vertex>{
		public Vector3 pos, uv, normal;
	
		public bool Equals(Vertex v2){
			return pos.Equals(v2.pos) &&
			    uv.Equals(v2.uv) &&
			    normal.Equals(v2.normal);
		}
	}
	
	public class WaveFrontObject{
		
		List<Vector3> pos, uv, normal;
		List<string> lines;
		public List<Model> models;
		List<Vertex> vertexList;
		Dictionary<string,ushort> vertexDict;
		
		public WaveFrontObject(){
			pos = new List<Vector3>();
			uv = new List<Vector3>();
			normal = new List<Vector3>();
			pos.Add(Vector3.One);
			uv.Add(Vector3.One);
			normal.Add(Vector3.One);
			models = new List<Model>();
			vertexList = new List<Vertex>();
		}
		
		public WaveFrontObject(string path){
			System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
			pos = new List<Vector3>();
			uv = new List<Vector3>();
			normal = new List<Vector3>();
			pos.Add(Vector3.One);
			uv.Add(Vector3.One);
			normal.Add(Vector3.One);
			models = new List<Model>();
			vertexList = new List<Vertex>();		
			lines = new List<string>(File.ReadAllLines(path));
			vertexDict = new Dictionary<string, ushort>(lines.Count / 2);
			Model currentModel = new Model();
			//models.Add(currentModel);
			char[] splitter = new char[]{' '}, splitter2 = new char[]{'/'};
			Vector3 tmpVector3 = new Vector3(0.0f, 0.0f, 0.0f);
			string[] tmpStrings, faceVertex;
			
			for(int i = 0; i < lines.Count; i++){
				if(lines[i].Length < 2)
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
							sw.Start();							
							faceVertex = tmpStrings[j].Split(splitter2);
							if(faceVertex.Length > 0 && faceVertex[0].Length > 0){
								tmpVertex.pos.X = pos[int.Parse(faceVertex[0])].X;
								tmpVertex.pos.Y = pos[int.Parse(faceVertex[0])].Y;
								tmpVertex.pos.Z = pos[int.Parse(faceVertex[0])].Z;
							}
							if(faceVertex.Length == 2){
								tmpVertex.uv.X = uv[int.Parse(faceVertex[1])].X;
								tmpVertex.uv.Y = uv[int.Parse(faceVertex[1])].Y;
							}
							if(faceVertex.Length == 3){
								tmpVertex.normal.X = normal[int.Parse(faceVertex[2])].X;
								tmpVertex.normal.Y = normal[int.Parse(faceVertex[2])].Y;
								tmpVertex.normal.Z = normal[int.Parse(faceVertex[2])].Z;
								if(faceVertex[1].Length > 0){
									tmpVertex.uv.X = uv[int.Parse(faceVertex[1])].X;
									tmpVertex.uv.Y = uv[int.Parse(faceVertex[1])].Y;
								}
							}
							//Does this vertex exist already?
							ushort vertexLocation;
							if(!vertexDict.TryGetValue(tmpStrings[j], out vertexLocation)){
								vertexLocation = (ushort)vertexList.Count;
								vertexDict.Add(tmpStrings[j], vertexLocation);
								vertexList.Add(tmpVertex);
								currentModel._vertex.Add(vertexList[vertexLocation]);
							}
							sw.Stop();
							currentModel._indices.Add(vertexLocation);
							
						}
						
						break;
					case 'v': // Vertex
						switch(lines[i][1]){
							case 't': // UV
								tmpVector3.X = float.Parse(tmpStrings[1]);
								tmpVector3.Y = float.Parse(tmpStrings[2]);
								if(tmpStrings.Length > 3)
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
		
		public WaveFrontObject(ref float[] verticiess, ref UShort2N[] uvss, float[] normalss, ref ushort[] indiciess){
			pos = new List<Vector3>();
			uv = new List<Vector3>();
			normal = new List<Vector3>();
			pos.Add(Vector3.One);
			uv.Add(Vector3.One);
			normal.Add(Vector3.One);
			models = new List<Model>();
			vertexList = new List<Vertex>();
			Model tmpModel = new Model();
			if(indiciess != null){
				tmpModel.indices = new ushort[indiciess.Length];
				Array.Copy(indiciess, tmpModel.indices, indiciess.Length);
			}
			if(verticiess != null){
				tmpModel.verticies = new float[verticiess.Length];
				Array.Copy(verticiess, tmpModel.verticies, verticiess.Length);
			}
			if(uvss != null){
				tmpModel.uv = new UShort2N[uvss.Length];
				Array.Copy(uvss, tmpModel.uv, uvss.Length);
			}
			if(normalss != null){
				tmpModel.normal = new float[normalss.Length];
				Array.Copy(normalss, tmpModel.normal, normalss.Length);	
			}
			models.Add(tmpModel);
		}
		
		public void PutModelVertexIntoArray(ref float[] input, int position){
			Array.Copy(models[0].verticies, 0, input, position, models[0].verticies.Length); 
		}
		
		public void PutModelUVIntoArray(ref UShort2N[] input, int position){
			Array.Copy(models[0].uv, 0, input, position, models[0].uv.Length);	
		}
		
		public void PutIndiciesIntoArray(ref ushort[] input, int position){
			Array.Copy(models[0].indices, 0, input, position, models[0].indices.Length);	
		}
		
		public void MakeCircle(int totalOutsideVerticies){
			models.Add(new Model());
			models[0].verticies = new float[((totalOutsideVerticies) + 2) * 3];
			models[0].uv = new UShort2N[(models[0].verticies.Length / 3)];
			models[0].indices = new ushort[(totalOutsideVerticies) * 3];
			models[0].verticies[0] = 0.0f;
			models[0].verticies[1] = 0.0f;
			models[0].verticies[2] = 0.0f;
			float tmpFloatOne = (models[0].verticies[0] + 1.0f) * 0.5f;
			float tmpFloatTwo = (models[0].verticies[1] + 1.0f) * 0.5f;
			models[0].uv[0] = new UShort2N(tmpFloatOne,tmpFloatTwo);
			models[0].indices[0] = 0;
			ushort currentIndicies = 2, previousIndicies = 1;
			double rotation = 0.0f;
			double x = Math.Cos(rotation);
			double y = Math.Sin(rotation);
			models[0].verticies[3] = (float)x;
			models[0].verticies[4] = (float)y;
			models[0].verticies[5] = 0.0f;			
			tmpFloatOne = (-models[0].verticies[3] + 1.0f) * 0.5f;
			tmpFloatTwo = (-models[0].verticies[4] + 1.0f) * 0.5f;
			models[0].uv[1] = new UShort2N(tmpFloatOne,tmpFloatTwo);
			models[0].indices[1] = previousIndicies;
			rotation += (Math.PI * 2) / (double)totalOutsideVerticies;
			x = Math.Cos(rotation);
			y = Math.Sin(rotation);
			models[0].verticies[6] = (float)x;//current
			models[0].verticies[7] = (float)y;
			models[0].verticies[8] = 0.0f;
			models[0].uv[2] = new UShort2N((-models[0].verticies[6] + 1.0f) * 0.5f,(-models[0].verticies[7] + 1.0f) * 0.5f);
			models[0].indices[2] = currentIndicies;
			for(int i = 1; i < totalOutsideVerticies; i++){
				previousIndicies++;
				currentIndicies++;
				models[0].indices[(i * 3)] = 0;
				models[0].indices[(i * 3) + 1] = previousIndicies;
				rotation += (Math.PI * 2) / (double)totalOutsideVerticies;
				x = Math.Cos(rotation);
				y = Math.Sin(rotation);
				models[0].verticies[(i * 3) + 6] = (float)x; //current
				models[0].verticies[(i * 3) + 7] = (float)y;
				models[0].verticies[(i * 3) + 8] = 0.0f;				
				models[0].uv[currentIndicies] = new UShort2N((-models[0].verticies[(i * 3) + 6] + 1.0f) * 0.5f,
				                                             (-models[0].verticies[(i * 3) + 7] + 1.0f) * 0.5f);
				models[0].indices[(i * 3) + 2] = currentIndicies;
			}
			models[0].indices[models[0].indices.Length - 1] = 1;
		}
	}
	
	public class Model{
		public string name;
		public List<Vertex> _vertex;
		public float[] verticies, normal;
		public UShort2N[] uv;
		public ushort[] indices;
		public List<ushort> _indices;
			
		public Model(){
			_vertex = new List<Vertex>();
			_indices = new List<ushort>();
			name = "ididntchangethename";
		}
			
		public void finalize(){
			System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
			sw.Start();
			verticies = new float[_vertex.Count * 3];
			normal = new float[_vertex.Count * 3];
			uv = new UShort2N[_vertex.Count];
			for(int i = 0; i < _vertex.Count; i++){
				verticies[(i * 3)] = _vertex[i].pos.X;
				verticies[(i * 3) + 1] = _vertex[i].pos.Y;
				verticies[(i * 3) + 2] = _vertex[i].pos.Z;
				normal[(i * 3)] = _vertex[i].normal.X;
				normal[(i * 3) + 1] = _vertex[i].normal.Y;
				normal[(i * 3) + 2] = _vertex[i].normal.Z;
				uv[i] = new UShort2N(_vertex[i].uv.X,_vertex[i].uv.Y);
			}
			indices = _indices.ToArray();
			_vertex.Clear();
			_indices.Clear();
			sw.Stop();
		}

	}
}

