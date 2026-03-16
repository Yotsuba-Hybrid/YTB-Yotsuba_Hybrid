using Microsoft.Xna.Framework;
using System;
using System.Linq;
using YotsubaEngine.Core.YotsubaGame;
using YotsubaEngine.Forms.Contract;
using YotsubaEngine.Forms.Contract.GumUI;
using YotsubaEngine.Forms.Contract.ImGUI;
using YotsubaEngine.Forms.Contract.Myra;
using GumUi = Gum.Forms.Controls;
using ImUi = ImGuiNET.ImGui;
using MyraUi = Myra.Graphics2D.UI;

namespace YotsubaEngine.Forms.Implementation
{
    public class Slider : ISlider, IMyraSlider, IGumSlider, IImGuiSlider
    {
        public IContainer Parent { get; set; }
        private float _value;
        private float _min;
        private float _max = 100f;
        private SliderDirection _direction = SliderDirection.Horizontal;

        public float Value
        {
            get => _value;
            set
            {
                if (!float.IsNaN(value) && !float.IsInfinity(value))
                {
                    float clamped = Math.Clamp(value, _min, _max);
                    if (Math.Abs(_value - clamped) > float.Epsilon)
                    {
                        _value = clamped;
                        GumControl.Value = clamped;
                        OnValueChanged?.Invoke(_value);
                    }
                }
            }
        }

        public float Min
        {
            get => _min;
            set
            {
                _min = value;
                GumControl.Minimum = value;
            }
        }

        public float Max
        {
            get => _max;
            set
            {
                _max = value;
                GumControl.Maximum = value;
            }
        }

        public SliderDirection Direction
        {
            get => _direction;
            set
            {
                _direction = value;
                UpdateMyraDirection();
            }
        }

        public string Text { get; set; } = string.Empty;
        public Color Color { get; set; }
        public Vector2 Position { get; set; }

        public event Action<float> OnValueChanged;

        private MyraUi.Slider MyraControl { get; set; }
        private GumUi.Slider GumControl { get; set; }

        public Slider()
        {
            MyraControl = new MyraUi.HorizontalSlider() { Tag = Text };
            GumControl = new GumUi.Slider { Width = 200 };

            GumControl.ValueChanged += (_, _) =>
            {
                _value = (float)GumControl.Value;
                OnValueChanged?.Invoke(_value);
            };
        }

        private void UpdateMyraDirection()
        {
            float currentValue = (float)MyraControl.Value;
            if (_direction == SliderDirection.Horizontal)
            {
                MyraControl = new MyraUi.HorizontalSlider()
                {
                    Tag = Text,
                    Value = currentValue
                };
            }
            else
            {
                MyraControl = new MyraUi.VerticalSlider()
                {
                    Tag = Text,
                    Value = currentValue
                };
            }
        }

        void IGum.DrawGumUI()
        {
            GumControl.X = Position.X;
            GumControl.Y = Position.Y;
        }

        void IImGui.DrawImGuI()
        {
            string label = Text ?? string.Empty;
            bool changed = _direction == SliderDirection.Horizontal
                ? ImUi.SliderFloat(label, ref _value, _min, _max)
                : ImUi.VSliderFloat(label, new System.Numerics.Vector2(150, 150), ref _value, _min, _max);

            if (changed)
            {
                OnValueChanged?.Invoke(_value);
            }
        }

        void IMyra.DrawMyra()
        {
        }

        MyraUi.Slider IMyraSlider.GetMyraSlider()
        {
            return MyraControl;
        }

        GumUi.Slider IGumSlider.GetGumSlider()
        {
            return GumControl;
        }

        Func<float, bool> IImGuiSlider.GetImGuiSliderAsFunc()
        {
            return (_) =>
            {
                string label = Text ?? string.Empty;
                bool changed = _direction == SliderDirection.Horizontal
                    ? ImUi.SliderFloat(label, ref _value, _min, _max)
                    : ImUi.VSliderFloat(label, new System.Numerics.Vector2(150, 150), ref _value, _min, _max);
                return changed;
            };
        }
    }
}