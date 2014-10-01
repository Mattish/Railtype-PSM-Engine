using System;

namespace RailTypePSMEngine.Entity{
    public interface IDrawable{
        bool Draw { get; }
        Tuple<int, int> ShaderTextureNo { get; }
    }
}