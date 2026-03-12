using MyraUi = Myra.Graphics2D.UI;

namespace YotsubaEngine.Forms.Contract.Myra
{
    public interface IMyraButton : IMyra
    {
        public MyraUi.Button GetMyraButton();
    }
}
