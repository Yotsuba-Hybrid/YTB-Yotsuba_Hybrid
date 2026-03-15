using GumUi = Gum.Forms.Controls;

namespace YotsubaEngine.Forms.Contract.GumUI
{
    public interface IGumContainer : IGum
    {
        GumUi.Panel GetGumContainer();
    }
}