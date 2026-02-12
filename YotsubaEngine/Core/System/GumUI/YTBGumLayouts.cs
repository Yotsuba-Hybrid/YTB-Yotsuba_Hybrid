using Gum.DataTypes;
using Gum.Forms.Controls;
using MonoGameGum;
using System;

namespace YotsubaEngine.Core.System.GumUI
{
    /// <summary>
    /// Utilidades de contenedores de diseño para GumUI.
    /// <para>Layout container utilities for GumUI.</para>
    /// </summary>
    public static class YTBGumLayouts
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

        #region StackPanel

        /// <summary>
        /// Crea un panel de apilado vertical.
        /// <para>Creates a vertical stack panel.</para>
        /// </summary>
        /// <param name="x">Posición X. <para>X position.</para></param>
        /// <param name="y">Posición Y. <para>Y position.</para></param>
        /// <returns>StackPanel creado. <para>Created stack panel.</para></returns>
        public static StackPanel CreateVerticalStack(float x = 0, float y = 0)
        {
            var stackPanel = new StackPanel
            {
                X = x,
                Y = y,
                Orientation = Orientation.Vertical
            };

            return stackPanel;
        }

        /// <summary>
        /// Crea un panel de apilado vertical y lo agrega a la raíz.
        /// <para>Creates a vertical stack panel and adds it to the root.</para>
        /// </summary>
        /// <param name="x">Posición X. <para>X position.</para></param>
        /// <param name="y">Posición Y. <para>Y position.</para></param>
        /// <returns>StackPanel agregado. <para>Added stack panel.</para></returns>
        public static StackPanel AddVerticalStack(float x = 0, float y = 0)
        {
            return AddToGumRoot(CreateVerticalStack(x, y));
        }

        /// <summary>
        /// Crea un panel de apilado horizontal.
        /// <para>Creates a horizontal stack panel.</para>
        /// </summary>
        /// <param name="x">Posición X. <para>X position.</para></param>
        /// <param name="y">Posición Y. <para>Y position.</para></param>
        /// <returns>StackPanel creado. <para>Created stack panel.</para></returns>
        public static StackPanel CreateHorizontalStack(float x = 0, float y = 0)
        {
            var stackPanel = new StackPanel
            {
                X = x,
                Y = y,
                Orientation = Orientation.Horizontal
            };

            return stackPanel;
        }

        /// <summary>
        /// Crea un panel de apilado horizontal y lo agrega a la raíz.
        /// <para>Creates a horizontal stack panel and adds it to the root.</para>
        /// </summary>
        /// <param name="x">Posición X. <para>X position.</para></param>
        /// <param name="y">Posición Y. <para>Y position.</para></param>
        /// <returns>StackPanel agregado. <para>Added stack panel.</para></returns>
        public static StackPanel AddHorizontalStack(float x = 0, float y = 0)
        {
            return AddToGumRoot(CreateHorizontalStack(x, y));
        }

        /// <summary>
        /// Crea un panel de apilado con orientación especificada.
        /// <para>Creates a stack panel with specified orientation.</para>
        /// </summary>
        /// <param name="orientation">Orientación del panel. <para>Stack orientation.</para></param>
        /// <param name="x">Posición X. <para>X position.</para></param>
        /// <param name="y">Posición Y. <para>Y position.</para></param>
        /// <returns>StackPanel creado. <para>Created stack panel.</para></returns>
        public static StackPanel CreateStackPanel(Orientation orientation, float x = 0, float y = 0)
        {
            return new StackPanel
            {
                X = x,
                Y = y,
                Orientation = orientation
            };
        }

        #endregion

        #region Panel / Container

        /// <summary>
        /// Crea un panel contenedor simple.
        /// <para>Creates a simple container panel.</para>
        /// </summary>
        /// <param name="x">Posición X. <para>X position.</para></param>
        /// <param name="y">Posición Y. <para>Y position.</para></param>
        /// <param name="width">Ancho del panel. <para>Panel width.</para></param>
        /// <param name="height">Alto del panel. <para>Panel height.</para></param>
        /// <returns>Panel creado. <para>Created panel.</para></returns>
        public static Panel CreatePanel(float x = 0, float y = 0, float width = 300, float height = 200)
        {
            return new Panel
            {
                X = x,
                Y = y,
                Width = width,
                Height = height
            };
        }

        /// <summary>
        /// Crea un panel y lo agrega a la raíz.
        /// <para>Creates a panel and adds it to the root.</para>
        /// </summary>
        /// <param name="x">Posición X. <para>X position.</para></param>
        /// <param name="y">Posición Y. <para>Y position.</para></param>
        /// <param name="width">Ancho del panel. <para>Panel width.</para></param>
        /// <param name="height">Alto del panel. <para>Panel height.</para></param>
        /// <returns>Panel agregado. <para>Added panel.</para></returns>
        public static Panel AddPanel(float x = 0, float y = 0, float width = 300, float height = 200)
        {
            return AddToGumRoot(CreatePanel(x, y, width, height));
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Agrega un control hijo a un panel de apilado.
        /// <para>Adds a child control to a stack panel.</para>
        /// </summary>
        /// <param name="child">Control hijo. <para>Child control.</para></param>
        /// <param name="stackPanel">Panel de apilado. <para>Stack panel.</para></param>
        /// <returns>Control hijo agregado. <para>Added child control.</para></returns>
        public static T AddTo<T>(this T child, StackPanel stackPanel) where T : FrameworkElement
        {
            stackPanel.AddChild(child);
            return child;
        }

        /// <summary>
        /// Agrega un control hijo a un panel.
        /// <para>Adds a child control to a panel.</para>
        /// </summary>
        /// <param name="child">Control hijo. <para>Child control.</para></param>
        /// <param name="panel">Panel contenedor. <para>Container panel.</para></param>
        /// <returns>Control hijo agregado. <para>Added child control.</para></returns>
        public static T AddTo<T>(this T child, Panel panel) where T : FrameworkElement
        {
            panel.AddChild(child);
            return child;
        }

        /// <summary>
        /// Agrega un control hijo a un scroll viewer.
        /// <para>Adds a child control to a scroll viewer.</para>
        /// </summary>
        /// <param name="child">Control hijo. <para>Child control.</para></param>
        /// <param name="scrollViewer">Scroll viewer contenedor. <para>Container scroll viewer.</para></param>
        /// <returns>Control hijo agregado. <para>Added child control.</para></returns>
        public static T AddTo<T>(this T child, ScrollViewer scrollViewer) where T : FrameworkElement
        {
            scrollViewer.InnerPanel.Children.Add(child.Visual);
            return child;
        }

        #endregion
    }
}
