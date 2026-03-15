using System;

namespace YotsubaEngine.Forms.Contract.ImGUI
{
    public interface IImGuiSlider : IImGui
    {
        Func<float, bool> GetImGuiSliderAsFunc();
    }
}