using GumUi = Gum.Forms.Controls;

namespace YotsubaEngine.Forms.Contract.GumUI
{
    public interface IGumComboBox : IGum
    {
        GumUi.ComboBox GetGumComboBox();
    }
}