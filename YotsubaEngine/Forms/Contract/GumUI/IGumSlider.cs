using GumUi = Gum.Forms.Controls;

namespace YotsubaEngine.Forms.Contract.GumUI
{
    public interface IGumSlider : IGum
    {
        GumUi.Slider GetGumSlider();
    }
}