using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using YotsubaEngine.Forms.Contract;
using YotsubaEngine.Forms.Contract.GumUI;
using YotsubaEngine.Forms.Contract.ImGUI;
using YotsubaEngine.Forms.Contract.Myra;
using GumUi = Gum.Forms.Controls;
using ImUi = ImGuiNET.ImGui;
using MyraUi = Myra.Graphics2D.UI;

namespace YotsubaEngine.Forms.Implementation
{
    public class ComboBox : IComboBox, IMyraComboBox, IGumComboBox, IImGuiComboBox
    {
        public IContainer Parent { get; set; }
        private List<string> _items = new();
        private int _selectedIndex = -1;
        private string _text = string.Empty;

        public IList<string> Items => _items;

        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                if (_selectedIndex != value && value >= -1 && value < _items.Count)
                {
                    _selectedIndex = value;
                    GumControl.SelectedIndex = value;
                    OnSelectionChanged?.Invoke(value);
                }
            }
        }

        public string SelectedItem => _selectedIndex >= 0 && _selectedIndex < _items.Count
            ? _items[_selectedIndex]
            : string.Empty;

        public string Text
        {
            get => _text;
            set => _text = value ?? string.Empty;
        }

        public Color Color { get; set; }
        private Vector2 _position;
        public Vector2 Position
        {
            get => _position;
            set
            {
                _position = value;
                MyraControl.Left = (int)value.X;
                MyraControl.Top = (int)value.Y;
            }
        }

        public event Action<int> OnSelectionChanged;

        private MyraUi.ComboBox MyraControl { get; set; }
        private GumUi.ComboBox GumControl { get; set; }

        public ComboBox()
        {
            MyraControl = new() { Tag = _text };
            GumControl = new() { Width = 150 };

            GumControl.SelectionChanged += (_, _) =>
            {
                _selectedIndex = GumControl.SelectedIndex;
                OnSelectionChanged?.Invoke(_selectedIndex);
            };
        }

        public void AddItem(string item)
        {
            _items.Add(item ?? string.Empty);
            UpdateMyraItems();
            UpdateGumItems();
        }

        public void RemoveItem(int index)
        {
            if (index >= 0 && index < _items.Count)
            {
                _items.RemoveAt(index);
                if (_selectedIndex >= _items.Count)
                {
                    _selectedIndex = _items.Count - 1;
                }
                UpdateMyraItems();
                UpdateGumItems();
            }
        }

        public void ClearItems()
        {
            _items.Clear();
            _selectedIndex = -1;
            UpdateMyraItems();
            UpdateGumItems();
        }

        private void UpdateMyraItems()
        {
            MyraControl.Items.Clear();
            foreach (var item in _items)
            {
                MyraControl.Items.Add(new MyraUi.ListItem(item ?? string.Empty));
            }
        }

        private void UpdateGumItems()
        {
            GumControl.Items?.Clear();
            foreach (var item in _items)
            {
                GumControl.Items.Add(item ?? string.Empty);
            }
        }

        void IGum.DrawGumUI()
        {
            GumControl.X = Position.X;
            GumControl.Y = Position.Y;
        }

        void IImGui.DrawImGuI()
        {
            string label = _text ?? string.Empty;
            string preview = SelectedItem ?? string.Empty;
            
            if (ImUi.BeginCombo(label, preview))
            {
                for (int i = 0; i < _items.Count; i++)
                {
                    bool isSelected = i == _selectedIndex;
                    string itemText = _items[i] ?? string.Empty;
                    if (ImUi.Selectable(itemText, isSelected))
                    {
                        _selectedIndex = i;
                        OnSelectionChanged?.Invoke(_selectedIndex);
                    }
                    if (isSelected)
                    {
                        ImUi.SetItemDefaultFocus();
                    }
                }
                ImUi.EndCombo();
            }
        }

        void IMyra.DrawMyra()
        {
        }

        MyraUi.ComboBox IMyraComboBox.GetMyraComboBox()
        {
            return MyraControl;
        }

        GumUi.ComboBox IGumComboBox.GetGumComboBox()
        {
            return GumControl;
        }

        Func<int, int> IImGuiComboBox.GetImGuiComboBoxAsFunc()
        {
            return (idx) => _selectedIndex;
        }
    }
}