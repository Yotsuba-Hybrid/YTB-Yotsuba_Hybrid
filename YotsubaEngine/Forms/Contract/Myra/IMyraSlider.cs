using MyraUi = Myra.Graphics2D.UI;

namespace YotsubaEngine.Forms.Contract.Myra
{
    public interface IMyraSlider : IMyra
    {
        MyraUi.Slider GetMyraSlider();
    }
}