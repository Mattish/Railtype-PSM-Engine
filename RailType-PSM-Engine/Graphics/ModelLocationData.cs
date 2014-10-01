using System;
using Sce.PlayStation.Core.Graphics;

namespace RailTypePSMEngine.Graphics{
    public struct ModelBufferLocation : IComparable{
        public Primitive prim;
        public int verticesIndex;
        public int verticesCount;

        public int CompareTo(object obj){
            ModelBufferLocation mbl = (ModelBufferLocation) obj;
            return verticesIndex.CompareTo(mbl.verticesIndex);
        }
    }
}