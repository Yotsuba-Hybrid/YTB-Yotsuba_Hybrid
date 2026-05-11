#if YTB
using System;
using System.Linq;
using Hexa.NET.ImGui;
using YotsubaEngine.Core.System.YotsubaEngineUI.UI;

namespace YotsubaEngine.Core.Component.C_AGNOSTIC
{
    public partial struct InputComponent
    {
        private static readonly string[] _actionNames =
            { " ", "MoveUp", "MoveDown", "MoveLeft", "MoveRight", "Jump", "Attack" };

        private static readonly string[] _keyNames =
            { " ", "W", "A", "S", "D", "Space", "Enter", "Esc", "Shift", "Ctrl" };

        /// <summary>
        /// Render UI editor para "InputsInUse": tres checkboxes (Ratón / Teclado / Mando) que actualizan la lista CSV.
        /// </summary>
        public static void RenderInputsInUseUI(IUIRenderContext ctx, string raw)
        {
            ImGui.TextDisabled("Entradas en uso");
            var inputs = (raw ?? "")
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim())
                .ToList();

            bool hasMouse = inputs.Contains("HasMouse");
            bool hasKeyboard = inputs.Contains("HasKeyboard");
            bool hasGamepad = inputs.Contains("HasGamepad");

            if (ImGui.Checkbox("Ratón", ref hasMouse))
            {
                if (hasMouse && !inputs.Contains("HasMouse")) inputs.Add("HasMouse");
                else inputs.Remove("HasMouse");
                ctx.UpdateProperty("InputsInUse", string.Join(",", inputs));
            }

            if (ImGui.Checkbox("Teclado", ref hasKeyboard))
            {
                if (hasKeyboard && !inputs.Contains("HasKeyboard")) inputs.Add("HasKeyboard");
                else inputs.Remove("HasKeyboard");
                ctx.UpdateProperty("InputsInUse", string.Join(",", inputs));
            }

            if (ImGui.Checkbox("Mando", ref hasGamepad))
            {
                if (hasGamepad && !inputs.Contains("HasGamepad")) inputs.Add("HasGamepad");
                else inputs.Remove("HasGamepad");
                ctx.UpdateProperty("InputsInUse", string.Join(",", inputs));
            }
        }

        /// <summary>
        /// Render UI editor para "KeyboardMappings": filas de combos pareados (Acción → Tecla) con botones de quitar/agregar.
        /// </summary>
        public static void RenderKeyboardMappingsUI(IUIRenderContext ctx, string raw)
        {
            ImGui.TextDisabled("Mapeo de teclado");

            var pairs = (raw ?? "")
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim().Split(':'))
                .Where(p => p.Length == 2)
                .Select(p => (Action: p[0].Trim(), Key: p[1].Trim()))
                .ToList();

            bool changed = false;

            for (int i = 0; i < pairs.Count; i++)
            {
                ImGui.PushID(i);

                int actionIdx = Array.IndexOf(_actionNames, pairs[i].Action);
                if (actionIdx < 0) actionIdx = 0;

                ImGui.PushItemWidth(150);
                if (ImGui.Combo("##Accion", ref actionIdx, _actionNames, _actionNames.Length))
                {
                    pairs[i] = (_actionNames[actionIdx], pairs[i].Key);
                    changed = true;
                }
                ImGui.PopItemWidth();

                ImGui.SameLine();

                int keyIdx = Array.IndexOf(_keyNames, pairs[i].Key);
                if (keyIdx < 0) keyIdx = 0;

                ImGui.PushItemWidth(100);
                if (ImGui.Combo("##Tecla", ref keyIdx, _keyNames, _keyNames.Length))
                {
                    pairs[i] = (pairs[i].Action, _keyNames[keyIdx]);
                    changed = true;
                }
                ImGui.PopItemWidth();

                ImGui.SameLine();
                if (ImGui.Button("X"))
                {
                    pairs.RemoveAt(i);
                    changed = true;
                    i--;
                }

                ImGui.PopID();
            }

            if (ImGui.Button("+ Agregar Mapeo"))
            {
                pairs.Add((_actionNames[0], _keyNames[0]));
                changed = true;
            }

            if (changed)
            {
                ctx.UpdateProperty("KeyboardMappings",
                    string.Join(", ", pairs.Select(p => $"{p.Action}:{p.Key}")));
            }
        }
    }
}
#endif
