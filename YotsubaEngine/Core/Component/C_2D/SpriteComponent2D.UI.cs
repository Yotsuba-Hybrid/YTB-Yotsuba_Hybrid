#if YTB
using System.Collections.Generic;
using Hexa.NET.ImGui;
using YotsubaEngine.Core.System.YotsubaEngineUI.UI;

namespace YotsubaEngine.Core.Component.C_2D
{
    public partial struct SpriteComponent2D
    {
        /// <summary>
        /// Render UI editor para "TextureAtlasPath": dropdown con los .xml encontrados bajo Content/.
        /// Al cambiar el atlas se limpia SpriteName para evitar referencias rotas.
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
            if (ImGui.BeginCombo("##TextureAtlasPath", preview))
            {
                for (int i = 0; i < atlases.Count; i++)
                {
                    bool isSelected = (selectedIndex == i);
                    if (ImGui.Selectable(atlases[i], isSelected))
                    {
                        ctx.UpdateProperty("TextureAtlasPath", atlases[i]);
                        ctx.UpdateProperty("SpriteName", "");
                    }
                    if (isSelected) ImGui.SetItemDefaultFocus();
                }
                ImGui.EndCombo();
            }
            ImGui.PopItemWidth();
        }

        /// <summary>
        /// Render UI editor para "SpriteName": dropdown con las subtexturas del atlas seleccionado.
        /// Al seleccionar un sprite, autocompleta SourceRectangle.
        /// </summary>
        public static void RenderSpriteNameUI(IUIRenderContext ctx, string raw)
        {
            ImGui.TextDisabled("Sprite");
            ImGui.SameLine();

            string atlasPath = null;
            foreach (var prop in ctx.Component.Propiedades)
                if (prop.Item1 == "TextureAtlasPath") { atlasPath = prop.Item2; break; }

            var subtextures = string.IsNullOrEmpty(atlasPath)
                ? new List<SubtextureInfo>()
                : ctx.ParseSubtextures(atlasPath);

            int selectedIndex = subtextures.FindIndex(s => s.Name == raw);
            string displayName = selectedIndex >= 0 ? subtextures[selectedIndex].Name : "Seleccionar sprite...";

            ImGui.PushItemWidth(300);
            if (ImGui.BeginCombo("##SpriteName", displayName))
            {
                for (int i = 0; i < subtextures.Count; i++)
                {
                    bool isSelected = (selectedIndex == i);
                    if (ImGui.Selectable(subtextures[i].Name, isSelected))
                    {
                        var s = subtextures[i];
                        ctx.UpdateProperty("SpriteName", s.Name);
                        ctx.UpdateProperty("SourceRectangle", $"{s.X},{s.Y},{s.Width},{s.Height}");
                    }
                    if (isSelected) ImGui.SetItemDefaultFocus();
                }
                ImGui.EndCombo();
            }
            ImGui.PopItemWidth();
        }
    }
}
#endif
