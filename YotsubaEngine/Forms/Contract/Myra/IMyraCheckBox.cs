using MyraUi = Myra.Graphics2D.UI;

namespace YotsubaEngine.Forms.Contract.Myra
{
    public interface IMyraCheckBox : IMyra
    {
        MyraUi.CheckBox GetMyraCheckBox();
    }
}