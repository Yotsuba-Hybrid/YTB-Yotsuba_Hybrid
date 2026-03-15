using GumUi = Gum.Forms.Controls;

namespace YotsubaEngine.Forms.Contract.GumUI
{
    public interface IGumWindow : IGumContainer
    {
        GumUi.Panel GetGumPanel();
    }
}