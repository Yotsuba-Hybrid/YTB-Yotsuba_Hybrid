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

        public ButtonBuilder CreateButton(string text = "")
        {
            return new ButtonBuilder(_library, text);
        }

        public LabelBuilder CreateLabel(string text = "")
        {
            return new LabelBuilder(_library, text);
        }

        public CheckBoxBuilder CreateCheckBox(string text = "", bool isChecked = false)
        {
            return new CheckBoxBuilder(_library, text, isChecked);
        }

        public TextBoxBuilder CreateTextBox(string text = "")
        {
            return new TextBoxBuilder(_library, text);
        }

        public SliderBuilder CreateSlider()
        {
            return new SliderBuilder(_library);
        }

        public ComboBoxBuilder CreateComboBox()
        {
            return new ComboBoxBuilder(_library);
        }

        public WindowBuilder CreateWindow(string title = "Window")
        {
            return new WindowBuilder(_library, title);
        }

        public PanelBuilder CreatePanel()
        {
            return new PanelBuilder(_library);
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