using MyraUi = Myra.Graphics2D.UI;

namespace YotsubaEngine.Forms.Contract.Myra
{
    public interface IMyraContainer : IMyra
    {
        MyraUi.Container GetMyraContainer();
    }
}