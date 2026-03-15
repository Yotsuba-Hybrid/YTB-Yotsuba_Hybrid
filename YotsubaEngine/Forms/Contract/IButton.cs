using System;

namespace YotsubaEngine.Forms.Contract
{
    public interface IButton : IForm
    {
        

        public event Action OnClick;
    }
}
