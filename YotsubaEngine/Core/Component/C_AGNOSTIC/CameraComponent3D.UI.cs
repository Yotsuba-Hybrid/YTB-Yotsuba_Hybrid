#if YTB
using System.Linq;
using Hexa.NET.ImGui;
using YotsubaEngine.Core.System.YotsubaEngineUI.UI;

namespace YotsubaEngine.Core.Component.C_AGNOSTIC
{
    public partial class CameraComponent3D
    {
        /// <summary>
        /// Render UI editor para "EntityName": dropdown con los nombres de entidades de la escena actual.
        /// </summary>
        public static void RenderEntityNameUI(IUIRenderContext ctx, string raw)
        {
            ImGui.TextDisabled("Entidad a Seguir");
            ImGui.SameLine();

            var names = ctx.SceneEntityNames.ToList();
            int selectedIndex = names.IndexOf(raw);
            string displayName = selectedIndex >= 0 ? names[selectedIndex] : "Seleccionar entidad...";

            ImGui.PushItemWidth(250);
            if (ImGui.BeginCombo("##EntityName", displayName))
            {
                for (int i = 0; i < names.Count; i++)
                {
                    bool isSelected = (selectedIndex == i);
                    if (ImGui.Selectable(names[i], isSelected))
                    {
                        ctx.UpdateProperty("EntityName", names[i]);
                    }
                    if (isSelected) ImGui.SetItemDefaultFocus();
                }
                ImGui.EndCombo();
            }
            ImGui.PopItemWidth();

            if (!names.Any())
            {
                ImGui.TextDisabled("(No hay entidades en esta escena)");
            }
        }
    }
}
#endif
