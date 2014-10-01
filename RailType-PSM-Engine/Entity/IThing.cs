using System;
using Sce.PlayStation.Core;

namespace RailTypePSMEngine.Entity
{
    public interface IThing : IEquatable<IThing>, IDisposable, INumberedThing, IDrawable {
        float[] Scalexyzrot { get; }
        Matrix4 ModelToWorld { get; }
        bool Disposable { get; }
        bool DirtyMatrix { get; }
        void Update();
        void ForceDirty();
    }
}