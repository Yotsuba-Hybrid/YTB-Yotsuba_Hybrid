using Microsoft.Xna.Framework;
using Gum.Forms.Controls;
using MonoGameGum;
using System;

namespace YotsubaEngine.Core.System.GumUI
{
    /// <summary>
    /// Factory class for creating GumUI controls with simplified API.
    /// Clase fábrica para crear controles GumUI con API simplificada.
    /// </summary>
    public static class YTBGumControls
    {
        /// <summary>
        /// Adds a control to the Gum root container.
        /// Agrega un control al contenedor raíz de Gum.
        /// </summary>
        private static T AddToGumRoot<T>(T control) where T : FrameworkElement
        {
            GumService.Default.Root.Children.Add(control.Visual);
            return control;
        }

        #region Button

        /// <summary>
        /// Creates a button with text and click handler.
        /// Crea un botón con texto y manejador de click.
        /// </summary>
        public static Button CreateButton(string text, Action onClick = null, float x = 0, float y = 0, float? width = null, float? height = null)
        {
            var button = new Button
            {
                Text = text,
                X = x,
                Y = y
            };

            if (width.HasValue) button.Width = width.Value;
            if (height.HasValue) button.Height = height.Value;

            if (onClick != null)
            {
                button.Click += (_, _) => onClick();
            }

            return button;
        }

        /// <summary>
        /// Creates a button and adds it to the root container.
        /// Crea un botón y lo agrega al contenedor raíz.
        /// </summary>
        public static Button AddButton(string text, Action onClick = null, float x = 0, float y = 0, float? width = null, float? height = null)
        {
            return AddToGumRoot(CreateButton(text, onClick, x, y, width, height));
        }

        #endregion

        #region Label

        /// <summary>
        /// Creates a text label.
        /// Crea una etiqueta de texto.
        /// </summary>
        public static Label CreateLabel(string text, float x = 0, float y = 0)
        {
            return new Label
            {
                Text = text,
                X = x,
                Y = y
            };
        }

        /// <summary>
        /// Creates a label and adds it to the root container.
        /// Crea una etiqueta y la agrega al contenedor raíz.
        /// </summary>
        public static Label AddLabel(string text, float x = 0, float y = 0)
        {
            return AddToGumRoot(CreateLabel(text, x, y));
        }

        #endregion

        #region TextBox

        /// <summary>
        /// Creates an editable text box.
        /// Crea una caja de texto editable.
        /// </summary>
        public static TextBox CreateTextBox(string placeholder = "", float x = 0, float y = 0, float width = 200, Action<string> onTextChanged = null)
        {
            var textBox = new TextBox
            {
                Text = placeholder,
                X = x,
                Y = y,
                Width = width
            };

            if (onTextChanged != null)
            {
                textBox.TextChanged += (_, _) => onTextChanged(textBox.Text);
            }

            return textBox;
        }

        /// <summary>
        /// Creates a text box and adds it to the root container.
        /// Crea un TextBox y lo agrega al contenedor raíz.
        /// </summary>
        public static TextBox AddTextBox(string placeholder = "", float x = 0, float y = 0, float width = 200, Action<string> onTextChanged = null)
        {
            return AddToGumRoot(CreateTextBox(placeholder, x, y, width, onTextChanged));
        }

        #endregion

        #region PasswordBox

        /// <summary>
        /// Creates a password input box.
        /// Crea una caja de entrada de contraseña.
        /// </summary>
        public static PasswordBox CreatePasswordBox(float x = 0, float y = 0, float width = 200)
        {
            return new PasswordBox
            {
                X = x,
                Y = y,
                Width = width
            };
        }

        /// <summary>
        /// Creates a password box and adds it to the root container.
        /// Crea un PasswordBox y lo agrega al contenedor raíz.
        /// </summary>
        public static PasswordBox AddPasswordBox(float x = 0, float y = 0, float width = 200)
        {
            return AddToGumRoot(CreatePasswordBox(x, y, width));
        }

        #endregion

        #region CheckBox

        /// <summary>
        /// Creates a checkbox control.
        /// Crea un control checkbox.
        /// </summary>
        public static CheckBox CreateCheckBox(string text, bool isChecked = false, float x = 0, float y = 0, Action<bool> onCheckedChanged = null)
        {
            var checkBox = new CheckBox
            {
                Text = text,
                IsChecked = isChecked,
                X = x,
                Y = y
            };

            if (onCheckedChanged != null)
            {
                checkBox.Checked += (_, _) => onCheckedChanged(true);
                checkBox.Unchecked += (_, _) => onCheckedChanged(false);
            }

            return checkBox;
        }

        /// <summary>
        /// Creates a checkbox and adds it to the root container.
        /// Crea un checkbox y lo agrega al contenedor raíz.
        /// </summary>
        public static CheckBox AddCheckBox(string text, bool isChecked = false, float x = 0, float y = 0, Action<bool> onCheckedChanged = null)
        {
            return AddToGumRoot(CreateCheckBox(text, isChecked, x, y, onCheckedChanged));
        }

        #endregion

        #region RadioButton

        /// <summary>
        /// Creates a radio button control.
        /// Crea un control radio button.
        /// </summary>
        public static RadioButton CreateRadioButton(string text, string groupName = "default", float x = 0, float y = 0, Action<bool> onCheckedChanged = null)
        {
            var radioButton = new RadioButton
            {
                Text = text,
                GroupName = groupName,
                X = x,
                Y = y
            };

            if (onCheckedChanged != null)
            {
                radioButton.Checked += (_, _) => onCheckedChanged(true);
                radioButton.Unchecked += (_, _) => onCheckedChanged(false);
            }

            return radioButton;
        }

        /// <summary>
        /// Creates a radio button and adds it to the root container.
        /// Crea un radio button y lo agrega al contenedor raíz.
        /// </summary>
        public static RadioButton AddRadioButton(string text, string groupName = "default", float x = 0, float y = 0, Action<bool> onCheckedChanged = null)
        {
            return AddToGumRoot(CreateRadioButton(text, groupName, x, y, onCheckedChanged));
        }

        #endregion

        #region ComboBox

        /// <summary>
        /// Creates a dropdown combo box.
        /// Crea una caja desplegable combo box.
        /// </summary>
        public static ComboBox CreateComboBox(string[] items = null, float x = 0, float y = 0, float width = 150, Action<int> onSelectionChanged = null)
        {
            var comboBox = new ComboBox
            {
                X = x,
                Y = y,
                Width = width
            };

            if (items != null)
            {
                foreach (var item in items)
                {
                    comboBox.Items.Add(item);
                }
            }

            if (onSelectionChanged != null)
            {
                comboBox.SelectionChanged += (_, _) => onSelectionChanged(comboBox.SelectedIndex);
            }

            return comboBox;
        }

        /// <summary>
        /// Creates a combo box and adds it to the root container.
        /// Crea un combo box y lo agrega al contenedor raíz.
        /// </summary>
        public static ComboBox AddComboBox(string[] items = null, float x = 0, float y = 0, float width = 150, Action<int> onSelectionChanged = null)
        {
            return AddToGumRoot(CreateComboBox(items, x, y, width, onSelectionChanged));
        }

        #endregion

        #region Slider

        /// <summary>
        /// Creates a slider control.
        /// Crea un control slider/deslizador.
        /// </summary>
        public static Slider CreateSlider(double minimum = 0, double maximum = 100, double value = 50, float x = 0, float y = 0, float width = 200, Action<double> onValueChanged = null)
        {
            var slider = new Slider
            {
                Minimum = minimum,
                Maximum = maximum,
                Value = value,
                X = x,
                Y = y,
                Width = width
            };

            if (onValueChanged != null)
            {
                slider.ValueChanged += (_, _) => onValueChanged(slider.Value);
            }

            return slider;
        }

        /// <summary>
        /// Creates a slider and adds it to the root container.
        /// Crea un slider y lo agrega al contenedor raíz.
        /// </summary>
        public static Slider AddSlider(double minimum = 0, double maximum = 100, double value = 50, float x = 0, float y = 0, float width = 200, Action<double> onValueChanged = null)
        {
            return AddToGumRoot(CreateSlider(minimum, maximum, value, x, y, width, onValueChanged));
        }

        #endregion

        #region ListBox

        /// <summary>
        /// Creates a list box control.
        /// Crea un control list box.
        /// </summary>
        public static ListBox CreateListBox(string[] items = null, float x = 0, float y = 0, float width = 200, float height = 150, Action<int> onSelectionChanged = null)
        {
            var listBox = new ListBox
            {
                X = x,
                Y = y,
                Width = width,
                Height = height
            };

            if (items != null)
            {
                foreach (var item in items)
                {
                    listBox.Items.Add(item);
                }
            }

            if (onSelectionChanged != null)
            {
                listBox.SelectionChanged += (_, _) => onSelectionChanged(listBox.SelectedIndex);
            }

            return listBox;
        }

        /// <summary>
        /// Creates a list box and adds it to the root container.
        /// Crea un list box y lo agrega al contenedor raíz.
        /// </summary>
        public static ListBox AddListBox(string[] items = null, float x = 0, float y = 0, float width = 200, float height = 150, Action<int> onSelectionChanged = null)
        {
            return AddToGumRoot(CreateListBox(items, x, y, width, height, onSelectionChanged));
        }

        #endregion

        #region ScrollViewer

        /// <summary>
        /// Creates a scroll viewer for scrollable content.
        /// Crea un scroll viewer para contenido desplazable.
        /// </summary>
        public static ScrollViewer CreateScrollViewer(float x = 0, float y = 0, float width = 300, float height = 200)
        {
            return new ScrollViewer
            {
                X = x,
                Y = y,
                Width = width,
                Height = height
            };
        }

        /// <summary>
        /// Creates a scroll viewer and adds it to the root container.
        /// Crea un scroll viewer y lo agrega al contenedor raíz.
        /// </summary>
        public static ScrollViewer AddScrollViewer(float x = 0, float y = 0, float width = 300, float height = 200)
        {
            return AddToGumRoot(CreateScrollViewer(x, y, width, height));
        }

        #endregion
    }
}
