using System;

namespace YotsubaEngine.Forms.Contract.ImGUI
{
    public interface IImGuiLabel : IImGui
    {
        public Func<string> GetImGuiLabelAsFunc();
    }
}
