//-:cnd:noEmit
#if YTB
using ImGuiNET;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using YotsubaEngine.ActionFiles.YTB_Files;
using YotsubaEngine.Core.Component.C_AGNOSTIC;
using YotsubaEngine.Core.Entity;
using YotsubaEngine.Core.YotsubaGame;
using Num = System.Numerics;

namespace YotsubaEngine.Core.System.YotsubaEngineUI.UI
{
    /// <summary>
    /// Panel ImGui para editar transforms de modelos 3D seleccionados en modo engine.
    /// Permite edición individual y global con sliders, y persistencia al archivo .ytb.
    /// <para>ImGui panel for editing transforms of selected 3D models in engine mode.
    /// Supports individual and global editing with sliders, and persistence to .ytb file.</para>
    /// </summary>
    internal class Model3DEditorUI
    {
        private float _sliderX = 0f;
        private float _sliderY = 0f;
        private float _sliderZ = 0f;
        private float _sliderScale = 0f;

        /// <summary>
        /// Renderiza el panel del editor de modelos 3D si hay modelos seleccionados.
        /// <para>Renders the 3D model editor panel if models are selected.</para>
        /// </summary>
        public void Render()
        {
            var selectedIds = YTBGlobalState.SelectedModel3DEntityIds;
            if (selectedIds.Count == 0) return;

            var game = (YTBGame)YTBGame.Instance;
            if (game?.SceneManager?.Scenes == null) return;

            // Buscar la escena activa con EntityManager
            EntityManager entityManager = null;
            Scene activeScene = null;

            activeScene = YTBGlobalState.Game.SceneManager.CurrentScene;
            entityManager = activeScene.EntityManager;

            if (entityManager == null) return;

            RenderIndividualTransforms(entityManager, selectedIds);
            RenderGlobalSliders(entityManager, selectedIds);
        }

        /// <summary>
        /// Ventana con los transforms individuales de cada modelo seleccionado.
        /// <para>Window with individual transforms for each selected model.</para>
        /// </summary>
        private void RenderIndividualTransforms(EntityManager entityManager, HashSet<int> selectedIds)
        {
            ImGui.SetNextWindowSize(new Num.Vector2(380, 400), ImGuiCond.FirstUseEver);
            if (!ImGui.Begin("Modelos 3D — Transforms", ImGuiWindowFlags.NoCollapse))
            {
                ImGui.End();
                return;
            }

            Span<Yotsuba> Entities = entityManager.YotsubaEntities.AsSpan();
            Span<TransformComponent> transformComponents = entityManager.TransformComponents.AsSpan();
            foreach (int entityId in selectedIds)
            {
                if (entityId >= Entities.Length) continue;
                var entity = Entities[entityId];

                ref TransformComponent transform = ref transformComponents[entityId];
                string label = string.IsNullOrEmpty(entity.Name) ? $"Entidad #{entityId}" : entity.Name;

                if (ImGui.CollapsingHeader(label, ImGuiTreeNodeFlags.DefaultOpen))
                {
                    ImGui.PushID(entityId);

                    // Position
                    var pos = new Num.Vector3(transform.Position.X, transform.Position.Y, transform.Position.Z);
                    if (ImGui.DragFloat3("Posición", ref pos, 1f))
                    {
                        transform.Position = new Vector3(pos.X, pos.Y, pos.Z);
                    }

                    // Scale
                    float scale = transform.Scale;
                    if (ImGui.DragFloat("Escala", ref scale, 0.01f, 0.01f, 100f))
                    {
                        transform.Scale = scale;
                    }

                    // Rotation
                    float rotation = transform.Rotation;
                    if (ImGui.DragFloat("Rotación", ref rotation, 0.5f))
                    {
                        transform.Rotation = rotation;
                    }

                    // Size
                    var size = new Num.Vector3(transform.Size.X, transform.Size.Y, transform.Size.Z);
                    if (ImGui.DragFloat3("Tamaño", ref size, 1f))
                    {
                        transform.Size = new Vector3(size.X, size.Y, size.Z);
                    }

                    ImGui.PopID();
                    ImGui.Separator();
                }
            }

            // Botón Guardar Todo
            ImGui.Spacing();
            if (ImGui.Button("Guardar Todo", new Num.Vector2(-1, 30)))
            {
                SaveAllSelectedModels(entityManager, selectedIds);
            }

            ImGui.End();
        }

        /// <summary>
        /// Ventana con sliders globales que afectan a todos los modelos seleccionados.
        /// Los sliders van de -300 a 300 y se reinician a 0 al soltarlos.
        /// <para>Window with global sliders affecting all selected models.
        /// Sliders range from -300 to 300 and reset to 0 on release.</para>
        /// </summary>
        private void RenderGlobalSliders(EntityManager entityManager, HashSet<int> selectedIds)
        {
            ImGui.SetNextWindowSize(new Num.Vector2(350, 200), ImGuiCond.FirstUseEver);
            if (!ImGui.Begin("Modelos 3D — Ajuste Global", ImGuiWindowFlags.NoCollapse))
            {
                ImGui.End();
                return;
            }

            ImGui.TextDisabled("Arrastra para mover todos los modelos seleccionados");
            ImGui.Separator();

            RenderGlobalSlider("Mover X", ref _sliderX, selectedIds, entityManager,
                (ref TransformComponent t, float delta) =>
                    t.Position = new Vector3(t.Position.X + delta, t.Position.Y, t.Position.Z));

            RenderGlobalSlider("Mover Y", ref _sliderY, selectedIds, entityManager,
                (ref TransformComponent t, float delta) =>
                    t.Position = new Vector3(t.Position.X, t.Position.Y + delta, t.Position.Z));

            RenderGlobalSlider("Mover Z", ref _sliderZ, selectedIds, entityManager,
                (ref TransformComponent t, float delta) =>
                    t.Position = new Vector3(t.Position.X, t.Position.Y, t.Position.Z + delta));

            RenderGlobalSlider("Escala", ref _sliderScale, selectedIds, entityManager,
                (ref TransformComponent t, float delta) =>
                    t.Scale = Math.Max(0.01f, t.Scale + delta));

            ImGui.End();
        }

        private delegate void ApplyDelta(ref TransformComponent t, float delta);

        private void RenderGlobalSlider(string label, ref float sliderValue, HashSet<int> selectedIds,
            EntityManager entityManager, ApplyDelta applyDelta)
        {
            float prev = sliderValue;
            ImGui.SliderFloat(label, ref sliderValue, -300f, 300f);

            float delta = sliderValue - prev;
            if (Math.Abs(delta) > 0.001f)
            {
                foreach (int id in selectedIds)
                {
                    if (id >= entityManager.TransformComponents.Count) continue;
                    ref TransformComponent t = ref entityManager.TransformComponents[id];
                    applyDelta(ref t, delta);
                }
            }

            // Al soltar el slider, reiniciar a 0
            if (ImGui.IsItemDeactivatedAfterEdit())
            {
                sliderValue = 0f;
            }
        }

        /// <summary>
        /// Persiste los transforms de todos los modelos seleccionados al archivo .ytb.
        /// <para>Persists the transforms of all selected models to the .ytb file.</para>
        /// </summary>
        private void SaveAllSelectedModels(EntityManager entityManager, HashSet<int> selectedIds)
        {
            var game = (YTBGame)YTBGame.Instance;
            if (game?.SceneManager?.Scenes == null) return;

            foreach (var scene in game.SceneManager.Scenes.AsSpan())
            {
                if (scene.EntityManager != entityManager) continue;

                var ytbScene = EngineUISystem.GameInfo?.Scene?
                    .FirstOrDefault(x => x.Name == scene.SceneName);
                if (ytbScene == null) continue;

                foreach (int entityId in selectedIds)
                {
                    if (entityId >= entityManager.YotsubaEntities.Count) continue;
                    var entity = entityManager.YotsubaEntities[entityId];

                    ref TransformComponent transform = ref entityManager.TransformComponents[entityId];

                    var ytbEntity = ytbScene.Entities.FirstOrDefault(x => x.Name == entity.Name);
                    if (ytbEntity == null) continue;

                    var transformComp = ytbEntity.Components
                        .FirstOrDefault(c => c.ComponentName == "TransformComponent");
                    if (transformComp == null) continue;

                    // Actualizar Position
                    UpdateProperty(transformComp, "Position",
                        $"{transform.Position.X},{transform.Position.Y},{transform.Position.Z}");

                    // Actualizar Size
                    UpdateProperty(transformComp, "Size",
                        $"{transform.Size.X},{transform.Size.Y},{transform.Size.Z}");

                    // Actualizar Scale
                    UpdateProperty(transformComp, "Scale", transform.Scale.ToString());

                    // Actualizar Rotation
                    UpdateProperty(transformComp, "Rotation", transform.Rotation.ToString());
                }

                break;
            }

            EngineUISystem.SaveChanges();
            EngineUISystem._instance?.ShowModeSwitchAlert("Transforms guardados correctamente");
        }

        private static void UpdateProperty(YTBComponents component, string propertyName, string newValue)
        {
            int index = component.Propiedades.FindIndex(x => x.Item1 == propertyName);
            if (index >= 0)
            {
                component.Propiedades[index] = new(propertyName, newValue);
            }
        }
    }
}
#endif
//+:cnd:noEmit
