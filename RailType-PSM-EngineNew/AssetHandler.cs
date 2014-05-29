using System;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core;


namespace RailTypePSMEngineNew{
	public class AssetHandler{
		private static AssetHandler _ah;
		
		private ShaderProgram _SPSimple;
		private AssetHandler(){
						
		}
		
		public static void Init(){
			_ah = new AssetHandler();
			_ah.SetupSimpleSP();
		}
		
		public static AssetHandler GetInstance(){
			return _ah;	
		}
		
		public ShaderProgram GetSPSimple(){
			return _SPSimple;
		}
		
		private void SetupSimpleSP(){
			_SPSimple = new ShaderProgram("/Application/shaders/Simple.cgx");	
			_SPSimple.SetAttributeBinding(0,"v_Position");
			_SPSimple.SetAttributeBinding(1,"v_TexCoord");
			_SPSimple.SetAttributeBinding(2,"v_Number");
			_SPSimple.SetUniformBinding(0,"WorldViewProj");
			_SPSimple.SetUniformBinding(1,"ModelMatricies");
			_SPSimple.SetUniformBinding(2,"ThingNumbers");
			
		}
	}
}

