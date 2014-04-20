using System;
using System.Collections.Generic;
using Sce.PlayStation.Core.Graphics;

namespace Railtype_PSM_Engine{
	public class TextureManager{
		const int MAX_TEXTURE_BUFFER_AMOUNT = 8;
		private Dictionary<string,Texture2D> _texturesByName;
		private Dictionary<int,Texture2D>_texturesByNumber;
		public int[] textureToBuffer;
		public int[] activeBufferConfig;
		int counter;
		
		public TextureManager(){
			_texturesByNumber = new Dictionary<int, Texture2D>(1);
			_texturesByName = new Dictionary<string, Texture2D>(1);
			activeBufferConfig = new int[MAX_TEXTURE_BUFFER_AMOUNT+1]{8,8,8,8,8,8,8,8,8};			
			textureToBuffer = new int[1999];
			for(int i = 0; i < textureToBuffer.Length; i++){
				textureToBuffer[i] = 8;	
			}
			counter = 0;
		}
		
		public int GetBufferForTextureNumber(int i){
			if (i == 1337)
				return 8;
			return textureToBuffer[i];
		}
		
		public int TryAddTexture(Texture2D texture){
			_texturesByNumber.Add(counter,texture);
			textureToBuffer[counter] = 8;
			return counter++;
		}
		
		public int TryAddTexture(string filename){
			Texture2D tmpTexture = new Texture2D("/Application/images/" + filename,false,PixelFormat.Rgba);
			_texturesByNumber.Add(counter,tmpTexture);
			textureToBuffer[counter] = 8;
			return counter++;
		}
		
		public void SetActiveTexture(int textureNumber, int bufferNumber){
			Globals.gc.SetTexture(bufferNumber,_texturesByNumber[textureNumber]);
			//if (activeBufferConfig[bufferNumber] == 8)
			//	textureToBuffer[activeBufferConfig[bufferNumber]] = 7;
			textureToBuffer[textureNumber] = bufferNumber;
			activeBufferConfig[bufferNumber] = textureNumber;
			
		}
		
	}
}

