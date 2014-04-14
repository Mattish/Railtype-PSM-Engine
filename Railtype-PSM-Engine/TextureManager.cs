using System;
using System.Collections.Generic;
using Sce.PlayStation.Core.Graphics;

namespace Railtype_PSM_Engine{
	public class TextureManager{
		const int MAX_TEXTURE_BUFFER_AMOUNT = 8;
		private Dictionary<string,Texture2D> _texturesByName;
		private Dictionary<int,Texture2D>_texturesByNumber;
		private GraphicsContext _graphics;
		public List<int> textureToBufferList;
		public int[] activeBufferConfig;
		int counter;
		
		public TextureManager(GraphicsContext graphic){
			_texturesByNumber = new Dictionary<int, Texture2D>(1);
			_texturesByName = new Dictionary<string, Texture2D>(1);
			_graphics = graphic;
			textureToBufferList = new List<int>(1);
			activeBufferConfig = new int[8]{-1,-1,-1,-1,-1,-1,-1,-1};
			counter = 0;
		}
		
		public int TryAddTexture(string filename){
			Texture2D tmpTexture = new Texture2D("/Application/images/" + filename,false,PixelFormat.Rgba);
			_texturesByNumber.Add(counter,tmpTexture);
			textureToBufferList.Add(7);
			return counter++;
		}
		
		public void SetActiveTexture(int textureNumber, int bufferNumber){
			_graphics.SetTexture(bufferNumber,_texturesByNumber[textureNumber]);
			if (activeBufferConfig[bufferNumber] > -1)
				textureToBufferList[activeBufferConfig[bufferNumber]] = 7;
			textureToBufferList[textureNumber] = bufferNumber;
			activeBufferConfig[bufferNumber] = textureNumber;
			
		}
		
	}
}

