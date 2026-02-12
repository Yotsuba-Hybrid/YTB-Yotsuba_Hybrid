using Microsoft.Xna.Framework;
using Gum.Forms.Controls;
using MonoGameGum;
using System;

namespace YotsubaEngine.Core.System.GumUI
{
    /// <summary>
    /// Clase fábrica para crear controles GumUI con API simplificada.
    /// <para>Factory class for creating GumUI controls with simplified API.</para>
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
        /// Crea un botón con texto y manejador de clic.
        /// <para>Creates a button with text and click handler.</para>
        /// </summary>
        /// <param name="text">Texto del botón. <para>Button text.</para></param>
        /// <param name="onClick">Acción al hacer clic. <para>Click action.</para></param>
        /// <param name="x">Posición X. <para>X position.</para></param>
        /// <param name="y">Posición Y. <para>Y position.</para></param>
        /// <param name="width">Ancho opcional. <para>Optional width.</para></param>
        /// <param name="height">Alto opcional. <para>Optional height.</para></param>
        /// <returns>Botón creado. <para>Created button.</para></returns>
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
        /// Crea un botón y lo agrega al contenedor raíz.
        /// <para>Creates a button and adds it to the root container.</para>
        /// </summary>
        /// <param name="text">Texto del botón. <para>Button text.</para></param>
        /// <param name="onClick">Acción al hacer clic. <para>Click action.</para></param>
        /// <param name="x">Posición X. <para>X position.</para></param>
        /// <param name="y">Posición Y. <para>Y position.</para></param>
        /// <param name="width">Ancho opcional. <para>Optional width.</para></param>
        /// <param name="height">Alto opcional. <para>Optional height.</para></param>
        /// <returns>Botón agregado. <para>Added button.</para></returns>
        public static Button AddButton(string text, Action onClick = null, float x = 0, float y = 0, float? width = null, float? height = null)
        {
            return AddToGumRoot(CreateButton(text, onClick, x, y, width, height));
        }

        #endregion

        #region Label

        /// <summary>
        /// Crea una etiqueta de texto.
        /// <para>Creates a text label.</para>
        /// </summary>
        /// <param name="text">Texto de la etiqueta. <para>Label text.</para></param>
        /// <param name="x">Posición X. <para>X position.</para></param>
        /// <param name="y">Posición Y. <para>Y position.</para></param>
        /// <returns>Etiqueta creada. <para>Created label.</para></returns>
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
        /// Crea una etiqueta y la agrega al contenedor raíz.
        /// <para>Creates a label and adds it to the root container.</para>
        /// </summary>
        /// <param name="text">Texto de la etiqueta. <para>Label text.</para></param>
        /// <param name="x">Posición X. <para>X position.</para></param>
        /// <param name="y">Posición Y. <para>Y position.</para></param>
        /// <returns>Etiqueta agregada. <para>Added label.</para></returns>
        public static Label AddLabel(string text, float x = 0, float y = 0)
        {
            return AddToGumRoot(CreateLabel(text, x, y));
        }

        #endregion

        #region TextBox

        /// <summary>
        /// Crea una caja de texto editable.
        /// <para>Creates an editable text box.</para>
        /// </summary>
        /// <param name="placeholder">Texto inicial. <para>Placeholder text.</para></param>
        /// <param name="x">Posición X. <para>X position.</para></param>
        /// <param name="y">Posición Y. <para>Y position.</para></param>
        /// <param name="width">Ancho del control. <para>Control width.</para></param>
        /// <param name="onTextChanged">Acción al cambiar el texto. <para>Action when text changes.</para></param>
        /// <returns>TextBox creado. <para>Created text box.</para></returns>
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
        /// Crea un TextBox y lo agrega al contenedor raíz.
        /// <para>Creates a text box and adds it to the root container.</para>
        /// </summary>
        /// <param name="placeholder">Texto inicial. <para>Placeholder text.</para></param>
        /// <param name="x">Posición X. <para>X position.</para></param>
        /// <param name="y">Posición Y. <para>Y position.</para></param>
        /// <param name="width">Ancho del control. <para>Control width.</para></param>
        /// <param name="onTextChanged">Acción al cambiar el texto. <para>Action when text changes.</para></param>
        /// <returns>TextBox agregado. <para>Added text box.</para></returns>
        public static TextBox AddTextBox(string placeholder = "", float x = 0, float y = 0, float width = 200, Action<string> onTextChanged = null)
        {
            return AddToGumRoot(CreateTextBox(placeholder, x, y, width, onTextChanged));
        }

        #endregion

        #region PasswordBox

        /// <summary>
        /// Crea una caja de entrada de contraseña.
        /// <para>Creates a password input box.</para>
        /// </summary>
        /// <param name="x">Posición X. <para>X position.</para></param>
        /// <param name="y">Posición Y. <para>Y position.</para></param>
        /// <param name="width">Ancho del control. <para>Control width.</para></param>
        /// <returns>PasswordBox creado. <para>Created password box.</para></returns>
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
        /// Crea un PasswordBox y lo agrega al contenedor raíz.
        /// <para>Creates a password box and adds it to the root container.</para>
        /// </summary>
        /// <param name="x">Posición X. <para>X position.</para></param>
        /// <param name="y">Posición Y. <para>Y position.</para></param>
        /// <param name="width">Ancho del control. <para>Control width.</para></param>
        /// <returns>PasswordBox agregado. <para>Added password box.</para></returns>
        public static PasswordBox AddPasswordBox(float x = 0, float y = 0, float width = 200)
        {
            return AddToGumRoot(CreatePasswordBox(x, y, width));
        }

        #endregion

        #region CheckBox

        /// <summary>
        /// Crea un control checkbox.
        /// <para>Creates a checkbox control.</para>
        /// </summary>
        /// <param name="text">Texto de la casilla. <para>Checkbox text.</para></param>
        /// <param name="isChecked">Estado inicial. <para>Initial checked state.</para></param>
        /// <param name="x">Posición X. <para>X position.</para></param>
        /// <param name="y">Posición Y. <para>Y position.</para></param>
        /// <param name="onCheckedChanged">Acción al cambiar. <para>Action when checked changes.</para></param>
        /// <returns>CheckBox creado. <para>Created checkbox.</para></returns>
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
        /// Crea un checkbox y lo agrega al contenedor raíz.
        /// <para>Creates a checkbox and adds it to the root container.</para>
        /// </summary>
        /// <param name="text">Texto de la casilla. <para>Checkbox text.</para></param>
        /// <param name="isChecked">Estado inicial. <para>Initial checked state.</para></param>
        /// <param name="x">Posición X. <para>X position.</para></param>
        /// <param name="y">Posición Y. <para>Y position.</para></param>
        /// <param name="onCheckedChanged">Acción al cambiar. <para>Action when checked changes.</para></param>
        /// <returns>CheckBox agregado. <para>Added checkbox.</para></returns>
        public static CheckBox AddCheckBox(string text, bool isChecked = false, float x = 0, float y = 0, Action<bool> onCheckedChanged = null)
        {
            return AddToGumRoot(CreateCheckBox(text, isChecked, x, y, onCheckedChanged));
        }

        #endregion

        #region RadioButton

        /// <summary>
        /// Crea un control radio button.
        /// <para>Creates a radio button control.</para>
        /// </summary>
        /// <param name="text">Texto del botón. <para>Radio button text.</para></param>
        /// <param name="groupName">Nombre del grupo. <para>Group name.</para></param>
        /// <param name="x">Posición X. <para>X position.</para></param>
        /// <param name="y">Posición Y. <para>Y position.</para></param>
        /// <param name="onCheckedChanged">Acción al cambiar. <para>Action when checked changes.</para></param>
        /// <returns>RadioButton creado. <para>Created radio button.</para></returns>
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
        /// Crea un radio button y lo agrega al contenedor raíz.
        /// <para>Creates a radio button and adds it to the root container.</para>
        /// </summary>
        /// <param name="text">Texto del botón. <para>Radio button text.</para></param>
        /// <param name="groupName">Nombre del grupo. <para>Group name.</para></param>
        /// <param name="x">Posición X. <para>X position.</para></param>
        /// <param name="y">Posición Y. <para>Y position.</para></param>
        /// <param name="onCheckedChanged">Acción al cambiar. <para>Action when checked changes.</para></param>
        /// <returns>RadioButton agregado. <para>Added radio button.</para></returns>
        public static RadioButton AddRadioButton(string text, string groupName = "default", float x = 0, float y = 0, Action<bool> onCheckedChanged = null)
        {
            return AddToGumRoot(CreateRadioButton(text, groupName, x, y, onCheckedChanged));
        }

        #endregion

        #region ComboBox

        /// <summary>
        /// Crea una caja desplegable combo box.
        /// <para>Creates a dropdown combo box.</para>
        /// </summary>
        /// <param name="items">Items del combo. <para>Combo box items.</para></param>
        /// <param name="x">Posición X. <para>X position.</para></param>
        /// <param name="y">Posición Y. <para>Y position.</para></param>
        /// <param name="width">Ancho del control. <para>Control width.</para></param>
        /// <param name="onSelectionChanged">Acción al cambiar selección. <para>Action when selection changes.</para></param>
        /// <returns>ComboBox creado. <para>Created combo box.</para></returns>
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
        /// Crea un combo box y lo agrega al contenedor raíz.
        /// <para>Creates a combo box and adds it to the root container.</para>
        /// </summary>
        /// <param name="items">Items del combo. <para>Combo box items.</para></param>
        /// <param name="x">Posición X. <para>X position.</para></param>
        /// <param name="y">Posición Y. <para>Y position.</para></param>
        /// <param name="width">Ancho del control. <para>Control width.</para></param>
        /// <param name="onSelectionChanged">Acción al cambiar selección. <para>Action when selection changes.</para></param>
        /// <returns>ComboBox agregado. <para>Added combo box.</para></returns>
        public static ComboBox AddComboBox(string[] items = null, float x = 0, float y = 0, float width = 150, Action<int> onSelectionChanged = null)
        {
            return AddToGumRoot(CreateComboBox(items, x, y, width, onSelectionChanged));
        }

        #endregion

        #region Slider

        /// <summary>
        /// Crea un control slider/deslizador.
        /// <para>Creates a slider control.</para>
        /// </summary>
        /// <param name="minimum">Valor mínimo. <para>Minimum value.</para></param>
        /// <param name="maximum">Valor máximo. <para>Maximum value.</para></param>
        /// <param name="value">Valor inicial. <para>Initial value.</para></param>
        /// <param name="x">Posición X. <para>X position.</para></param>
        /// <param name="y">Posición Y. <para>Y position.</para></param>
        /// <param name="width">Ancho del control. <para>Control width.</para></param>
        /// <param name="onValueChanged">Acción al cambiar el valor. <para>Action when value changes.</para></param>
        /// <returns>Slider creado. <para>Created slider.</para></returns>
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
        /// Crea un slider y lo agrega al contenedor raíz.
        /// <para>Creates a slider and adds it to the root container.</para>
        /// </summary>
        /// <param name="minimum">Valor mínimo. <para>Minimum value.</para></param>
        /// <param name="maximum">Valor máximo. <para>Maximum value.</para></param>
        /// <param name="value">Valor inicial. <para>Initial value.</para></param>
        /// <param name="x">Posición X. <para>X position.</para></param>
        /// <param name="y">Posición Y. <para>Y position.</para></param>
        /// <param name="width">Ancho del control. <para>Control width.</para></param>
        /// <param name="onValueChanged">Acción al cambiar el valor. <para>Action when value changes.</para></param>
        /// <returns>Slider agregado. <para>Added slider.</para></returns>
        public static Slider AddSlider(double minimum = 0, double maximum = 100, double value = 50, float x = 0, float y = 0, float width = 200, Action<double> onValueChanged = null)
        {
            return AddToGumRoot(CreateSlider(minimum, maximum, value, x, y, width, onValueChanged));
        }

        #endregion

        #region ListBox

        /// <summary>
        /// Crea un control list box.
        /// <para>Creates a list box control.</para>
        /// </summary>
        /// <param name="items">Items de la lista. <para>List items.</para></param>
        /// <param name="x">Posición X. <para>X position.</para></param>
        /// <param name="y">Posición Y. <para>Y position.</para></param>
        /// <param name="width">Ancho del control. <para>Control width.</para></param>
        /// <param name="height">Alto del control. <para>Control height.</para></param>
        /// <param name="onSelectionChanged">Acción al cambiar selección. <para>Action when selection changes.</para></param>
        /// <returns>ListBox creado. <para>Created list box.</para></returns>
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
        /// Crea un list box y lo agrega al contenedor raíz.
        /// <para>Creates a list box and adds it to the root container.</para>
        /// </summary>
        /// <param name="items">Items de la lista. <para>List items.</para></param>
        /// <param name="x">Posición X. <para>X position.</para></param>
        /// <param name="y">Posición Y. <para>Y position.</para></param>
        /// <param name="width">Ancho del control. <para>Control width.</para></param>
        /// <param name="height">Alto del control. <para>Control height.</para></param>
        /// <param name="onSelectionChanged">Acción al cambiar selección. <para>Action when selection changes.</para></param>
        /// <returns>ListBox agregado. <para>Added list box.</para></returns>
        public static ListBox AddListBox(string[] items = null, float x = 0, float y = 0, float width = 200, float height = 150, Action<int> onSelectionChanged = null)
        {
            return AddToGumRoot(CreateListBox(items, x, y, width, height, onSelectionChanged));
        }

        #endregion

        #region ScrollViewer

        /// <summary>
        /// Crea un scroll viewer para contenido desplazable.
        /// <para>Creates a scroll viewer for scrollable content.</para>
        /// </summary>
        /// <param name="x">Posición X. <para>X position.</para></param>
        /// <param name="y">Posición Y. <para>Y position.</para></param>
        /// <param name="width">Ancho del control. <para>Control width.</para></param>
        /// <param name="height">Alto del control. <para>Control height.</para></param>
        /// <returns>ScrollViewer creado. <para>Created scroll viewer.</para></returns>
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
        /// Crea un scroll viewer y lo agrega al contenedor raíz.
        /// <para>Creates a scroll viewer and adds it to the root container.</para>
        /// </summary>
        /// <param name="x">Posición X. <para>X position.</para></param>
        /// <param name="y">Posición Y. <para>Y position.</para></param>
        /// <param name="width">Ancho del control. <para>Control width.</para></param>
        /// <param name="height">Alto del control. <para>Control height.</para></param>
        /// <returns>ScrollViewer agregado. <para>Added scroll viewer.</para></returns>
        public static ScrollViewer AddScrollViewer(float x = 0, float y = 0, float width = 300, float height = 200)
        {
            return AddToGumRoot(CreateScrollViewer(x, y, width, height));
        }

        #endregion
    }
}
