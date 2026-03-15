using System;

namespace YotsubaEngine.Forms.Contract
{
    public interface IWindow : IContainer
    {
        new string Text { get; set; }
        bool IsOpen { get; set; }
        event Action OnClosed;
    }
}