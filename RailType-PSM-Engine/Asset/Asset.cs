using System;
using Sce.PlayStation.Core.Graphics;


namespace RailTypePSMEngine.Asset{
    public abstract class Asset{
        private static int _globalNumber = 1;
        private int _refCount;
        public int globalNumber;

        public Asset(){
            globalNumber = _globalNumber++;
            _refCount = 0;
        }

        public void AddReferenceCount(){
            _refCount++;
        }

        public void RemoveReferenceCount(){
            if (_refCount > 0) _refCount--;
        }

        public int ReferenceCount(){
            return this._refCount;
        }
    }

    public class TextureAsset : Asset{
        private static int _textureNumber = 1;
        private static string _location;
        public Texture2D texture;
        public int textureNumber;
        public bool loaded;

        public TextureAsset(string name) : base(){
            textureNumber = _textureNumber++;
            _location = "/Application/images/" + name;
            loaded = false;
        }

        public void Load(){
            if (texture == null){
                texture = new Texture2D(_location, true);
                loaded = true;
            }
        }

        public void Unload(){
            if (texture != null){
                texture.Dispose();
                texture = null;
                loaded = false;
            }
        }
    }

    public class ShaderAsset : Asset{
        private static int _shaderNumber = 1;
        public int shaderNumber;
        public ShaderProgram shader;

        public ShaderAsset(string shaderLocation) : base(){
            shaderNumber = _shaderNumber++;
            shader = new ShaderProgram(shaderLocation);
        }
    }
}