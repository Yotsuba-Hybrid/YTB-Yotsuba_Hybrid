using MyraUi = Myra.Graphics2D.UI;

namespace YotsubaEngine.Forms.Contract.Myra
{
    public interface IMyraTextBox : IMyra
    {
        MyraUi.TextBox GetMyraTextBox();
    }
}