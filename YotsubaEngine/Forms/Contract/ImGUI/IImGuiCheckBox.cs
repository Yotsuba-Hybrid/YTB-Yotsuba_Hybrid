using System;

namespace YotsubaEngine.Forms.Contract.ImGUI
{
    public interface IImGuiCheckBox : IImGui
    {
        Func<bool, bool> GetImGuiCheckBoxAsFunc();
    }
}