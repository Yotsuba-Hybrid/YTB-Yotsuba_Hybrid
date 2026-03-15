using MyraUi = Myra.Graphics2D.UI;

namespace YotsubaEngine.Forms.Contract.Myra
{
    public interface IMyraWindow : IMyraContainer
    {
        MyraUi.Window GetMyraWindow();
    }
}