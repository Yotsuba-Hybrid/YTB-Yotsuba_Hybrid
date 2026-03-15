using MyraUi = Myra.Graphics2D.UI;

namespace YotsubaEngine.Forms.Contract.Myra
{
    public interface IMyraComboBox : IMyra
    {
        MyraUi.ComboBox GetMyraComboBox();
    }
}