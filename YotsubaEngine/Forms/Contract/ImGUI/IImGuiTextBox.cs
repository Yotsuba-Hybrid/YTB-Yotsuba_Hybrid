using System;

namespace YotsubaEngine.Forms.Contract.ImGUI
{
    public interface IImGuiTextBox : IImGui
    {
        Func<string> GetImGuiTextBoxAsFunc();
    }
}