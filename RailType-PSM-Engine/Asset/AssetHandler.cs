using System;
using System.Collections.Generic;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core;


namespace RailTypePSMEngine.Asset{
	public class AssetHandler{
		private static AssetHandler _ah;
		
		private ShaderAsset _SPSimple;
		private List<Asset> _assetList;
		private Dictionary<string, TextureAsset> _textureAssetDict;
		private List<TextureAsset> _textureAssetList;
		private Dictionary<string, ShaderAsset> _shaderAssetDict;
		private List<ShaderAsset> _shaderAssetList;
		
		private AssetHandler(){
		}
		
		public static void Init(){
			_ah = new AssetHandler();
			_ah.SetupSimpleSP();
			
			_ah._assetList = new List<Asset>(1);
			_ah._textureAssetList = new List<TextureAsset>(1);
			_ah._shaderAssetList = new List<ShaderAsset>(1);
			
			_ah._textureAssetDict = new Dictionary<string, TextureAsset>(1);
			_ah._shaderAssetDict = new Dictionary<string, ShaderAsset>(1);
		}
		
		public static AssetHandler GetInstance(){
			return _ah;	
		}
		
		public void AddTextureCount(string name){
			if (_textureAssetDict.ContainsKey(name)){
				_textureAssetDict[name].AddReferenceCount();	
			}
			else{
				TextureAsset tmpTA = new TextureAsset(name);
				_textureAssetList.Add(tmpTA);
				_textureAssetDict.Add(name, tmpTA);
				tmpTA.Load();
			}
		}
		
		public void RemoveTextureCount(string name){
			if (_textureAssetDict.ContainsKey(name)){
				_textureAssetDict[name].RemoveReferenceCount();
				if(_textureAssetDict[name].ReferenceCount() == 0){
					_textureAssetDict[name].Unload();	
				}
			}	
		}
		
		public ShaderProgram GetSPSimple(){
			return _SPSimple.shader;
		}
		
		private void SetupSimpleSP(){
			_SPSimple = new ShaderAsset("/Application/shaders/Simple.cgx");	
			_SPSimple.shader.SetAttributeBinding(0,"v_Position");
			_SPSimple.shader.SetAttributeBinding(1,"v_TexCoord");
			_SPSimple.shader.SetAttributeBinding(2,"v_Number");
			_SPSimple.shader.SetUniformBinding(0,"WorldViewProj");
			_SPSimple.shader.SetUniformBinding(1,"ModelMatricies");
			_SPSimple.shader.SetUniformBinding(2,"ThingNumbers");
			
		}
	}
}

