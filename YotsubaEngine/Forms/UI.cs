using YotsubaEngine.Forms.Contract;

namespace YotsubaEngine.Forms
{
    public class UI
    {
        private readonly UILibrary _library;

        public UI(UILibrary library = UILibrary.GumUI)
        {
            _library = library;
            FormsManager.Instance.SetActiveLibrary(library);
        }

        public ButtonBuilder CreateButton(string text)
        {
            return new ButtonBuilder(_library, text ?? string.Empty);
        }

        public LabelBuilder CreateLabel(string text)
        {
            return new LabelBuilder(_library, text ?? string.Empty);
        }

        public CheckBoxBuilder CreateCheckBox(string text, bool isChecked = false)
        {
            return new CheckBoxBuilder(_library, text ?? string.Empty, isChecked);
        }

        public TextBoxBuilder CreateTextBox(string text = "")
        {
            return new TextBoxBuilder(_library, text ?? string.Empty);
        }

        public SliderBuilder CreateSlider(string text)
        {
            return new SliderBuilder(_library, text ?? string.Empty);
        }

        public ComboBoxBuilder CreateComboBox(string text = "ComboBox")
        {
            return new ComboBoxBuilder(_library, text ?? string.Empty);
        }

        public WindowBuilder CreateWindow(string title)
        {
            return new WindowBuilder(_library, title ?? "Window");
        }

        public PanelBuilder CreatePanel(string text = "Panel")
        {
            return new PanelBuilder(_library, text ?? string.Empty);
        }

        public void AddToRoot(IForm control)
        {
            FormsManager.Instance.AddToRoot(control);
        }

        public void RemoveFromRoot(IForm control)
        {
            FormsManager.Instance.RemoveFromRoot(control);
        }

        public void Clear()
        {
            FormsManager.Instance.Clear();
        }

        public void SetLibrary(UILibrary library)
        {
            FormsManager.Instance.SetActiveLibrary(library);
        }

        public T Cast<T>(IForm control) where T : class, IForm
        {
            return control as T;
        }
    }
}