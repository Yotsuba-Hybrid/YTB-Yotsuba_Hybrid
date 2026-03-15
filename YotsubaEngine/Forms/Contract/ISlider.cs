using System;

namespace YotsubaEngine.Forms.Contract
{
    public enum SliderDirection
    {
        Horizontal,
        Vertical
    }

    public interface ISlider : IForm
    {
        float Value { get; set; }
        float Min { get; set; }
        float Max { get; set; }
        SliderDirection Direction { get; set; }
        event Action<float> OnValueChanged;
    }
}