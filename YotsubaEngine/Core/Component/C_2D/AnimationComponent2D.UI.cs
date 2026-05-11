#if YTB
using System;
using System.Collections.Generic;
using System.Linq;
using Hexa.NET.ImGui;
using YotsubaEngine.Core.System.YotsubaEngineUI.UI;

namespace YotsubaEngine.Core.Component.C_2D
{
    public partial struct AnimationComponent2D
    {
        private static readonly string[] _animationTypeNames = Enum.GetNames(typeof(AnimationType));

        /// <summary>
        /// Render UI editor para "TextureAtlasPath" del AnimationComponent2D: dropdown con los .xml encontrados bajo Content/.
        /// </summary>
        public static void RenderTextureAtlasUI(IUIRenderContext ctx, string raw)
        {
            ImGui.TextDisabled("Atlas de texturas");
            ImGui.SameLine();

            var atlases = ctx.TextureAtlasFiles;
            int selectedIndex = -1;
            for (int i = 0; i < atlases.Count; i++)
                if (atlases[i] == raw) { selectedIndex = i; break; }

            string preview = (selectedIndex >= 0 && selectedIndex < atlases.Count)
                ? atlases[selectedIndex]
                : "Seleccionar...";

            ImGui.PushItemWidth(300);
            if (ImGui.BeginCombo("##AnimTextureAtlasPath", preview))
            {
                for (int i = 0; i < atlases.Count; i++)
                {
                    bool isSelected = (selectedIndex == i);
                    if (ImGui.Selectable(atlases[i], isSelected))
                    {
                        ctx.UpdateProperty("TextureAtlasPath", atlases[i]);
                        ctx.UpdateProperty("AnimationBindings", "");
                    }
                    if (isSelected) ImGui.SetItemDefaultFocus();
                }
                ImGui.EndCombo();
            }
            ImGui.PopItemWidth();
        }

        /// <summary>
        /// Render UI editor para "AnimationBindings": combos pareados (AnimationType → nombre de animación del atlas)
        /// con botones de agregar/quitar.
        /// </summary>
        public static void RenderAnimationBindingsUI(IUIRenderContext ctx, string raw)
        {
            ImGui.TextDisabled("Animaciones Vinculadas");
            ImGui.Spacing();

            string atlasPath = null;
            foreach (var prop in ctx.Component.Propiedades)
                if (prop.Item1 == "TextureAtlasPath") { atlasPath = prop.Item2; break; }

            var availableAnimations = string.IsNullOrEmpty(atlasPath)
                ? new List<AnimationInfo>()
                : ctx.ParseAnimations(atlasPath);

            var bindings = (raw ?? "")
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim())
                .Where(s => s.Contains(':'))
                .Select(s =>
                {
                    var parts = s.Split(':');
                    return (Type: parts[0], Name: parts[1]);
                })
                .ToList();

            for (int i = 0; i < bindings.Count; i++)
            {
                ImGui.PushID($"binding_{i}");

                int typeIndex = Array.IndexOf(_animationTypeNames, bindings[i].Type);
                if (typeIndex < 0) typeIndex = 0;

                ImGui.PushItemWidth(120);
                if (ImGui.BeginCombo($"##type_{i}", _animationTypeNames[typeIndex]))
                {
                    for (int t = 0; t < _animationTypeNames.Length; t++)
                    {
                        bool isSelected = (typeIndex == t);
                        if (ImGui.Selectable(_animationTypeNames[t], isSelected))
                        {
                            bindings[i] = (_animationTypeNames[t], bindings[i].Name);
                            ctx.UpdateProperty("AnimationBindings",
                                string.Join(",", bindings.Select(b => $"{b.Type}:{b.Name}")));
                        }
                        if (isSelected) ImGui.SetItemDefaultFocus();
                    }
                    ImGui.EndCombo();
                }
                ImGui.PopItemWidth();

                ImGui.SameLine();
                ImGui.Text("→");
                ImGui.SameLine();

                int animIndex = availableAnimations.FindIndex(a => a.Name == bindings[i].Name);
                string displayAnim = animIndex >= 0 ? availableAnimations[animIndex].Name : bindings[i].Name;

                ImGui.PushItemWidth(200);
                if (ImGui.BeginCombo($"##anim_{i}", displayAnim))
                {
                    for (int a = 0; a < availableAnimations.Count; a++)
                    {
                        bool isSelected = (animIndex == a);
                        if (ImGui.Selectable(availableAnimations[a].Name, isSelected))
                        {
                            bindings[i] = (bindings[i].Type, availableAnimations[a].Name);
                            ctx.UpdateProperty("AnimationBindings",
                                string.Join(",", bindings.Select(b => $"{b.Type}:{b.Name}")));
                        }
                        if (isSelected) ImGui.SetItemDefaultFocus();
                    }
                    ImGui.EndCombo();
                }
                ImGui.PopItemWidth();

                ImGui.SameLine();
                if (ImGui.Button($"X##remove_{i}"))
                {
                    bindings.RemoveAt(i);
                    ctx.UpdateProperty("AnimationBindings",
                        string.Join(",", bindings.Select(b => $"{b.Type}:{b.Name}")));
                }

                ImGui.PopID();
            }

            if (ImGui.Button("+ Agregar Vinculación"))
            {
                if (availableAnimations.Any())
                {
                    bindings.Add((_animationTypeNames[0], availableAnimations[0].Name));
                    ctx.UpdateProperty("AnimationBindings",
                        string.Join(",", bindings.Select(b => $"{b.Type}:{b.Name}")));
                }
            }

            if (!availableAnimations.Any())
            {
                ImGui.TextDisabled("(Selecciona un atlas de texturas con animaciones)");
            }
        }
    }
}
#endif
