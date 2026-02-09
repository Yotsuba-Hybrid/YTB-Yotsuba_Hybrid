using Microsoft.Xna.Framework;
using Gum.Forms.Controls;
using System;

namespace YotsubaEngine.Core.System.GumUI
{
    /// <summary>
    /// Styling utilities for GumUI controls.
    /// Utilidades de estilizado para controles GumUI.
    /// </summary>
    public static class YTBGumStyles
    {
        #region Color Presets

        /// <summary>
        /// Dark theme primary color.
        /// Color primario del tema oscuro.
        /// </summary>
        public static Color DarkPrimary => new(45, 45, 48);

        /// <summary>
        /// Dark theme secondary color.
        /// Color secundario del tema oscuro.
        /// </summary>
        public static Color DarkSecondary => new(62, 62, 66);

        /// <summary>
        /// Dark theme accent color.
        /// Color de acento del tema oscuro.
        /// </summary>
        public static Color DarkAccent => new(0, 122, 204);

        /// <summary>
        /// Light theme primary color.
        /// Color primario del tema claro.
        /// </summary>
        public static Color LightPrimary => new(240, 240, 240);

        /// <summary>
        /// Light theme secondary color.
        /// Color secundario del tema claro.
        /// </summary>
        public static Color LightSecondary => new(200, 200, 200);

        /// <summary>
        /// Light theme accent color.
        /// Color de acento del tema claro.
        /// </summary>
        public static Color LightAccent => new(0, 120, 215);

        /// <summary>
        /// Success color (green).
        /// Color de éxito (verde).
        /// </summary>
        public static Color Success => new(76, 175, 80);

        /// <summary>
        /// Warning color (yellow/orange).
        /// Color de advertencia (amarillo/naranja).
        /// </summary>
        public static Color Warning => new(255, 152, 0);

        /// <summary>
        /// Error color (red).
        /// Color de error (rojo).
        /// </summary>
        public static Color Error => new(244, 67, 54);

        /// <summary>
        /// Info color (blue).
        /// Color de información (azul).
        /// </summary>
        public static Color Info => new(33, 150, 243);

        #endregion

        #region Common Sizing

        /// <summary>
        /// Sets the size of a framework element.
        /// Establece el tamaño de un elemento de framework.
        /// </summary>
        public static T WithSize<T>(this T element, float width, float height) where T : FrameworkElement
        {
            element.Width = width;
            element.Height = height;
            return element;
        }

        /// <summary>
        /// Sets the position of a framework element.
        /// Establece la posición de un elemento de framework.
        /// </summary>
        public static T WithPosition<T>(this T element, float x, float y) where T : FrameworkElement
        {
            element.X = x;
            element.Y = y;
            return element;
        }

        /// <summary>
        /// Sets only the width of a framework element.
        /// Establece solo el ancho de un elemento de framework.
        /// </summary>
        public static T WithWidth<T>(this T element, float width) where T : FrameworkElement
        {
            element.Width = width;
            return element;
        }

        /// <summary>
        /// Sets only the height of a framework element.
        /// Establece solo la altura de un elemento de framework.
        /// </summary>
        public static T WithHeight<T>(this T element, float height) where T : FrameworkElement
        {
            element.Height = height;
            return element;
        }

        #endregion

        #region Visibility

        /// <summary>
        /// Shows a framework element.
        /// Muestra un elemento de framework.
        /// </summary>
        public static T Show<T>(this T element) where T : FrameworkElement
        {
            element.IsVisible = true;
            return element;
        }

        /// <summary>
        /// Hides a framework element.
        /// Oculta un elemento de framework.
        /// </summary>
        public static T Hide<T>(this T element) where T : FrameworkElement
        {
            element.IsVisible = false;
            return element;
        }

        /// <summary>
        /// Toggles visibility of a framework element.
        /// Alterna la visibilidad de un elemento de framework.
        /// </summary>
        public static T ToggleVisibility<T>(this T element) where T : FrameworkElement
        {
            element.IsVisible = !element.IsVisible;
            return element;
        }

        #endregion

        #region Enable/Disable

        /// <summary>
        /// Enables a framework element.
        /// Habilita un elemento de framework.
        /// </summary>
        public static T Enable<T>(this T element) where T : FrameworkElement
        {
            element.IsEnabled = true;
            return element;
        }

        /// <summary>
        /// Disables a framework element.
        /// Deshabilita un elemento de framework.
        /// </summary>
        public static T Disable<T>(this T element) where T : FrameworkElement
        {
            element.IsEnabled = false;
            return element;
        }

        #endregion
    }
}
