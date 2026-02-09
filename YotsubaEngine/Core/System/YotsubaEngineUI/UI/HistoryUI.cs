#if YTB
using ImGuiNET;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using YotsubaEngine.ActionFiles.YTB_Files;
using YotsubaEngine.Core.System.YotsubaEngineCore;
using YotsubaEngine.Core.YotsubaGame;
using YotsubaEngine.Events.YTBEvents.EngineEvents;
using Num = System.Numerics;

namespace YotsubaEngine.Core.System.YotsubaEngineUI.UI
{
    /// <summary>
    /// Ventana mejorada de historial de cambios del archivo .ytb
    /// Solo disponible en modo DEBUG
    /// </summary>
    public static class HistoryUI
    {
        private static string _searchFilter = "";
        private static int _selectedIndex = -1;
        private static YTBEngineHistory _previewVersion = null;
        private static bool _showConfirmRestore = false;
        private static string _currentVersionTime = "";
        private static List<YTBEngineHistory> _cachedHistory = null;
        private static DateTime _lastCacheUpdate = DateTime.MinValue;
        private static readonly TimeSpan CacheDuration = TimeSpan.FromSeconds(2);
        
        // Configuración de la ventana
        private const int MaxHistoryDisplay = 1000;
        private const float MinWindowWidth = 600f;
        private const float MinWindowHeight = 400f;

        /// <summary>
        /// Renderiza la ventana principal del historial
        /// </summary>
        public static void Render()
        {
            if (!ImGui.BeginPopupModal("Historial de Cambios del proyecto", 
                ImGuiWindowFlags.Popup | ImGuiWindowFlags.Modal | ImGuiWindowFlags.AlwaysAutoResize))
            {
                return;
            }

            try
            {
                // Establecer tamaño mínimo de ventana
                var windowSize = ImGui.GetWindowSize();
                if (windowSize.X < MinWindowWidth || windowSize.Y < MinWindowHeight)
                {
                    ImGui.SetWindowSize(new Num.Vector2(
                        Math.Max(windowSize.X, MinWindowWidth),
                        Math.Max(windowSize.Y, MinWindowHeight)
                    ));
                }

                RenderHeader();
                ImGui.Separator();
                
                RenderSearchBar();
                ImGui.Separator();

                RenderHistoryList();
                
                // Ventana de confirmación (si está activa)
                RenderConfirmRestoreDialog();
            }
            catch (Exception ex)
            {
                EngineUISystem.SendLog($"ERROR en HistoryUI: {ex.Message}");
                ImGui.TextColored(new Num.Vector4(1, 0, 0, 1), 
                    $"Error al renderizar el historial: {ex.Message}");
            }
            finally
            {
                ImGui.EndPopup();
            }
        }

        /// <summary>
        /// Renderiza el encabezado con información general
        /// </summary>
        private static void RenderHeader()
        {
            var history = GetCachedHistory();
            
            ImGui.PushStyleColor(ImGuiCol.Text, new Num.Vector4(0.4f, 0.8f, 1.0f, 1.0f));
            ImGui.TextWrapped("Historial de Cambios del Proyecto");
            ImGui.PopStyleColor();

            ImGui.Spacing();
            
            // Información general
            ImGui.Text($"Total de versiones guardadas: {history.Count}");
            
            if (history.Count > 0)
            {
                var oldest = history.First();
                var newest = history.Last();
                ImGui.Text($"Primera versión: {FormatRelativeTime(oldest.SaveTime)}");
                ImGui.Text($"Última versión: {FormatRelativeTime(newest.SaveTime)}");
            }

            // Versión actual cargada
            if (!string.IsNullOrEmpty(_currentVersionTime))
            {
                ImGui.PushStyleColor(ImGuiCol.Text, new Num.Vector4(0.2f, 1.0f, 0.2f, 1.0f));
                ImGui.Text($"✓ Versión actual: {_currentVersionTime}");
                ImGui.PopStyleColor();
            }

            ImGui.Spacing();

            // Botón de cerrar
            if (ImGui.Button("Cerrar"))
            {
                CloseWindow();
            }
        }

        /// <summary>
        /// Renderiza la barra de búsqueda y filtros
        /// </summary>
        private static void RenderSearchBar()
        {
            ImGui.PushItemWidth(300);
            if (ImGui.InputTextWithHint("##search", "Buscar por fecha (ej: 2024, 12/25, 14:30)...", 
                ref _searchFilter, 100))
            {
                // Resetear selección cuando cambia el filtro
                _selectedIndex = -1;
                _previewVersion = null;
            }
            ImGui.PopItemWidth();

            ImGui.SameLine();
            if (ImGui.SmallButton("Limpiar"))
            {
                _searchFilter = "";
                _selectedIndex = -1;
                _previewVersion = null;
            }

            // Mostrar resultados de búsqueda
            if (!string.IsNullOrEmpty(_searchFilter))
            {
                var filtered = GetFilteredHistory();
                ImGui.TextColored(new Num.Vector4(0.7f, 0.7f, 0.7f, 1.0f),
                    $"Mostrando {filtered.Count} de {GetCachedHistory().Count} versiones");
            }
        }

        /// <summary>
        /// Renderiza la lista de versiones del historial
        /// </summary>
        private static void RenderHistoryList()
        {
            var history = GetFilteredHistory();

            if (history.Count == 0)
            {
                ImGui.TextColored(new Num.Vector4(1, 1, 0, 1),
                    string.IsNullOrEmpty(_searchFilter) 
                        ? "No hay versiones guardadas en el historial"
                        : "No se encontraron versiones con ese filtro");
                return;
            }

            // Contenedor con scroll
            ImGui.BeginChild("HistoryScrollRegion", 
                new Num.Vector2(0, 300), 
                ImGuiChildFlags.Borders);

            try
            {
                RenderGroupedHistory(history);
            }
            finally
            {
                ImGui.EndChild();
            }

            // Panel de previsualización (abajo de la lista)
            if (_previewVersion != null)
            {
                ImGui.Separator();
                RenderPreviewPanel();
            }
        }

        /// <summary>
        /// Agrupa y renderiza el historial por día
        /// </summary>
        private static void RenderGroupedHistory(List<YTBEngineHistory> history)
        {
            var groupedByDay = new Dictionary<string, List<(YTBEngineHistory, int)>>();

            for (int i = 0; i < history.Count; i++)
            {
                if (i >= MaxHistoryDisplay) break;

                var version = history[i];
                if (!DateTime.TryParse(version.SaveTime, out DateTime saveDate))
                {
                    saveDate = DateTime.MinValue;
                }

                string dayKey = saveDate != DateTime.MinValue 
                    ? saveDate.ToString("dddd, dd MMMM yyyy") 
                    : "Fecha desconocida";

                if (!groupedByDay.ContainsKey(dayKey))
                {
                    groupedByDay[dayKey] = new List<(YTBEngineHistory, int)>();
                }

                groupedByDay[dayKey].Add((version, i));
            }

            // Renderizar grupos (más recientes primero)
            var sortedGroups = groupedByDay.OrderByDescending(g => g.Key);

            foreach (var group in sortedGroups)
            {
                // Header del grupo (día)
                bool isOpen = ImGui.CollapsingHeader(
                    $"{group.Key} ({group.Value.Count} cambios)", 
                    ImGuiTreeNodeFlags.DefaultOpen);

                if (isOpen)
                {
                    ImGui.Indent();

                    // Renderizar versiones del día (más recientes primero)
                    var sortedVersions = group.Value.OrderByDescending(v => v.Item1.SaveTime);

                    foreach (var (version, originalIndex) in sortedVersions)
                    {
                        RenderHistoryItem(version, originalIndex);
                    }

                    ImGui.Unindent();
                }

                ImGui.Spacing();
            }
        }

        /// <summary>
        /// Renderiza un item individual del historial
        /// </summary>
        private static void RenderHistoryItem(YTBEngineHistory version, int index)
        {
            ImGui.PushID(index);

            bool isSelected = _selectedIndex == index;
            bool isCurrent = version.SaveTime == _currentVersionTime;

            // Cambiar color si es la versión actual
            if (isCurrent)
            {
                ImGui.PushStyleColor(ImGuiCol.Text, new Num.Vector4(0.2f, 1.0f, 0.2f, 1.0f));
            }

            // Botón selectable para la versión
            string displayText = isCurrent 
                ? $"✓ {version.SaveTime} (actual)" 
                : $"   {version.SaveTime}";

            if (ImGui.Selectable(displayText, isSelected))
            {
                _selectedIndex = index;
                _previewVersion = version;
            }

            if (isCurrent)
            {
                ImGui.PopStyleColor();
            }

            // Tooltip con información adicional
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.Text($"Versión: {version.SaveTime}");
                ImGui.Text($"Tiempo relativo: {FormatRelativeTime(version.SaveTime)}");
                
                if (version.GameDevelopVersion != null)
                {
                    ImGui.Separator();
                    ImGui.Text($"Escenas: {version.GameDevelopVersion.ScenesCount}");
                    int totalEntities = version.GameDevelopVersion.Scene
                        .Sum(s => s.EntitiesCount);
                    ImGui.Text($"Entidades totales: {totalEntities}");
                }

                if (!isCurrent)
                {
                    ImGui.Separator();
                    ImGui.TextColored(new Num.Vector4(1, 1, 0, 1),
                        "Click para previsualizar");
                    ImGui.TextColored(new Num.Vector4(0.5f, 0.5f, 1, 1),
                        "Doble click para restaurar (con confirmación)");
                }

                ImGui.EndTooltip();
            }

            // Doble click para restaurar (con confirmación)
            if (!isCurrent && ImGui.IsItemHovered() && ImGui.IsMouseDoubleClicked(ImGuiMouseButton.Left))
            {
                _selectedIndex = index;
                _previewVersion = version;
                _showConfirmRestore = true;
            }

            ImGui.PopID();
        }

        /// <summary>
        /// Renderiza el panel de previsualización de una versión
        /// </summary>
        private static void RenderPreviewPanel()
        {
            ImGui.PushStyleColor(ImGuiCol.ChildBg, new Num.Vector4(0.1f, 0.1f, 0.15f, 1.0f));
            ImGui.BeginChild("PreviewPanel", new Num.Vector2(0, 150), ImGuiChildFlags.Borders);

            try
            {
                ImGui.TextColored(new Num.Vector4(0.4f, 0.8f, 1.0f, 1.0f), 
                    "Previsualización de Versión");
                ImGui.Separator();

                ImGui.Text($"Fecha: {_previewVersion.SaveTime}");
                ImGui.Text($"Hace: {FormatRelativeTime(_previewVersion.SaveTime)}");

                if (_previewVersion.GameDevelopVersion != null)
                {
                    var gameInfo = _previewVersion.GameDevelopVersion;

                    ImGui.Spacing();
                    ImGui.TextColored(new Num.Vector4(1, 1, 0.5f, 1), 
                        $"Contenido ({gameInfo.ScenesCount} escenas):");

                    ImGui.Indent();

                    int sceneIndex = 0;
                    foreach (var scene in gameInfo.Scene)
                    {
                        if (sceneIndex >= 10)
                        {
                            ImGui.Text($"... y {gameInfo.ScenesCount - 10} escenas más");
                            break;
                        }

                        ImGui.BulletText($"{scene.Name} ({scene.EntitiesCount} entidades)");
                        sceneIndex++;
                    }

                    ImGui.Unindent();

                    // Botón de restaurar
                    ImGui.Spacing();
                    ImGui.Separator();

                    if (_previewVersion.SaveTime != _currentVersionTime)
                    {
                        ImGui.PushStyleColor(ImGuiCol.Button, new Num.Vector4(0.8f, 0.4f, 0.1f, 1.0f));
                        ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Num.Vector4(1.0f, 0.5f, 0.1f, 1.0f));

                        if (ImGui.Button("Restaurar esta versión"))
                        {
                            _showConfirmRestore = true;
                        }

                        ImGui.PopStyleColor(2);

                        if (ImGui.IsItemHovered())
                        {
                            ImGui.SetTooltip("Se pedirá confirmación antes de restaurar");
                        }
                    }
                    else
                    {
                        ImGui.TextColored(new Num.Vector4(0.2f, 1.0f, 0.2f, 1.0f),
                            "Esta es la versión actual");
                    }
                }
                else
                {
                    ImGui.TextColored(new Num.Vector4(1, 0, 0, 1),
                        "Error: No se pudo cargar la información de esta versión");
                }
            }
            finally
            {
                ImGui.EndChild();
                ImGui.PopStyleColor();
            }
        }

        /// <summary>
        /// Renderiza el diálogo de confirmación para restaurar
        /// </summary>
        private static void RenderConfirmRestoreDialog()
        {
            if (!_showConfirmRestore || _previewVersion == null)
            {
                return;
            }

            ImGui.OpenPopup("Confirmar Restauración");

            if (ImGui.BeginPopupModal("Confirmar Restauración", 
                ref _showConfirmRestore,
                ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoMove))
            {
                try
                {
                    ImGui.TextColored(new Num.Vector4(1, 1, 0, 1),
                        "⚠ ¿Estás seguro de restaurar esta versión?");

                    ImGui.Spacing();
                    ImGui.TextWrapped($"Se restaurará la versión del: {_previewVersion.SaveTime}");
                    ImGui.TextWrapped($"Hace: {FormatRelativeTime(_previewVersion.SaveTime)}");

                    ImGui.Spacing();
                    ImGui.Separator();
                    ImGui.Spacing();

                    ImGui.TextWrapped("NOTA: La versión actual se guardará automáticamente en el historial antes de restaurar.");

                    ImGui.Spacing();
                    ImGui.Separator();

                    // Botones
                    ImGui.PushStyleColor(ImGuiCol.Button, new Num.Vector4(0.2f, 0.8f, 0.2f, 1.0f));
                    ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Num.Vector4(0.3f, 1.0f, 0.3f, 1.0f));

                    if (ImGui.Button("Sí, Restaurar", new Num.Vector2(150, 0)))
                    {
                        RestoreVersion(_previewVersion);
                        _showConfirmRestore = false;
                    }

                    ImGui.PopStyleColor(2);

                    ImGui.SameLine();

                    ImGui.PushStyleColor(ImGuiCol.Button, new Num.Vector4(0.8f, 0.2f, 0.2f, 1.0f));
                    ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Num.Vector4(1.0f, 0.3f, 0.3f, 1.0f));

                    if (ImGui.Button("Cancelar", new Num.Vector2(150, 0)))
                    {
                        _showConfirmRestore = false;
                    }

                    ImGui.PopStyleColor(2);
                }
                finally
                {
                    ImGui.EndPopup();
                }
            }
        }

        /// <summary>
        /// Restaura una versión del historial
        /// </summary>
        private static void RestoreVersion(YTBEngineHistory version)
        {
            try
            {
                if (version?.GameDevelopVersion == null)
                {
                    EngineUISystem.SendLog("ERROR: No se pudo restaurar - versión inválida");
                    return;
                }

                // Guardar versión actual antes de restaurar (esto crea un nuevo entry en el historial)
                WriteYTBFile.EditYTBGameFile(version.GameDevelopVersion);

                // Actualizar versión actual
                _currentVersionTime = version.SaveTime;

                // Invalidar caché
                _cachedHistory = null;

                // Publicar evento
                EventManager.Instance.Publish(new OnGameFileWasReplaceByHistory(version.SaveTime));

                EngineUISystem.SendLog($"✓ Versión restaurada: {version.SaveTime}");

                // Resetear selección
                _selectedIndex = -1;
                _previewVersion = null;
            }
            catch (Exception ex)
            {
                EngineUISystem.SendLog($"ERROR al restaurar versión: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtiene el historial con caché para mejor rendimiento
        /// </summary>
        private static List<YTBEngineHistory> GetCachedHistory()
        {
            // Verificar si necesitamos actualizar el caché
            if (_cachedHistory == null || (DateTime.Now - _lastCacheUpdate) > CacheDuration)
            {
                try
                {
                    _cachedHistory = ReadYTBFile.ReadYTBGameFileHistory();
                    _lastCacheUpdate = DateTime.Now;

                    // Log solo la primera vez que se abre
                    if (_cachedHistory != null)
                    {
                        EngineUISystem.SendLog($"Historial cargado: {_cachedHistory.Count} versiones");
                    }
                }
                catch (Exception ex)
                {
                    EngineUISystem.SendLog($"ERROR al cargar historial: {ex.Message}");
                    _cachedHistory = new List<YTBEngineHistory>();
                }
            }

            return _cachedHistory ?? new List<YTBEngineHistory>();
        }

        /// <summary>
        /// Obtiene el historial filtrado según el texto de búsqueda
        /// </summary>
        private static List<YTBEngineHistory> GetFilteredHistory()
        {
            var history = GetCachedHistory();

            if (string.IsNullOrWhiteSpace(_searchFilter))
            {
                return history;
            }

            return history
                .Where(h => h.SaveTime.Contains(_searchFilter, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        /// <summary>
        /// Formatea una fecha como tiempo relativo (hace X horas, ayer, etc.)
        /// </summary>
        private static string FormatRelativeTime(string dateTimeString)
        {
            if (!DateTime.TryParse(dateTimeString, out DateTime dateTime))
            {
                return dateTimeString;
            }

            var timeSpan = DateTime.Now - dateTime;

            if (timeSpan.TotalSeconds < 60)
                return "hace unos segundos";
            
            if (timeSpan.TotalMinutes < 60)
            {
                int minutes = (int)timeSpan.TotalMinutes;
                return $"hace {minutes} minuto{(minutes != 1 ? "s" : "")}";
            }
            
            if (timeSpan.TotalHours < 24)
            {
                int hours = (int)timeSpan.TotalHours;
                return $"hace {hours} hora{(hours != 1 ? "s" : "")}";
            }
            
            if (timeSpan.TotalDays < 7)
            {
                int days = (int)timeSpan.TotalDays;
                if (days == 1) return "ayer";
                return $"hace {days} días";
            }
            
            if (timeSpan.TotalDays < 30)
            {
                int weeks = (int)(timeSpan.TotalDays / 7);
                return $"hace {weeks} semana{(weeks != 1 ? "s" : "")}";
            }
            
            if (timeSpan.TotalDays < 365)
            {
                int months = (int)(timeSpan.TotalDays / 30);
                return $"hace {months} mes{(months != 1 ? "es" : "")}";
            }
            
            int years = (int)(timeSpan.TotalDays / 365);
            return $"hace {years} año{(years != 1 ? "s" : "")}";
        }

        /// <summary>
        /// Cierra la ventana y resetea el estado
        /// </summary>
        private static void CloseWindow()
        {
            ImGui.CloseCurrentPopup();
            _searchFilter = "";
            _selectedIndex = -1;
            _previewVersion = null;
            _showConfirmRestore = false;
        }

        /// <summary>
        /// Abre la ventana de historial
        /// Debe ser llamado antes de Render()
        /// </summary>
        public static void Open()
        {
            ImGui.OpenPopup("Historial de Cambios del proyecto");
            
            // Resetear estado
            _searchFilter = "";
            _selectedIndex = -1;
            _previewVersion = null;
            _showConfirmRestore = false;
            
            // Invalidar caché para obtener datos frescos
            _cachedHistory = null;
            
            // Intentar obtener la versión actual
            try
            {
                var currentGame = ReadYTBFile.ReadYTBGameFile().GetAwaiter().GetResult();
                var history = GetCachedHistory();
                
                // La última versión del historial es la actual
                if (history.Count > 0)
                {
                    _currentVersionTime = history.Last().SaveTime;
                }
            }
            catch (Exception ex)
            {
                EngineUISystem.SendLog($"ERROR al determinar versión actual: {ex.Message}");
            }
        }
    }
}
#endif
