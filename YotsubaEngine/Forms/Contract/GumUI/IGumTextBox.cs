using GumUi = Gum.Forms.Controls;

namespace YotsubaEngine.Forms.Contract.GumUI
{
    public interface IGumTextBox : IGum
    {
        GumUi.TextBox GetGumTextBox();
    }
}