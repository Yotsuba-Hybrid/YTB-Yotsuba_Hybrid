using System.Collections.Generic;

namespace YotsubaEngine.Forms.Contract
{
    public interface IContainer : IForm
    {
        IReadOnlyList<IForm> Children { get; }
        void AddChild(IForm child);
        void RemoveChild(IForm child);
        void ClearChildren();
    }
}