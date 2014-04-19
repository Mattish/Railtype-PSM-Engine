using System;
using System.Collections.Generic;
using Sce.PlayStation.Core.Graphics;

namespace Railtype_PSM_Engine{
	public class TextureManager{
		const int MAX_TEXTURE_BUFFER_AMOUNT = 8;
		private Dictionary<string,Texture2D> _texturesByName;
		private Dictionary<int,Texture2D>_texturesByNumber;
		public List<int> textureToBufferList;
		public int[] activeBufferConfig;
		int counter;
		
		public TextureManager(){
			_texturesByNumber = new Dictionary<int, Texture2D>(1);
			_texturesByName = new Dictionary<string, Texture2D>(1);
			activeBufferConfig = new int[MAX_TEXTURE_BUFFER_AMOUNT+1]{8,8,8,8,8,8,8,8,8};			
			textureToBufferList = new List<int>(activeBufferConfig);
			counter = 0;
		}
		
		public int GetBufferForTextureNumber(int i){
			if (i == 1337)
				return 8;
			return textureToBufferList[i];
		}
		
		public int TryAddTexture(Texture2D texture){
			_texturesByNumber.Add(counter,texture);
			textureToBufferList.Add(7);
			return counter++;
		}
		
		public int TryAddTexture(string filename){
			Texture2D tmpTexture = new Texture2D("/Application/images/" + filename,false,PixelFormat.Rgba);
			_texturesByNumber.Add(counter,tmpTexture);
			textureToBufferList.Add(7);
			return counter++;
		}
		
		public void SetActiveTexture(int textureNumber, int bufferNumber){
			Globals.gc.SetTexture(bufferNumber,_texturesByNumber[textureNumber]);
			if (activeBufferConfig[bufferNumber] > -1)
				textureToBufferList[activeBufferConfig[bufferNumber]] = 7;
			textureToBufferList[textureNumber] = bufferNumber;
			activeBufferConfig[bufferNumber] = textureNumber;
			
		}
		
	}
}

