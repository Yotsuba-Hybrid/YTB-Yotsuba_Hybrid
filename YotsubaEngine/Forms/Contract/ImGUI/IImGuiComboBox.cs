using System;

namespace YotsubaEngine.Forms.Contract.ImGUI
{
    public interface IImGuiComboBox : IImGui
    {
        Func<int, int> GetImGuiComboBoxAsFunc();
    }
}