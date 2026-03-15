using System;

namespace YotsubaEngine.Forms.Contract
{
    public interface ICheckBox : IForm
    {
        bool IsChecked { get; set; }
        event Action<bool> OnCheckedChanged;
    }
}