using System;

namespace YotsubaEngine.Forms.Contract.ImGUI
{
    public interface IImGuiWindow : IImGui
    {
        Action GetImGuiWindowAsAction();
    }
}