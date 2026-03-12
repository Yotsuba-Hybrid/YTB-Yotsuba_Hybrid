using System;
using System.Collections.Generic;
using System.Text;

namespace YotsubaEngine.Forms.Contract.ImGUI
{
    public interface IImGuiButton : IImGui
    {
        public Func<bool> GetImGuiButtonAsFunc();
    }
}
