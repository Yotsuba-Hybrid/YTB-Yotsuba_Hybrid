using Microsoft.Xna.Framework;
using Gum.Forms.Controls;
using System;

namespace YotsubaEngine.Core.System.GumUI
{
    /// <summary>
    /// Utilidades de estilizado para controles GumUI.
    /// <para>Styling utilities for GumUI controls.</para>
    /// </summary>
    public static class YTBGumStyles
    {
        #region Color Presets

        /// <summary>
        /// Color primario del tema oscuro.
        /// <para>Dark theme primary color.</para>
        /// </summary>
        public static Color DarkPrimary => new(45, 45, 48);

        /// <summary>
        /// Color secundario del tema oscuro.
        /// <para>Dark theme secondary color.</para>
        /// </summary>
        public static Color DarkSecondary => new(62, 62, 66);

        /// <summary>
        /// Color de acento del tema oscuro.
        /// <para>Dark theme accent color.</para>
        /// </summary>
        public static Color DarkAccent => new(0, 122, 204);

        /// <summary>
        /// Color primario del tema claro.
        /// <para>Light theme primary color.</para>
        /// </summary>
        public static Color LightPrimary => new(240, 240, 240);

        /// <summary>
        /// Color secundario del tema claro.
        /// <para>Light theme secondary color.</para>
        /// </summary>
        public static Color LightSecondary => new(200, 200, 200);

        /// <summary>
        /// Color de acento del tema claro.
        /// <para>Light theme accent color.</para>
        /// </summary>
        public static Color LightAccent => new(0, 120, 215);

        /// <summary>
        /// Color de éxito (verde).
        /// <para>Success color (green).</para>
        /// </summary>
        public static Color Success => new(76, 175, 80);

        /// <summary>
        /// Color de advertencia (amarillo/naranja).
        /// <para>Warning color (yellow/orange).</para>
        /// </summary>
        public static Color Warning => new(255, 152, 0);

        /// <summary>
        /// Color de error (rojo).
        /// <para>Error color (red).</para>
        /// </summary>
        public static Color Error => new(244, 67, 54);

        /// <summary>
        /// Color de información (azul).
        /// <para>Info color (blue).</para>
        /// </summary>
        public static Color Info => new(33, 150, 243);

        #endregion

        #region Common Sizing

        /// <summary>
        /// Establece el tamaño de un elemento de framework.
        /// <para>Sets the size of a framework element.</para>
        /// </summary>
        /// <param name="element">Elemento a ajustar. <para>Element to size.</para></param>
        /// <param name="width">Ancho. <para>Width.</para></param>
        /// <param name="height">Alto. <para>Height.</para></param>
        /// <returns>Elemento actualizado. <para>Updated element.</para></returns>
        public static T WithSize<T>(this T element, float width, float height) where T : FrameworkElement
        {
            element.Width = width;
            element.Height = height;
            return element;
        }

        /// <summary>
        /// Establece la posición de un elemento de framework.
        /// <para>Sets the position of a framework element.</para>
        /// </summary>
        /// <param name="element">Elemento a posicionar. <para>Element to position.</para></param>
        /// <param name="x">Posición X. <para>X position.</para></param>
        /// <param name="y">Posición Y. <para>Y position.</para></param>
        /// <returns>Elemento actualizado. <para>Updated element.</para></returns>
        public static T WithPosition<T>(this T element, float x, float y) where T : FrameworkElement
        {
            element.X = x;
            element.Y = y;
            return element;
        }

        /// <summary>
        /// Establece solo el ancho de un elemento de framework.
        /// <para>Sets only the width of a framework element.</para>
        /// </summary>
        /// <param name="element">Elemento a ajustar. <para>Element to resize.</para></param>
        /// <param name="width">Ancho. <para>Width.</para></param>
        /// <returns>Elemento actualizado. <para>Updated element.</para></returns>
        public static T WithWidth<T>(this T element, float width) where T : FrameworkElement
        {
            element.Width = width;
            return element;
        }

        /// <summary>
        /// Establece solo la altura de un elemento de framework.
        /// <para>Sets only the height of a framework element.</para>
        /// </summary>
        /// <param name="element">Elemento a ajustar. <para>Element to resize.</para></param>
        /// <param name="height">Alto. <para>Height.</para></param>
        /// <returns>Elemento actualizado. <para>Updated element.</para></returns>
        public static T WithHeight<T>(this T element, float height) where T : FrameworkElement
        {
            element.Height = height;
            return element;
        }

        #endregion

        #region Visibility

        /// <summary>
        /// Muestra un elemento de framework.
        /// <para>Shows a framework element.</para>
        /// </summary>
        /// <param name="element">Elemento a mostrar. <para>Element to show.</para></param>
        /// <returns>Elemento actualizado. <para>Updated element.</para></returns>
        public static T Show<T>(this T element) where T : FrameworkElement
        {
            element.IsVisible = true;
            return element;
        }

        /// <summary>
        /// Oculta un elemento de framework.
        /// <para>Hides a framework element.</para>
        /// </summary>
        /// <param name="element">Elemento a ocultar. <para>Element to hide.</para></param>
        /// <returns>Elemento actualizado. <para>Updated element.</para></returns>
        public static T Hide<T>(this T element) where T : FrameworkElement
        {
            element.IsVisible = false;
            return element;
        }

        /// <summary>
        /// Alterna la visibilidad de un elemento de framework.
        /// <para>Toggles visibility of a framework element.</para>
        /// </summary>
        /// <param name="element">Elemento a alternar. <para>Element to toggle.</para></param>
        /// <returns>Elemento actualizado. <para>Updated element.</para></returns>
        public static T ToggleVisibility<T>(this T element) where T : FrameworkElement
        {
            element.IsVisible = !element.IsVisible;
            return element;
        }

        #endregion

        #region Enable/Disable

        /// <summary>
        /// Habilita un elemento de framework.
        /// <para>Enables a framework element.</para>
        /// </summary>
        /// <param name="element">Elemento a habilitar. <para>Element to enable.</para></param>
        /// <returns>Elemento actualizado. <para>Updated element.</para></returns>
        public static T Enable<T>(this T element) where T : FrameworkElement
        {
            element.IsEnabled = true;
            return element;
        }

        /// <summary>
        /// Deshabilita un elemento de framework.
        /// <para>Disables a framework element.</para>
        /// </summary>
        /// <param name="element">Elemento a deshabilitar. <para>Element to disable.</para></param>
        /// <returns>Elemento actualizado. <para>Updated element.</para></returns>
        public static T Disable<T>(this T element) where T : FrameworkElement
        {
            element.IsEnabled = false;
            return element;
        }

        #endregion
    }
}
