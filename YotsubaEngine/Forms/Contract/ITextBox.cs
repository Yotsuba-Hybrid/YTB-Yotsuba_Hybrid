using System;

namespace YotsubaEngine.Forms.Contract
{
    public interface ITextBox : IForm
    {
        new string Text { get; set; }
        event Action<string> OnValueChanged;
        event Action<string> OnSubmit;
    }
}