using System;

namespace YotsubaEngine.Forms.Contract
{
    public interface IButton : ChildElement.Forms
    {
        public event Action OnClick;
    }
}
