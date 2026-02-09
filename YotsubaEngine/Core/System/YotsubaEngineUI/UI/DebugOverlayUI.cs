using ImGuiNET;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using YotsubaEngine.Core.Entity;
using YotsubaEngine.Core.System.S_2D;
using Num = System.Numerics;

namespace YotsubaEngine.Core.System.YotsubaEngineUI.UI
{
#if YTB
    /// <summary>
    /// UI para controlar el debug overlay del juego (solo disponible en DEBUG).
    /// Permite mostrar/ocultar colisiones, grid cartesiano y lista de colisiones activas.
    /// </summary>
    public class DebugOverlayUI
    {
        // Flags de estado para cada overlay
        /// <summary>
        /// Shows entity collision overlays.
        /// Muestra overlays de colisiones de entidades.
        /// </summary>
        public static bool ShowEntityCollisions { get; set; } = false;
        /// <summary>
        /// Shows tilemap collision overlays.
        /// Muestra overlays de colisiones de tilemaps.
        /// </summary>
        public static bool ShowTilemapCollisions { get; set; } = false;
        /// <summary>
        /// Shows the debug grid overlay.
        /// Muestra el overlay de grilla de depuración.
        /// </summary>
        public static bool ShowGrid { get; set; } = false;
        /// <summary>
        /// Shows the collision list panel.
        /// Muestra el panel de lista de colisiones.
        /// </summary>
        public static bool ShowCollisionList { get; set; } = false;
        /// <summary>
        /// Shows button interaction logs.
        /// Muestra los logs de interacción de botones.
        /// </summary>
        public static bool ShowButtonLogs { get; set; } = false;
        /// <summary>
        /// Shows font drag handles by default.
        /// Muestra los handles de texto por defecto.
        /// </summary>
        public static bool ShowFontHandles { get; set; } = false; // Mostrar handles de texto por defecto en debug

        // Lista de colisiones detectadas (será actualizada por PhysicsSystem2D)
        private static readonly List<CollisionPair> _activeCollisions = new();

        /// <summary>
        /// Clase para representar un par de entidades que están colisionando
        /// </summary>
        public class CollisionPair
        {
            /// <summary>
            /// Name of the entity attempting to move.
            /// Nombre de la entidad que intenta moverse.
            /// </summary>
            public string EntityTryingToMove { get; set; }
            /// <summary>
            /// Name of the entity blocking movement.
            /// Nombre de la entidad que bloquea el movimiento.
            /// </summary>
            public string EntityImpediment { get; set; }
            /// <summary>
            /// Timestamp of the last collision update.
            /// Marca de tiempo de la última actualización de colisión.
            /// </summary>
            public double TimeStamp { get; set; }
        }

        /// <summary>
        /// Agrega una colisión a la lista activa
        /// </summary>
        public static void AddCollision(string entityTryingToMove, string entityImpediment, GameTime gameTime)
        {
            // Remover colisiones antiguas (más de 0.1 segundos)
            _activeCollisions.RemoveAll(c => gameTime.TotalGameTime.TotalSeconds - c.TimeStamp > 0.1);

            // Verificar si ya existe esta combinación
            bool exists = _activeCollisions.Exists(c =>
                c.EntityTryingToMove == entityTryingToMove &&
                c.EntityImpediment == entityImpediment);

            if (!exists)
            {
                _activeCollisions.Add(new CollisionPair
                {
                    EntityTryingToMove = entityTryingToMove,
                    EntityImpediment = entityImpediment,
                    TimeStamp = gameTime.TotalGameTime.TotalSeconds
                });
            }
            else
            {
                // Actualizar timestamp si ya existe
                var existing = _activeCollisions.Find(c =>
                    c.EntityTryingToMove == entityTryingToMove &&
                    c.EntityImpediment == entityImpediment);
                if (existing != null)
                {
                    existing.TimeStamp = gameTime.TotalGameTime.TotalSeconds;
                }
            }
        }

        /// <summary>
        /// Limpia todas las colisiones activas
        /// </summary>
        public static void ClearCollisions()
        {
            _activeCollisions.Clear();
        }

        /// <summary>
        /// Renderiza los controles del Debug Overlay
        /// </summary>
        public void Render()
        {
            //// Solo renderizar si estamos en modo juego activo
            if (RenderSystem2D.IsGameActive)
                return;

            ImGui.SetNextWindowSize(new Num.Vector2(300, 200), ImGuiCond.FirstUseEver);
            ImGui.SetNextWindowPos(new Num.Vector2(10, 10), ImGuiCond.FirstUseEver);

            if (ImGui.Begin("Debug Overlay"))
            {
                ImGui.SeparatorText("Visualización Debug");

                // Checkboxes para cada overlay (usar variables locales temporales)
                bool showEntityColl = ShowEntityCollisions;
                bool showTilemapColl = ShowTilemapCollisions;
                bool showGrid = ShowGrid;
                bool showCollList = ShowCollisionList;
                bool showButtonLogs = ShowButtonLogs;
                bool showFontHandles = ShowFontHandles;

                if (ImGui.Checkbox("Mostrar Colisiones Entidades", ref showEntityColl))
                    ShowEntityCollisions = showEntityColl;
                    
                if (ImGui.Checkbox("Mostrar Colisiones Tilemap", ref showTilemapColl))
                    ShowTilemapCollisions = showTilemapColl;
                    
                if (ImGui.Checkbox("Mostrar Cuadrícula", ref showGrid))
                    ShowGrid = showGrid;
                    
                if (ImGui.Checkbox("Mostrar Lista Colisiones", ref showCollList))
                    ShowCollisionList = showCollList;
                    
                if (ImGui.Checkbox("Mostrar Logs de Botones", ref showButtonLogs))
                    ShowButtonLogs = showButtonLogs;
                    
                if (ImGui.Checkbox("Mostrar Handles de Texto", ref showFontHandles))
                    ShowFontHandles = showFontHandles;

                ImGui.Spacing();
                ImGui.Separator();

                if (ImGui.Button("Limpiar Colisiones"))
                {
                    ClearCollisions();
                }
            }
            ImGui.End();

            // Ventana de colisiones activas
            if (ShowCollisionList)
            {
                RenderCollisionList();
            }
        }

        /// <summary>
        /// Renderiza la ventana con la lista de colisiones activas
        /// </summary>
        private void RenderCollisionList()
        {
            ImGui.SetNextWindowSize(new Num.Vector2(400, 300), ImGuiCond.FirstUseEver);
            ImGui.SetNextWindowPos(new Num.Vector2(320, 10), ImGuiCond.FirstUseEver);
            ImGui.SetNextWindowBgAlpha(0.85f); // Semi-transparente

            bool showList = ShowCollisionList;
            if (ImGui.Begin("Colisiones Activas", ref showList))
            {
                ShowCollisionList = showList;
                
                ImGui.Text($"Total: {_activeCollisions.Count} colision(es)");
                ImGui.Separator();

                if (_activeCollisions.Count == 0)
                {
                    ImGui.TextColored(new Num.Vector4(0.5f, 0.5f, 0.5f, 1.0f), "No hay colisiones activas");
                }
                else
                {
                    ImGui.BeginChild("CollisionListScroll", new Num.Vector2(0, 0), ImGuiChildFlags.None);

                    foreach (var collision in _activeCollisions)
                    {
                        ImGui.PushStyleColor(ImGuiCol.Text, new Num.Vector4(1.0f, 0.8f, 0.2f, 1.0f));
                        ImGui.Text($"• {collision.EntityTryingToMove}");
                        ImGui.PopStyleColor();

                        ImGui.Indent(20);
                        ImGui.TextColored(new Num.Vector4(1.0f, 0.3f, 0.3f, 1.0f), "⚠ colisiona con");
                        ImGui.Indent(20);

                        ImGui.PushStyleColor(ImGuiCol.Text, new Num.Vector4(0.3f, 0.8f, 1.0f, 1.0f));
                        ImGui.Text($"▸ {collision.EntityImpediment}");
                        ImGui.PopStyleColor();

                        ImGui.Unindent(40);
                        ImGui.Spacing();
                        ImGui.Separator();
                    }

                    ImGui.EndChild();
                }
            }
            ImGui.End();
        }
    }
#endif
}
