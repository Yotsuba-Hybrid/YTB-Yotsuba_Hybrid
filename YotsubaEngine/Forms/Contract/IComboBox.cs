using System;
using System.Collections.Generic;

namespace YotsubaEngine.Forms.Contract
{
    public interface IComboBox : IForm
    {
        IList<string> Items { get; }
        int SelectedIndex { get; set; }
        string SelectedItem { get; }
        event Action<int> OnSelectionChanged;
    }
}