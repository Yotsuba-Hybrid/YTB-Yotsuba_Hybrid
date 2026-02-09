using Gum.DataTypes;
using Gum.Forms.Controls;
using MonoGameGum;
using System;

namespace YotsubaEngine.Core.System.GumUI
{
    /// <summary>
    /// Layout container utilities for GumUI.
    /// Utilidades de contenedores de diseño para GumUI.
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
        /// Creates a vertical stack panel.
        /// Crea un panel de apilado vertical.
        /// </summary>
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
        /// Creates a vertical stack panel and adds it to the root.
        /// Crea un panel de apilado vertical y lo agrega a la raíz.
        /// </summary>
        public static StackPanel AddVerticalStack(float x = 0, float y = 0)
        {
            return AddToGumRoot(CreateVerticalStack(x, y));
        }

        /// <summary>
        /// Creates a horizontal stack panel.
        /// Crea un panel de apilado horizontal.
        /// </summary>
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
        /// Creates a horizontal stack panel and adds it to the root.
        /// Crea un panel de apilado horizontal y lo agrega a la raíz.
        /// </summary>
        public static StackPanel AddHorizontalStack(float x = 0, float y = 0)
        {
            return AddToGumRoot(CreateHorizontalStack(x, y));
        }

        /// <summary>
        /// Creates a stack panel with specified orientation.
        /// Crea un panel de apilado con orientación especificada.
        /// </summary>
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
        /// Creates a simple container panel.
        /// Crea un panel contenedor simple.
        /// </summary>
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
        /// Creates a panel and adds it to the root.
        /// Crea un panel y lo agrega a la raíz.
        /// </summary>
        public static Panel AddPanel(float x = 0, float y = 0, float width = 300, float height = 200)
        {
            return AddToGumRoot(CreatePanel(x, y, width, height));
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Adds a child control to a stack panel.
        /// Agrega un control hijo a un panel de apilado.
        /// </summary>
        public static T AddTo<T>(this T child, StackPanel stackPanel) where T : FrameworkElement
        {
            stackPanel.AddChild(child);
            return child;
        }

        /// <summary>
        /// Adds a child control to a panel.
        /// Agrega un control hijo a un panel.
        /// </summary>
        public static T AddTo<T>(this T child, Panel panel) where T : FrameworkElement
        {
            panel.AddChild(child);
            return child;
        }

        /// <summary>
        /// Adds a child control to a scroll viewer.
        /// Agrega un control hijo a un scroll viewer.
        /// </summary>
        public static T AddTo<T>(this T child, ScrollViewer scrollViewer) where T : FrameworkElement
        {
            scrollViewer.InnerPanel.Children.Add(child.Visual);
            return child;
        }

        #endregion
    }
}
