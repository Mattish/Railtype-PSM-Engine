using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Input;
using Sce.PlayStation.Core.Imaging;
using Railtype_PSM_Engine.Entities;
using Railtype_PSM_Engine.Util;

namespace Railtype_PSM_Engine{
	public class FontManager{
		
		const float _textureSizeMax = 1024.0f;
		const int _spaceGrace = 2;
		public Texture2D _fontTexture;
		Font _font;
		char[] _chars;
		CharMetrics[][] _charMetrics;
		
		public FontManager(){
			_font = new Font(FontAlias.System,48,FontStyle.Regular);
			_chars = new char[256];
			for(int i = 0; i < 256; i++){
				_chars[i] = (char)i;	
			}
			
			_charMetrics = new CharMetrics[16][];
			string totalString = new string(_chars);
			for(int j = 0; j < 16; j++){
				_charMetrics[j] = _font.GetTextMetrics(totalString,j*16,16);
             }
			_fontTexture = createTexture(totalString,_font,0xff000000);
			
		}
		public Thing NewThingText(string text){
			if (text.Length < 1)
				throw new Exception("String needs be at least 1 character!");
			float[] vert = new float[1];
			float[] uvs = new float[1];
			ushort[] indicies = new ushort[1];
			createArraysForText(text, ref vert, ref uvs, ref indicies);
			WaveFrontObject wfo = new WaveFrontObject(ref vert,ref uvs,new float[1]{0f},ref indicies);
			Globals.modelManager.AddModel("text:" + text,wfo);
			Thing tmpThing = new Thing("text:" + text);
			tmpThing.scalexyzrot[0] = 1.0f;
			return tmpThing;
		}
		private void createArraysForText(string text, ref float[] verticies, ref float[] uvs, ref ushort[] indicies){
			CharMetrics[] tmpCharMetrics = _font.GetTextMetrics(text);
			int characterMaxHeight = _font.Metrics.Height;
			float[] upLeft = {-0.5000f, -0.5000f, 0.0000f};
			float[] upRight = {0.5000f, -0.5000f, 0.0000f};
			float[] downLeft = {-0.5000f, 0.5000f, 0.0000f};
			float[] downRight = {0.5000f, 0.5000f, 0.0000f};
			verticies = new float[3*(text.Length*4)];
			uvs = new float[2*(text.Length*4)];
			indicies = new ushort[(text.Length*3)*2];
			ushort indiciesCount = 0;
			float biggestDistance = tmpCharMetrics[0].HorizontalAdvance > 
					Math.Abs(tmpCharMetrics[0].HorizontalBearingX)+tmpCharMetrics[0].Width ? 
						tmpCharMetrics[0].HorizontalAdvance :
						Math.Abs(tmpCharMetrics[0].HorizontalBearingX)+tmpCharMetrics[0].Width;			
			float charRatio = (biggestDistance/(float)characterMaxHeight);
			
			upRight[0] = upLeft[0]-(1.0f*charRatio);
			downRight[0] = downLeft[0]-(1.0f*charRatio);
			Array.Copy(upLeft,0,verticies,0,3);
			Array.Copy(downLeft,0,verticies,3,3);
			Array.Copy(upRight,0,verticies,6,3);
			Array.Copy(downRight,0,verticies,9,3);
			// end of vertex
			indicies[0] = 0;
			indicies[1] = 2;
			indicies[2] = 1;
			indicies[3] = 1;
			indicies[4] = 3;
			indicies[5] = 2;
			indiciesCount = 4;
			// end of indicies
			float[] uv = new float[2];
			int charNumber = (int)text[0];
			int row = charNumber/16;			
			charNumber = charNumber % 16;
			uv[0] = ((float)64*charNumber/_textureSizeMax);
			//uv[0] += biggestDistance/_textureSizeMax;
			uv[1] = (float)_charMetrics[row][charNumber].Y-
			                 (float)_font.Metrics.Ascent;
			uv[1] += row*(_font.Metrics.Height+_spaceGrace);
			uv[1] /= _textureSizeMax;
			uvs[2] = uv[0]; //upleft
			uvs[3] = uv[1];
			
			uvs[0] = uvs[2]; //downleft
			uvs[1] = uvs[3]+(((float)_font.Metrics.Ascent+(float)_font.Metrics.Descent)/_textureSizeMax);
			
			uvs[6] = uvs[0]+(biggestDistance/
								_textureSizeMax);
			uvs[7] = uvs[3]; //upright
			
			uvs[4] = uvs[6]; //downright
			uvs[5] = uvs[1];
			
			for(int i = 1; i < text.Length;i++){
				biggestDistance = tmpCharMetrics[i].HorizontalAdvance > 
					Math.Abs(tmpCharMetrics[i].HorizontalBearingX)+tmpCharMetrics[i].Width ? 
						tmpCharMetrics[i].HorizontalAdvance :
						Math.Abs(tmpCharMetrics[i].HorizontalBearingX)+tmpCharMetrics[i].Width;
				charRatio = (biggestDistance/(float)characterMaxHeight);
				Array.Copy(upRight,0,upLeft,0,3);
				Array.Copy(downRight,0,downLeft,0,3);
				
				upRight[0] = upLeft[0]-(1.0f*charRatio);
				downRight[0] = downLeft[0]-(1.0f*charRatio);
				Array.Copy(upLeft,0,verticies,(i*12),3);
				Array.Copy(downLeft,0,verticies,(i*12)+3,3);
				Array.Copy(upRight,0,verticies,(i*12)+6,3);
				Array.Copy(downRight,0,verticies,(i*12)+9,3);
				// end of vertex
				indicies[i*6] = indiciesCount;//4
				indicies[(i*6)+1] = (ushort)(indiciesCount+2);//6
				indicies[(i*6)+2] = (ushort)(indiciesCount+1);//5
				indicies[(i*6)+3] = (ushort)(indiciesCount+1);//5
				indicies[(i*6)+4] = (ushort)(indiciesCount+3);//7
				indicies[(i*6)+5] = (ushort)(indiciesCount+2);//6
				indiciesCount += 4;
				// end of indicies
				uv = new float[2];
				charNumber = (int)text[i];
				row = charNumber/16;			
				charNumber = charNumber % 16;
			
			
				uv[0] = ((float)64*charNumber/_textureSizeMax);
				//uv[0] += (float)tmpCharMetrics[i].HorizontalBearingX/_textureSizeMax;
				uv[1] = (float)tmpCharMetrics[i].Y-
				                 (float)_font.Metrics.Ascent;
				uv[1] += row*(_font.Metrics.Height+_spaceGrace);
				uv[1] /= _textureSizeMax;
				uvs[(i*8)+2] = uv[0]; //upleft
				uvs[(i*8)+3] = uv[1];
				
				uvs[(i*8)+0] = uvs[(i*8)+2]; //downleft
				uvs[(i*8)+1] = uvs[(i*8)+3]+(((float)_font.Metrics.Ascent+(float)_font.Metrics.Descent)/_textureSizeMax);
				
				uvs[(i*8)+6] = uvs[(i*8)+0]+(biggestDistance/
											_textureSizeMax);
				uvs[(i*8)+7] = uvs[(i*8)+3]; //upright
				
				uvs[(i*8)+4] = uvs[(i*8)+6]; //downright
				uvs[(i*8)+5] = uvs[(i*8)+1];

			}
			
		}
		
	    private Texture2D createTexture(string text, Font font, uint argb){
	        int width = (int)_textureSizeMax;
	        int height = (int)_textureSizeMax;
	
	        var image = new Image(ImageMode.Rgba,
	                              new ImageSize(width, height),
	                              new ImageColor(0, 0, 0, 0));
			for(int i = 0; i < 16;i++){
				for(int j = 0; j < 16;j++){
	        	image.DrawText(text,(i*16)+j,1,
                   new ImageColor((int)((argb >> 16) & 0xff),
                                  (int)((argb >> 8) & 0xff),
                                  (int)((argb >> 0) & 0xff),
                                  (int)((argb >> 24) & 0xff)),
                   font, new ImagePosition(j*64, i*(_font.Metrics.Height+_spaceGrace)));
				}
			}
	        var texture = new Texture2D(width, height, false, PixelFormat.Rgba);
	        texture.SetPixels(0, image.ToBuffer());
	        image.Dispose();
	
	        return texture;
	    }
	}
}

