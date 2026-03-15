using GumUi = Gum.Forms.Controls;

namespace YotsubaEngine.Forms.Contract.GumUI
{
    public interface IGumCheckBox : IGum
    {
        GumUi.CheckBox GetGumCheckBox();
    }
}