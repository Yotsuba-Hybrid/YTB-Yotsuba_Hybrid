using ImGuiNET;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YotsubaEngine.Core.System.YotsubaEngineCore;

namespace YotsubaEngine.Core.System.YotsubaEngineUI
{
    /// <summary>
    /// Proporciona envoltorios auxiliares para llamadas de UI ImGui.
    /// <para>Provides helper wrappers for ImGui UI calls.</para>
    /// </summary>
    public static class YTBGui
    {
        /// <summary>
        /// Dibuja texto con color con estados opcionales de deshabilitado y ajuste.
        /// <para>Draws colored text with optional disable and wrap states.</para>
        /// </summary>
        /// <param name="texto">Texto a mostrar. <para>Text to display.</para></param>
        /// <param name="color">Color del texto. <para>Text color.</para></param>
        /// <param name="disabled">Indica si el texto está deshabilitado. <para>Whether the text is disabled.</para></param>
        /// <param name="wrap">Indica si debe ajustarse el texto. <para>Whether to wrap the text.</para></param>
        public static void Text(string texto, Color color, bool disabled = false, bool wrap = false)
        {
            if (disabled)
            {
                Text(texto, true);
                return;
            }

            if (wrap)
            {
                ImGui.TextWrapped(texto);
                return;
            }

            ImGui.TextColored(color.ToVector4().ToNumerics(), texto);
        }

        /// <summary>
        /// Dibuja texto con estados opcionales de deshabilitado y ajuste.
        /// <para>Draws text with optional disable and wrap states.</para>
        /// </summary>
        /// <param name="texto">Texto a mostrar. <para>Text to display.</para></param>
        /// <param name="disabled">Indica si el texto está deshabilitado. <para>Whether the text is disabled.</para></param>
        /// <param name="wrap">Indica si debe ajustarse el texto. <para>Whether to wrap the text.</para></param>
        public static void Text(string texto, bool disabled = false, bool wrap = false)
        {
            if (disabled)
            {
                ImGui.TextDisabled(texto);
                return;
            }

            if (wrap)
            {
                ImGui.TextWrapped(texto);
                return;
            }

            ImGui.TextColored(Color.White.ToVector4().ToNumerics(), texto);
        }

        /// <summary>
        /// Abre un modal emergente y ejecuta la acción proporcionada.
        /// <para>Opens a popup modal and executes the provided action.</para>
        /// </summary>
        /// <param name="titulo">Título del modal. <para>Modal title.</para></param>
        /// <param name="action">Acción a ejecutar dentro del modal. <para>Action to execute inside the modal.</para></param>
        /// <param name="pos">Posición inicial del modal. <para>Initial modal position.</para></param>
        /// <param name="width">Ancho del modal. <para>Modal width.</para></param>
        /// <param name="height">Alto del modal. <para>Modal height.</para></param>
        /// <param name="abrir">Indica si se abre el popup. <para>Whether to open the popup.</para></param>
        /// <param name="flags">Flags de ventana ImGui. <para>ImGui window flags.</para></param>
        public static void AbrirModal(string titulo, Action action, Vector2 pos, float width = 500f, float height = 500f, bool abrir = true, ImGuiWindowFlags flags = ImGuiWindowFlags.None)
        {

            ImGui.SetNextWindowPos(pos.ToNumerics());
            ImGui.SetNextWindowSize(new Vector2(width, height).ToNumerics());
            ImGui.OpenPopup(titulo);
            if (ImGui.BeginPopup(titulo, flags))
            {
                action.Invoke();

                ImGui.EndPopup();
            }
        }

        /// <summary>
        /// Dibuja una ventana modal emergente con la acción proporcionada.
        /// <para>Draws a modal popup window with the provided action.</para>
        /// </summary>
        /// <param name="titulo">Título del modal. <para>Modal title.</para></param>
        /// <param name="action">Acción a ejecutar dentro del modal. <para>Action to execute inside the modal.</para></param>
        /// <param name="width">Ancho del modal. <para>Modal width.</para></param>
        /// <param name="height">Alto del modal. <para>Modal height.</para></param>
        /// <param name="flags">Flags de ventana ImGui. <para>ImGui window flags.</para></param>
        public static void DibujarModal(string titulo, Action action, float width = 500f, float height = 500f, ImGuiWindowFlags flags = ImGuiWindowFlags.None)
        {
            // Solo establecemos el tamaño, la posición la maneja el sistema modal (o puedes forzarla si quieres)
            ImGui.SetNextWindowSize(new Vector2(width, height).ToNumerics(), ImGuiCond.FirstUseEver);

            // IMPORTANTE: Aquí NO llamamos a OpenPopup.
            // Usamos BeginPopupModal que oscurece el fondo y bloquea clicks fuera.
            bool isOpen = true;
            if (ImGui.BeginPopupModal(titulo, ref isOpen, flags))
            {
                action.Invoke();
                ImGui.EndPopup();
            }
        }


        /// <summary>
        /// Abre una sección y ejecuta la acción proporcionada.
        /// <para>Opens a section window and invokes the provided action.</para>
        /// </summary>
        /// <param name="title">Título de la sección. <para>Section title.</para></param>
        /// <param name="fn">Acción a ejecutar. <para>Action to execute.</para></param>
        public static void AbrirSeccion(string title, Action fn)
        {
            ImGui.Begin(title);

            fn.Invoke();

            ImGui.End();
        }

        /// <summary>
        /// Asegura que un popup se abra si aún no está abierto.
        /// <para>Ensures a popup is opened if it is not already open.</para>
        /// </summary>
        /// <param name="titulo">Título del popup. <para>Popup title.</para></param>
        public static void DispararModal(string titulo)
        {
            if (!ImGui.IsPopupOpen(titulo))
            {
                ImGui.OpenPopup(titulo);
            }
        }

        /// <summary>
        /// Dibuja una línea separadora en la UI.
        /// <para>Draws a separator line in the UI.</para>
        /// </summary>
        public static void Line()
        {
            ImGui.Separator();
        }

        /// <summary>
        /// Dibuja una casilla de verificación y actualiza el estado proporcionado.
        /// <para>Draws a checkbox and updates the provided state.</para>
        /// </summary>
        /// <param name="title">Etiqueta de la casilla. <para>Checkbox label.</para></param>
        /// <param name="state">Estado actual. <para>Current state.</para></param>
        public static bool CheckBox(string title, ref bool state)
        {
            return ImGui.Checkbox(title, ref state);
        }

        /// <summary>
        /// Mueve el cursor a la misma línea para elementos de UI posteriores.
        /// <para>Moves the cursor to the same line for subsequent UI items.</para>
        /// </summary>
        public static void Continue()
        {
            ImGui.SameLine();
        }

        /// <summary>
        /// Inserta espaciado vertical en el diseño de la UI.
        /// <para>Inserts vertical spacing in the UI layout.</para>
        /// </summary>
        public static void Space()
        {
            ImGui.Spacing();
        }

        /// <summary>
        /// Dibuja un botón y ejecuta la acción cuando se pulsa.
        /// <para>Draws a button and invokes the action when clicked.</para>
        /// </summary>
        /// <param name="text">Texto del botón. <para>Button text.</para></param>
        /// <param name="action">Acción a ejecutar. <para>Action to execute.</para></param>
        public static void Button(string text, Action action)
        {
            if (ImGui.Button(text))
            {
                action.Invoke();
            }
        }
    }
}
