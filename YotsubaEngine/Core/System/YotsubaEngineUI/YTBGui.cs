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
    /// Provides helper wrappers for ImGui UI calls.
    /// Proporciona envoltorios auxiliares para llamadas de UI ImGui.
    /// </summary>
    public static class YTBGui
    {
        /// <summary>
        /// Draws colored text with optional disable and wrap states.
        /// Dibuja texto con color con estados opcionales de deshabilitado y ajuste.
        /// </summary>
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
        /// Draws text with optional disable and wrap states.
        /// Dibuja texto con estados opcionales de deshabilitado y ajuste.
        /// </summary>
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
        /// Opens a popup modal and executes the provided action.
        /// Abre un modal emergente y ejecuta la acción proporcionada.
        /// </summary>
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
        /// Draws a modal popup window with the provided action.
        /// Dibuja una ventana modal emergente con la acción proporcionada.
        /// </summary>
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
        /// Opens a section window and invokes the provided action.
        /// Abre una sección y ejecuta la acción proporcionada.
        /// </summary>
        public static void AbrirSeccion(string title, Action fn)
        {
            ImGui.Begin(title);

            fn.Invoke();

            ImGui.End();
        }

        /// <summary>
        /// Ensures a popup is opened if it is not already open.
        /// Asegura que un popup se abra si aún no está abierto.
        /// </summary>
        public static void DispararModal(string titulo)
        {
            if (!ImGui.IsPopupOpen(titulo))
            {
                ImGui.OpenPopup(titulo);
            }
        }

        /// <summary>
        /// Draws a separator line in the UI.
        /// Dibuja una línea separadora en la UI.
        /// </summary>
        public static void Line()
        {
            ImGui.Separator();
        }

        /// <summary>
        /// Draws a checkbox and updates the provided state.
        /// Dibuja una casilla de verificación y actualiza el estado proporcionado.
        /// </summary>
        public static void CheckBox(string title, ref bool state)
        {
            ImGui.Checkbox(title, ref state);
        }

        /// <summary>
        /// Moves the cursor to the same line for subsequent UI items.
        /// Mueve el cursor a la misma línea para elementos de UI posteriores.
        /// </summary>
        public static void Continue()
        {
            ImGui.SameLine();
        }

        /// <summary>
        /// Inserts vertical spacing in the UI layout.
        /// Inserta espaciado vertical en el diseño de la UI.
        /// </summary>
        public static void Space()
        {
            ImGui.Spacing();
        }

        /// <summary>
        /// Draws a button and invokes the action when clicked.
        /// Dibuja un botón y ejecuta la acción cuando se pulsa.
        /// </summary>
        public static void Button(string text, Action action)
        {
            if (ImGui.Button(text))
            {
                action.Invoke();
            }
        }
    }
}
