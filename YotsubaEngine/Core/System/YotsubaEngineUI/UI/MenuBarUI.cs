
using ImGuiNET;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using YotsubaEngine.ActionFiles.YTB_Files;
using YotsubaEngine.Audio;
using YotsubaEngine.Core.System.YotsubaEngineCore;
using YotsubaEngine.Core.YotsubaGame;
using YotsubaEngine.Events.YTBEvents.EngineEvents;
using Num = System.Numerics;

namespace YotsubaEngine.Core.System.YotsubaEngineUI.UI
{
    /// <summary>
    /// Renderiza la barra de menú del editor para herramientas del motor.
    /// <para>Renders the editor menu bar for engine tooling.</para>
    /// </summary>
    public class MenuBarUI
    {
        /// <summary>
        /// Retrieves the current background color.
        /// Obtiene el color de fondo actual.
        /// </summary>
        private readonly Func<Color> _getBackground;

        /// <summary>
        /// Applies a new background color.
        /// Aplica un nuevo color de fondo.
        /// </summary>
        private readonly Action<Color> _applyBackground;

        /// <summary>
        /// Persists the background name when changed.
        /// Persiste el nombre del fondo cuando cambia.
        /// </summary>
        private readonly Action<string> _onSaveBackgroundName;

        /// <summary>
        /// Stores the date filter tuple.
        /// Almacena la tupla de filtro de fecha.
        /// </summary>
        private (int, int, int) filtroFecha = (0, 0, 0);

        /// <summary>
        /// Crea una barra de menú con controladores de fondo.
        /// <para>Creates a menu bar UI with background handlers.</para>
        /// </summary>
        /// <param name="getBackground">Función para obtener el color de fondo. <para>Function to get the background color.</para></param>
        /// <param name="applyBackground">Acción para aplicar el color de fondo. <para>Action to apply the background color.</para></param>
        /// <param name="onSaveBackgroundName">Acción para guardar el nombre del fondo. <para>Action to save the background name.</para></param>
        public MenuBarUI(Func<Color> getBackground, Action<Color> applyBackground, Action<string> onSaveBackgroundName)
        {
            _getBackground = getBackground;
            _applyBackground = applyBackground;
            _onSaveBackgroundName = onSaveBackgroundName;
		}

        /// <summary>
        /// Renderiza la barra de menú y maneja acciones del usuario.
        /// <para>Renders the menu bar and handles user actions.</para>
        /// </summary>
        public async Task RenderMenuBarAsync()
        {
            ImGui.SeparatorText("YTB");

            //https://www.leshylabs.com/apps/sstool/
            if (ImGui.BeginMenu("Archivos"))
            {

                if (ImGui.MenuItem("Guardar Cambios"))
                {
                    EngineUISystem.SaveChanges();
                    YTBGame game = (YTBGame)YTBGlobalState.Game;
                    YTBGlobalState.LastSceneNameBeforeUpdate = game.SceneManager.CurrentScene.SceneName;
                    Task.Run(async () => await YTBFileToGameData.UpdateStateOfSceneManager());
                    
                }
				

                if (ImGui.MenuItem("WEB - Generador de hojas de sprites"))
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = "https://www.leshylabs.com/apps/sstool/",
                        UseShellExecute = true
                    });
                }
                if (ImGui.MenuItem("WEB - Formatear JSON")) 
                {
                    Process.Start(
                        new ProcessStartInfo
                        {
                            FileName = "https://jam.dev/utilities/json-formatter",
                            UseShellExecute = true
                        });
                }
                //if (ImGui.MenuItem("Salir")) { /* acción */ }
                ImGui.EndMenu();
            }
            ImGui.Separator();

            if (ImGui.Button("Historial"))
            {
//-:cnd:noEmit
#if YTB
                HistoryUI.Open();
#endif
//+:cnd:noEmit
            }


            if (ImGui.BeginMenu("Fondo"))
            {
                ImGui.PushItemWidth(140);
                Color c = _getBackground();
                ColorPicker.RenderColorCombo("Fondo", ref c, (string name) =>
                {
                    _applyBackground(c);
                    _onSaveBackgroundName?.Invoke(name);
                });
                ImGui.PopItemWidth();

                ImGui.EndMenu();
            }

            //if (ImGui.BeginMenu("Build Game Executable"))
            //{
            //    // Mantén aquí tu estructura de submenús
            //    ImGui.EndMenu();
            //}

            if (ImGui.Button("Recompilar Assets"))
            {
                YTBContentBuilder.Rebuild(async () =>
                {
                    await YTBFileToGameData.UpdateStateOfSceneManager();
                });
            }

            if (ImGui.Button("Recompilar y Ejecutar"))
            {
//-:cnd:noEmit
#if YTB
                YTBContentBuilder.Rebuild(async () =>
                {
                    await YTBFileToGameData.UpdateStateOfSceneManager();
                    EventManager.Instance.Publish(new OnShowGameUIHiddeEngineEditor(true, false));
                });
                //EventManager.Instance.Publish(new OnShowGameUIHiddeEngineEditor(false, false));
                //await YTBFileToGameData.UpdateStateOfSceneManager();
                //EventManager.Instance.Publish(new OnShowGameUIHiddeEngineEditor(true, false));
#endif
//+:cnd:noEmit
            }
            ImGui.Separator();
            if (ImGui.Button("Reproducir"))
            {
//-:cnd:noEmit
#if YTB
                AudioSystem.ResumeAll();
                EventManager.Instance.Publish(new OnShowGameUIHiddeEngineEditor(true,false));
#endif
//+:cnd:noEmit
            }
            ImGui.SameLine();
            if (ImGui.Button("Pausar")) 
            {
//-:cnd:noEmit
#if YTB

                AudioSystem.PauseAll();
                EventManager.Instance.Publish(new OnShowGameUIHiddeEngineEditor(false,true));
#endif
//+:cnd:noEmit
            }

            // --- Ventana de Historial (nueva implementación mejorada) ---
//-:cnd:noEmit
#if YTB
            HistoryUI.Render();
#endif
//+:cnd:noEmit

        }


    }
}
